using Linyx.Services.Project;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace Linyx.Services.Audio
{
    public sealed class AudioPeerService : MonoBehaviour, IAudioPeerService
    {
        #region Serialize Fields

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioListener _audioListener;
        [SerializeField] private float _audioProfile;
        [SerializeField] private int _samples = 512;
        [SerializeField] private int _frequencyBands = 8;
        [SerializeField] private FFTWindow _window = FFTWindow.Blackman;
        [SerializeField] private RawImage _waveformImage;

        #endregion

        #region Injections

        [Inject] public SongStartedSignal SongStartedSignal { get; set; }
        [Inject] public SongStoppedSignal SongStoppedSignal { get; set; }

        #endregion

        #region Private Attributes

        private float[] _samplesBuffer;
        private float[] _cached_samples;
        private float[] _cached_freqBands;
        private float[] _cached_freqBandBuffer;
        private float[] _cached_freqBufferDecrease;
        private float[] _cached_freqBandHighest;
        private float[] _cached_audioBand;
        private float[] _cached_audioBandBuffer;
        private float[] _waveform;

        private float _cached_amplitude;
        private float _cached_amplitudeBuffer;
        private float _cached_amplitudeHighest;
        

        #endregion

        public void Initialize()
        {
            _cached_samples = new float[_samples];
            _cached_freqBands = new float[_frequencyBands];
            _cached_freqBandBuffer = new float[_frequencyBands];
            _cached_freqBufferDecrease = new float[_frequencyBands];
            _cached_freqBandHighest = new float[_frequencyBands];
            _cached_audioBand = new float[_frequencyBands];
            _cached_audioBandBuffer = new float[_frequencyBands];
            InitialiseAudioProfile(_audioProfile);
            CreateTexture();
        }

        private void Update()
        {
            if (_audioSource.clip != null && _audioSource.isPlaying)
            {
                GenerateFrequencyBands();
                GenerateAudioBands();
                GenerateAmplitude();
            }
        }

        public bool HasClip()
        {
            return _audioSource.clip != null;
        }

        public float GetAmplitude()
        {
            return _cached_amplitude;
        }

        public float GetAmplitudeBuffer()
        {
            return _cached_amplitudeBuffer;
        }

        public float GetFrequencyBand(int i)
        {
            return _cached_freqBands[i];
        }

        public float GetFrequencyBandBuffer(int i)
        {
            return _cached_freqBandBuffer[i];
        }

        public float GetAudioBand(int i)
        {
            return _cached_audioBand[i];
        }

        public float GetAudioBandBuffer(int i)
        {
            return _cached_audioBandBuffer[i];
        }

        public void SetupAudioSource(AudioClip clip)
        {
            Texture2D texture = (Texture2D)_waveformImage.texture;

            float[] samples = new float[clip.samples];
            float[] waveform = new float[texture.width];
            clip.GetData(samples, 0);
            int packSize = (clip.samples / texture.width) + 1;
            int s = 0;

            for (int i = 0; i < clip.samples; i += packSize)
            {
                waveform[s] = Mathf.Abs(samples[i]);
                s++;
            }

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }

            for (int x = 0; x < waveform.Length; x++)
            {
                for (int y = 0; y <= waveform[x] * ((float)texture.height * .75f); y++)
                {
                    texture.SetPixel(x, (texture.height / 2) + y, Color.blue);
                    texture.SetPixel(x, (texture.height / 2) - y, Color.blue);
                }
            }

            texture.Apply();
            _waveformImage.texture = texture;
            _waveformImage.gameObject.SetActive(true);
        }

        public AudioClip GetAudioClip()
        {
            return _audioSource.clip;
        }

        public AudioListener GetAudioListener()
        {
            return _audioListener;
        }

        public void Stop()
        {
            _audioSource.Stop();
            SongStoppedSignal.Dispatch();
        }

        public void Play()
        {
            _audioSource.Play();
            SongStartedSignal.Dispatch();
        }

        private void CreateTexture()
        {
            RectTransform rt = _waveformImage.GetComponent<RectTransform>();
            int width = (int)rt.rect.size.x;
            int height = (int) rt.rect.size.y;

            _waveformImage.texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                name = _waveformImage.name,
                wrapMode = TextureWrapMode.Clamp
            };

        }

        private void InitialiseAudioProfile(float value)
        {
            for (int i = 0; i < _cached_freqBandHighest.Length; ++i)
            {
                _cached_freqBandHighest[i] = value;
            }
        }

        private void GenerateAmplitude()
        {
            _cached_amplitude = 0.0f;
            _cached_amplitudeBuffer = 0.0f;

            int length = Mathf.Min(_cached_audioBand.Length, _cached_audioBandBuffer.Length);
            for (int i = 0; i < length; ++i)
            {
                _cached_amplitude += _cached_audioBand[i];
                _cached_amplitudeBuffer += _cached_audioBandBuffer[i];
            }

            if (_cached_amplitude > _cached_amplitudeHighest)
            {
                _cached_amplitudeHighest = _cached_amplitude;
            }
            _cached_amplitude /= _cached_amplitudeHighest;
            _cached_amplitudeBuffer /= _cached_amplitudeHighest;
        }

        private void GenerateAudioBands()
        {
            for (int i = 0; i < _cached_freqBands.Length; ++i)
            {
                if (_cached_freqBands[i] > _cached_freqBandHighest[i])
                {
                    _cached_freqBandHighest[i] = _cached_freqBands[i];
                }
                _cached_audioBand[i] = _cached_freqBands[i] / _cached_freqBandHighest[i];
                _cached_audioBandBuffer[i] = _cached_freqBandBuffer[i] / _cached_freqBandHighest[i];
            }
        }

        private void GenerateFrequencyBands()
        {
            // 22050 / 512 = 43Hz per sample
            // 20 - 60
            // 60 - 250
            // 250 - 500
            // 500 - 2000
            // 2000 - 4000
            // 4000 - 6000
            // 6000 - 20000

            _audioSource.GetSpectrumData(_cached_samples, 0, _window);

            int count = 0;
            for (int i = 0; i < _frequencyBands; ++i)
            {
                int sampleCount = (int)Mathf.Pow(2, i) * 2;
                if (i == 7) sampleCount += 2;

                float average = 0.0f;
                for (int j = 0; j < sampleCount; ++j)
                {
                    average += _cached_samples[count] * (count + 1);
                    ++count;
                }
                average /= count;
                _cached_freqBands[i] = average;
            }

            ModulateFrequencyBands();
        }

        private void ModulateFrequencyBands()
        {
            for (int i = 0; i < _cached_freqBandBuffer.Length; ++i)
            {
                if (_cached_freqBands[i] > _cached_freqBandBuffer[i])
                {
                    _cached_freqBandBuffer[i] = _cached_freqBands[i];
                    _cached_freqBufferDecrease[i] = 0.005f;
                }

                if (_cached_freqBands[i] < _cached_freqBandBuffer[i])
                {
                    _cached_freqBandBuffer[i] -= _cached_freqBufferDecrease[i];
                    _cached_freqBufferDecrease[i] *= 1.2f;
                }
            }
        }
    }

    public sealed class SongStartedSignal : Signal
    {

    }

    public sealed class SongStoppedSignal : Signal
    {

    }
}
