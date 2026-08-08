using System;
using System.Collections.Generic;
using DG.Tweening;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Views
{
    public class BoardView : MonoBehaviour
    {
        public event Action<int, int> TileClicked;

        [SerializeField] private GridLayoutGroup _boardContainer;
        [SerializeField] private GridCellSizeFitter _cellSizeFitter;
        [SerializeField] private TileTypeConfig _tileTypeConfig;
        [SerializeField] private TileSpotView _tileSpotPrefab;

        private GameObject[,] _tiles;
        private TileSpotView[,] _tileSpots;

        public void CreateBoard(Board board)
        {
            _cellSizeFitter.SetBoardSize(board.Width, board.Height);
            _tiles = new GameObject[board.Width, board.Height];
            _tileSpots = new TileSpotView[board.Width, board.Height];

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
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
                        GameObject tile = Instantiate(tilePrefab);
                        tileSpot.SetTile(tile);

                        _tiles[x, y] = tile;
                    }
                }
            }
        }

        public Tween CreateTile(List<AddedTileInfo> addedTiles)
        {
            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < addedTiles.Count; i++)
            {
                AddedTileInfo addedTileInfo = addedTiles[i];
                Vector2Int position = addedTileInfo.Position;

                TileSpotView tileSpot = _tileSpots[position.x, position.y];

                GameObject tilePrefab = _tileTypeConfig.GetPrefab(addedTileInfo.Type);
                GameObject tile = Instantiate(tilePrefab);
                tileSpot.SetTile(tile);

                _tiles[position.x, position.y] = tile;

                tile.transform.localScale = Vector2.zero;
                sequence.Join(tile.transform.DOScale(1.0f, 0.2f));
            }

            return sequence;
        }

        public Tween DestroyTiles(List<Vector2Int> matchedPosition)
        {
            for (int i = 0; i < matchedPosition.Count; i++)
            {
                Vector2Int position = matchedPosition[i];
                Destroy(_tiles[position.x, position.y]);
                _tiles[position.x, position.y] = null;
            }

            return DOVirtual.DelayedCall(0.2f, () => { });
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

        public Tween MoveTiles(List<MovedTileInfo> movedTiles)
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
            TileClicked(x, y);
        }
        #endregion
    }
}
