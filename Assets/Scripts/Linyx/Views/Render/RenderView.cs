using Linyx.Utils;
using MaterialUI;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace Linyx.Views.Render
{
    public sealed class RenderView : BaseView
    {
        #region UI
        [SerializeField] private RectTransform _verticalLayoutRectTransform;

        [Header("Kaleidoscope")]
        [SerializeField] private MaterialSwitch _effectEnableSwitch;
        [SerializeField] private MaterialRadio _circleRadio;
        [SerializeField] private MaterialRadio _triangle60Radio;
        [SerializeField] private MaterialRadio _triangle90Radio;

        [SerializeField] private MaterialSlider _centerXSlider;
        [SerializeField] private MaterialSlider _centerYSlider;

        [SerializeField] private MaterialSlider _numberSlider;
        [SerializeField] private MaterialSlider _radiusSlider;
        [SerializeField] private MaterialSlider _angleSlider;

        [Header("Film")]
        [SerializeField] private MaterialSlider _bloomSlider;
        [SerializeField] private MaterialSlider _chromaticSlider;
        [SerializeField] private MaterialSlider _vignetteSlider;

        [Header("Background")]
        [SerializeField] private MaterialButton _topBackgroundColorButton;
        [SerializeField] private MaterialButton _bottomBackgroundColorButton;
        [SerializeField] private MaterialSlider _intensityBackgroundSlider;
        [SerializeField] private MaterialSlider _exponentBackgroundSlider;
        [SerializeField] private MaterialSlider _directionXAngleSlider;
        [SerializeField] private MaterialSlider _directionYAngleSlider;

        [Header("Camera")]
        [SerializeField] private MaterialSwitch _rotateCameraSwitch;
        [SerializeField] private MaterialSlider _rotateCameraSpeedSlider;
        #endregion

        #region Local Signals
        public Signal<bool> KaileidoscopeEffectValueChangedSignal = new Signal<bool>(); 
        public Signal CircleModeRequestSignal = new Signal();
        public Signal Triangle60ModeRequestSignal = new Signal();
        public Signal Triangle90ModeRequestSignal = new Signal();
        public Signal<float> RadiusValueChangedSignal = new Signal<float>();
        public Signal<float> AngleValueChangedSignal = new Signal<float>();
        public Signal<int> NumberValueChangedSignal = new Signal<int>();
        public Signal<float> BloomValueChangedSignal = new Signal<float>();
        public Signal<float> ChromaticValueChangedSignal = new Signal<float>();
        public Signal<float> VignetteValueChangedSignal = new Signal<float>();
        public Signal<float> CenterXValueChangedSignal = new Signal<float>();
        public Signal<float> CenterYValueChangedSignal = new Signal<float>();
        public Signal TopBackgroundColorRequestSignal = new Signal();
        public Signal BottomBackgroundColorRequestSignal = new Signal();
        public Signal<float> IntensityBackgroundValueChangedSignal = new Signal<float>();
        public Signal<float> ExponentBackgroundValueChangedSignal = new Signal<float>();
        public Signal<int> DirectionXAngleValueChangedSignal = new Signal<int>();
        public Signal<int> DirectionYAngleValueChangedSignal = new Signal<int>();
        public Signal<bool> RotateCameraValueChangedSignal = new Signal<bool>();
        public Signal<float> RotateCameraSpeedValueChanged = new Signal<float>();
        #endregion

        #region Public Methods

        public override void Initialize()
        {
            base.Initialize();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayoutRectTransform.GetComponent<RectTransform>());

            _effectEnableSwitch.toggle.onValueChanged.AddListener(OnEffectEnableValueChanged);
            _circleRadio.toggle.onValueChanged.AddListener(OnCircleValueChanged);
            _triangle60Radio.toggle.onValueChanged.AddListener(OnTriangle60ValueChanged);
            _triangle90Radio.toggle.onValueChanged.AddListener(OnTriangle90ValueChanged);
            _radiusSlider.slider.onValueChanged.AddListener(OnRadiusValueChanged);
            _angleSlider.slider.onValueChanged.AddListener(OnAngleValueChanged);
            _numberSlider.slider.onValueChanged.AddListener(OnNumberValueChanged);
            _centerXSlider.slider.onValueChanged.AddListener(OnCenterXValueChanged);
            _centerYSlider.slider.onValueChanged.AddListener(OnCenterYValueChanged);

            _bloomSlider.slider.onValueChanged.AddListener(OnBloomValueChanged);
            _chromaticSlider.slider.onValueChanged.AddListener(OnChromaticValueChanged);
            _vignetteSlider.slider.onValueChanged.AddListener(OnVignetteValueChanged);

            _topBackgroundColorButton.buttonObject.onClick.AddListener(OnTopBackgroundColorClicked);
            _bottomBackgroundColorButton.buttonObject.onClick.AddListener(OnBottomBackgroundColorClicked);
            _intensityBackgroundSlider.slider.onValueChanged.AddListener(OnIntensityValueChanged);
            _exponentBackgroundSlider.slider.onValueChanged.AddListener(OnExponentValueChanged);
            _directionXAngleSlider.slider.onValueChanged.AddListener(OnDirectionXValueChanged);
            _directionYAngleSlider.slider.onValueChanged.AddListener(OnDirectionYValueChanged);

            _rotateCameraSwitch.toggle.onValueChanged.AddListener(OnRotateCameraValueChanged);
            _rotateCameraSpeedSlider.slider.onValueChanged.AddListener(OnRotateCameraSpeedValueChanged);
        }

        public void RefreshTopBackgroundColor(Color color)
        {
            _topBackgroundColorButton.backgroundColor = color;
        }

        public void RefreshBottomBackgroundColor(Color color)
        {
            _bottomBackgroundColorButton.backgroundColor = color;
        }

        #endregion

        #region Private Methods

        private void OnEffectEnableValueChanged(bool value)
        {
            KaileidoscopeEffectValueChangedSignal.Dispatch(value);
        }

        private void OnCircleValueChanged(bool value)
        {
            CircleModeRequestSignal.Dispatch();
            _numberSlider.gameObject.SetActive(true);
        }

        private void OnTriangle90ValueChanged(bool value)
        {
            Triangle90ModeRequestSignal.Dispatch();
            _numberSlider.gameObject.SetActive(false);
        }

        private void OnTriangle60ValueChanged(bool value)
        {
            Triangle60ModeRequestSignal.Dispatch();
            _numberSlider.gameObject.SetActive(false);
        }

        private void OnRadiusValueChanged(float value)
        {
            RadiusValueChangedSignal.Dispatch(value);
        }

        private void OnAngleValueChanged(float value)
        {
            AngleValueChangedSignal.Dispatch(value);
        }

        private void OnNumberValueChanged(float value)
        {
            NumberValueChangedSignal.Dispatch((int)value);
        }

        private void OnVignetteValueChanged(float value)
        {
            VignetteValueChangedSignal.Dispatch(value);
        }

        private void OnChromaticValueChanged(float value)
        {
            ChromaticValueChangedSignal.Dispatch(value);
        }

        private void OnBloomValueChanged(float value)
        {
            BloomValueChangedSignal.Dispatch(value);
        }

        private void OnCenterXValueChanged(float value)
        {
            CenterXValueChangedSignal.Dispatch(value);
        }

        private void OnCenterYValueChanged(float value)
        {
            CenterYValueChangedSignal.Dispatch(value);
        }

        private void OnTopBackgroundColorClicked()
        {
            TopBackgroundColorRequestSignal.Dispatch();
        }

        private void OnBottomBackgroundColorClicked()
        {
            BottomBackgroundColorRequestSignal.Dispatch();
        }

        private void OnIntensityValueChanged(float value)
        {
            IntensityBackgroundValueChangedSignal.Dispatch(value);
        }

        private void OnExponentValueChanged(float value)
        {
            ExponentBackgroundValueChangedSignal.Dispatch(value);
        }

        private void OnDirectionXValueChanged(float value)
        {
            DirectionXAngleValueChangedSignal.Dispatch((int)value);
        }

        private void OnDirectionYValueChanged(float value)
        {
            DirectionYAngleValueChangedSignal.Dispatch((int)value);
        }

        private void OnRotateCameraValueChanged(bool enabled)
        {
            RotateCameraValueChangedSignal.Dispatch(enabled);
            _rotateCameraSpeedSlider.gameObject.SetActive(enabled);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayoutRectTransform);
        }

        private void OnRotateCameraSpeedValueChanged(float value)
        {
            RotateCameraSpeedValueChanged.Dispatch(value);
        }
        #endregion



    }
}
