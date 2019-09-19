using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace Linyx.Views.Utils
{
    public sealed class UtilsView : BaseView
    {
        #region SerializeFields
        [Header("UtilsView")]
        [SerializeField] private GameObject _positionXCameraPanel;
        [SerializeField] private GameObject _positionYCameraPanel;
        [SerializeField] private InputField _positionXCameraInputField;
        [SerializeField] private InputField _positionYCameraInputField;
        #endregion

        #region Local Signals

        public Signal<Vector2> CameraNewPositionSignal = new Signal<Vector2>();

        #endregion

        #region Private Attributes

        private Vector2 _cameraPosition;

        #endregion

        #region Public Methods

        public override void Initialize()
        {
            base.Initialize();
            _positionXCameraInputField.onValueChanged.AddListener(OnPositionXValueChanged);
            _positionYCameraInputField.onValueChanged.AddListener(OnPositionYValueChanged);
        }

        public void ShowRuler(bool show)
        {
            _positionXCameraPanel.SetActive(show);
            _positionYCameraPanel.SetActive(show);
        }

        public void SetPositionCamera(Vector2 position)
        {
            _positionXCameraInputField.SetTextWithoutNotify(position.x.ToString("0.00"));
            _positionYCameraInputField.SetTextWithoutNotify(position.y.ToString("0.00"));
            _cameraPosition = position;
        }

        #endregion

        #region Private Methods
        private void OnPositionYValueChanged(string arg0)
        {
            float y = float.Parse(arg0);
            float x = _cameraPosition.x;
            CameraNewPositionSignal.Dispatch(new Vector2(x, y));
        }

        private void OnPositionXValueChanged(string arg0)
        {
            float x = float.Parse(arg0);
            float y = _cameraPosition.y;
            CameraNewPositionSignal.Dispatch(new Vector2(x, y));
        }

        #endregion
    }
}
