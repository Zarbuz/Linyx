using UnityEngine;

namespace Linyx.Services.Audio
{
    public interface IAudioPeerService : IServiceBase
    {
        bool HasClip();
        float GetAmplitude();
        float GetAmplitudeBuffer();
        float GetFrequencyBand(int i);
        float GetFrequencyBandBuffer(int i);
        float GetAudioBand(int i);
        float GetAudioBandBuffer(int i);
        void SetupAudioSource(AudioClip clip);
        void Stop();
        void Play();

        AudioClip GetAudioClip();

        AudioListener GetAudioListener();
    }

    public enum AudioFrequencyType
    {
        Band,
        BandBuffer,
        Amplitude,
        AmplitudeBuffer,
        Frequency,
        FrequencyBuffer,
    }
}
