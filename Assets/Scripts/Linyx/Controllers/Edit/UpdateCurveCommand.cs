using System;
using System.Threading.Tasks;
using Linyx.Models;
using Linyx.Models.Line;
using Linyx.Services.Curve;
using Linyx.Utils;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace Linyx.Controllers.Edit
{
    public enum CurveType
    {
        Animation,
        Width,
        Lerp,
        Scale
    }

    public sealed class UpdateCurveCommand : Command
    {
        [Inject] public ICurveService CurveService { get; set; }
        [Inject] public LineModel LineModel { get; set; }
        [Inject] public UpdateLineSignal UpdateLineSignal { get; set; }
        [Inject] public CurveType CurveType { get; set; }

        public override async void Execute()
        {
            RTAnimationCurve rtAnimationCurve = CurveService.GetAnimationCurve();
            rtAnimationCurve.ShowCurveEditor();
            rtAnimationCurve.NewWindow();
            AnimationCurve animationCurve = AnimationCurve.Constant(0, 1, 0.5f);

            switch (CurveType)
            {
                case CurveType.Animation:
                    animationCurve = LineModel.KochLineProperty.AnimationCurve;
                    break;
                case CurveType.Width:
                    animationCurve = LineModel.WidthCurve;
                    break;
                case CurveType.Lerp:
                    animationCurve = LineModel.PhyllotaxisProperty.LerpInterpolationCurve;
                    break;
                case CurveType.Scale:
                    animationCurve = LineModel.PhyllotaxisProperty.ScaleInterpolationCurve;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            rtAnimationCurve.Add(ref animationCurve);

            switch (CurveType)
            {
                case CurveType.Animation:
                case CurveType.Lerp:
                case CurveType.Scale:
                    rtAnimationCurve.SetGradYRange(0, 1);
                    break;
                case CurveType.Width:
                    rtAnimationCurve.SetGradYRange(0, 0.5f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            rtAnimationCurve.SetGradXRange(0, 1);

            await TaskEx.WaitWhile(CurveService.IsEditorOpen);
            switch (CurveType)
            {
                case CurveType.Animation:
                    LineModel.KochLineProperty.AnimationCurve = animationCurve;
                    break;
                case CurveType.Width:
                    LineModel.WidthCurve = animationCurve;
                    break;
                case CurveType.Lerp:
                    LineModel.PhyllotaxisProperty.LerpInterpolationCurve = animationCurve;
                    break;
                case CurveType.Scale:
                    LineModel.PhyllotaxisProperty.ScaleInterpolationCurve = animationCurve;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            UpdateLineSignal.Dispatch(LineModel);
        }

    }

    public sealed class UpdateCurveSignal : Signal<LineModel, CurveType> { }
}
