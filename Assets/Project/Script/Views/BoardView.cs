using System;
using System.Collections.Generic;
using DG.Tweening;
using Gazeus.DesafioMatch3.Core.Pooling;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.ScriptableObjects;
using Gazeus.DesafioMatch3.ScriptableObjects.Feedback;
using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Views
{
    public class BoardView : MonoBehaviour
    {
        public event Action<int, int> TileClicked;
        public event Action<BoardSequence, int> CascadeStarted;
        public event Action CascadeFinished;

        [SerializeField] private GridLayoutGroup _boardContainer;
        [SerializeField] private GridCellSizeFitter _cellSizeFitter;
        [SerializeField] private TileTypeConfig _tileTypeConfig;
        [SerializeField] private TileSpotView _tileSpotPrefab;
        [SerializeField] private EffectsView _effects;
        [SerializeField] private EffectCue _matchCue;
        [SerializeField] private EffectCue _spawnCue;
        [SerializeField] private EffectCue _hintCue;
        [SerializeField] private float _cascadePitchStep = 1f;

        private PrefabPoolRegistry _tilePools;
        private GameObject[,] _tiles;
        private TileSpotView[,] _tileSpots;
        private TileSpotView _hintFrom;
        private TileSpotView _hintTo;
        private TileSpotView _selected;

        #region Unity
        private void Awake()
        {
            _tilePools = new PrefabPoolRegistry(transform, "[Tile Pool]");
        }
        #endregion

        public void CreateBoard(Board board)
        {
            ClearHint();
            ClearSelection();

            _cellSizeFitter.SetBoardSize(board.Width, board.Height);
            _tiles = new GameObject[board.Width, board.Height];
            _tileSpots = new TileSpotView[board.Width, board.Height];

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    // Tile spots live as long as the board does, so they stay out of the pool: nothing to recycle.
                    TileSpotView tileSpot = Instantiate(_tileSpotPrefab);
                    tileSpot.transform.SetParent(_boardContainer.transform, false);
                    tileSpot.SetPosition(x, y);

                    _tileSpots[x, y] = tileSpot;

                    if (board.IsDead(x, y))
                    {
                        tileSpot.SetDead();
                        continue;
                    }

                    tileSpot.Clicked += TileSpot_Clicked;

                    int tileTypeIndex = board[x, y].Type;
                    if (tileTypeIndex > -1)
                    {
                        GameObject tilePrefab = _tileTypeConfig.GetPrefab(tileTypeIndex);
                        GameObject tile = _tilePools.Get(tilePrefab, tileSpot.transform);
                        tileSpot.SetTile(tile);

                        _tiles[x, y] = tile;
                    }
                }
            }
        }

        public void Play(List<BoardSequence> sequences)
        {
            if (sequences.Count == 0)
            {
                CascadeFinished?.Invoke();
                return;
            }

            PlayStep(sequences, 0);
        }

        private void PlayStep(List<BoardSequence> sequences, int index)
        {
            BoardSequence step = sequences[index];

            CascadeStarted?.Invoke(step, index);

            Sequence sequence = DOTween.Sequence();
            sequence.Append(ReleaseTiles(step.MatchedPosition, index));
            sequence.Append(MoveTiles(step.MovedTiles));
            sequence.Append(CreateTile(step.AddedTiles));

            index += 1;
            if (index < sequences.Count) sequence.onComplete += () => PlayStep(sequences, index);
            else sequence.onComplete += () => CascadeFinished?.Invoke();
        }

        private Tween CreateTile(List<AddedTileInfo> addedTiles)
        {
            if (addedTiles.Count > 0) _effects.PlaySound(_spawnCue);

            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < addedTiles.Count; i++)
            {
                AddedTileInfo addedTileInfo = addedTiles[i];
                Vector2Int position = addedTileInfo.Position;

                TileSpotView tileSpot = _tileSpots[position.x, position.y];

                GameObject tilePrefab = _tileTypeConfig.GetPrefab(addedTileInfo.Type);
                GameObject tile = _tilePools.Get(tilePrefab, tileSpot.transform);
                tileSpot.SetTile(tile);

                _tiles[position.x, position.y] = tile;

                tile.transform.localScale = Vector2.zero;
                sequence.Join(tile.transform.DOScale(1.0f, 0.2f));
            }

            return sequence;
        }

        private Tween ReleaseTiles(List<Vector2Int> matchedPosition, int cascadeStep)
        {
            _effects.PlaySound(_matchCue, Mathf.Pow(2f, cascadeStep * _cascadePitchStep / 12f));

            for (int i = 0; i < matchedPosition.Count; i++)
            {
                Vector2Int position = matchedPosition[i];
                TileSpotView tileSpot = _tileSpots[position.x, position.y];

                _effects.Spawn(_matchCue, tileSpot.transform.position);

                _tilePools.Release(_tiles[position.x, position.y]);
                _tiles[position.x, position.y] = null;
            }

            return DOVirtual.DelayedCall(0.2f, () => { });
        }

        public void SetSelected(int x, int y)
        {
            ClearSelection();

            _selected = _tileSpots[x, y];
            _selected.SetSelected(true);
        }

        public void ClearSelection()
        {
            if (_selected != null) _selected.SetSelected(false);

            _selected = null;
        }

        public void ShowHint(Move move)
        {
            ClearHint();

            _hintFrom = _tileSpots[move.From.x, move.From.y];
            _hintTo = _tileSpots[move.To.x, move.To.y];

            _hintFrom.SetHighlighted(true);
            _hintTo.SetHighlighted(true);

            _effects.PlaySound(_hintCue);
        }

        public void ClearHint()
        {
            if (_hintFrom != null) _hintFrom.SetHighlighted(false);
            if (_hintTo != null) _hintTo.SetHighlighted(false);

            _hintFrom = null;
            _hintTo = null;
        }

        public Vector3 GetTilePosition(int x, int y)
        {
            return _tileSpots[x, y].transform.position;
        }

        public Vector3 GetMatchCenter(List<Vector2Int> positions)
        {
            if (positions.Count == 0) return transform.position;

            Vector3 sum = Vector3.zero;
            for (int i = 0; i < positions.Count; i++)
            {
                Vector2Int position = positions[i];
                sum += _tileSpots[position.x, position.y].transform.position;
            }

            return sum / positions.Count;
        }

        private Tween MoveTiles(List<MovedTileInfo> movedTiles)
        {
            GameObject[,] tiles = (GameObject[,])_tiles.Clone();

            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < movedTiles.Count; i++)
            {
                MovedTileInfo movedTileInfo = movedTiles[i];

                Vector2Int from = movedTileInfo.From;
                Vector2Int to = movedTileInfo.To;

                sequence.Join(_tileSpots[to.x, to.y].AnimatedSetTile(_tiles[from.x, from.y]));

                tiles[to.x, to.y] = _tiles[from.x, from.y];
            }

            _tiles = tiles;

            return sequence;
        }

        public Tween SwapTiles(int fromX, int fromY, int toX, int toY)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_tileSpots[fromX, fromY].AnimatedSetTile(_tiles[toX, toY]));
            sequence.Join(_tileSpots[toX, toY].AnimatedSetTile(_tiles[fromX, fromY]));

            (_tiles[toX, toY], _tiles[fromX, fromY]) = (_tiles[fromX, fromY], _tiles[toX, toY]);

            return sequence;
        }

        #region Events
        private void TileSpot_Clicked(int x, int y)
        {
            TileClicked?.Invoke(x, y);
        }
        #endregion
    }
}
