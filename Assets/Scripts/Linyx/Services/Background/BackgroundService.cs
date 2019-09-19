using UnityEngine;

namespace Linyx.Services.Background
{
    public sealed class BackgroundService : MonoBehaviour, IBackgroundService
    {
        #region UI
        [SerializeField] private Material _skyMaterial;
        [SerializeField] private Color _defaultColor;
        #endregion


        #region Private Attributes

        private Color _topColor;
        private Color _bottomColor;
        private static readonly int m_Exponent = Shader.PropertyToID("_Exponent");
        private static readonly int m_Intensity = Shader.PropertyToID("_Intensity");
        private static readonly int m_Color1 = Shader.PropertyToID("_Color1");
        private static readonly int m_Color2 = Shader.PropertyToID("_Color2");
        private static readonly int m_DirectionYaw = Shader.PropertyToID("_DirectionYaw");
        private static readonly int m_DirectionPitch = Shader.PropertyToID("_DirectionPitch");
        private static readonly int m_Direction = Shader.PropertyToID("_Direction");

        #endregion

        #region Public Methods

        public void Initialize()
        {
            _topColor = _defaultColor;
            _bottomColor = _defaultColor;
            ResetBackgroundColor();
        }

        public void SetTopBackgroundColor(Color color)
        {
            _topColor = color;
            _skyMaterial.SetColor(m_Color2, color);
        }

        public void SetBottomBackgroundColor(Color color)
        {
            _bottomColor = color;
            _skyMaterial.SetColor(m_Color1, color);
        }

        public void SetBackgroundIntensity(float value)
        {
            _skyMaterial.SetFloat(m_Intensity, value);
        }

        public void SetBackgroundExponent(float value)
        {
            _skyMaterial.SetFloat(m_Exponent, value);
        }

        public void SetDirectionXAngle(int value)
        {
            _skyMaterial.SetInt(m_DirectionYaw, value);
            float dirPitch = _skyMaterial.GetInt(m_DirectionPitch);
            float dirYaw = _skyMaterial.GetInt(m_DirectionYaw);

            float dirPitchRad = dirPitch * Mathf.Deg2Rad;
            float dirYawRad = dirYaw * Mathf.Deg2Rad;

            Vector4 direction = new Vector4(Mathf.Sin(dirPitchRad) * Mathf.Sin(dirYawRad), Mathf.Cos(dirPitchRad),
                Mathf.Sin(dirPitchRad) * Mathf.Cos(dirYawRad), 0.0f);

            _skyMaterial.SetVector(m_Direction, direction);
        }

        public void SetDirectionYAngle(int value)
        {
            _skyMaterial.SetInt(m_DirectionPitch, value);
            float dirPitch = _skyMaterial.GetInt(m_DirectionPitch);
            float dirYaw = _skyMaterial.GetInt(m_DirectionYaw);

            float dirPitchRad = dirPitch * Mathf.Deg2Rad;
            float dirYawRad = dirYaw * Mathf.Deg2Rad;

            Vector4 direction = new Vector4(Mathf.Sin(dirPitchRad) * Mathf.Sin(dirYawRad), Mathf.Cos(dirPitchRad),
                Mathf.Sin(dirPitchRad) * Mathf.Cos(dirYawRad), 0.0f);

            _skyMaterial.SetVector(m_Direction, direction);
        }

        public void ResetBackgroundColor()
        {
            _skyMaterial.SetColor(m_Color1, _defaultColor);
            _skyMaterial.SetColor(m_Color2, _defaultColor);
        }

        public void Refresh()
        {
            SetTopBackgroundColor(_topColor);
            SetBottomBackgroundColor(_bottomColor);
        }

        public Color GetTopBackgroundColor()
        {
            return _topColor;
        }

        public Color GetBottomBackgroundColor()
        {
            return _bottomColor;
        }

        #endregion

    }
}
