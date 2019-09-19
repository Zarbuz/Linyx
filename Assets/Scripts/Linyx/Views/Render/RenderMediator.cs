using System;
using Linyx.Controllers.Background;
using Linyx.Controllers.Bottom;
using Linyx.Controllers.Camera;
using Linyx.Controllers.ViewManager;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace Linyx.Views.Render
{
    public sealed class RenderMediator : Mediator
    {
        #region Injections
        [Inject] public RenderView View { get; set; }
        [Inject] public SetRenderModeSignal SetRenderModeSignal { get; set; }
        [Inject] public SetRadiusValueSignal SetRadiusValueSignal { get; set; }
        [Inject] public SetAngleValueSignal SetAngleValueSignal { get; set; }
        [Inject] public SetNumberValueSignal SetNumberValueSignal { get; set; }
        [Inject] public SetBloomValueSignal SetBloomValueSignal { get; set; }
        [Inject] public SetChromaticValueSignal SetChromaticValueSignal { get; set; }
        [Inject] public SetVignetteValueSignal SetVignetteValueSignal { get; set; }
        [Inject] public SetCenterXValueSignal SetCenterXValueSignal { get; set; }
        [Inject] public SetCenterYValueSignal SetCenterYValueSignal { get; set; }
        [Inject] public SetTopBackgroundColorSignal SetTopBackgroundColorSignal { get; set; }
        [Inject] public TopBackgroundColorSelectedSignal TopBackgroundColorSelectedSignal { get; set; }
        [Inject] public SetBottomBackgroundColorSignal SetBottomBackgroundColorSignal { get; set; }
        [Inject] public BottomBackgroundColorSelectedSignal BottomBackgroundColorSelectedSignal { get; set; }
        [Inject] public SetIntensityBackgroundSignal SetIntensityBackgroundSignal { get; set; }
        [Inject] public SetExponentBackgroundSignal SetExponentBackgroundSignal { get; set; }
        [Inject] public SetDirectionXAngleSignal SetDirectionXAngleSignal { get; set; }
        [Inject] public SetDirectionYAngleSignal SetDirectionYAngleSignal { get; set; }
        [Inject] public SetCameraRotationSignal SetCameraRotationSignal { get; set; }
        [Inject] public SetCameraRotationSpeedSignal SetCameraRotationSpeedSignal { get; set; }
        [Inject] public ToggleViewSignal ToggleViewSignal { get; set; }
        [Inject] public SetInfoBarTextSignal SetInfoBarTextSignal { get; set; }
        [Inject] public SetKaleidoscopeEffectSignal SetKaleidoscopeEffectSignal { get; set; }

        #endregion


        public override void OnRegister()
        {
            base.OnRegister();
            View.PointerInfoSignal.AddListener(OnPointerInfoSignal);
            View.TogglePanelSignal.AddListener(OnTogglePanelSignal);
            View.KaileidoscopeEffectValueChangedSignal.AddListener(OnKaileidoscopeValueChanged);
            View.CircleModeRequestSignal.AddListener(() => OnRenderModeChanged(0));
            View.Triangle60ModeRequestSignal.AddListener(() => OnRenderModeChanged(1));
            View.Triangle90ModeRequestSignal.AddListener(() => OnRenderModeChanged(2));
            View.AngleValueChangedSignal.AddListener(OnAngleValueChanged);
            View.RadiusValueChangedSignal.AddListener(OnRadiusValueChanged);
            View.NumberValueChangedSignal.AddListener(OnNumberValueChanged);
            View.BloomValueChangedSignal.AddListener(OnBloomValueChanged);
            View.ChromaticValueChangedSignal.AddListener(OnChromaticValueChanged);
            View.VignetteValueChangedSignal.AddListener(OnVignetteValueChanged);
            View.CenterXValueChangedSignal.AddListener(OnCenterXValueChanged);
            View.CenterYValueChangedSignal.AddListener(OnCenterYValueChanged);

            View.TopBackgroundColorRequestSignal.AddListener(OnTopBackgroundColorRequest);
            View.BottomBackgroundColorRequestSignal.AddListener(OnBottomBackgroundColorRequest);
            View.IntensityBackgroundValueChangedSignal.AddListener(OnIntensityBackgroundValueChanged);
            View.ExponentBackgroundValueChangedSignal.AddListener(OnExponentBackgroundValueChanged);
            View.DirectionXAngleValueChangedSignal.AddListener(OnDirectionXAngleValueChanged);
            View.DirectionYAngleValueChangedSignal.AddListener(OnDirectionYAngleValueChanged);

            View.RotateCameraValueChangedSignal.AddListener(OnRotateCameraValueChanged);
            View.RotateCameraSpeedValueChanged.AddListener(OnRotateCameraSpeedValueChanged);

            TopBackgroundColorSelectedSignal.AddListener(OnTopBackgroundColorSelected);
            BottomBackgroundColorSelectedSignal.AddListener(OnBottomBackgroundColorSelected);
            View.Initialize();
        }

        public override void OnRemove()
        {
            base.OnRemove();
            View.PointerInfoSignal.RemoveListener(OnPointerInfoSignal);
            View.TogglePanelSignal.RemoveListener(OnTogglePanelSignal);
            View.KaileidoscopeEffectValueChangedSignal.RemoveListener(OnKaileidoscopeValueChanged);
            View.CircleModeRequestSignal.RemoveListener(() => OnRenderModeChanged(0));
            View.Triangle60ModeRequestSignal.RemoveListener(() => OnRenderModeChanged(1));
            View.Triangle90ModeRequestSignal.RemoveListener(() => OnRenderModeChanged(2));
            View.AngleValueChangedSignal.RemoveListener(OnAngleValueChanged);
            View.RadiusValueChangedSignal.RemoveListener(OnRadiusValueChanged);
            View.NumberValueChangedSignal.RemoveListener(OnNumberValueChanged);
            View.BloomValueChangedSignal.RemoveListener(OnBloomValueChanged);
            View.ChromaticValueChangedSignal.RemoveListener(OnChromaticValueChanged);
            View.VignetteValueChangedSignal.RemoveListener(OnVignetteValueChanged);
            View.CenterXValueChangedSignal.RemoveListener(OnCenterXValueChanged);
            View.CenterYValueChangedSignal.RemoveListener(OnCenterYValueChanged);

            View.TopBackgroundColorRequestSignal.RemoveListener(OnTopBackgroundColorRequest);
            View.BottomBackgroundColorRequestSignal.RemoveListener(OnBottomBackgroundColorRequest);
            View.IntensityBackgroundValueChangedSignal.RemoveListener(OnIntensityBackgroundValueChanged);
            View.ExponentBackgroundValueChangedSignal.RemoveListener(OnExponentBackgroundValueChanged);
            View.DirectionXAngleValueChangedSignal.RemoveListener(OnDirectionXAngleValueChanged);
            View.DirectionYAngleValueChangedSignal.RemoveListener(OnDirectionYAngleValueChanged);

            View.RotateCameraValueChangedSignal.RemoveListener(OnRotateCameraValueChanged);
            View.RotateCameraSpeedValueChanged.RemoveListener(OnRotateCameraSpeedValueChanged);

            TopBackgroundColorSelectedSignal.RemoveListener(OnTopBackgroundColorSelected);
            BottomBackgroundColorSelectedSignal.RemoveListener(OnBottomBackgroundColorSelected);
        }

        private void OnPointerInfoSignal(string value)
        {
            SetInfoBarTextSignal.Dispatch(value);
        }

        private void OnTogglePanelSignal(string value)
        {
            ToggleViewSignal.Dispatch(value);
        }

        private void OnKaileidoscopeValueChanged(bool value)
        {
            SetKaleidoscopeEffectSignal.Dispatch(value);
        }

        private void OnRenderModeChanged(int mode)
        {
            SetRenderModeSignal.Dispatch(mode);
        }

        private void OnAngleValueChanged(float value)
        {
            SetAngleValueSignal.Dispatch(value);
        }

        private void OnRadiusValueChanged(float value)
        {
            SetRadiusValueSignal.Dispatch(value);
        }

        private void OnNumberValueChanged(int value)
        {
            SetNumberValueSignal.Dispatch(value);
        }

        private void OnBloomValueChanged(float value)
        {
            SetBloomValueSignal.Dispatch(value);
        }

        private void OnChromaticValueChanged(float value)
        {
            SetChromaticValueSignal.Dispatch(value);
        }

        private void OnVignetteValueChanged(float value)
        {
            SetVignetteValueSignal.Dispatch(value);
        }

        private void OnCenterXValueChanged(float value)
        {
            SetCenterXValueSignal.Dispatch(value);
        }

        private void OnCenterYValueChanged(float value)
        {
            SetCenterYValueSignal.Dispatch(value);
        }

        private void OnTopBackgroundColorRequest()
        {
            SetTopBackgroundColorSignal.Dispatch();
        }

        private void OnBottomBackgroundColorRequest()
        {
            SetBottomBackgroundColorSignal.Dispatch();
        }

        private void OnIntensityBackgroundValueChanged(float value)
        {
            SetIntensityBackgroundSignal.Dispatch(value);
        }

        private void OnExponentBackgroundValueChanged(float value)
        {
            SetExponentBackgroundSignal.Dispatch(value);
        }

        private void OnDirectionXAngleValueChanged(int value)
        {
            SetDirectionXAngleSignal.Dispatch(value);
        }

        private void OnDirectionYAngleValueChanged(int value)
        {
            SetDirectionYAngleSignal.Dispatch(value);
        }

        private void OnTopBackgroundColorSelected(Color value)
        {
            View.RefreshTopBackgroundColor(value);
        }

        private void OnBottomBackgroundColorSelected(Color value)
        {
            View.RefreshBottomBackgroundColor(value);
        }

        private void OnRotateCameraValueChanged(bool enabled)
        {
            SetCameraRotationSignal.Dispatch(enabled);
        }

        private void OnRotateCameraSpeedValueChanged(float value)
        {
            SetCameraRotationSpeedSignal.Dispatch(value);
        }
    }
}
