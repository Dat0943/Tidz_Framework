using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Tidz
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        public static LValue<float> volumeMusic = new LValue<float>(1.0f);
        public static LValue<float> volumeSound = new LValue<float>(1.0f);

        private IObjectPool<AudioScript> _pool;
        private static readonly Dictionary<int, AudioScript> _single = new Dictionary<int, AudioScript>(2);

        protected override void Awake()
        {
            base.Awake();
            InitPool();
        }

        protected override void OnDestroy()
        {
            if (_pool is ObjectPool<AudioScript> pool)
                pool.Dispose();
        }

        public static AudioScript Play(AudioConfig config, bool loop = false, Transform pos = null, bool followParent = true)
        {
            if (config == null || config.clip == null || Instance == null)
                return null;

            var audio = Instance._pool.Get();
            Instance.SetupAudioTransform(audio, config, pos, followParent);
            audio.Play(config, loop);
            return audio;
        }

        public static AudioScript PlaySingle(AudioConfig config, bool loop = false, int channel = 0, Transform pos = null, bool followParent = false)
        {
            if (config == null || config.clip == null || Instance == null)
                return null;

            if (_single.TryGetValue(channel, out var existing) && existing != null)
            {
                existing.Stop();
                _single[channel] = null;
            }

            var audio = Instance._pool.Get();
            Instance.SetupAudioTransform(audio, config, pos, followParent);

            audio.Play(config, loop);
            _single[channel] = audio;

            return audio;
        }

        public static void StopSingleAudio()
        {
            StopSingleAudio(0);
        }

        public static void StopSingleAudio(int channel)
        {
            if (_single.TryGetValue(channel, out var audio) && audio != null)
            {
                audio.Stop();
                _single[channel] = null;
            }
        }

        public static void StopAllSingles()
        {
            if (_single.Count == 0)
                return;

            var keys = new List<int>(_single.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                int key = keys[i];
                if (_single.TryGetValue(key, out var audio) && audio != null)
                {
                    audio.Stop();
                    _single[key] = null;
                }
            }
        }

        public static void ReturnPool(AudioScript audioScript)
        {
            if (audioScript == null || Instance == null)
                return;

            int? foundChannel = null;
            foreach (var pair in _single)
            {
                if (ReferenceEquals(pair.Value, audioScript))
                {
                    foundChannel = pair.Key;
                    break;
                }
            }

            if (foundChannel.HasValue)
                _single[foundChannel.Value] = null;

            Instance.ResetAudioTransform(audioScript);
            Instance._pool.Release(audioScript);
        }

        private void SetupAudioTransform(AudioScript audio, AudioConfig config, Transform pos, bool followParent)
        {
            if (audio == null)
                return;

            Transform t = audio.transform;

            if (!config.is3D)
            {
                t.SetParent(transform, false);
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                return;
            }

            if (followParent && pos != null)
            {
                t.SetParent(pos, false);
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
            }
            else
            {
                t.SetParent(transform, false);
                t.position = pos != null ? pos.position : transform.position;
                t.rotation = Quaternion.identity;
            }
        }

        private void ResetAudioTransform(AudioScript audio)
        {
            if (audio == null)
                return;

            Transform t = audio.transform;
            t.SetParent(transform, false);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
        }

        private void InitPool()
        {
            const int defaultCapacity = 8;
            const int maxSize = 64;

            _pool = new ObjectPool<AudioScript>(
                createFunc: () =>
                {
                    var go = new GameObject(nameof(AudioScript), typeof(AudioSource));
                    go.transform.SetParent(transform, false);

                    var audio = go.AddComponent<AudioScript>();
                    go.SetActive(false);

                    return audio;
                },
                actionOnGet: audio =>
                {
                    if (audio == null) return;

                    audio.transform.SetParent(transform, false);
                    audio.transform.localPosition = Vector3.zero;
                    audio.transform.localRotation = Quaternion.identity;
                    audio.gameObject.SetActive(true);
                },
                actionOnRelease: audio =>
                {
                    if (audio == null) return;

                    audio.transform.SetParent(transform, false);
                    audio.transform.localPosition = Vector3.zero;
                    audio.transform.localRotation = Quaternion.identity;
                    audio.gameObject.SetActive(false);
                },
                actionOnDestroy: audio =>
                {
                    if (audio != null)
                        Destroy(audio.gameObject);
                },
                collectionCheck: false,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }
    }
}