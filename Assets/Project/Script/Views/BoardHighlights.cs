using Gazeus.DesafioMatch3.Models;

namespace Gazeus.DesafioMatch3.Views
{
    public class BoardHighlights
    {
        private readonly TileSpotView[,] _tileSpots;

        private TileSpotView _hintFrom;
        private TileSpotView _hintTo;
        private TileSpotView _selected;

        public BoardHighlights(TileSpotView[,] tileSpots)
        {
            _tileSpots = tileSpots;
        }

        public void ShowHint(Move move)
        {
            ClearHint();

            _hintFrom = _tileSpots[move.From.x, move.From.y];
            _hintTo = _tileSpots[move.To.x, move.To.y];

            _hintFrom.SetHighlighted(true);
            _hintTo.SetHighlighted(true);
        }

        public void ClearHint()
        {
            if (_hintFrom != null) _hintFrom.SetHighlighted(false);
            if (_hintTo != null) _hintTo.SetHighlighted(false);

            _hintFrom = null;
            _hintTo = null;
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

        public void Clear()
        {
            ClearHint();
            ClearSelection();
        }
    }
}
