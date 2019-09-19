using System;
using Knife.PostProcessing;
using Linyx.Controllers.Project;
using Linyx.Models;
using Linyx.Services.Audio;
using Linyx.Services.Brush;
using Linyx.Services.Draw;
using MaterialUI;
using System.Collections.Generic;
using Linyx.Effect;
using Linyx.Models.Line;
using UnityEngine;

namespace Linyx.Services.Project
{
    public sealed class ProjectService : MonoBehaviour, IProjectService
    {
        #region UI

        [SerializeField] private Color _selectionColor;
        [SerializeField] private GameObject _kochTrailPrefab;
        [SerializeField] private PhyllotaxisTrail _phylloTrailPrefab;
        [SerializeField] private Material _trailMaterial;
        [SerializeField] private Material _phyllotaxisMaterial;
        #endregion

        #region Injections

        [Inject] public IAudioPeerService AudioPeerService { get; set; }
        [Inject] public IDrawService DrawService { get; set; }
        [Inject] public AddLineSignal AddLineSignal { get; set; }
        [Inject] public FullCleanupSignal FullCleanupSignal { get; set; }

        #endregion

        #region Private Attributes

        private readonly Dictionary<string, ILineModel> _lines = new Dictionary<string, ILineModel>();
        private bool _curveEditorOpen;

        #endregion

        #region Public Methods

        public void Initialize()
        {
            //TODO RemoveListener
            AddLineSignal.AddListener(OnAddLineReceived);
            FullCleanupSignal.AddListener(OnFullCleanup);
        }

        public void SelectLine(string guid)
        {
            OutlineRegister outline = _lines[guid].LineGameObject.gameObject.GetAddComponent<OutlineRegister>();
            outline.OutlineTint = _selectionColor;
            outline.enabled = false;
            outline.enabled = true; //Refresh
        }

        public void UnselectLine(string guid)
        {
            if (_lines.ContainsKey(guid))
                Destroy((_lines[guid].LineGameObject.GetComponent<OutlineRegister>()));
        }

        public void CreateLine(ILineModel line)
        {
            GameObject currentLine = Instantiate(DrawService.GetLinePrefab(), Vector3.zero, Quaternion.identity);
            currentLine.transform.SetParent(DrawService.GetRoot3D().transform, false);
            LineRenderer lineRenderer = currentLine.GetComponent<LineRenderer>();
            lineRenderer.sharedMaterial = new Material(DrawService.GetLineMaterial());
            lineRenderer.positionCount = line.KochLineProperty.OriginalPositions.Length;
            lineRenderer.SetPositions(line.KochLineProperty.OriginalPositions);
            line.LineGameObject = lineRenderer;
            _lines.Add(line.Guid, line);
            UpdateLine((LineModel)line);
        }

        public void DeleteLine(string guid)
        {
            Destroy(_lines[guid].LineGameObject.gameObject);
            _lines.Remove(guid);
        }

        public void UpdateLine(LineModel lineModel)
        {
            LineRenderer line = _lines[lineModel.Guid].LineGameObject;
            line.colorGradient = lineModel.Gradient;

            line.widthCurve = lineModel.WidthCurve;
            line.sortingOrder = lineModel.Layer;
            if (lineModel.IsEmissionEnabled)
            {
                line.sharedMaterial.EnableKeyword("_EMISSION");
                line.sharedMaterial.SetColor("_EmissionColor", lineModel.EmissionColor * lineModel.EmissionIntensity); //TODO Fix start color
            }
            else
            {
                line.sharedMaterial.DisableKeyword("_EMISSION");
            }

            //clear all childrens of the line, avoid to check if a line/trail is already here
            foreach (Transform child in lineModel.LineGameObject.transform)
            {
                Destroy(child.gameObject);
            }

            if (lineModel.Shape != Shape.Line && lineModel.Shape != Shape.Phyllotaxis && lineModel.KochLineProperty.IsKochEnabled)
            {
                lineModel.LineGameObject.enabled = !lineModel.KochTrailProperty.IsTrailEnabled;
                lineModel.LineGameObject.transform.localScale = Vector3.one;

                if (lineModel.KochTrailProperty.IsTrailEnabled)
                {
                    GameObject childKochTrail = new GameObject("ChildKochTrail");
                    childKochTrail.transform.SetParent(lineModel.LineGameObject.transform, false);

                    KochTrail kochTrail = childKochTrail.AddComponent<KochTrail>();
                    kochTrail.Initialize(lineModel, childKochTrail.transform, _kochTrailPrefab, _trailMaterial, lineModel.KochLineProperty.AnimationCurve, AudioPeerService);
                }
                else
                {
                    KochLine kochLine = lineModel.LineGameObject.GetComponent<KochLine>();
                    if (kochLine == null)
                    {
                        kochLine = lineModel.LineGameObject.gameObject.AddComponent<KochLine>();
                    }

                    kochLine.Initialize(lineModel, AudioPeerService);
                }
            }
            else switch (lineModel.Shape)
                {
                    case Shape.Phyllotaxis:
                        lineModel.LineGameObject.enabled = false;
                        GameObject childPhyllotaxisTrail = new GameObject("ChildPhyllotaxisTrail");
                        childPhyllotaxisTrail.transform.SetParent(lineModel.LineGameObject.transform, false);
                        childPhyllotaxisTrail.transform.localScale = new Vector3(0.001f, 0.001f);

                        PhyllotaxisTrail phyllotaxisTrail = Instantiate(_phylloTrailPrefab, childPhyllotaxisTrail.transform, false);
                        phyllotaxisTrail.Initialize(lineModel, AudioPeerService, _phyllotaxisMaterial);

                        break;
                    case Shape.Line:
                        lineModel.LineGameObject.transform.localScale = Vector3.one;
                        lineModel.LineGameObject.enabled = true;
                        line.positionCount = lineModel.KochLineProperty.OriginalPositions.Length;
                        line.SetPositions(lineModel.KochLineProperty.OriginalPositions);
                        Destroy(lineModel.LineGameObject.GetComponent<KochLine>());
                        break;
                }


            _lines[lineModel.Guid] = lineModel;
        }

