using Linyx.Models;
using System;
using System.Collections.Generic;
using Linyx.Models.Koch;
using Linyx.Models.Line;
using Linyx.Services.Audio;
using UnityEngine;

namespace Linyx.Services.Brush
{
    public sealed class BrushService : MonoBehaviour, IBrushService
    {
        //Set default values here
        #region Private Attributes

        //Common
        private AnimationCurve _brushWidthCurve = AnimationCurve.Constant(0, 1, 0.01f);
        private float _brushEmissionIntensity = 1;
        private Gradient _brushGradient = new Gradient();
        private Color _brushEmissionColor = Color.white;
        private bool _brushEmissionEnabled;
        private Shape _brushShape = Shape.Line;
        private float _brushInitialShapeSize = 1f;

        //Emission
        private bool _brushEmissionReactOnAudio;
        private int _brushEmissionBandBuffer;
        private float _brushEmissionThreshold = 0.1f;
        private AudioFrequencyType _brushEmissionFrequencyType = AudioFrequencyType.Band;

        // Scale
        private bool _brushScaleReactOnAudio;
        private int _brushScaleBandBuffer;
        private float _brushScaleMultiplier;
        private float _brushScaleThreshold = 0.1f;
        private AudioFrequencyType _brushScaleFrequencyType = AudioFrequencyType.Band;

        //Koch
        private bool _brushKochEnabled;
        private bool _brushUseBezierCurves;
        private int _brushBezierVertexCount = 8;
        private List<int> _brushKochAudioBand = new List<int>();
        private AnimationCurve _brushAnimationCurve = new AnimationCurve();
        private List<StartGen> _brushStartGeneration = new List<StartGen>();


        // Trails
        private bool _brushTrailEnabled;
        private Vector2 _brushTrailSpeedMinMax = Vector2.one;
        private Vector2 _brushTrailTimeMinMax = Vector2.one;
        private Vector2 _brushTrailWidthMinMax = new Vector2(0.1f, 0.1f);

        //Phyllotaxis
        private float _brushDegree;
        private float _brushScale;
        private int _brushNumberStart;
        private int _brushStepSize;
        private int _brushMaxIterations;
        private bool _brushUseLerping;
        private AudioFrequencyType _brushLerpFrequencyType = AudioFrequencyType.Band;
        private int _brushLerpAudioBand;
        private Vector2 _brushSpeedMinMax = Vector2.one;
        private AnimationCurve _brushLerpInterpolationCurve = AnimationCurve.Constant(0, 1, 1);
        private bool _brushRepeat;
        private bool _brushInvert;

        private bool _brushUseScaling;
        private AudioFrequencyType _brushScalePhylloFrequencyType = AudioFrequencyType.Band;
        private int _brushScaleAudioBand;
        private Vector2 _brushScaleMinMax = Vector2.one;
        private bool _brushUseScaleCurve;
        private AnimationCurve _brushScaleInterpolationCurve = AnimationCurve.Constant(0, 1, 1);
        private float _brushInterpolationSpeed;

        #endregion


        public void Initialize()
        {
            _brushAnimationCurve.AddKey(0f, 0);
            _brushAnimationCurve.AddKey(0.5f, 0.5f);
            _brushAnimationCurve.AddKey(1f, 0);
            _brushStartGeneration.Add(new StartGen()
            {
                Guid = Guid.NewGuid().ToString("N"),
                Scale = 1,
                Outwards = false
            });
        }

        public void SetBrushShape(Shape shape)
        {
            _brushShape = shape;
        }

        public void SetBrushShapeInitSize(float size)
        {
            _brushInitialShapeSize = size;
        }

        public void SetBrushWidthCurve(AnimationCurve curve)
        {
            _brushWidthCurve = curve;
        }

        public void SetBrushEmissionIntensity(float emission)
        {
            _brushEmissionIntensity = emission;
        }

        public void SetEmission(bool enabled)
        {
            _brushEmissionEnabled = enabled;
        }

        public void SetBrushGradient(Gradient gradient)
        {
            _brushGradient = gradient;
        }

