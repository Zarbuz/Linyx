using Linyx.Services.Audio;

namespace Linyx.Models.Emission
{
    public class EmissionProperty : IEmissionProperty
    {
        public bool IsEmissionReactOnAudio { get; set; }
        public int EmissionBandBuffer { get; set; }
        public float EmissionThreshold { get; set; }
        public AudioFrequencyType EmissionFrequencyType { get; set; }
        public IEmissionProperty DeepCopy()
        {
            return new EmissionProperty
            {
                IsEmissionReactOnAudio = IsEmissionReactOnAudio,
                EmissionBandBuffer = EmissionBandBuffer,
                EmissionThreshold = EmissionThreshold,
                EmissionFrequencyType = EmissionFrequencyType
            };
        }
    }
}
