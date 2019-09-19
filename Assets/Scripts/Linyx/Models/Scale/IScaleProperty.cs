using Linyx.Services.Audio;

namespace Linyx.Models.Scale
{
    public interface IScaleProperty
    {
        /// Scale React
        bool IsScaleReactOnAudio { get; set; }
        int ScaleBandBuffer { get; set; }
        float ScaleMultiplier { get; set; }
        float ScaleThreshold { get; set; }
        AudioFrequencyType ScaleFrequencyType { get; set; }

        IScaleProperty DeepCopy();
    }
}
