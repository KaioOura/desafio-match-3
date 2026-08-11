using UnityEngine;

namespace Gazeus.DesafioMatch3.ScriptableObjects.Feedback
{
    [CreateAssetMenu(fileName = "SoundCue", menuName = "Feedback/SoundCue")]
    public class SoundCue : ScriptableObject
    {
        [Tooltip("Picked at random on every play, so repeated events do not sound identical.")]
        [SerializeField] private AudioClip[] _clips;
        [Range(0f, 1f)]
        [SerializeField] private float _volume = 1f;
        [Tooltip("Random pitch range. Keep both values at 1 to play the clip untouched.")]
        [SerializeField] private Vector2 _pitchRange = Vector2.one;

        public float Volume => _volume;

        public AudioClip PickClip()
        {
            if (_clips == null || _clips.Length == 0) return null;

            return _clips[Random.Range(0, _clips.Length)];
        }

        public float PickPitch()
        {
            return Random.Range(_pitchRange.x, _pitchRange.y);
        }
    }
}
