using Linyx.Services.Audio;

namespace Linyx.Models.Scale
{
    public class ScaleProperty : IScaleProperty
    {
        public bool IsScaleReactOnAudio { get; set; }
        public int ScaleBandBuffer { get; set; }
        public float ScaleMultiplier { get; set; }
        public float ScaleThreshold { get; set; }
        public AudioFrequencyType ScaleFrequencyType { get; set; }
        public IScaleProperty DeepCopy()
        {
            return new ScaleProperty
            {
                IsScaleReactOnAudio = IsScaleReactOnAudio,
                ScaleBandBuffer = ScaleBandBuffer,
                ScaleMultiplier = ScaleMultiplier,
                ScaleThreshold = ScaleThreshold,
                ScaleFrequencyType = ScaleFrequencyType
            };
        }
    }
}
