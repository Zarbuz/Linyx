using System;
using System.Collections;
using Linyx.Controllers.Camera;
using Linyx.Services.Audio;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace Linyx.Services.Camera
{
    public sealed class CameraService : MonoBehaviour, ICameraService
    {
        #region UI

        [SerializeField] private Transform _rotateTransform;
        [SerializeField] private UnityEngine.Camera _camera;
        [SerializeField] private KaleidoscopeEffect _kaleidoscopeEffect;
        [SerializeField] private PostProcessVolume _postProcessVolume;
        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private Vector3 _rotateAxis;
        [SerializeField] private Vector3 _rotateSpeed;

        [SerializeField] private float _maxScroll = 15f;
        [SerializeField] private float _scrollDistanceIncrement = 0.05f;
        [SerializeField] private float _damping = 5f;
        #endregion

        #region Private Attributes

        private Vector3 _cameraPosition;
        private Vector2 _mouseClickPosition;
        private Vector2 _mouseCurrentPosition;
        private bool _panning;
        private bool _isDrawMode = true;
        private bool _isRotationEnabled;
        private bool _isKaleidoscopeEffectEnabled = true;
        #endregion

        #region Injections

        [Inject] public IAudioPeerService AudioPeerService { get; set; }
        [Inject] public NewCameraPositionSignal NewCameraPositionSignal { get; set; }
        #endregion

        #region Public Methods

        public void Initialize()
        {
        }

        public void SetKaleidoscope(bool value)
        {
            _kaleidoscopeEffect.enabled = value;
            _isKaleidoscopeEffectEnabled = value;
        }

        public void SetRenderMode(int mode)
        {
            switch (mode)
            {
                case 0:
                    _kaleidoscopeEffect.effectType = KaleidoscopeEffect.EffectType.Circle;
                    break;
                case 1:
                    _kaleidoscopeEffect.effectType = KaleidoscopeEffect.EffectType.Triangle60Tiling;
                    break;
                case 2:
                    _kaleidoscopeEffect.effectType = KaleidoscopeEffect.EffectType.Triangle90Tiling;
                    break;
                default:
                    Debug.Log("[CameraService] Unknown mode value for SetRenderMode: " + mode);
                    break;
            }
        }

        public void SetAngleValue(float value)
        {
            _kaleidoscopeEffect.angle = value;
        }

        public void SetNumberValue(int value)
        {
            _kaleidoscopeEffect.number = value;
        }

        public void SetRadiusValue(float value)
        {
            _kaleidoscopeEffect.radius = value;
        }

        public void SetChromaticValue(float value)
        {
            _postProcessVolume.profile.GetSetting<ChromaticAberration>().intensity.value = value;
        }

        public void SetBloomValue(float value)
        {
            _postProcessVolume.profile.GetSetting<Bloom>().intensity.value = value;
        }

        public void SetVignetteValue(float value)
        {
            _postProcessVolume.profile.GetSetting<Vignette>().intensity.value = value;
        }

        public void SetCenterXValue(float value)
        {
            Vector2 tmp = _kaleidoscopeEffect.center;
            _kaleidoscopeEffect.center = new Vector2(value, tmp.y);
        }

        public void SetCenterYValue(float value)
        {
            Vector2 tmp = _kaleidoscopeEffect.center;
            _kaleidoscopeEffect.center = new Vector2(tmp.x, value);
        }

        public void ToggleDrawMode()
        {
            _isDrawMode = !_isDrawMode;
            _kaleidoscopeEffect.enabled = !_isDrawMode && _isKaleidoscopeEffectEnabled;
            _postProcessVolume.profile.GetSetting<Vignette>().enabled.value = !_isDrawMode;
            _postProcessVolume.profile.GetSetting<ChromaticAberration>().enabled.value = !_isDrawMode;
            _postProcessVolume.profile.GetSetting<Bloom>().enabled.value = !_isDrawMode;
            if (_isDrawMode)
                _rotateTransform.eulerAngles = Vector3.zero;
        }

        public void CaptureScreenshot(string title)
        {
            StartCoroutine(_DelayCaptureScreenshot(title));
        }

        public void SetCameraRotation(bool enabled)
        {
            _isRotationEnabled = enabled;
            if (!enabled)
                _rotateTransform.eulerAngles = Vector3.zero;
        }

        public void SetCameraPosition(Vector2 position)
        {
            _camera.transform.localPosition = position;
        }

        public void SetCameraRotationSpeed(float speed)
        {
            _rotateSpeed.z = speed; //TODO Add other axis rotation ?
        }

        public void RecenterCamera()
        {
            StartCoroutine(_LerpFromTo(_camera.transform.position, Vector3.zero, _camera.transform.rotation,
                Quaternion.identity, 2f));
        }

        #endregion

        #region Unity Methods
        private void Update()
        {
            HandleMovement();
            HandleZoom();
            HandleRotation();
            HandlePosition();
        }

        #endregion

        #region Private Methods

        private void HandleMovement()
        {
            if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2) && !_panning)
            {
                _mouseClickPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                _panning = true;
            }

            if (_panning)
            {
                _mouseCurrentPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
                Vector2 distance = _mouseCurrentPosition - _mouseClickPosition;
                _camera.transform.position += new Vector3(-distance.x, -distance.y, 0);
            }

            if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
            {
                _panning = false;
            }
        }

        private void HandleZoom()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && _camera.orthographicSize > 1 && !Input.GetKey(KeyCode.LeftShift))
            {
                float orthographicSize = _camera.orthographicSize;
                float distance = orthographicSize - _scrollDistanceIncrement;
                _camera.orthographicSize = Mathf.Lerp(orthographicSize, distance, Time.deltaTime * _damping);
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0 && _camera.orthographicSize < _maxScroll && !Input.GetKey(KeyCode.LeftShift))
            {
                float orthographicSize = _camera.orthographicSize;
                float distance = orthographicSize + _scrollDistanceIncrement;
                _camera.orthographicSize = Mathf.Lerp(orthographicSize, distance, Time.deltaTime * _damping);
            }
        }

        private void HandleRotation()
        {
            if (_isRotationEnabled && !_isDrawMode && AudioPeerService.HasClip())
            {
                _rotateTransform.Rotate(_rotateAxis.x * _rotateSpeed.x * Time.deltaTime * AudioPeerService.GetAmplitudeBuffer(),
                    _rotateAxis.y * _rotateSpeed.y * Time.deltaTime * AudioPeerService.GetAmplitudeBuffer(),
                    _rotateAxis.z * _rotateSpeed.z * Time.deltaTime * AudioPeerService.GetAmplitudeBuffer());
            }
        }

        private void HandlePosition()
        {
            if (_camera.transform.localPosition != _cameraPosition)
            {
                _cameraPosition = _camera.transform.localPosition;
                NewCameraPositionSignal.Dispatch(_cameraPosition);
            }
        }

        #endregion

        #region Coroutines

        private IEnumerator _LerpFromTo(Vector3 pos1, Vector3 pos2, Quaternion rot1, Quaternion rot2, float duration)
        {
            for (float t = 0f; t < duration; t += Time.deltaTime)
            {
                _camera.transform.position = Vector3.Lerp(pos1, pos2, t / duration);
                _camera.transform.rotation = Quaternion.Slerp(rot1, rot2, t / duration);
                yield return 0;
            }
            _camera.transform.position = pos2;
            _camera.transform.rotation = rot2;
        }

        private IEnumerator _DelayCaptureScreenshot(string title)
        {
            _mainCanvas.gameObject.SetActive(false);
            ScreenCapture.CaptureScreenshot(title);
            yield return new WaitForSeconds(0.1f);
            _mainCanvas.gameObject.SetActive(true);
        }

        #endregion
    }
}