        public void SetBrushEmissionColor(Color color)
        {
            _brushEmissionColor = color;
        }

        public void SetBrushFromCopy(LineModel lineModel)
        {
            _brushShape = lineModel.Shape;
            _brushGradient = lineModel.Gradient;
            _brushEmissionColor = lineModel.EmissionColor;
            _brushEmissionIntensity = lineModel.EmissionIntensity;
            _brushWidthCurve = lineModel.WidthCurve;
            _brushEmissionEnabled = lineModel.IsEmissionEnabled;

            //Properties still not display in brush view (only edit view)
            _brushEmissionReactOnAudio = lineModel.EmissionProperty.IsEmissionReactOnAudio;
            _brushEmissionBandBuffer = lineModel.EmissionProperty.EmissionBandBuffer;
            _brushEmissionThreshold = lineModel.EmissionProperty.EmissionThreshold;
            _brushEmissionFrequencyType = lineModel.EmissionProperty.EmissionFrequencyType;

            _brushScaleBandBuffer = lineModel.ScaleProperty.ScaleBandBuffer;
            _brushScaleMultiplier = lineModel.ScaleProperty.ScaleMultiplier;
            _brushScaleReactOnAudio = lineModel.ScaleProperty.IsScaleReactOnAudio;
            _brushScaleThreshold = lineModel.ScaleProperty.ScaleThreshold;
            _brushScaleFrequencyType = lineModel.ScaleProperty.ScaleFrequencyType;

            _brushKochEnabled = lineModel.KochLineProperty.IsKochEnabled;
            _brushAnimationCurve = lineModel.KochLineProperty.AnimationCurve;
            _brushBezierVertexCount = lineModel.KochLineProperty.BezierVertexCount;
            _brushKochAudioBand = lineModel.KochLineProperty.KochAudioBand;
            _brushTrailSpeedMinMax = lineModel.KochTrailProperty.TrailSpeedMinMax;
            _brushTrailTimeMinMax = lineModel.KochTrailProperty.TrailTimeMinMax;
            _brushTrailWidthMinMax = lineModel.KochTrailProperty.TrailWidthMinMax;
            _brushTrailEnabled = lineModel.KochTrailProperty.IsTrailEnabled;
            _brushUseBezierCurves = lineModel.KochLineProperty.UseBezierCurves;
            _brushStartGeneration = lineModel.KochLineProperty.ListStartGeneration;

            _brushDegree = lineModel.PhyllotaxisProperty.Degree;
            _brushScale = lineModel.PhyllotaxisProperty.Scale;
            _brushNumberStart = lineModel.PhyllotaxisProperty.NumberStart;
            _brushStepSize = lineModel.PhyllotaxisProperty.StepSize;
            _brushMaxIterations = lineModel.PhyllotaxisProperty.MaxIterations;
            _brushUseLerping = lineModel.PhyllotaxisProperty.UseLerping;
            _brushLerpFrequencyType = lineModel.PhyllotaxisProperty.LerpFrequencyType;
            _brushLerpAudioBand = lineModel.PhyllotaxisProperty.LerpAudioBand;
            _brushSpeedMinMax = lineModel.PhyllotaxisProperty.SpeedMinMax;
            _brushLerpInterpolationCurve = lineModel.PhyllotaxisProperty.LerpInterpolationCurve;
            _brushRepeat = lineModel.PhyllotaxisProperty.Repeat;
            _brushInvert = lineModel.PhyllotaxisProperty.Invert;

            _brushUseScaling = lineModel.PhyllotaxisProperty.UseScaling;
            _brushScalePhylloFrequencyType = lineModel.PhyllotaxisProperty.ScaleFrequencyType;
            _brushScaleAudioBand = lineModel.PhyllotaxisProperty.ScaleAudioBand;
            _brushScaleMinMax = lineModel.PhyllotaxisProperty.ScaleMinMax;
            _brushUseScaleCurve = lineModel.PhyllotaxisProperty.UseScaleCurve;
            _brushScaleInterpolationCurve = lineModel.PhyllotaxisProperty.ScaleInterpolationCurve;
            _brushInterpolationSpeed = lineModel.PhyllotaxisProperty.InterpolationSpeed;
        }

