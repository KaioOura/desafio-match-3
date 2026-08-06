namespace Gazeus.DesafioMatch3.Models
{
    public sealed class Board
    {
        private readonly Tile[,] _tiles;

        public int Width { get; }
        public int Height { get; }

        public Board(int width, int height)
        {
            Width = width;
            Height = height;
            _tiles = new Tile[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _tiles[x, y] = Tile.Empty;
                }
            }
        }

        private Board(Board source)
        {
            Width = source.Width;
            Height = source.Height;
            _tiles = (Tile[,])source._tiles.Clone();
        }

        public Tile this[int x, int y]
        {
            get => _tiles[x, y];
            set => _tiles[x, y] = value;
        }

        public bool Contains(int x, int y)
        {
            return (uint)x < (uint)Width && (uint)y < (uint)Height;
        }

        public Board Clone()
        {
            return new Board(this);
        }

        public void Swap(int fromX, int fromY, int toX, int toY)
        {
            (_tiles[fromX, fromY], _tiles[toX, toY]) = (_tiles[toX, toY], _tiles[fromX, fromY]);
        }
    }
}
