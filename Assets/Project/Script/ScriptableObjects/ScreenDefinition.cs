using Gazeus.DesafioMatch3.Models;
using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ScreenDefinition", menuName = "UI/ScreenDefinition")]
    public class ScreenDefinition : ScriptableObject
    {
        [SerializeField] private ScreenLayer _layer;
        [Tooltip("Adds a full screen raycast target behind the screen content.")]
        [SerializeField] private bool _blocksInput;
        [SerializeField] private Color _blockerColor = new(0f, 0f, 0f, 0.72f);

        public ScreenLayer Layer => _layer;
        public bool BlocksInput => _blocksInput;
        public Color BlockerColor => _blockerColor;
    }
}
