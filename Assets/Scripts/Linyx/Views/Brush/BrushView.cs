using Linyx.Models;
using Linyx.Models.Line;
using Linyx.Utils;
using MaterialUI;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace Linyx.Views.Brush
{
    public class BrushView : BaseView
    {
        #region UI

        [SerializeField] private VerticalLayoutGroup _verticalLayoutGroup;

        [Header("Brush Width")]
        [SerializeField] private MaterialButton _widthCurveButton;

        [Header("Brush Color")]
        [SerializeField] private MaterialButton _gradientButton;
        [SerializeField] private RawImage _gradientImage;

        [Header("Emission")]
        [SerializeField] private MaterialSwitch _emissionSwitch;
        [SerializeField] private MaterialSlider _brushEmissionSlider;
        [SerializeField] private MaterialButton _brushEmissionColorButton;

        [Header("Shapes")]
        [SerializeField] private MaterialDropdown _brushShapeDropdown;
        [SerializeField] private MaterialSlider _brushShapeSizeSlider;

        #endregion

        #region Local Signals

        public Signal<float> BrushEmissionValueChangedSignal = new Signal<float>();
        public Signal BrushWidthCurveRequestSignal = new Signal();
        public Signal BrushGradientRequestedSignal = new Signal();
        public Signal BrushEmissionColorRequestedSignal = new Signal();
        public Signal<bool> EmissionValueChangedSignal = new Signal<bool>();

        public Signal<int> BrushShapeIndexValueChangedSignal = new Signal<int>();
        public Signal<float> BrushShapeInitialSizeValueChangedSignal = new Signal<float>();

        #endregion

        #region Public Methods

        public override void Initialize()
        {
            base.Initialize();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayoutGroup.GetComponent<RectTransform>());

            _widthCurveButton.buttonObject.onClick.AddListener(OnBrushWidthCurveClicked);

            _gradientButton.buttonObject.onClick.AddListener(OnBrushGradientClicked);
            _brushEmissionSlider.slider.onValueChanged.AddListener(OnBrushEmissionValueChanged);
            _emissionSwitch.toggle.onValueChanged.AddListener(OnEmissionValueChanged);

            _brushShapeDropdown.onItemSelected.AddListener(OnBrushShapeItemSelected);
            _brushShapeSizeSlider.slider.onValueChanged.AddListener(OnBrushSizeValueChanged);
            _brushEmissionColorButton.buttonObject.onClick.AddListener(OnBrushEmissionColorClicked);
            CreateTextureGradient();
        }

        public void UpdateSettings(LineModel line)
        {
            _brushEmissionColorButton.backgroundColor = line.EmissionColor;
            _brushShapeDropdown.Select((int)line.Shape);
            _emissionSwitch.toggle.isOn = line.IsEmissionEnabled;
            UpdateGradient(line.Gradient);
        }

        public void UpdateGradient(Gradient gradient)
        {
            Texture2D tex = (Texture2D) _gradientImage.texture;
            int width = tex.width;
            int height = tex.height;
            Color[] pixels = new Color[width * height];

            for (int x = 0; x < width; x++)
            {
                float normX = (float) x / (width - 1);
                for (int y = 0; y < height; y++)
                {
                    Color color = gradient.Evaluate(normX);
                    int pixelIndex = x + y * width;
                    pixels[pixelIndex] = color;
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
        }

        public void SetEmissionColor(Color color)
        {
            _brushEmissionColorButton.backgroundColor = color;
        }

        #endregion

        #region Private Methods

        private void CreateTextureGradient()
        {
            Vector2 size = _gradientImage.rectTransform.rect.size;
            int width = (int) size.x;
            int height = (int) size.y;
            _gradientImage.texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                name = _gradientImage.name,
                wrapMode = TextureWrapMode.Clamp
            };
        }

        private void OnBrushWidthCurveClicked()
        {
            BrushWidthCurveRequestSignal.Dispatch();
        }


        private void OnBrushEmissionValueChanged(float value)
        {
            BrushEmissionValueChangedSignal.Dispatch(value);
        }

        private void OnEmissionValueChanged(bool enabled)
        {
            EmissionValueChangedSignal.Dispatch(enabled);
            _brushEmissionSlider.transform.parent.gameObject.SetActive(enabled);
            _brushEmissionColorButton.transform.parent.gameObject.SetActive(enabled);
        }

        private void OnBrushEmissionColorClicked()
        {
            BrushEmissionColorRequestedSignal.Dispatch();
        }

        private void OnBrushGradientClicked()
        {
            BrushGradientRequestedSignal.Dispatch();
        }

        private void OnBrushShapeItemSelected(int index)
        {
            if (index == -1)
                return;
            _brushShapeSizeSlider.gameObject.SetActive(index != 0);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayoutGroup.GetComponent<RectTransform>());
            BrushShapeIndexValueChangedSignal.Dispatch(index);
        }

        private void OnBrushSizeValueChanged(float value)
        {
            BrushShapeInitialSizeValueChangedSignal.Dispatch(value);
        }

        #endregion



    }
}