        public void ResetBrushSettings()
        {
            _brushAnimationCurve = new AnimationCurve();
            _brushAnimationCurve.AddKey(0f, 0);
            _brushAnimationCurve.AddKey(0.5f, 0.5f);
            _brushAnimationCurve.AddKey(1f, 0);

            _brushBezierVertexCount = 8;

            _brushEmissionReactOnAudio = false;
            _brushEmissionBandBuffer = 0;
            _brushEmissionEnabled = false;
            _brushEmissionThreshold = 0.1f;
            _brushEmissionFrequencyType = AudioFrequencyType.Band;

            _brushScaleReactOnAudio = false;
            _brushScaleBandBuffer = 0;
            _brushScaleMultiplier = 1;
            _brushScaleThreshold = 0.1f;
            _brushScaleFrequencyType = AudioFrequencyType.Band;

            _brushKochEnabled = false;
            _brushTrailSpeedMinMax = Vector2.one;
            _brushTrailTimeMinMax = Vector2.one;
            _brushTrailWidthMinMax = new Vector2(0.1f, 0.1f);
            _brushTrailEnabled = false;
            _brushUseBezierCurves = false;
            _brushStartGeneration = new List<StartGen>
            {
                new StartGen()
                {
                    Guid = Guid.NewGuid().ToString("N"),
                    Scale = 1,
                    Outwards = false
                }
            };


            _brushDegree = 0;
            _brushScale = 0;
            _brushNumberStart = 0;
            _brushStepSize = 0;
            _brushMaxIterations = 0;
            _brushUseLerping = false;
            _brushLerpFrequencyType = AudioFrequencyType.Band;
            _brushLerpAudioBand = 0;
            _brushSpeedMinMax = Vector2.one;
            _brushLerpInterpolationCurve = AnimationCurve.Constant(0, 1, 1);
            _brushRepeat = false;
            _brushInvert = false;

            _brushUseScaling = false;
            _brushScalePhylloFrequencyType = AudioFrequencyType.Band;
            _brushScaleAudioBand = 0;
            _brushScaleMinMax = Vector2.one;
            _brushUseScaleCurve = false;
            _brushScaleInterpolationCurve = AnimationCurve.Constant(0, 1, 1);
            _brushInterpolationSpeed = 0;
        }

        public Shape GetBrushShape()
        {
            return _brushShape;
        }

        public float GetBrushShapeInitSize()
        {
            return _brushInitialShapeSize;
        }

        public AnimationCurve GetBrushWidthCurve()
        {
            return _brushWidthCurve;
        }

        public float GetBrushEmissionIntensity()
        {
            return _brushEmissionIntensity;
        }

        public bool IsEmissionEnabled()
        {
            return _brushEmissionEnabled;
        }

        public Gradient GetBrushGradient()
        {
            return _brushGradient;
        }

        public Color GetBrushEmissionColor()
        {
            return _brushEmissionColor;
        }

        public AnimationCurve GetBrushAnimationCurve()
        {
            return _brushAnimationCurve;
        }

        public List<StartGen> GetBrushStartGeneration()
        {
            return _brushStartGeneration;
        }

        public bool GetBrushEmissionReactOnAudio()
        {
            return _brushEmissionReactOnAudio;
        }

        public int GetBrushEmissionBandBuffer()
        {
            return _brushEmissionBandBuffer;
        }

        public float GetBrushEmissionThreshold()
        {
            return _brushEmissionThreshold;
        }

        public AudioFrequencyType GetBrushEmissionFrequencyType()
        {
            return _brushEmissionFrequencyType;
        }

        public bool GetBrushScaleReactOnAudio()
        {
            return _brushScaleReactOnAudio;
        }

        public float GetBrushScaleMultiplier()
        {
            return _brushScaleMultiplier;
        }

        public float GetBrushScaleThreshold()
        {
            return _brushScaleThreshold;
        }

        public AudioFrequencyType GetBrushScaleFrequencyType()
        {
            return _brushScaleFrequencyType;
        }