        public Dictionary<string, ILineModel> GetAllLines()
        {
            return _lines;
        }

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (AudioPeerService.HasClip())
            {
                foreach (KeyValuePair<string, ILineModel> line in _lines)
                {
                    if (!line.Value.KochTrailProperty.IsTrailEnabled)
                    {
                        EmissionReact(line.Value);
                        ScaleReact(line.Value);
                    }
                }
            }

        }

        #endregion

        #region Private Methods

        private void OnFullCleanup()
        {
            foreach (KeyValuePair<string, ILineModel> line in _lines)
            {
                Destroy(line.Value.LineGameObject.gameObject);
            }
            _lines.Clear();
        }

        private void OnAddLineReceived(ILineModel line)
        {
            _lines.Add(line.Guid, line);
            UpdateLine((LineModel)line);
        }

        private void EmissionReact(ILineModel line)
        {
            if (!line.EmissionProperty.IsEmissionReactOnAudio || !line.IsEmissionEnabled) return;
            float value;
            switch (line.EmissionProperty.EmissionFrequencyType)
            {
                case AudioFrequencyType.Band:
                    value = AudioPeerService.GetAudioBand(line.EmissionProperty.EmissionBandBuffer);
                    break;
                case AudioFrequencyType.BandBuffer:
                    value = AudioPeerService.GetAudioBandBuffer(line.EmissionProperty.EmissionBandBuffer);
                    break;
                case AudioFrequencyType.Amplitude:
                    value = AudioPeerService.GetAmplitude();
                    break;
                case AudioFrequencyType.AmplitudeBuffer:
                    value = AudioPeerService.GetAmplitudeBuffer();
                    break;
                case AudioFrequencyType.Frequency:
                    value = AudioPeerService.GetFrequencyBand(line.EmissionProperty.EmissionBandBuffer);
                    break;
                case AudioFrequencyType.FrequencyBuffer:
                    value = AudioPeerService.GetFrequencyBandBuffer(line.EmissionProperty.EmissionBandBuffer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (value > line.EmissionProperty.EmissionThreshold)
            {
                line.LineGameObject.sharedMaterial.SetColor("_EmissionColor", line.EmissionIntensity * line.EmissionColor * value);
            }
            else
            {
                line.LineGameObject.sharedMaterial.SetColor("_EmissionColor", Color.black);
            }
        }

        private void ScaleReact(ILineModel line)
        {
            if (!line.ScaleProperty.IsScaleReactOnAudio) return;
            float value;
            switch (line.EmissionProperty.EmissionFrequencyType)
            {
                case AudioFrequencyType.Band:
                    value = AudioPeerService.GetAudioBand(line.ScaleProperty.ScaleBandBuffer);
                    break;
                case AudioFrequencyType.BandBuffer:
                    value = AudioPeerService.GetAudioBandBuffer(line.ScaleProperty.ScaleBandBuffer);
                    break;
                case AudioFrequencyType.Amplitude:
                    value = AudioPeerService.GetAmplitude();
                    break;
                case AudioFrequencyType.AmplitudeBuffer:
                    value = AudioPeerService.GetAmplitudeBuffer();
                    break;
                case AudioFrequencyType.Frequency:
                    value = AudioPeerService.GetFrequencyBand(line.ScaleProperty.ScaleBandBuffer);
                    break;
                case AudioFrequencyType.FrequencyBuffer:
                    value = AudioPeerService.GetFrequencyBandBuffer(line.ScaleProperty.ScaleBandBuffer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (value > line.ScaleProperty.ScaleThreshold)
            {
                line.LineGameObject.widthMultiplier = value * line.ScaleProperty.ScaleMultiplier;
            }
            else
            {
                line.LineGameObject.widthMultiplier = 1;

            }
        }



        #endregion

    }

    public struct LineSegment
    {
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public Vector3 Direction;
        public float Length;
    }

    public enum ActionType
    {
        Add,
        Edit,
        Delete
    }
}
