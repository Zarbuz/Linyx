using Linyx.Services.Audio;

namespace Linyx.Models.Emission
{
    public interface IEmissionProperty
    {
        /// Emission React
        bool IsEmissionReactOnAudio { get; set; }
        int EmissionBandBuffer { get; set; }
        float EmissionThreshold { get; set; }
        AudioFrequencyType EmissionFrequencyType { get; set; }

        IEmissionProperty DeepCopy();
    }
}
