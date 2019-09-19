using System.Collections.Generic;
using Linyx.Models.Emission;
using Linyx.Models.Koch;
using Linyx.Models.Line;
using Linyx.Models.Phyllotaxis;
using Linyx.Models.Scale;
using Linyx.Services.Audio;
using Linyx.Services.Brush;
using OPS.Serialization.Attributes;
using UnityEngine;

namespace Linyx.Models.Save
{
    [SerializeAbleClass]
    public class SaveLineModel
    {
        [SerializeAbleField(0)] public string Guid;
        [SerializeAbleField(1)] public string DisplayName;
        [SerializeAbleField(2)] public int Layer;
        [SerializeAbleField(3)] public Shape Shape;
        [SerializeAbleField(4)] public GradientAlphaSave GradientAlphaSave;
        [SerializeAbleField(5)] public GradientColorSave GradientColorSave;
        [SerializeAbleField(6)] public AnimationCurveSave WidthCurve;
        [SerializeAbleField(8)] public bool IsEmissionEnabled;
        [SerializeAbleField(9)] public float EmissionIntensity;
        [SerializeAbleField(10)] public Color EmissionColor;

        [SerializeAbleField(11)] public bool IsEmissionReactOnAudio;
        [SerializeAbleField(12)] public int EmissionBandBuffer;
        [SerializeAbleField(13)] public float EmissionThreshold;
        [SerializeAbleField(14)] public AudioFrequencyType EmissionFrequencyType;

        [SerializeAbleField(15)] public bool IsScaleReactOnAudio;
        [SerializeAbleField(16)] public int ScaleBandBuffer;
        [SerializeAbleField(17)] public float ScaleMultiplier;
        [SerializeAbleField(18)] public float ScaleThreshold;
        [SerializeAbleField(19)] public AudioFrequencyType ScaleFrequencyType;

        [SerializeAbleField(20)] public bool IsKochEnabled;
        [SerializeAbleField(21)] public int ShapePointAmount;
        [SerializeAbleField(22)] public List<StartGen> ListStartGeneration;
        [SerializeAbleField(23)] public AnimationCurveSave AnimationCurve;
        [SerializeAbleField(24)] public bool UseBezierCurves;
        [SerializeAbleField(25)] public List<int> KochAudioBand;
        [SerializeAbleField(26)] public Vector3[] OriginalPositions;
        [SerializeAbleField(27)] public int BezierVertexCount;
        [SerializeAbleField(28)] public bool IsTrailEnabled;
        [SerializeAbleField(29)] public Vector2 TrailSpeedMinMax;
        [SerializeAbleField(30)] public Vector2 TrailWidthMinMax;
        [SerializeAbleField(31)] public Vector2 TrailTimeMinMax;
        [SerializeAbleField(32)] public float Degree;
        [SerializeAbleField(33)] public float Scale;
        [SerializeAbleField(34)] public int NumberStart;
        [SerializeAbleField(35)] public int StepSize;
        [SerializeAbleField(36)] public int MaxIterations;
        [SerializeAbleField(37)] public bool UseLerping;
        [SerializeAbleField(38)] public AudioFrequencyType LerpFrequencyType;
        [SerializeAbleField(39)] public int LerpAudioBand;
        [SerializeAbleField(40)] public Vector2 SpeedMinMax;
        [SerializeAbleField(41)] public AnimationCurveSave LerpInterpolationCurve;
        [SerializeAbleField(42)] public bool Repeat;
        [SerializeAbleField(43)] public bool Invert;
        [SerializeAbleField(44)] public bool UseScaling;
        [SerializeAbleField(45)] public AudioFrequencyType ScalePhylloFrequencyType;
        [SerializeAbleField(46)] public int ScaleAudioBand;
        [SerializeAbleField(47)] public Vector2 ScaleMinMax;
        [SerializeAbleField(48)] public bool UseScaleCurve;
        [SerializeAbleField(49)] public AnimationCurveSave ScaleInterpolationCurve;
        [SerializeAbleField(50)] public float InterpolationSpeed;


