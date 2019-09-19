using Linyx.Models.Emission;
using Linyx.Models.Koch;
using Linyx.Models.Phyllotaxis;
using Linyx.Models.Save;
using Linyx.Models.Scale;
using Linyx.Services.Audio;
using Linyx.Services.Brush;
using UnityEngine;

namespace Linyx.Models.Line
{
    public sealed class LineModel : ILineModel
    {
        public string Guid { get; set; }
        public string DisplayName { get; set; }
        public int Layer { get; set; }
        public Shape Shape { get; set; }
        public Gradient Gradient { get; set; }
        public AnimationCurve WidthCurve { get; set; }
        public bool IsEmissionEnabled { get; set; }
        public float EmissionIntensity { get; set; }
        public Color EmissionColor { get; set; }
        public LineRenderer LineGameObject { get; set; }
        public IEmissionProperty EmissionProperty { get; set; }
        public IScaleProperty ScaleProperty { get; set; }
        public IKochLineProperty KochLineProperty { get; set; }
        public IKochTrailProperty KochTrailProperty { get; set; }
        public IPhyllotaxisProperty PhyllotaxisProperty { get; set; }

        public LineModel DeepCopy()
        {
            LineModel lineModel = new LineModel
            {
                Guid = Guid,
                DisplayName = DisplayName,
                LineGameObject = LineGameObject,
                Layer = Layer,
                Shape = Shape,
                Gradient = Gradient,
                WidthCurve = WidthCurve,
                IsEmissionEnabled = IsEmissionEnabled,
                EmissionIntensity = EmissionIntensity,
                EmissionColor = EmissionColor,

                EmissionProperty = EmissionProperty.DeepCopy(),
                ScaleProperty = ScaleProperty.DeepCopy(),
                KochLineProperty = KochLineProperty.DeepCopy(),
                KochTrailProperty = KochTrailProperty.DeepCopy(),
                PhyllotaxisProperty = PhyllotaxisProperty.DeepCopy()
            };

            return lineModel;
        }

