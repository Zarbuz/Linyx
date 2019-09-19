using Linyx.Controllers.Bottom;
using Linyx.Controllers.Edit;
using Linyx.Controllers.Project;
using Linyx.Controllers.ViewManager;
using Linyx.Models;
using Linyx.Models.Line;
using Linyx.Services.Curve;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace Linyx.Views.Edit
{
    public sealed class EditMediator : Mediator
    {
        [Inject] public EditView View { get; set; }
        [Inject] public SelectLineSignal SelectLineSignal { get; set; }
        [Inject] public UpdateLineSignal UpdateLineSignal { get; set; }
        [Inject] public LineUpdatedSignal LineUpdatedSignal { get; set; }
        [Inject] public ToggleViewSignal ToggleViewSignal { get; set; }
        [Inject] public SetInfoBarTextSignal SetInfoBarTextSignal { get; set; }
        [Inject] public UpdateEmissionColorSelectedSignal UpdateEmissionColorSelectedSignal { get; set; }
        [Inject] public UpdateEmissionColorSignal UpdateEmissionColorSignal { get; set; }
        [Inject] public UpdateCurveSignal UpdateCurveSignal { get; set; }

        [Inject] public UpdateGradientSignal UpdateGradientSignal { get; set; }
        [Inject] public UpdateGradientSelectedSignal UpdateGradientSelectedSignal { get; set; }

        public override void OnRegister()
        {
            base.OnRegister();
            View.PointerInfoSignal.AddListener(OnPointerInfoSignal);
            View.TogglePanelSignal.AddListener(OnTogglePanelSignal);
            View.UpdateLineSignal.AddListener(OnUpdateLineRequested);
            View.BrushGradientSignal.AddListener(OnBrushGradientRequested);
            View.BrushEmissionColorSignal.AddListener(OnBrushEmissionColorRequested);
            View.ShowKochCurveEditorSignal.AddListener(OnShowKochCurveEditorRequested);
            View.ShowWidthCurveEditorSignal.AddListener(OnShowWidthCurveEditorRequested);
            View.ShowLerpInterpolationCurveEditorSignal.AddListener(OnShowLerpInterpolationCurveEditorRequested);
            View.ShowScaleInterpolationCurveEditorSignal.AddListener(OnShowScaleInterpolationCurveEditorRequested);

            SelectLineSignal.AddListener(OnLineSelected);
            LineUpdatedSignal.AddListener(OnLineUpdatedReceived);
            UpdateGradientSelectedSignal.AddListener(OnBrushGradientReceived);
            UpdateEmissionColorSelectedSignal.AddListener(OnBrushEmissionColorSelected);
            View.Initialize();
        }


        public override void OnRemove()
        {
            base.OnRemove();
            View.PointerInfoSignal.RemoveListener(OnPointerInfoSignal);
            View.TogglePanelSignal.RemoveListener(OnTogglePanelSignal);
            View.UpdateLineSignal.RemoveListener(OnUpdateLineRequested);
            View.BrushGradientSignal.RemoveListener(OnBrushGradientRequested);
            View.BrushEmissionColorSignal.RemoveListener(OnBrushEmissionColorRequested);
            View.ShowKochCurveEditorSignal.RemoveListener(OnShowKochCurveEditorRequested);
            View.ShowWidthCurveEditorSignal.RemoveListener(OnShowWidthCurveEditorRequested);
            View.ShowLerpInterpolationCurveEditorSignal.RemoveListener(OnShowLerpInterpolationCurveEditorRequested);
            View.ShowScaleInterpolationCurveEditorSignal.RemoveListener(OnShowScaleInterpolationCurveEditorRequested);

            SelectLineSignal.RemoveListener(OnLineSelected);
            LineUpdatedSignal.RemoveListener(OnLineUpdatedReceived);
            UpdateGradientSelectedSignal.RemoveListener(OnBrushGradientReceived);
            UpdateEmissionColorSelectedSignal.RemoveListener(OnBrushEmissionColorSelected);
        }

        private void OnPointerInfoSignal(string value)
        {
            SetInfoBarTextSignal.Dispatch(value);
        }

        private void OnTogglePanelSignal(string value)
        {
            ToggleViewSignal.Dispatch(value);
        }

        private void OnUpdateLineRequested(LineModel line)
        {
            if (View.CanEmitEvent())
            {
                UpdateLineSignal.Dispatch(line);
            }
        }

        private void OnLineSelected(LineModel lineModel)
        {
            View.Refresh(lineModel);
        }

        private void OnLineUpdatedReceived(ILineModel line)
        {
            View.Refresh((LineModel)line);
        }

        private void OnBrushGradientReceived(Gradient gradient)
        {
            View.UpdateGradient(gradient);
        }

        private void OnBrushGradientRequested(LineModel lineModel)
        {
            UpdateGradientSignal.Dispatch(lineModel);
        }

        private void OnBrushEmissionColorRequested(LineModel lineModel)
        {
            UpdateEmissionColorSignal.Dispatch(lineModel);
        }

        private void OnBrushEmissionColorSelected(Color color)
        {
            View.UpdateEmissionColor(color);
        }

        private void OnShowKochCurveEditorRequested(LineModel lineModel)
        {
            UpdateCurveSignal.Dispatch(lineModel, CurveType.Animation);
        }

        private void OnShowWidthCurveEditorRequested(LineModel lineModel)
        {
            UpdateCurveSignal.Dispatch(lineModel, CurveType.Width);
        }

        private void OnShowLerpInterpolationCurveEditorRequested(LineModel lineModel)
        {
            UpdateCurveSignal.Dispatch(lineModel, CurveType.Lerp);
        }

        private void OnShowScaleInterpolationCurveEditorRequested(LineModel lineModel)
        {
            UpdateCurveSignal.Dispatch(lineModel, CurveType.Scale);
        }
    }
}
