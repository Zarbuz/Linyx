using Linyx.Services.Audio;
using UnityEngine;

namespace Linyx.Models.Phyllotaxis
{
    public class PhyllotaxisProperty : IPhyllotaxisProperty
    {
        public float Degree { get; set; }
        public float Scale { get; set; }
        public int NumberStart { get; set; }
        public int StepSize { get; set; }
        public int MaxIterations { get; set; }
        public bool UseLerping { get; set; }
        public AudioFrequencyType LerpFrequencyType { get; set; }
        public int LerpAudioBand { get; set; }
        public Vector2 SpeedMinMax { get; set; }
        public AnimationCurve LerpInterpolationCurve { get; set; }
        public bool Repeat { get; set; }
        public bool Invert { get; set; }
        public bool UseScaling { get; set; }
        public AudioFrequencyType ScaleFrequencyType { get; set; }
        public int ScaleAudioBand { get; set; }
        public Vector2 ScaleMinMax { get; set; }
        public bool UseScaleCurve { get; set; }
        public AnimationCurve ScaleInterpolationCurve { get; set; }
        public float InterpolationSpeed { get; set; }
        public IPhyllotaxisProperty DeepCopy()
        {
            return new PhyllotaxisProperty
            {
                Degree = Degree,
                Scale = Scale,
                NumberStart = NumberStart,
                StepSize = StepSize,
                MaxIterations = MaxIterations,
                UseLerping = UseLerping,
                LerpFrequencyType = LerpFrequencyType,
                LerpAudioBand = LerpAudioBand,
                SpeedMinMax = SpeedMinMax,
                LerpInterpolationCurve = LerpInterpolationCurve,
                Repeat = Repeat,
                Invert = Invert,
                UseScaling = UseScaling,
                ScaleFrequencyType = ScaleFrequencyType,
                ScaleAudioBand = ScaleAudioBand,
                ScaleMinMax = ScaleMinMax,
                UseScaleCurve = UseScaleCurve,
                ScaleInterpolationCurve = ScaleInterpolationCurve,
                InterpolationSpeed = InterpolationSpeed,

            };
        }
    }
}