        public static explicit operator SaveLineModel(LineModel line)
        {
            SaveLineModel saveLineModel = new SaveLineModel()
            {
                Guid = line.Guid,
                DisplayName = line.DisplayName,
                Layer = line.Layer,
                Shape = line.Shape,
                GradientAlphaSave = new GradientAlphaSave(),
                GradientColorSave = new GradientColorSave(),
                WidthCurve = new AnimationCurveSave(),

                IsEmissionEnabled = line.IsEmissionEnabled,
                EmissionIntensity = line.EmissionIntensity,
                EmissionColor = line.EmissionColor,

                IsEmissionReactOnAudio = line.EmissionProperty.IsEmissionReactOnAudio,
                EmissionBandBuffer = line.EmissionProperty.EmissionBandBuffer,
                EmissionThreshold = line.EmissionProperty.EmissionThreshold,
                EmissionFrequencyType = line.EmissionProperty.EmissionFrequencyType,

                IsScaleReactOnAudio = line.ScaleProperty.IsScaleReactOnAudio,
                ScaleBandBuffer = line.ScaleProperty.ScaleBandBuffer,
                ScaleMultiplier = line.ScaleProperty.ScaleMultiplier,
                ScaleThreshold = line.ScaleProperty.ScaleThreshold,
                ScaleFrequencyType = line.ScaleProperty.ScaleFrequencyType,

                IsKochEnabled = line.KochLineProperty.IsKochEnabled,
                ShapePointAmount = line.KochLineProperty.ShapePointAmount,
                ListStartGeneration = line.KochLineProperty.ListStartGeneration,
                AnimationCurve = new AnimationCurveSave(),
                UseBezierCurves = line.KochLineProperty.UseBezierCurves,
                BezierVertexCount = line.KochLineProperty.BezierVertexCount,
                KochAudioBand = line.KochLineProperty.KochAudioBand,
                OriginalPositions = line.KochLineProperty.OriginalPositions,

                IsTrailEnabled = line.KochTrailProperty.IsTrailEnabled,
                TrailSpeedMinMax = line.KochTrailProperty.TrailSpeedMinMax,
                TrailTimeMinMax = line.KochTrailProperty.TrailTimeMinMax,
                TrailWidthMinMax = line.KochTrailProperty.TrailWidthMinMax,

                Degree = line.PhyllotaxisProperty.Degree,
                InterpolationSpeed = line.PhyllotaxisProperty.InterpolationSpeed,
                Invert = line.PhyllotaxisProperty.Invert,
                LerpAudioBand = line.PhyllotaxisProperty.LerpAudioBand,
                LerpFrequencyType = line.PhyllotaxisProperty.LerpFrequencyType,
                LerpInterpolationCurve = new AnimationCurveSave(),
                MaxIterations = line.PhyllotaxisProperty.MaxIterations,
                NumberStart = line.PhyllotaxisProperty.NumberStart,
                Repeat = line.PhyllotaxisProperty.Repeat,
                Scale = line.PhyllotaxisProperty.Scale,
                ScalePhylloFrequencyType = line.PhyllotaxisProperty.ScaleFrequencyType,
                ScaleAudioBand = line.PhyllotaxisProperty.ScaleAudioBand,
                ScaleInterpolationCurve = new AnimationCurveSave(),
                ScaleMinMax = line.PhyllotaxisProperty.ScaleMinMax,
                SpeedMinMax = line.PhyllotaxisProperty.SpeedMinMax,
                StepSize = line.PhyllotaxisProperty.StepSize,
                UseScaleCurve = line.PhyllotaxisProperty.UseScaleCurve,
                UseLerping = line.PhyllotaxisProperty.UseLerping,
                UseScaling = line.PhyllotaxisProperty.UseScaling,

            };

            saveLineModel.GradientAlphaSave.AlphaTimes = new float[line.Gradient.alphaKeys.Length];
            saveLineModel.GradientAlphaSave.AlphaValues = new float[line.Gradient.alphaKeys.Length];

            saveLineModel.GradientColorSave.ColorTimes = new float[line.Gradient.colorKeys.Length];
            saveLineModel.GradientColorSave.ColorValues = new Color[line.Gradient.colorKeys.Length];

            for (int i = 0; i < line.Gradient.alphaKeys.Length; i++)
            {
                GradientAlphaKey gradientAlphaKey = line.Gradient.alphaKeys[i];
                saveLineModel.GradientAlphaSave.AlphaTimes[i] = gradientAlphaKey.time;
                saveLineModel.GradientAlphaSave.AlphaValues[i] = gradientAlphaKey.alpha;
            }

            for (int i = 0; i < line.Gradient.colorKeys.Length; i++)
            {
                GradientColorKey gradientColorKey = line.Gradient.colorKeys[i];
                saveLineModel.GradientColorSave.ColorTimes[i] = gradientColorKey.time;
                saveLineModel.GradientColorSave.ColorValues[i] = gradientColorKey.color;
            }

            saveLineModel.AnimationCurve.KeyFrame = new KeyFrameSave[line.KochLineProperty.AnimationCurve.length];

            for (int i = 0; i < line.KochLineProperty.AnimationCurve.length; i++)
            {
                saveLineModel.AnimationCurve.KeyFrame[i] = new KeyFrameSave
                {
                    Value = line.KochLineProperty.AnimationCurve.keys[i].value,
                    Time = line.KochLineProperty.AnimationCurve.keys[i].time,
                    WeightedMode = line.KochLineProperty.AnimationCurve.keys[i].weightedMode,
                    OutWeight = line.KochLineProperty.AnimationCurve.keys[i].outWeight,
                    OutTangent = line.KochLineProperty.AnimationCurve.keys[i].outTangent,
                    InTangent = line.KochLineProperty.AnimationCurve.keys[i].inTangent,
                    InWeight = line.KochLineProperty.AnimationCurve.keys[i].inWeight
                };
            }

            saveLineModel.WidthCurve.KeyFrame = new KeyFrameSave[line.WidthCurve.length];

            for (int i = 0; i < line.WidthCurve.length; i++)
            {
                saveLineModel.WidthCurve.KeyFrame[i] = new KeyFrameSave
                {
                    Value = line.WidthCurve.keys[i].value,
                    Time = line.WidthCurve.keys[i].time,
                    WeightedMode = line.WidthCurve.keys[i].weightedMode,
                    OutWeight = line.WidthCurve.keys[i].outWeight,
                    OutTangent = line.WidthCurve.keys[i].outTangent,
                    InTangent = line.WidthCurve.keys[i].inTangent,
                    InWeight = line.WidthCurve.keys[i].inWeight
                };
            }

            saveLineModel.LerpInterpolationCurve.KeyFrame = new KeyFrameSave[line.PhyllotaxisProperty.LerpInterpolationCurve.length];

            for (int i = 0; i < line.PhyllotaxisProperty.LerpInterpolationCurve.length; i++)
            {
                saveLineModel.LerpInterpolationCurve.KeyFrame[i] = new KeyFrameSave
                {
                    Value = line.PhyllotaxisProperty.LerpInterpolationCurve.keys[i].value,
                    Time = line.PhyllotaxisProperty.LerpInterpolationCurve.keys[i].time,
                    WeightedMode = line.PhyllotaxisProperty.LerpInterpolationCurve.keys[i].weightedMode,
                    OutWeight = line.PhyllotaxisProperty.LerpInterpolationCurve.keys[i].outWeight,
                    OutTangent = line.PhyllotaxisProperty.LerpInterpolationCurve.keys[i].outTangent,
                    InTangent = line.PhyllotaxisProperty.LerpInterpolationCurve.keys[i].inTangent,
                    InWeight = line.PhyllotaxisProperty.LerpInterpolationCurve.keys[i].inWeight
                };
            }

            saveLineModel.ScaleInterpolationCurve.KeyFrame = new KeyFrameSave[line.PhyllotaxisProperty.ScaleInterpolationCurve.length];

            for (int i = 0; i < line.PhyllotaxisProperty.ScaleInterpolationCurve.length; i++)
            {
                saveLineModel.ScaleInterpolationCurve.KeyFrame[i] = new KeyFrameSave
                {
                    Value = line.PhyllotaxisProperty.ScaleInterpolationCurve.keys[i].value,
                    Time = line.PhyllotaxisProperty.ScaleInterpolationCurve.keys[i].time,
                    WeightedMode = line.PhyllotaxisProperty.ScaleInterpolationCurve.keys[i].weightedMode,
                    OutWeight = line.PhyllotaxisProperty.ScaleInterpolationCurve.keys[i].outWeight,
                    OutTangent = line.PhyllotaxisProperty.ScaleInterpolationCurve.keys[i].outTangent,
                    InTangent = line.PhyllotaxisProperty.ScaleInterpolationCurve.keys[i].inTangent,
                    InWeight = line.PhyllotaxisProperty.ScaleInterpolationCurve.keys[i].inWeight
                };
            }

            return saveLineModel;
        }
    }
}
