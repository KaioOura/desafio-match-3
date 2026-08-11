using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects.Feedback
{
    [CreateAssetMenu(fileName = "EffectCue", menuName = "Feedback/EffectCue")]
    public class EffectCue : ScriptableObject
    {
        [Tooltip("Optional. Leave empty for a cue that only plays a sound.")]
        [SerializeField] private ParticleSystem _particles;
        [Tooltip("Optional. Leave empty for a cue that only spawns particles.")]
        [SerializeField] private SoundCue _sound;

        public ParticleSystem Particles => _particles;
        public SoundCue Sound => _sound;
    }
}