        public static explicit operator LineModel(SaveLineModel saveLine)
        {
            LineModel line = new LineModel
            {
                Guid = saveLine.Guid,
                DisplayName = saveLine.DisplayName,
                Layer = saveLine.Layer,
                Shape = saveLine.Shape,
                Gradient = new Gradient(),
                WidthCurve = new AnimationCurve(),
                IsEmissionEnabled = saveLine.IsEmissionEnabled,
                EmissionIntensity = saveLine.EmissionIntensity,
                EmissionColor = saveLine.EmissionColor,

                EmissionProperty = new EmissionProperty
                {
                    IsEmissionReactOnAudio = saveLine.IsEmissionReactOnAudio,
                    EmissionBandBuffer = saveLine.EmissionBandBuffer,
                    EmissionThreshold = saveLine.EmissionThreshold,
                    EmissionFrequencyType = saveLine.EmissionFrequencyType,
                },

                ScaleProperty = new ScaleProperty
                {
                    IsScaleReactOnAudio = saveLine.IsScaleReactOnAudio,
                    ScaleBandBuffer = saveLine.ScaleBandBuffer,
                    ScaleMultiplier = saveLine.ScaleMultiplier,
                    ScaleThreshold = saveLine.ScaleThreshold,
                    ScaleFrequencyType = saveLine.ScaleFrequencyType,
                },
                
                KochLineProperty = new KochLineProperty
                {
                    IsKochEnabled = saveLine.IsKochEnabled,
                    ShapePointAmount = saveLine.ShapePointAmount,
                    ListStartGeneration = saveLine.ListStartGeneration,
                    AnimationCurve = new AnimationCurve(),
                    UseBezierCurves = saveLine.UseBezierCurves,
                    KochAudioBand = saveLine.KochAudioBand,
                    OriginalPositions = saveLine.OriginalPositions,
                    BezierVertexCount = saveLine.BezierVertexCount,
                },

                KochTrailProperty = new KochTrailProperty
                {
                    IsTrailEnabled = saveLine.IsTrailEnabled,
                    TrailSpeedMinMax = saveLine.TrailSpeedMinMax,
                    TrailTimeMinMax = saveLine.TrailTimeMinMax,
                    TrailWidthMinMax = saveLine.TrailWidthMinMax
                },

                PhyllotaxisProperty = new PhyllotaxisProperty
                {
                    Degree = saveLine.Degree,
                    InterpolationSpeed = saveLine.InterpolationSpeed,
                    Invert = saveLine.Invert,
                    LerpAudioBand = saveLine.LerpAudioBand,
                    LerpFrequencyType = saveLine.LerpFrequencyType,
                    LerpInterpolationCurve = new AnimationCurve(),
                    MaxIterations = saveLine.MaxIterations,
                    NumberStart = saveLine.NumberStart,
                    Repeat = saveLine.Repeat,
                    Scale = saveLine.Scale,
                    ScaleFrequencyType = saveLine.ScalePhylloFrequencyType,
                    ScaleAudioBand = saveLine.ScaleAudioBand,
                    ScaleInterpolationCurve = new AnimationCurve(),
                    ScaleMinMax = saveLine.ScaleMinMax,
                    SpeedMinMax = saveLine.SpeedMinMax,
                    StepSize = saveLine.StepSize,
                    UseScaleCurve = saveLine.UseScaleCurve,
                    UseLerping = saveLine.UseLerping,
                    UseScaling = saveLine.UseScaling,
                    
                }
            };


            GradientAlphaKey[] gradientAlphaKeys = new GradientAlphaKey[saveLine.GradientAlphaSave.AlphaTimes.Length];
            GradientColorKey[] gradientColorKeys = new GradientColorKey[saveLine.GradientColorSave.ColorTimes.Length];

            for (int i = 0; i < saveLine.GradientAlphaSave.AlphaTimes.Length; i++)
            {
                float time = saveLine.GradientAlphaSave.AlphaTimes[i];
                float value = saveLine.GradientAlphaSave.AlphaValues[i];
                gradientAlphaKeys[i].time = time;
                gradientAlphaKeys[i].alpha = value;
            }

            for (int i = 0; i < saveLine.GradientColorSave.ColorTimes.Length; i++)
            {
                float time = saveLine.GradientColorSave.ColorTimes[i];
                Color value = saveLine.GradientColorSave.ColorValues[i];
                gradientColorKeys[i].time = time;
                gradientColorKeys[i].color = value;
            }

            line.Gradient.colorKeys = gradientColorKeys;
            line.Gradient.alphaKeys = gradientAlphaKeys;


            Keyframe[] curveKeys = new Keyframe[saveLine.AnimationCurve.KeyFrame.Length];
            for (int i = 0; i < saveLine.AnimationCurve.KeyFrame.Length; i++)
            {
                curveKeys[i].value = saveLine.AnimationCurve.KeyFrame[i].Value;
                curveKeys[i].inTangent = saveLine.AnimationCurve.KeyFrame[i].InTangent;
                curveKeys[i].inWeight = saveLine.AnimationCurve.KeyFrame[i].InWeight;
                curveKeys[i].outTangent = saveLine.AnimationCurve.KeyFrame[i].OutTangent;
                curveKeys[i].outWeight = saveLine.AnimationCurve.KeyFrame[i].OutWeight;
                curveKeys[i].time = saveLine.AnimationCurve.KeyFrame[i].Time;
                curveKeys[i].weightedMode = saveLine.AnimationCurve.KeyFrame[i].WeightedMode;
            }
            line.KochLineProperty.AnimationCurve.keys = curveKeys;


            Keyframe[] widthKeys = new Keyframe[saveLine.WidthCurve.KeyFrame.Length];
            for (int i = 0; i < saveLine.WidthCurve.KeyFrame.Length; i++)
            {
                widthKeys[i].value = saveLine.WidthCurve.KeyFrame[i].Value;
                widthKeys[i].inTangent = saveLine.WidthCurve.KeyFrame[i].InTangent;
                widthKeys[i].inWeight = saveLine.WidthCurve.KeyFrame[i].InWeight;
                widthKeys[i].outTangent = saveLine.WidthCurve.KeyFrame[i].OutTangent;
                widthKeys[i].outWeight = saveLine.WidthCurve.KeyFrame[i].OutWeight;
                widthKeys[i].time = saveLine.WidthCurve.KeyFrame[i].Time;
                widthKeys[i].weightedMode = saveLine.WidthCurve.KeyFrame[i].WeightedMode;
            }
            line.WidthCurve.keys = widthKeys;


            Keyframe[] lerpKeys = new Keyframe[saveLine.LerpInterpolationCurve.KeyFrame.Length];
            for (int i = 0; i < saveLine.LerpInterpolationCurve.KeyFrame.Length; i++)
            {
                lerpKeys[i].value = saveLine.LerpInterpolationCurve.KeyFrame[i].Value;
                lerpKeys[i].inTangent = saveLine.LerpInterpolationCurve.KeyFrame[i].InTangent;
                lerpKeys[i].inWeight = saveLine.LerpInterpolationCurve.KeyFrame[i].InWeight;
                lerpKeys[i].outTangent = saveLine.LerpInterpolationCurve.KeyFrame[i].OutTangent;
                lerpKeys[i].outWeight = saveLine.LerpInterpolationCurve.KeyFrame[i].OutWeight;
                lerpKeys[i].time = saveLine.LerpInterpolationCurve.KeyFrame[i].Time;
                lerpKeys[i].weightedMode = saveLine.LerpInterpolationCurve.KeyFrame[i].WeightedMode;
            }
            line.PhyllotaxisProperty.LerpInterpolationCurve.keys = lerpKeys;

            Keyframe[] scaleKeys = new Keyframe[saveLine.ScaleInterpolationCurve.KeyFrame.Length];
            for (int i = 0; i < saveLine.ScaleInterpolationCurve.KeyFrame.Length; i++)
            {
                scaleKeys[i].value = saveLine.ScaleInterpolationCurve.KeyFrame[i].Value;
                scaleKeys[i].inTangent = saveLine.ScaleInterpolationCurve.KeyFrame[i].InTangent;
                scaleKeys[i].inWeight = saveLine.ScaleInterpolationCurve.KeyFrame[i].InWeight;
                scaleKeys[i].outTangent = saveLine.ScaleInterpolationCurve.KeyFrame[i].OutTangent;
                scaleKeys[i].outWeight = saveLine.ScaleInterpolationCurve.KeyFrame[i].OutWeight;
                scaleKeys[i].time = saveLine.ScaleInterpolationCurve.KeyFrame[i].Time;
                scaleKeys[i].weightedMode = saveLine.ScaleInterpolationCurve.KeyFrame[i].WeightedMode;
            }
            line.PhyllotaxisProperty.ScaleInterpolationCurve.keys = scaleKeys;

            return line;
        }

    }

    [SerializeAbleClass]
    public sealed class GradientAlphaSave
    {
        [SerializeAbleField(0)]
        public float[] AlphaValues;

        [SerializeAbleField(1)]
        public float[] AlphaTimes;
    }

    [SerializeAbleClass]
    public sealed class GradientColorSave
    {
        [SerializeAbleField(0)]
        public Color[] ColorValues;

        [SerializeAbleField(1)]
        public float[] ColorTimes;
    }

    [SerializeAbleClass]
    public sealed class AnimationCurveSave
    {
        [SerializeAbleField(0)]
        public KeyFrameSave[] KeyFrame;
    }

    [SerializeAbleClass]
    public sealed class KeyFrameSave
    {
        [SerializeAbleField(0)]
        public float Value;

        [SerializeAbleField(1)]
        public float InTangent;

        [SerializeAbleField(2)]
        public float InWeight;

        [SerializeAbleField(3)]
        public float OutTangent;

        [SerializeAbleField(4)]
        public float OutWeight;

        [SerializeAbleField(5)]
        public float Time;

        [SerializeAbleField(6)]
        public WeightedMode WeightedMode;
    }
}
