using System.Collections.Generic;
using Linyx.Models.Koch;
using Linyx.Models.Line;
using Linyx.Services.Audio;
using UnityEngine;

namespace Linyx.Services.Brush
{
    public interface IBrushService : IServiceBase
    {
        void SetBrushShape(Shape shape);
        void SetBrushShapeInitSize(float size);
        void SetBrushWidthCurve(AnimationCurve curve);
        void SetBrushEmissionIntensity(float emission);
        void SetEmission(bool enabled);
        void SetBrushGradient(Gradient gradient);
        void SetBrushEmissionColor(Color color);
        void SetBrushFromCopy(LineModel lineModel);

        void ResetBrushSettings();

        // Common
        Shape GetBrushShape();
        float GetBrushShapeInitSize();
        AnimationCurve GetBrushWidthCurve();
        float GetBrushEmissionIntensity();
        bool IsEmissionEnabled();
        Gradient GetBrushGradient();
        Color GetBrushEmissionColor();
        
        // Emission
        bool GetBrushEmissionReactOnAudio();
        int GetBrushEmissionBandBuffer();
        float GetBrushEmissionThreshold();
        AudioFrequencyType GetBrushEmissionFrequencyType();

        // Scale
        bool GetBrushScaleReactOnAudio();
        int GetBrushScaleBandBuffer();
        float GetBrushScaleMultiplier();
        float GetBrushScaleThreshold();
        AudioFrequencyType GetBrushScaleFrequencyType();

        // Koch
        bool GetBrushKochEnabled();
        bool GetBrushUseBezierCurves();
        int GetBrushBezierVertexCount();
        List<int> GetBrushKochAudioBand();
        AnimationCurve GetBrushAnimationCurve();
        List<StartGen> GetBrushStartGeneration();

        // Trails
        bool GetBrushTrailEnabled();
        Vector2 GetBrushTrailSpeedMinMax();
        Vector2 GetBrushTrailTimeMinMax();
        Vector2 GetBrushTrailWidthMinMax();

        // Phyllotaxis
        float GetBrushDegree();
        float GetBrushScale();
        int GetBrushNumberStart();
        int GetBrushStepSize();
        int GetBrushMaxIterations();
        bool GetBrushUseLerping();
        AudioFrequencyType GetBrushLerpFrequencyType();
        int GetBrushLerpAudioBand();
        Vector2 GetBrushSpeedMinMax();
        AnimationCurve GetBrushLerpInterpolationCurve();
        bool GetBrushRepeat();
        bool GetBrushInvert();
        bool GetBrushUseScaling();
        AudioFrequencyType GetBrushScalePhylloFrequencyType();
        int GetBrushScaleAudioBand();
        Vector2 GetBrushScaleMinMax();
        bool GetBrushUseScaleCurve();
        AnimationCurve GetBrushScaleInterpolationCurve();
        float GetBrushInterpolationSpeed();



    }
}
