using Linyx.Services.Audio;
using UnityEngine;

namespace Linyx.Models.Phyllotaxis
{
    public interface IPhyllotaxisProperty
    {
        //Setup
        float Degree { get; set; }
        float Scale { get; set; }
        int NumberStart { get; set; }
        int StepSize { get; set; }
        int MaxIterations { get; set; }

        //Lerp Postions
        bool UseLerping { get; set; }
        AudioFrequencyType LerpFrequencyType { get; set; }
        int LerpAudioBand { get; set; }
        Vector2 SpeedMinMax { get; set; }
        AnimationCurve LerpInterpolationCurve { get; set; }
        bool Repeat { get; set; }
        bool Invert { get; set; }


        //Scale
        bool UseScaling { get; set; }
        AudioFrequencyType ScaleFrequencyType { get; set; }
        int ScaleAudioBand { get; set; }
        Vector2 ScaleMinMax { get; set; }
        bool UseScaleCurve { get; set; }
        AnimationCurve ScaleInterpolationCurve { get; set; }
        float InterpolationSpeed { get; set; }



        IPhyllotaxisProperty DeepCopy();
    }
}
