using Linyx.Services.Brush;
using Linyx.Services.Curve;
using Linyx.Utils;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace Linyx.Controllers.Brush
{
    public sealed class SetBrushWidthCurveCommand : Command
    {
        [Inject] public IBrushService BrushService { get; set; }
        [Inject] public ICurveService CurveService { get; set; }

        public override async void Execute()
        {
            RTAnimationCurve rtAnimationCurve = CurveService.GetAnimationCurve();
            rtAnimationCurve.ShowCurveEditor();
            rtAnimationCurve.NewWindow();

            AnimationCurve animationCurve = BrushService.GetBrushWidthCurve();
            rtAnimationCurve.Add(ref animationCurve);

            rtAnimationCurve.SetGradXRange(0, 1f);
            rtAnimationCurve.SetGradYRange(0, 0.5f);

            await TaskEx.WaitWhile(CurveService.IsEditorOpen);
            BrushService.SetBrushWidthCurve(animationCurve);
        }
    }

    public sealed class SetBrushWidthCurveSignal : Signal { }
}
