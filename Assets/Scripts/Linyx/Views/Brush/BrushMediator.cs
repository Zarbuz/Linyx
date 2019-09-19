using System;
using Linyx.Controllers.Bottom;
using Linyx.Controllers.Brush;
using Linyx.Controllers.ViewManager;
using Linyx.Models;
using Linyx.Models.Line;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace Linyx.Views.Brush
{
    public class BrushMediator : Mediator
    {
        [Inject] public BrushView View { get; set; }
        [Inject] public SetBrushWidthCurveSignal SetBrushWidthCurveSignal { get; set; }
        [Inject] public SetBrushEmissionIntensitySignal SetBrushEmissionIntensitySignal { get; set; }
        [Inject] public SetBrushEmissionValueSignal SetBrushEmissionValueSignal { get; set; }
        [Inject] public SetBrushShapeIndexSignal SetBrushShapeIndexSignal { get; set; }
        [Inject] public SetBrushShapeInitSizeSignal SetBrushShapeInitSizeSignal { get; set; }
        [Inject] public SetBrushGradientSignal SetBrushGradientSignal { get; set; }
        [Inject] public ToggleViewSignal ToggleViewSignal { get; set; }
        [Inject] public SetInfoBarTextSignal SetInfoBarTextSignal { get; set; }
        [Inject] public BrushGradientSelectedSignal BrushGradientSelectedSignal { get; set; }
        [Inject] public SetBrushEmissionColorSignal SetBrushEmissionColorSignal { get; set; }
        [Inject] public BrushEmissionColorSelectedSignal BrushEmissionColorSelectedSignal { get; set; }
        [Inject] public SetBrushFromCopySelectedSignal SetBrushFromCopySelectedSignal { get; set; }

        public override void OnRegister()
        {
            base.OnRegister();
            View.PointerInfoSignal.AddListener(OnPointerInfoSignal);
            View.TogglePanelSignal.AddListener(OnTogglePanelSignal);
            View.BrushWidthCurveRequestSignal.AddListener(OnBrushWidthCurveRequested);
            View.BrushEmissionValueChangedSignal.AddListener(OnBrushEmissionValueChanged);
            View.EmissionValueChangedSignal.AddListener(OnEmissionValueChanged);
            View.BrushShapeIndexValueChangedSignal.AddListener(OnBrushShapeIndexValueChanged);
            View.BrushShapeInitialSizeValueChangedSignal.AddListener(OnBrushShapeInitialSizeValueChanged);
            View.BrushGradientRequestedSignal.AddListener(OnBrushGradientRequested);
            View.BrushEmissionColorRequestedSignal.AddListener(OnBrushEmissionColorRequested);

            BrushGradientSelectedSignal.AddListener(OnBrushGradientReceived);
            BrushEmissionColorSelectedSignal.AddListener(OnBrushEmissionColorReceived);
            SetBrushFromCopySelectedSignal.AddListener(OnBrushFromCopyReceived);
            View.Initialize();
        }


        public override void OnRemove()
        {
            base.OnRemove();
            View.PointerInfoSignal.RemoveListener(OnPointerInfoSignal);
            View.TogglePanelSignal.RemoveListener(OnTogglePanelSignal);

            View.BrushWidthCurveRequestSignal.RemoveListener(OnBrushWidthCurveRequested);
            View.BrushEmissionValueChangedSignal.RemoveListener(OnBrushEmissionValueChanged);
            View.BrushEmissionValueChangedSignal.RemoveListener(OnBrushEmissionValueChanged);
            View.EmissionValueChangedSignal.RemoveListener(OnEmissionValueChanged);
            View.BrushShapeIndexValueChangedSignal.RemoveListener(OnBrushShapeIndexValueChanged);
            View.BrushShapeInitialSizeValueChangedSignal.RemoveListener(OnBrushShapeInitialSizeValueChanged);
            View.BrushGradientRequestedSignal.RemoveListener(OnBrushGradientRequested);
            View.BrushEmissionColorRequestedSignal.RemoveListener(OnBrushEmissionColorRequested);

            BrushGradientSelectedSignal.RemoveListener(OnBrushGradientReceived);
            BrushEmissionColorSelectedSignal.RemoveListener(OnBrushEmissionColorReceived);
            SetBrushFromCopySelectedSignal.RemoveListener(OnBrushFromCopyReceived);
        }

        private void OnBrushEmissionColorRequested()
        {
            SetBrushEmissionColorSignal.Dispatch();
        }

        private void OnBrushEmissionColorReceived(Color color)
        {
            View.SetEmissionColor(color);
        }

        private void OnPointerInfoSignal(string value)
        {
            SetInfoBarTextSignal.Dispatch(value);
        }

        private void OnTogglePanelSignal(string value)
        {
            ToggleViewSignal.Dispatch(value);
        }

        private void OnBrushWidthCurveRequested()
        {
            SetBrushWidthCurveSignal.Dispatch();
        }

        private void OnBrushEmissionValueChanged(float value)
        {
            SetBrushEmissionIntensitySignal.Dispatch(value);
        }

        private void OnEmissionValueChanged(bool enabled)
        {
            SetBrushEmissionValueSignal.Dispatch(enabled);
        }

        private void OnBrushShapeIndexValueChanged(int index)
        {
            SetBrushShapeIndexSignal.Dispatch(index);
        }

        private void OnBrushShapeInitialSizeValueChanged(float value)
        {
            SetBrushShapeInitSizeSignal.Dispatch(value);
        }

        private void OnBrushGradientRequested()
        {
            SetBrushGradientSignal.Dispatch();
        }

        private void OnBrushGradientReceived(Gradient gradient)
        {
            View.UpdateGradient(gradient);
        }

        private void OnBrushFromCopyReceived(LineModel line)
        {
            View.UpdateSettings(line);
        }
    }
}
