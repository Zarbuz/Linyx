using MaterialUI;
using strange.extensions.signal.impl;
using TMPro;
using UnityEngine;

namespace Linyx.Views.Bottom
{
    public sealed class BottomView : BaseView
    {
        #region UI
        [Header("Bottom View")]
        [SerializeField] private Color _selectedColor;
        [SerializeField] private TextMeshProUGUI _infoText;
        [SerializeField] private MaterialButton _recenterCameraButton;
        [SerializeField] private MaterialButton _rulerCameraButton;
        [SerializeField] private MaterialButton _screenshotButton;
        [SerializeField] private MaterialButton _exportVideoButton;
        #endregion

        #region Private Attributes

        private bool _isRulerCameraDisplay;
        #endregion

        #region Local Signals

        public Signal RecenterCameraSignal = new Signal();
        public Signal ScreenshotSignal = new Signal();
        public Signal ExportVideoSignal = new Signal();
        public Signal<bool> RulerCameraSignal = new Signal<bool>();

        #endregion

        #region Public Methods

        public override void Initialize()
        {
            base.Initialize();
            _recenterCameraButton.buttonObject.onClick.AddListener(OnRecenterCameraClicked);
            _rulerCameraButton.buttonObject.onClick.AddListener(OnRulerCameraClicked);
            _screenshotButton.buttonObject.onClick.AddListener(OnScreenshotClicked);
            _exportVideoButton.buttonObject.onClick.AddListener(OnExportVideoClicked);
        }

        public void RefreshText(string text)
        {
            _infoText.SetText(text);
        }

        public void SetExportVideoButtonEnabled()
        {
            _exportVideoButton.interactable = true;
        }
        #endregion

        #region Private Methods

        private void OnRecenterCameraClicked()
        {
            RecenterCameraSignal.Dispatch();
        }

        private void OnRulerCameraClicked()
        {
            _isRulerCameraDisplay = !_isRulerCameraDisplay;
            _rulerCameraButton.iconColor = _isRulerCameraDisplay ? _selectedColor : Color.white;
            RulerCameraSignal.Dispatch(_isRulerCameraDisplay);
        }

        private void OnScreenshotClicked()
        {
            ScreenshotSignal.Dispatch();
        }

        private void OnExportVideoClicked()
        {
            ExportVideoSignal.Dispatch();
        }
        #endregion

    }
}
