using Linyx.Models.Emission;
using Linyx.Models.Koch;
using Linyx.Models.Phyllotaxis;
using Linyx.Models.Scale;
using Linyx.Services.Brush;
using UnityEngine;

namespace Linyx.Models.Line
{
    public interface ILineModel
    {
        string Guid { get; set; }
        string DisplayName { get; set; }
        int Layer { get; set; }
        Shape Shape { get; set; }
        Gradient Gradient { get; set; }
        AnimationCurve WidthCurve { get; set; }
        bool IsEmissionEnabled { get; set; }
        float EmissionIntensity { get; set; }
        Color EmissionColor { get; set; }
        LineRenderer LineGameObject { get; set; }

        
        IEmissionProperty EmissionProperty { get; set; }
        IScaleProperty ScaleProperty { get; set; }
        IKochLineProperty KochLineProperty { get; set; }
        IKochTrailProperty KochTrailProperty { get; set; }
        IPhyllotaxisProperty PhyllotaxisProperty { get; set; }
    }
}
