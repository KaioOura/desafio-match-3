namespace Gazeus.DesafioMatch3.Models
{
    public class Tile
    {
        public static readonly Tile Empty = new Tile(-1);

        public int Type { get; }

        public Tile(int type)
        {
            Type = type;
        }

        public bool IsEmpty => Type < 0;
    }
}
