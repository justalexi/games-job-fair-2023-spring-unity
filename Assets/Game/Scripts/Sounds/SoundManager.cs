using UnityEngine;

namespace Game.Sounds
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        // PlayerPrefs
        private static string SOUNDS_VOLUME_KEY = "SoundsVolume";

        [Header("Audio Clips")]
        [SerializeField]
        private AudioClip _jump;

        [SerializeField]
        private AudioClip _land;

        [SerializeField]
        private AudioClip _attack1;

        [SerializeField]
        private AudioClip _attack2;

        [SerializeField]
        private AudioClip _collect;

        [SerializeField]
        private AudioClip _death;

        [SerializeField]
        private AudioClip _enemyDeath;

        [SerializeField]
        private AudioClip _showPause;

        [SerializeField]
        private AudioClip _hidePause;

        [SerializeField]
        private AudioClip _showGameSuccess;

        [SerializeField]
        private AudioClip _showGameOver;

        [SerializeField]
        private AudioClip _click;

        [Header("Audio Sources")]
        [SerializeField]
        private AudioSource _musicAudioSource;

        [SerializeField]
        private AudioSource _soundsAudioSource;

        public AudioClip Jump => _jump;
        public AudioClip Land => _land;
        public AudioClip Attack1 => _attack1;
        public AudioClip Attack2 => _attack2;
        public AudioClip Collect => _collect;
        public AudioClip Death => _death;
        public AudioClip EnemyDeath => _enemyDeath;
        public AudioClip ShowPause => _showPause;
        public AudioClip HidePause => _hidePause;
        public AudioClip ShowGameSuccess => _showGameSuccess;
        public AudioClip ShowGameOver => _showGameOver;
        public AudioClip Click => _click;

        public bool IsSoundEnabled
        {
            get
            {
                if (PlayerPrefs.HasKey(SOUNDS_VOLUME_KEY))
                {
                    var soundsVolume = PlayerPrefs.GetFloat(SOUNDS_VOLUME_KEY);
                    if (soundsVolume < 0.1f)
                        return false;
                }

                return true;
            }
        }


        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        // PlayerPrefs
        private void Start()
        {
            if (PlayerPrefs.HasKey(SOUNDS_VOLUME_KEY))
            {
                var soundsVolume = PlayerPrefs.GetFloat(SOUNDS_VOLUME_KEY);
                _musicAudioSource.volume = soundsVolume;
                _soundsAudioSource.volume = soundsVolume;
            }

            _musicAudioSource.Play();
        }

        public void PlaySound(AudioClip audioClip)
        {
            if (audioClip != null)
            {
                _soundsAudioSource.PlayOneShot(audioClip);
            }
        }

        public void PlayClick()
        {
            if (_click != null)
            {
                _soundsAudioSource.PlayOneShot(_click);
            }
        }

        public void ToggleSounds()
        {
            _musicAudioSource.volume = _musicAudioSource.volume > 0f ? 0f : 1f;
            _soundsAudioSource.volume = _soundsAudioSource.volume > 0f ? 0f : 1f;

            PlayerPrefs.SetFloat(SOUNDS_VOLUME_KEY, _soundsAudioSource.volume);
        }
    }
}