        public int GetBrushScaleBandBuffer()
        {
            return _brushScaleBandBuffer;
        }

        public bool GetBrushKochEnabled()
        {
            return _brushKochEnabled;
        }

        public bool GetBrushUseBezierCurves()
        {
            return _brushUseBezierCurves;
        }

        public int GetBrushBezierVertexCount()
        {
            return _brushBezierVertexCount;
        }

        public List<int> GetBrushKochAudioBand()
        {
            _brushKochAudioBand.Clear();
            switch (_brushShape)
            {
                case Shape.Line:
                    _brushKochAudioBand.AddRange(new List<int>() { 0 });
                    break;
                case Shape.Triangle:
                    _brushKochAudioBand.AddRange(new List<int>() { 0, 0, 0 });
                    break;
                case Shape.Square:
                    _brushKochAudioBand.AddRange(new List<int>() { 0, 0, 0, 0 });
                    break;
                case Shape.Pentagon:
                    _brushKochAudioBand.AddRange(new List<int>() { 0, 0, 0, 0, 0 });
                    break;
                case Shape.Hexagon:
                    _brushKochAudioBand.AddRange(new List<int>() { 0, 0, 0, 0, 0, 0 });
                    break;
                case Shape.Heptagon:
                    _brushKochAudioBand.AddRange(new List<int>() { 0, 0, 0, 0, 0, 0, 0 });
                    break;
                case Shape.Octagon:
                    _brushKochAudioBand.AddRange(new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0 });
                    break;
            }
            return _brushKochAudioBand;
        }

        public bool GetBrushTrailEnabled()
        {
            return _brushTrailEnabled;
        }

        public Vector2 GetBrushTrailSpeedMinMax()
        {
            return _brushTrailSpeedMinMax;
        }

        public Vector2 GetBrushTrailTimeMinMax()
        {
            return _brushTrailTimeMinMax;
        }

        public Vector2 GetBrushTrailWidthMinMax()
        {
            return _brushTrailWidthMinMax;
        }

        public float GetBrushDegree()
        {
            return _brushDegree;
        }

        public float GetBrushScale()
        {
            return _brushScale;
        }

        public int GetBrushNumberStart()
        {
            return _brushNumberStart;
        }

        public int GetBrushStepSize()
        {
            return _brushStepSize;
        }

        public int GetBrushMaxIterations()
        {
            return _brushMaxIterations;
        }

        public bool GetBrushUseLerping()
        {
            return _brushUseLerping;
        }

        public AudioFrequencyType GetBrushLerpFrequencyType()
        {
            return _brushLerpFrequencyType;
        }

        public int GetBrushLerpAudioBand()
        {
            return _brushLerpAudioBand;
        }

        public Vector2 GetBrushSpeedMinMax()
        {
            return _brushSpeedMinMax;
        }

        public AnimationCurve GetBrushLerpInterpolationCurve()
        {
            return _brushLerpInterpolationCurve;
        }

        public bool GetBrushRepeat()
        {
            return _brushRepeat;
        }

        public bool GetBrushInvert()
        {
            return _brushInvert;
        }

        public bool GetBrushUseScaling()
        {
            return _brushUseScaling;
        }

        public AudioFrequencyType GetBrushScalePhylloFrequencyType()
        {
            return _brushScalePhylloFrequencyType;
        }

        public int GetBrushScaleAudioBand()
        {
            return _brushScaleAudioBand;
        }

        public Vector2 GetBrushScaleMinMax()
        {
            return _brushScaleMinMax;
        }

        public bool GetBrushUseScaleCurve()
        {
            return _brushUseScaleCurve;
        }

        public AnimationCurve GetBrushScaleInterpolationCurve()
        {
            return _brushScaleInterpolationCurve;
        }

        public float GetBrushInterpolationSpeed()
        {
            return _brushInterpolationSpeed;
        }
    }

    public enum Shape
    {
        Line = -2,
        Triangle = 1,
        Square = 2,
        Pentagon = 3,
        Hexagon = 4,
        Heptagon = 5,
        Octagon = 6,
        Phyllotaxis = 7
    }
}
