using Linyx.Models.Koch;
using Linyx.Models.Line;
using Linyx.Services.Audio;
using Linyx.Services.Brush;
using MaterialUI;
using strange.extensions.signal.impl;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Linyx.Views.Edit
{
    public sealed class EditView : BaseView
    {
        #region UI
        [SerializeField] private VerticalLayoutGroup _verticalLayoutGroup;

        [Header("Main Panels")]
        [SerializeField] private GameObject _audioPanel;
        [SerializeField] private GameObject _phyllotaxisPanel;

        [Header("Main Config")]
        [SerializeField] private MaterialInputField _titleInputField;
        [SerializeField] private MaterialSlider _layerSlider;
        [SerializeField] private MaterialButton _widthCurveButton;
        [SerializeField] private MaterialButton _brushGradientButton;
        [SerializeField] private RawImage _gradientImage;
        [SerializeField] private MaterialSwitch _emissionSwitch;
        [SerializeField] private MaterialSlider _brushEmissionSlider;
        [SerializeField] private MaterialButton _brushEmissionColorButton;

        [Header("Emission React")]
        [SerializeField] private MaterialSwitch _emissionReactSwitch;
        [SerializeField] private MaterialSlider _emissionThresholdSlider;
        [SerializeField] private MaterialSlider _emissionBandBufferSlider;
        [SerializeField] private MaterialDropdown _emissionFrequencyDropdown;

        [Header("Scale React")]
        [SerializeField] private MaterialSwitch _scaleReactSwitch;
        [SerializeField] private MaterialSlider _scaleMultiplierSlider;
        [SerializeField] private MaterialSlider _scaleBandBufferSlider;
        [SerializeField] private MaterialSlider _scaleThresholdSlider;
        [SerializeField] private MaterialDropdown _scaleFrequencyDropdown;

        [Header("Koch React")]
        [SerializeField] private MaterialSwitch _kochReactSwitch;
        [SerializeField] private VerticalLayoutGroup _startGenerationList;
        [SerializeField] private StartGenerationItem _startGenerationItem;
        [SerializeField] private MaterialButton _addElementButton;
        [SerializeField] private MaterialButton _showCurveEditorButton;
        [SerializeField] private MaterialCheckbox _useBezierCurvesCheckbox;
        [SerializeField] private MaterialSlider _bezierVertexCountSlider;
        [SerializeField] private VerticalLayoutGroup _audioBandLayoutGroup;
        [SerializeField] private MaterialSlider _audioBandSlider;

        [Header("Trail React")]
        [SerializeField] private MaterialSwitch _trailSwitch;
        [SerializeField] private VerticalLayoutGroup _trailsMinMaxParent;

        [Header("Trail Speed")]
        [SerializeField] private MaterialSlider _trailMinSpeedSlider;
        [SerializeField] private MaterialSlider _trailMaxSpeedSlider;

        [Header("Trail Time")]
        [SerializeField] private MaterialSlider _trailMinTimeSlider;
        [SerializeField] private MaterialSlider _trailMaxTimeSlider;

        [Header("Trail Width")]
        [SerializeField] private MaterialSlider _trailMinWidthSlider;
        [SerializeField] private MaterialSlider _trailMaxWidthSlider;

        [Space(2)]
        [Header("Phyllotaxis")]
        [Space(2)]

        [Header("Setup")]
        [SerializeField] private MaterialSlider _degreeSlider;
        [SerializeField] private MaterialSlider _scaleSlider;
        [SerializeField] private MaterialSlider _numberStartSlider;
        [SerializeField] private MaterialSlider _stepSizeSlider;
        [SerializeField] private MaterialSlider _maxIterationSlider;

        [Header("Lerp Position")]
        [SerializeField] private MaterialCheckbox _useLerpingCheckbox;
        [SerializeField] private MaterialDropdown _lerpFrequencyDropdown;
        [SerializeField] private MaterialSlider _lerpBandSlider;
        [SerializeField] private MaterialSlider _speedMinSlider;
        [SerializeField] private MaterialSlider _speedMaxSlider;
        [SerializeField] private MaterialButton _lerpInterpolationButton;
        [SerializeField] private MaterialCheckbox _repeatCheckbox;
        [SerializeField] private MaterialCheckbox _invertCheckbox;

        [Header("Scale")]
        [SerializeField] private MaterialCheckbox _useScaleCheckbox;
        [SerializeField] private MaterialDropdown _scalePhylloFrequencyDropdown;
        [SerializeField] private MaterialSlider _scaleBandSlider;
        [SerializeField] private MaterialSlider _scaleMinSlider;
        [SerializeField] private MaterialSlider _scaleMaxSlider;
        [SerializeField] private MaterialCheckbox _useScaleCurveCheckbox;
        [SerializeField] private MaterialButton _scaleInterpolationButton;
        [SerializeField] private MaterialSlider _scaleSpeedSlider;
        #endregion

        #region Private Attributes

        private bool _emitEvent = true;
        private LineModel _lineModel;
        private List<StartGenerationItem> _generationItems = new List<StartGenerationItem>();
        private List<MaterialSlider> _audioBands = new List<MaterialSlider>();
        #endregion

        #region Local Signals
        public Signal<LineModel> UpdateLineSignal = new Signal<LineModel>();
        public Signal<LineModel> BrushGradientSignal = new Signal<LineModel>();
        public Signal<LineModel> BrushEmissionColorSignal = new Signal<LineModel>();
        public Signal<LineModel> ShowKochCurveEditorSignal = new Signal<LineModel>();
        public Signal<LineModel> ShowWidthCurveEditorSignal = new Signal<LineModel>();
        public Signal<LineModel> ShowLerpInterpolationCurveEditorSignal = new Signal<LineModel>();
        public Signal<LineModel> ShowScaleInterpolationCurveEditorSignal = new Signal<LineModel>();
        #endregion

        #region Public Methods

        public override void Initialize()
        {
            base.Initialize();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayoutGroup.GetComponent<RectTransform>());

            _layerSlider.slider.onValueChanged.AddListener(OnLayerValueChanged);
            _titleInputField.inputField.onValueChanged.AddListener(OnTitleValueChanged);

            _widthCurveButton.buttonObject.onClick.AddListener(OnWidthCurveClicked);

            _brushEmissionSlider.slider.onValueChanged.AddListener(OnBrushEmissionValueChanged);
            _emissionSwitch.toggle.onValueChanged.AddListener(OnEmissionValueChanged);
            _brushEmissionColorButton.buttonObject.onClick.AddListener(OnBrushEmissionColorClicked);

            _brushGradientButton.buttonObject.onClick.AddListener(OnBrushGradientClicked);

            _emissionReactSwitch.toggle.onValueChanged.AddListener(OnEmissionReactValueChanged);
            _emissionThresholdSlider.slider.onValueChanged.AddListener(OnThresholdEmissionValueChanged);
            _emissionBandBufferSlider.slider.onValueChanged.AddListener(OnEmissionBandBufferValueChanged);
            _emissionFrequencyDropdown.onItemSelected.AddListener(OnEmissionFrequencyItemSelected);

            _scaleReactSwitch.toggle.onValueChanged.AddListener(OnScaleReactValueChanged);
            _scaleMultiplierSlider.slider.onValueChanged.AddListener(OnScaleMultiplierValueChanged);
            _scaleBandBufferSlider.slider.onValueChanged.AddListener(OnScaleBandBufferValueChanged);
            _scaleThresholdSlider.slider.onValueChanged.AddListener(OnScaleThresholdValueChanged);
            _scaleFrequencyDropdown.onItemSelected.AddListener(OnScaleFrequencyItemSelected);

            _kochReactSwitch.toggle.onValueChanged.AddListener(OnKochReactValueChanged);
            _addElementButton.buttonObject.onClick.AddListener(OnAddElementClicked);
            _showCurveEditorButton.buttonObject.onClick.AddListener(OnShowCurveEditorClicked);
            _useBezierCurvesCheckbox.toggle.onValueChanged.AddListener(OnUseBezierCurvesValueChanged);
            _bezierVertexCountSlider.slider.onValueChanged.AddListener(OnBezierVertexCountValueChanged);

            _trailSwitch.toggle.onValueChanged.AddListener(OnTrailValueChanged);
            _trailMinSpeedSlider.slider.onValueChanged.AddListener(OnTrailMinSpeedValueChanged);
            _trailMaxSpeedSlider.slider.onValueChanged.AddListener(OnTrailMaxSpeedValueChanged);

            _trailMinTimeSlider.slider.onValueChanged.AddListener(OnTrailMinTimeValueChanged);
            _trailMaxTimeSlider.slider.onValueChanged.AddListener(OnTrailMaxTimeValueChanged);

            _trailMinWidthSlider.slider.onValueChanged.AddListener(OnTrailMinWidthValueChanged);
            _trailMaxWidthSlider.slider.onValueChanged.AddListener(OnTrailMaxWidthValueChanged);


            _degreeSlider.slider.onValueChanged.AddListener(OnDegreeValueChanged);
            _scaleSlider.slider.onValueChanged.AddListener(OnScaleValueChanged);
            _numberStartSlider.slider.onValueChanged.AddListener(OnNumberStartValueChanged);
            _stepSizeSlider.slider.onValueChanged.AddListener(OnStepSizeValueChanged);
            _maxIterationSlider.slider.onValueChanged.AddListener(OnMaxIterationsValueChanged);

            _useLerpingCheckbox.toggle.onValueChanged.AddListener(OnUseLerpingValueChanged);
            _lerpFrequencyDropdown.onItemSelected.AddListener(OnLerpFrequencyItemSelected);
            _lerpBandSlider.slider.onValueChanged.AddListener(OnLerpBandValueChanged);
            _speedMinSlider.slider.onValueChanged.AddListener(OnSpeedMinValueChanged);
            _speedMaxSlider.slider.onValueChanged.AddListener(OnSpeedMaxValueChanged);
            _repeatCheckbox.toggle.onValueChanged.AddListener(OnRepeatValueChanged);
            _invertCheckbox.toggle.onValueChanged.AddListener(OnInvertValueChanged);
            _lerpInterpolationButton.buttonObject.onClick.AddListener(OnLerpInterpolationClicked);

            _useScaleCheckbox.toggle.onValueChanged.AddListener(OnUseScaleValueChanged);
            _scalePhylloFrequencyDropdown.onItemSelected.AddListener(OnScalePhylloFrequencyItemSelected);
            _scaleBandSlider.slider.onValueChanged.AddListener(OnScalePhylloBandValueChanged);
            _scaleMinSlider.slider.onValueChanged.AddListener(OnScaleMinValueChanged);
            _scaleMaxSlider.slider.onValueChanged.AddListener(OnScaleMaxValueChanged);
            _useScaleCurveCheckbox.toggle.onValueChanged.AddListener(OnUseScaleCurveValueChanged);
            _scaleSpeedSlider.slider.onValueChanged.AddListener(OnScaleSpeedValueChanged);
            _scaleInterpolationButton.buttonObject.onClick.AddListener(OnScaleInterpolationClicked);

            CreateTextureGradient();
        }



        public bool CanEmitEvent()
        {
            return _emitEvent;
        }

        public void Refresh(LineModel lineModel)
        {
            _emitEvent = false;
            _lineModel = lineModel;
            _titleInputField.inputField.text = lineModel.DisplayName;
            _layerSlider.slider.value = lineModel.Layer;

            _emissionSwitch.toggle.isOn = lineModel.IsEmissionEnabled;
            _brushEmissionSlider.slider.value = lineModel.EmissionIntensity;
            _brushEmissionColorButton.backgroundColor = lineModel.EmissionColor;

            _audioPanel.SetActive(lineModel.Shape != Shape.Phyllotaxis);
            _phyllotaxisPanel.SetActive(lineModel.Shape == Shape.Phyllotaxis);

            _emissionReactSwitch.toggle.isOn = lineModel.EmissionProperty.IsEmissionReactOnAudio;
            _emissionThresholdSlider.slider.value = (lineModel.EmissionProperty.EmissionThreshold);
            _emissionBandBufferSlider.slider.value = (lineModel.EmissionProperty.EmissionBandBuffer);
            _emissionFrequencyDropdown.currentlySelected = (int)lineModel.EmissionProperty.EmissionFrequencyType;

            _scaleReactSwitch.toggle.isOn = lineModel.ScaleProperty.IsScaleReactOnAudio;
            _scaleMultiplierSlider.slider.value = (lineModel.ScaleProperty.ScaleMultiplier);
            _scaleBandBufferSlider.slider.value = (lineModel.ScaleProperty.ScaleBandBuffer);

            _brushEmissionSlider.gameObject.SetActive(lineModel.IsEmissionEnabled);
            _emissionThresholdSlider.gameObject.SetActive(lineModel.EmissionProperty.IsEmissionReactOnAudio);
            _emissionBandBufferSlider.gameObject.SetActive(lineModel.EmissionProperty.IsEmissionReactOnAudio);
            _emissionFrequencyDropdown.gameObject.SetActive(lineModel.EmissionProperty.IsEmissionReactOnAudio);

            _scaleMultiplierSlider.gameObject.SetActive(lineModel.ScaleProperty.IsScaleReactOnAudio);
            _scaleBandBufferSlider.gameObject.SetActive(lineModel.ScaleProperty.IsScaleReactOnAudio);
            _scaleThresholdSlider.gameObject.SetActive(lineModel.ScaleProperty.IsScaleReactOnAudio);
            _scaleFrequencyDropdown.gameObject.SetActive(lineModel.ScaleProperty.IsScaleReactOnAudio);

            _kochReactSwitch.toggle.isOn = lineModel.KochLineProperty.IsKochEnabled;
            _kochReactSwitch.gameObject.SetActive(lineModel.Shape != Shape.Line);

            _startGenerationList.transform.parent.parent.gameObject.SetActive(lineModel.Shape != Shape.Line && lineModel.KochLineProperty.IsKochEnabled);
            _audioBandLayoutGroup.transform.parent.parent.gameObject.SetActive(lineModel.Shape != Shape.Line && lineModel.KochLineProperty.IsKochEnabled);

            _trailSwitch.toggle.isOn = _lineModel.KochTrailProperty.IsTrailEnabled;
            _trailSwitch.gameObject.SetActive(lineModel.Shape != Shape.Line && lineModel.KochLineProperty.IsKochEnabled);

            _trailsMinMaxParent.gameObject.SetActive(lineModel.Shape != Shape.Line && lineModel.KochLineProperty.IsKochEnabled && lineModel.KochTrailProperty.IsTrailEnabled);
            _trailMinSpeedSlider.slider.value = (lineModel.KochTrailProperty.TrailSpeedMinMax.x);
            _trailMaxSpeedSlider.slider.value = (lineModel.KochTrailProperty.TrailSpeedMinMax.y);

            _trailMinTimeSlider.slider.value = (lineModel.KochTrailProperty.TrailTimeMinMax.x);
            _trailMaxTimeSlider.slider.value = (lineModel.KochTrailProperty.TrailTimeMinMax.y);

            _trailMinWidthSlider.slider.value = (lineModel.KochTrailProperty.TrailWidthMinMax.x);
            _trailMaxWidthSlider.slider.value = (lineModel.KochTrailProperty.TrailWidthMinMax.y);

            _degreeSlider.slider.value = lineModel.PhyllotaxisProperty.Degree;
            _scaleSlider.slider.value = lineModel.PhyllotaxisProperty.Scale;
            _numberStartSlider.slider.value = lineModel.PhyllotaxisProperty.NumberStart;
            _stepSizeSlider.slider.value = lineModel.PhyllotaxisProperty.StepSize;
            _maxIterationSlider.slider.value = lineModel.PhyllotaxisProperty.MaxIterations;

            _useLerpingCheckbox.toggle.isOn = lineModel.PhyllotaxisProperty.UseLerping;
            _lerpFrequencyDropdown.currentlySelected = (int)lineModel.PhyllotaxisProperty.LerpFrequencyType;
            _lerpBandSlider.slider.value = lineModel.PhyllotaxisProperty.LerpAudioBand;
            _speedMinSlider.slider.value = lineModel.PhyllotaxisProperty.SpeedMinMax.x;
            _speedMaxSlider.slider.value = lineModel.PhyllotaxisProperty.SpeedMinMax.y;
            _repeatCheckbox.toggle.isOn = lineModel.PhyllotaxisProperty.Repeat;
            _invertCheckbox.toggle.isOn = lineModel.PhyllotaxisProperty.Invert;

            _useScaleCheckbox.toggle.isOn = lineModel.PhyllotaxisProperty.UseScaling;
            _scalePhylloFrequencyDropdown.currentlySelected = (int)lineModel.PhyllotaxisProperty.ScaleFrequencyType;
            _scaleBandSlider.slider.value = lineModel.PhyllotaxisProperty.ScaleAudioBand;
            _scaleMinSlider.slider.value = lineModel.PhyllotaxisProperty.ScaleMinMax.x;
            _scaleMaxSlider.slider.value = lineModel.PhyllotaxisProperty.ScaleMinMax.y;
            _useScaleCurveCheckbox.toggle.isOn = lineModel.PhyllotaxisProperty.UseScaleCurve;
            _scaleSpeedSlider.slider.value = lineModel.PhyllotaxisProperty.InterpolationSpeed;

            _audioBands.Clear();
            int audioBandSliderToGenerate = (int)lineModel.Shape + 2;

            foreach (Transform child in _audioBandLayoutGroup.transform)
            {
                Destroy(child.gameObject);
            }

            if (lineModel.Shape != Shape.Phyllotaxis)
            {
                for (int i = 0; i < audioBandSliderToGenerate; i++)
                {
                    MaterialSlider slider = Instantiate(_audioBandSlider, _audioBandLayoutGroup.transform, false);
                    slider.leftContentTransform.GetComponentInChildren<Text>().text = i.ToString();
                    slider.slider.value = (lineModel.KochLineProperty.KochAudioBand[i]);
                    slider.slider.onValueChanged.AddListener(OnAudioBandValueChanged);
                    _audioBands.Add(slider);
                }
            }

            RefreshKochStartGeneration();
            _emitEvent = true;
            UpdateGradient(lineModel.Gradient);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayoutGroup.GetComponent<RectTransform>());
        }

        public void UpdateGradient(Gradient gradient)
        {
            Texture2D tex = (Texture2D)_gradientImage.texture;
            int width = tex.width;
            int height = tex.height;
            Color[] pixels = new Color[width * height];

            for (int x = 0; x < width; x++)
            {
                float normX = (float)x / (width - 1);
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

        public void UpdateEmissionColor(Color color)
        {
            _brushEmissionColorButton.backgroundColor = color;
        }
        #endregion

        #region Private Methods

        private void CreateTextureGradient()
        {
            Vector2 size = _gradientImage.rectTransform.rect.size;
            int width = (int)size.x;
            int height = (int)size.y;
            _gradientImage.texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                name = _gradientImage.name,
                wrapMode = TextureWrapMode.Clamp
            };
        }

        private void UpdateLineModel(LineModel lineModel)
        {
            _lineModel = lineModel;
        }

        private void OnLayerValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.Layer = (int)value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnTitleValueChanged(string value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.DisplayName = value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnWidthCurveClicked()
        {
            ShowWidthCurveEditorSignal.Dispatch(_lineModel);
        }

        private void OnBrushEmissionValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.EmissionIntensity = value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnEmissionValueChanged(bool enabled)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.IsEmissionEnabled = enabled;
            UpdateLineModel(newLineModel);
            _brushEmissionSlider.gameObject.SetActive(enabled);
            _brushEmissionColorButton.transform.parent.gameObject.SetActive(enabled);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayoutGroup.GetComponent<RectTransform>());
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnBrushEmissionColorClicked()
        {
            BrushEmissionColorSignal.Dispatch(_lineModel);
        }

        private void OnBrushGradientClicked()
        {
            BrushGradientSignal.Dispatch(_lineModel);
        }

        private void OnEmissionReactValueChanged(bool enabled)
        {
            LineModel newLineModel = _lineModel.DeepCopy();

            if (enabled)
            {
                newLineModel.IsEmissionEnabled = true;
                _emissionSwitch.toggle.isOn = true;
            }

            newLineModel.EmissionProperty.IsEmissionReactOnAudio = enabled;
            UpdateLineModel(newLineModel);
            _emissionThresholdSlider.gameObject.SetActive(enabled);
            _emissionBandBufferSlider.gameObject.SetActive(enabled);
            _emissionFrequencyDropdown.gameObject.SetActive(enabled);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayoutGroup.GetComponent<RectTransform>());
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnThresholdEmissionValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.EmissionProperty.EmissionThreshold = value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnEmissionBandBufferValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.EmissionProperty.EmissionBandBuffer = (int)value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnEmissionFrequencyItemSelected(int index)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.EmissionProperty.EmissionFrequencyType = (AudioFrequencyType)index;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnScaleReactValueChanged(bool value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.ScaleProperty.IsScaleReactOnAudio = value;
            UpdateLineModel(newLineModel);
            _scaleBandBufferSlider.gameObject.SetActive(value);
            _scaleMultiplierSlider.gameObject.SetActive(value);
            _scaleThresholdSlider.gameObject.SetActive(value);
            _scaleFrequencyDropdown.gameObject.SetActive(value);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayoutGroup.GetComponent<RectTransform>());
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnScaleMultiplierValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.ScaleProperty.ScaleMultiplier = value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnScaleBandBufferValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.ScaleProperty.ScaleBandBuffer = (int)value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnScaleThresholdValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.ScaleProperty.ScaleThreshold = value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnScaleFrequencyItemSelected(int value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.ScaleProperty.ScaleFrequencyType = (AudioFrequencyType)value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnKochReactValueChanged(bool value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.KochLineProperty.IsKochEnabled = value;
            UpdateLineModel(newLineModel);
            _startGenerationList.transform.parent.parent.gameObject.SetActive(value);
            _audioBandLayoutGroup.transform.parent.parent.gameObject.SetActive(value);
            _trailSwitch.gameObject.SetActive(value);
            _trailsMinMaxParent.gameObject.SetActive(value && _lineModel.KochTrailProperty.IsTrailEnabled);
            RefreshKochStartGeneration();
            LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayoutGroup.GetComponent<RectTransform>());
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnAddElementClicked()
        {
            LineModel newLineModel = _lineModel.DeepCopy();

            StartGenerationItem startGenerationItem =
                Instantiate(_startGenerationItem, _startGenerationList.transform, false);
            string guid = Guid.NewGuid().ToString("N");
            startGenerationItem.Initialize("Element " + _generationItems.Count, guid, OnDeleteStartGen, OnUpdateStartGen);
            _generationItems.Add(startGenerationItem);
            List<StartGen> startGens = new List<StartGen>();
            startGens.AddRange(newLineModel.KochLineProperty.ListStartGeneration);
            startGens.Add(new StartGen() { Outwards = false, Scale = 1, Guid = guid });
            newLineModel.KochLineProperty.ListStartGeneration = startGens;
            UpdateLineModel(newLineModel);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayoutGroup.GetComponent<RectTransform>());
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnDeleteStartGen(StartGenerationItem startGenerationItem)
        {
            if (_generationItems.Count > 1)
            {
                LineModel newLineModel = _lineModel.DeepCopy();
                List<StartGen> startGens = new List<StartGen>();
                startGens.AddRange(newLineModel.KochLineProperty.ListStartGeneration);
                startGens.RemoveAll(t => t.Guid == startGenerationItem.Guid);
                _generationItems.RemoveAll(t => t.Guid == startGenerationItem.Guid);
                Destroy(startGenerationItem.gameObject);
                for (int i = 0; i < _generationItems.Count; i++)
                {
                    StartGenerationItem item = _generationItems[i];
                    item.SetTitle("Element " + i);
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayoutGroup.GetComponent<RectTransform>());
                newLineModel.KochLineProperty.ListStartGeneration = startGens;
                UpdateLineModel(newLineModel);
                UpdateLineSignal.Dispatch(newLineModel);
            }
        }

        private void OnUpdateStartGen(StartGenerationItem startGenerationItem)
        {
            LineModel newLineModel = _lineModel.DeepCopy();

            int index = newLineModel.KochLineProperty.ListStartGeneration.FindIndex(t => t.Guid == startGenerationItem.Guid);
            List<StartGen> startGens = new List<StartGen>();
            startGens.AddRange(newLineModel.KochLineProperty.ListStartGeneration);

            StartGen startGen = startGens[index].DeepCopy();
            startGen.Outwards = startGenerationItem.IsOutwards;
            startGen.Scale = startGenerationItem.Scale;

            startGens[index] = startGen;
            newLineModel.KochLineProperty.ListStartGeneration = startGens;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnShowCurveEditorClicked()
        {
            ShowKochCurveEditorSignal.Dispatch(_lineModel);
        }

        private void OnUseBezierCurvesValueChanged(bool enabled)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.KochLineProperty.UseBezierCurves = enabled;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnBezierVertexCountValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.KochLineProperty.BezierVertexCount = (int)value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnAudioBandValueChanged(float value)
        {
            //This method is called from each slider for audio band. 
            //We have to update all fields
            LineModel newLineModel = _lineModel.DeepCopy();

            List<int> values = new List<int>();
            foreach (MaterialSlider slider in _audioBands)
            {
                values.Add((int)slider.slider.value);
            }

            newLineModel.KochLineProperty.KochAudioBand = values;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }


        private void OnTrailValueChanged(bool value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.KochTrailProperty.IsTrailEnabled = value;
            UpdateLineModel(newLineModel);

            _trailsMinMaxParent.gameObject.SetActive(value);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayoutGroup.GetComponent<RectTransform>());
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnTrailMinSpeedValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();

            newLineModel.KochTrailProperty.TrailSpeedMinMax = new Vector2(value, newLineModel.KochTrailProperty.TrailSpeedMinMax.y);
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnTrailMaxSpeedValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.KochTrailProperty.TrailSpeedMinMax = new Vector2(newLineModel.KochTrailProperty.TrailSpeedMinMax.x, value);
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnTrailMinTimeValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();

            newLineModel.KochTrailProperty.TrailTimeMinMax = new Vector2(value, newLineModel.KochTrailProperty.TrailTimeMinMax.y);
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnTrailMaxTimeValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.KochTrailProperty.TrailTimeMinMax = new Vector2(newLineModel.KochTrailProperty.TrailTimeMinMax.x, value);
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnTrailMinWidthValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.KochTrailProperty.TrailWidthMinMax = new Vector2(value, newLineModel.KochTrailProperty.TrailWidthMinMax.y);
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnTrailMaxWidthValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.KochTrailProperty.TrailWidthMinMax = new Vector2(newLineModel.KochTrailProperty.TrailWidthMinMax.x, value);
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void RefreshKochStartGeneration()
        {
            foreach (Transform child in _startGenerationList.transform)
            {
                Destroy(child.gameObject);
            }

            _generationItems.Clear();
            for (int i = 0; i < _lineModel.KochLineProperty.ListStartGeneration.Count; i++)
            {
                StartGen startGen = _lineModel.KochLineProperty.ListStartGeneration[i];
                StartGenerationItem startGenerationItem =
                    Instantiate(_startGenerationItem, _startGenerationList.transform, false);
                startGenerationItem.Initialize("Element " + i, startGen.Guid, OnDeleteStartGen, OnUpdateStartGen);
                startGenerationItem.SetOutwards(startGen.Outwards);
                startGenerationItem.SetScale(startGen.Scale);
                _generationItems.Add(startGenerationItem);
            }
        }

        private void OnScaleSpeedValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.PhyllotaxisProperty.InterpolationSpeed = value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnUseScaleCurveValueChanged(bool value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.PhyllotaxisProperty.UseScaleCurve = value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnScaleMaxValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.PhyllotaxisProperty.ScaleMinMax = new Vector2(newLineModel.PhyllotaxisProperty.ScaleMinMax.y, value);
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnScaleMinValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.PhyllotaxisProperty.ScaleMinMax = new Vector2(value, newLineModel.PhyllotaxisProperty.ScaleMinMax.y);
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnScalePhylloBandValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.PhyllotaxisProperty.ScaleAudioBand = (int)value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnScalePhylloFrequencyItemSelected(int value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.PhyllotaxisProperty.ScaleFrequencyType = (AudioFrequencyType) value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnUseScaleValueChanged(bool value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.PhyllotaxisProperty.UseScaling = value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnInvertValueChanged(bool value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.PhyllotaxisProperty.Invert = value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnRepeatValueChanged(bool value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.PhyllotaxisProperty.Repeat = value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnSpeedMaxValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.PhyllotaxisProperty.SpeedMinMax = new Vector2(newLineModel.PhyllotaxisProperty.SpeedMinMax.x, value);
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnSpeedMinValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.PhyllotaxisProperty.SpeedMinMax = new Vector2(value, newLineModel.PhyllotaxisProperty.SpeedMinMax.y);
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnLerpBandValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.PhyllotaxisProperty.LerpAudioBand = (int) value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnLerpFrequencyItemSelected(int value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.PhyllotaxisProperty.LerpFrequencyType = (AudioFrequencyType) value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnUseLerpingValueChanged(bool value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.PhyllotaxisProperty.UseLerping = value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnMaxIterationsValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.PhyllotaxisProperty.MaxIterations = (int) value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnStepSizeValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.PhyllotaxisProperty.StepSize = (int) value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnNumberStartValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.PhyllotaxisProperty.NumberStart = (int) value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnScaleValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.PhyllotaxisProperty.Scale = value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnDegreeValueChanged(float value)
        {
            LineModel newLineModel = _lineModel.DeepCopy();
            newLineModel.PhyllotaxisProperty.Degree = value;
            UpdateLineModel(newLineModel);
            UpdateLineSignal.Dispatch(newLineModel);
        }

        private void OnScaleInterpolationClicked()
        {
            ShowScaleInterpolationCurveEditorSignal.Dispatch(_lineModel);
        }

        private void OnLerpInterpolationClicked()
        {
            ShowLerpInterpolationCurveEditorSignal.Dispatch(_lineModel);
        }

        #endregion
    }
}
