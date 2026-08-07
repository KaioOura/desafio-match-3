using System.Collections.Generic;
using Gazeus.DesafioMatch3.Core;
using Gazeus.DesafioMatch3.Models;
using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects.Matching
{
    // Recognizing a new shape means inheriting from here and creating an asset.
    public abstract class MatchDetector : ScriptableObject
    {
        public abstract void Detect(BoardScan scan, List<Match> matches);
    }
}
