using System;
using Gazeus.DesafioMatch3.ScriptableObjects.Effects;
using Gazeus.DesafioMatch3.ScriptableObjects.Matching;
using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects
{
    [Serializable]
    public class MatchRule
    {
        [SerializeField] private MatchDetector _detector;

        [Tooltip("Empty: the match only removes its own tiles, with no special mechanic.")]
        [SerializeField] private MatchEffect _effect;

        public MatchDetector Detector => _detector;
        public MatchEffect Effect => _effect;
    }
}
