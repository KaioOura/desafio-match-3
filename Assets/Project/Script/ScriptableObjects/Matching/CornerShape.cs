using System;

namespace Gazeus.DesafioMatch3.ScriptableObjects.Matching
{
    // How two runs of the same type cross, told apart by where the shared tile sits.
    [Flags]
    public enum CornerShape
    {
        None = 0,

        // End of both runs.
        L = 1,

        // End of one run, middle of the other.
        T = 2,

        // Middle of both runs.
        Plus = 4,
    }
}
