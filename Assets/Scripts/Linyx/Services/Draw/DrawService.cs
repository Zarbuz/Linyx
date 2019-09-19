using System;
using System.Collections.Generic;
using Linyx.Models;
using Linyx.Models.Emission;
using Linyx.Models.Koch;
using Linyx.Models.Line;
using Linyx.Models.Phyllotaxis;
using Linyx.Models.Scale;
using Linyx.Services.Brush;
using Linyx.Services.Curve;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Linyx.Services.Draw
{
    public sealed class DrawService : MonoBehaviour, IDrawService
    {
        #region UI
        [SerializeField] private UnityEngine.Camera _camera;
        [SerializeField] private GameObject _linePrefab;
        [SerializeField] private GameObject _3dRoot;
        [SerializeField] private Material _lineMaterial;

        #endregion

        #region Injections
        [Inject] public IBrushService BrushService { get; set; }
        [Inject] public ICurveService CurveService { get; set; }
        [Inject] public AddLineSignal AddLineSignal { get; set; }
        #endregion

        #region Private Attributes

        private GameObject _currentLine;
        private GameObject _cursorLine;
        private LineRenderer _lineRenderer;
        private LineRenderer _lineCursorRenderer;
        private readonly List<Vector2> _fingerPositions = new List<Vector2>();

        private int _shapePointAmout;
        private float _initialRotation;
        private Vector3[] _shapePoints;
        private Vector3 _rotateVector = new Vector3(1, 0, 0);

        private bool _drawing;
        #endregion

        #region Public Methods
        public void Initialize()
        {

        }

        public GameObject GetLinePrefab()
        {
            return _linePrefab;
        }

        public GameObject GetRoot3D()
        {
            return _3dRoot;
        }

        public Material GetLineMaterial()
        {
            return _lineMaterial;
        }

        #endregion

        #region Unity Methods
        private void Update()
        {
            if (CurveService.IsEditorOpen())
                return;

            if (EventSystem.current.IsPointerOverGameObject(-1))
            {
                if (BrushService.GetBrushShape() == Shape.Line && _drawing)
                {
                    ReleaseLine();
                }
                _cursorLine.gameObject.SetActive(false);
                return;
            }

            DrawCursor();

            if (Input.GetMouseButtonDown(0))
            {
                CreateLine();
            }

            if (Input.GetMouseButton(0) && _lineRenderer != null && BrushService.GetBrushShape() == Shape.Line)
            {
                Vector2 tmpFingerPos = _camera.ScreenToWorldPoint(Input.mousePosition);
                if (Vector2.Distance(tmpFingerPos, _fingerPositions[_fingerPositions.Count - 1]) > 0.1f)
                {
                    UpdateLine(tmpFingerPos);
                }

                _drawing = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                ReleaseLine();
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0 && Input.GetKey(KeyCode.LeftShift))
            {
                _initialRotation++;
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0 && Input.GetKey(KeyCode.LeftShift))
            {
                _initialRotation--;
            }
        }


        #endregion

        #region Private Methods
        private void CreateLine()
        {
            _currentLine = Instantiate(_linePrefab, Vector3.zero, Quaternion.identity);
            _currentLine.transform.SetParent(_3dRoot.transform, false);
            _lineRenderer = _currentLine.GetComponent<LineRenderer>();
            _lineRenderer.sharedMaterial = new Material(_lineMaterial);
            _lineRenderer.sortingOrder = 0;

            if (BrushService.IsEmissionEnabled())
            {
                _lineRenderer.sharedMaterial.EnableKeyword("_EMISSION");
                _lineRenderer.sharedMaterial.SetColor("_EmissionColor", BrushService.GetBrushEmissionColor() * BrushService.GetBrushEmissionIntensity());
            }

            _lineRenderer.colorGradient = BrushService.GetBrushGradient();

            _lineRenderer.widthCurve = BrushService.GetBrushWidthCurve();

            _fingerPositions.Clear();
            _fingerPositions.Add(_camera.ScreenToWorldPoint(Input.mousePosition));
            _fingerPositions.Add(_camera.ScreenToWorldPoint(Input.mousePosition));

            if (BrushService.GetBrushShape() == Shape.Line) 
            {
                _lineRenderer.SetPosition(0, _fingerPositions[0]);
                _lineRenderer.SetPosition(1, _fingerPositions[1]);
            }
            else if (BrushService.GetBrushShape() == Shape.Phyllotaxis)
            {
                _lineRenderer.gameObject.transform.localPosition = _camera.ScreenToWorldPoint((Input.mousePosition));
            }
            else
            {
                _lineRenderer.positionCount = _shapePointAmout + 1;
                _lineRenderer.SetPositions(_shapePoints);
                _lineRenderer.SetPosition(_shapePointAmout, _shapePoints[0]);
            }
        }

        private void CreateCursor()
        {
            _cursorLine = Instantiate(_linePrefab, Vector3.zero, Quaternion.identity);
            _cursorLine.transform.SetParent(_3dRoot.transform, false);
            _lineCursorRenderer = _cursorLine.GetComponent<LineRenderer>();
            _lineCursorRenderer.sharedMaterial = new Material(_lineMaterial);
        }

        private void UpdateLine(Vector3 newFingerPos)
        {
            _fingerPositions.Add(newFingerPos);
            _lineRenderer.positionCount++;
            _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, newFingerPos);
        }

        private void ReleaseLine()
        {
            if (_lineRenderer == null)
                return;
            _drawing = false;
            Vector3[] originalPositions = new Vector3[_lineRenderer.positionCount];
            _lineRenderer.GetPositions(originalPositions);

            ILineModel lineModel = new LineModel
            {
                Guid = Guid.NewGuid().ToString("N"),
                DisplayName = BrushService.GetBrushShape().ToString(),
                Layer = _lineRenderer.sortingOrder,
                WidthCurve = _lineRenderer.widthCurve,
                Gradient = _lineRenderer.colorGradient,
                IsEmissionEnabled = BrushService.IsEmissionEnabled(),
                EmissionIntensity = BrushService.GetBrushEmissionIntensity(),
                LineGameObject = _lineRenderer,
                Shape = BrushService.GetBrushShape(),
                EmissionColor = BrushService.GetBrushEmissionColor(),

                EmissionProperty = new EmissionProperty(),
                ScaleProperty = new ScaleProperty(),
                KochLineProperty = new KochLineProperty(),
                KochTrailProperty = new KochTrailProperty(),
                PhyllotaxisProperty = new PhyllotaxisProperty()
            };

            lineModel.EmissionProperty.IsEmissionReactOnAudio = BrushService.GetBrushEmissionReactOnAudio();
            lineModel.EmissionProperty.EmissionBandBuffer = BrushService.GetBrushEmissionBandBuffer();
            lineModel.EmissionProperty.EmissionThreshold = BrushService.GetBrushEmissionThreshold();
            lineModel.EmissionProperty.EmissionFrequencyType = BrushService.GetBrushEmissionFrequencyType();

            lineModel.ScaleProperty.IsScaleReactOnAudio = BrushService.GetBrushScaleReactOnAudio();
            lineModel.ScaleProperty.ScaleBandBuffer = BrushService.GetBrushScaleBandBuffer();
            lineModel.ScaleProperty.ScaleMultiplier = BrushService.GetBrushScaleMultiplier();
            lineModel.ScaleProperty.ScaleThreshold = BrushService.GetBrushScaleThreshold();
            lineModel.ScaleProperty.ScaleFrequencyType = BrushService.GetBrushScaleFrequencyType();

            lineModel.KochLineProperty.IsKochEnabled = BrushService.GetBrushKochEnabled();
            lineModel.KochLineProperty.ShapePointAmount = _shapePointAmout;
            lineModel.KochLineProperty.ListStartGeneration = BrushService.GetBrushStartGeneration();
            lineModel.KochLineProperty.AnimationCurve = BrushService.GetBrushAnimationCurve();
            lineModel.KochLineProperty.UseBezierCurves = BrushService.GetBrushUseBezierCurves();
            lineModel.KochLineProperty.BezierVertexCount = BrushService.GetBrushBezierVertexCount();
            lineModel.KochLineProperty.KochAudioBand = BrushService.GetBrushKochAudioBand();
            lineModel.KochLineProperty.OriginalPositions = originalPositions;

            lineModel.KochTrailProperty.IsTrailEnabled = BrushService.GetBrushTrailEnabled();
            lineModel.KochTrailProperty.TrailSpeedMinMax = BrushService.GetBrushTrailSpeedMinMax();
            lineModel.KochTrailProperty.TrailTimeMinMax = BrushService.GetBrushTrailTimeMinMax();
            lineModel.KochTrailProperty.TrailWidthMinMax = BrushService.GetBrushTrailWidthMinMax();

            lineModel.PhyllotaxisProperty.Degree = BrushService.GetBrushDegree();
            lineModel.PhyllotaxisProperty.Scale = BrushService.GetBrushScale();
            lineModel.PhyllotaxisProperty.NumberStart = BrushService.GetBrushNumberStart();
            lineModel.PhyllotaxisProperty.StepSize = BrushService.GetBrushStepSize();
            lineModel.PhyllotaxisProperty.MaxIterations = BrushService.GetBrushMaxIterations();
            lineModel.PhyllotaxisProperty.UseLerping = BrushService.GetBrushUseLerping();
            lineModel.PhyllotaxisProperty.LerpFrequencyType = BrushService.GetBrushLerpFrequencyType();
            lineModel.PhyllotaxisProperty.LerpAudioBand = BrushService.GetBrushLerpAudioBand();
            lineModel.PhyllotaxisProperty.SpeedMinMax = BrushService.GetBrushSpeedMinMax();
            lineModel.PhyllotaxisProperty.LerpInterpolationCurve = BrushService.GetBrushLerpInterpolationCurve();
            lineModel.PhyllotaxisProperty.Repeat = BrushService.GetBrushRepeat();
            lineModel.PhyllotaxisProperty.Invert = BrushService.GetBrushInvert();
            lineModel.PhyllotaxisProperty.UseScaling = BrushService.GetBrushUseScaling();
            lineModel.PhyllotaxisProperty.ScaleFrequencyType = BrushService.GetBrushScalePhylloFrequencyType();
            lineModel.PhyllotaxisProperty.ScaleAudioBand = BrushService.GetBrushScaleAudioBand();
            lineModel.PhyllotaxisProperty.ScaleMinMax = BrushService.GetBrushScaleMinMax();
            lineModel.PhyllotaxisProperty.UseScaleCurve = BrushService.GetBrushUseScaleCurve();
            lineModel.PhyllotaxisProperty.ScaleInterpolationCurve = BrushService.GetBrushScaleInterpolationCurve();
            lineModel.PhyllotaxisProperty.InterpolationSpeed = BrushService.GetBrushInterpolationSpeed();

            AddLineSignal.Dispatch(lineModel);
        }

        private void DrawCursor()
        {
            if (_lineCursorRenderer == null)
                CreateCursor();

            _cursorLine.gameObject.SetActive(true);

            if (BrushService.IsEmissionEnabled())
            {
                _lineCursorRenderer.sharedMaterial.EnableKeyword("_EMISSION");
                _lineCursorRenderer.sharedMaterial.SetColor("_EmissionColor", BrushService.GetBrushEmissionColor() * BrushService.GetBrushEmissionIntensity());
            }

            _lineCursorRenderer.colorGradient = BrushService.GetBrushGradient();

            _lineCursorRenderer.widthCurve = BrushService.GetBrushWidthCurve();


            switch (BrushService.GetBrushShape())
            {
                case Shape.Line:
                case Shape.Phyllotaxis:
                    _lineCursorRenderer.positionCount = 2;
                    _lineCursorRenderer.SetPosition(0, _camera.ScreenToWorldPoint(Input.mousePosition));
                    _lineCursorRenderer.SetPosition(1, _camera.ScreenToWorldPoint(Input.mousePosition));
                    return;
                case Shape.Triangle:
                    _shapePointAmout = 3;
                    break;
                case Shape.Square:
                    _shapePointAmout = 4;
                    break;
                case Shape.Pentagon:
                    _shapePointAmout = 5;
                    break;
                case Shape.Hexagon:
                    _shapePointAmout = 6;
                    break;
                case Shape.Heptagon:
                    _shapePointAmout = 7;
                    break;
                case Shape.Octagon:
                    _shapePointAmout = 8;
                    break;
            }

            _shapePoints = new Vector3[_shapePointAmout];
            _rotateVector = Quaternion.AngleAxis(_initialRotation, new Vector3(0, 0, 1)) * new Vector3(1, 0, 0);

            for (int i = 0; i < _shapePointAmout; i++)
            {
                _shapePoints[i] = (_rotateVector *
                                   BrushService.GetBrushShapeInitSize()) + _camera.ScreenToWorldPoint(Input.mousePosition);
                _rotateVector = Quaternion.AngleAxis(360 / _shapePointAmout, new Vector3(0, 0, 1)) * _rotateVector;
            }

            _lineCursorRenderer.positionCount = _shapePointAmout + 1;
            for (int i = 0; i < _shapePointAmout; i++)
            {
                _lineCursorRenderer.SetPosition(i, _shapePoints[i]);
            }
            _lineCursorRenderer.SetPosition(_shapePointAmout, _shapePoints[0]);
        }


        #endregion
       
    }


    public sealed class AddLineSignal : Signal<ILineModel> { }
    public sealed class LineAddedSignal : Signal<ILineModel> { }
}
