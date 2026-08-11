using Gazeus.DesafioMatch3.ScriptableObjects.Feedback;
using UnityEngine;

namespace Gazeus.DesafioMatch3.Core.Audio
{
    public class AudioService : PersistentSingleton<AudioService>
    {
        private const int SourceCount = 8;

        private AudioSource[] _sources;
        private int _next;

        #region Unity
        protected override void Awake()
        {
            base.Awake();

            if (Instance != this) return;

            _sources = new AudioSource[SourceCount];
            for (int i = 0; i < SourceCount; i++)
            {
                _sources[i] = gameObject.AddComponent<AudioSource>();
                _sources[i].playOnAwake = false;
            }
        }
        #endregion

        public void Play(SoundCue cue, float pitchScale = 1f)
        {
            if (cue == null) return;

            AudioClip clip = cue.PickClip();
            if (clip == null) return;

            AudioSource source = _sources[_next];
            _next = (_next + 1) % _sources.Length;

            source.pitch = cue.PickPitch() * pitchScale;
            source.PlayOneShot(clip, cue.Volume);
        }
    }
}
