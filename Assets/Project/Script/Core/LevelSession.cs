using Gazeus.DesafioMatch3.ScriptableObjects;

namespace Gazeus.DesafioMatch3.Core
{
    public static class LevelSession
    {
        public static LevelConfig Current { get; private set; }

        public static void Select(LevelConfig level)
        {
            Current = level;
        }
    }
}
