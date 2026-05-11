using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Simonshouse.UI
{
    /// <summary>
    /// Audio global (DontDestroyOnLoad): ambiente por escena y efectos one-shot.
    /// Los clips se asignan en postproducción; si faltan, las llamadas no hacen nada.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Sources")]
        [SerializeField] private AudioSource sourceAmbient;
        [SerializeField] private AudioSource sourceEffect;

        [Header("Música / Ambiente (postproducción)")]
        [SerializeField] private AudioClip clipMainMenu;
        [SerializeField] private AudioClip clipChapter1;
        [SerializeField] private AudioClip clipChapter2;
        [SerializeField] private AudioClip clipChapter3;
        [SerializeField] private AudioClip clipChapter4;
        [SerializeField] private AudioClip clipChapter5;
        [SerializeField] private AudioClip clipDeath;
        [SerializeField] private AudioClip clipEndings;
        [SerializeField] private AudioClip clipPostCredits;

        [Header("Efectos (postproducción)")]
        [SerializeField] private AudioClip clipGlitch;
        [SerializeField] private AudioClip clipBeep;
        [SerializeField] private AudioClip clipPageTurn;
        [SerializeField] private AudioClip clipDoorCreak;

        private float _ambientVolumeDefault = 1f;
        private Coroutine _fadeCo;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            EnsureSources();

            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;

            PlayAmbient(SceneManager.GetActiveScene().name);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                Instance = null;
            }
        }

        private void EnsureSources()
        {
            if (sourceAmbient == null)
            {
                sourceAmbient = gameObject.AddComponent<AudioSource>();
                sourceAmbient.playOnAwake = false;
                sourceAmbient.loop = true;
            }

            if (sourceEffect == null)
            {
                sourceEffect = gameObject.AddComponent<AudioSource>();
                sourceEffect.playOnAwake = false;
                sourceEffect.loop = false;
            }

            _ambientVolumeDefault = sourceAmbient.volume > 0f ? sourceAmbient.volume : 1f;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
            => PlayAmbient(scene.name);

        public void PlayAmbient(string sceneName)
        {
            AudioClip clip = sceneName switch
            {
                "MainMenu"    => clipMainMenu,
                "Chapter1"    => clipChapter1,
                "Chapter2"    => clipChapter2,
                "Chapter3"    => clipChapter3,
                "Chapter4"    => clipChapter4,
                "Chapter5"    => clipChapter5,
                "DeathScreen" => clipDeath,
                "Endings"     => clipEndings,
                "PostCredits" => clipPostCredits,
                _             => null
            };

            if (clip == null || sourceAmbient == null)
                return;

            if (_fadeCo != null)
            {
                StopCoroutine(_fadeCo);
                _fadeCo = null;
            }

            sourceAmbient.volume = _ambientVolumeDefault;
            sourceAmbient.clip = clip;
            sourceAmbient.loop = true;
            sourceAmbient.Play();
        }

        public void PlayEffect(string effectName)
        {
            AudioClip clip = effectName switch
            {
                "glitch"    => clipGlitch,
                "beep"      => clipBeep,
                "pageTurn"  => clipPageTurn,
                "doorCreak" => clipDoorCreak,
                _           => null
            };

            if (clip == null || sourceEffect == null)
                return;

            sourceEffect.PlayOneShot(clip);
        }

        public void FadeOut(float duration)
        {
            if (_fadeCo != null)
                StopCoroutine(_fadeCo);
            _fadeCo = StartCoroutine(FadeOutRoutine(duration));
        }

        private IEnumerator FadeOutRoutine(float duration)
        {
            if (sourceAmbient == null)
                yield break;

            float start = sourceAmbient.volume;
            float t = 0f;
            duration = Mathf.Max(0.01f, duration);
            while (t < duration)
            {
                sourceAmbient.volume = Mathf.Lerp(start, 0f, t / duration);
                t += Time.unscaledDeltaTime;
                yield return null;
            }

            sourceAmbient.Stop();
            sourceAmbient.volume = _ambientVolumeDefault > 0f ? _ambientVolumeDefault : 1f;
            _fadeCo = null;
        }
    }
}
