using System.Collections.Generic;
using Gazeus.DesafioMatch3.Core;
using Gazeus.DesafioMatch3.Models;
using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects.Matching
{
    [CreateAssetMenu(fileName = "LineMatchDetector", menuName = "Gameplay/Matching/Line")]
    public class LineMatchDetector : MatchDetector
    {
        [Tooltip("How many tiles a run needs for this detector to recognize it.")]
        [SerializeField] private int _minimumTiles = 3;

        public override void Detect(BoardScan scan, List<Match> matches)
        {
            // Runs come ready from the scan: filtering by size allocates nothing.
            for (int i = 0; i < scan.Runs.Count; i++)
            {
                if (scan.Runs[i].Size >= _minimumTiles) matches.Add(scan.Runs[i]);
            }
        }
    }
}
