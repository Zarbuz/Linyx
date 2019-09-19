using System;
using Linyx.Models.Line;
using Linyx.Services.Audio;
using UnityEngine;

namespace Linyx.Effect
{
    public class PhyllotaxisTrail : MonoBehaviour
    {
        private bool _isLerping;
        private Vector3 _startPosition, _endPosition;
        private float _lerpPosTimer, _lerpPosSpeed;
        private int _number;
        private int _currentIteration;
        private TrailRenderer _trailRenderer;
        private Vector2 _phyllotaxisPosition;
        private bool _forward;
        private float _scaleTimer, _currentScale;

        private ILineModel _lineModel;
        private IAudioPeerService _audioPeer;

        public void Initialize(ILineModel lineModel, IAudioPeerService audioPeer, Material trailMaterial)
        {
            _lineModel = lineModel;
            _audioPeer = audioPeer;

            _currentScale = lineModel.PhyllotaxisProperty.Scale;
            _forward = true;
            _trailRenderer = GetComponent<TrailRenderer>();
            _trailRenderer.material = new Material(trailMaterial);
            _trailRenderer.colorGradient = lineModel.Gradient;
            _trailRenderer.widthCurve = lineModel.WidthCurve;

            if (lineModel.IsEmissionEnabled)
            {
                _trailRenderer.sharedMaterial.EnableKeyword("_EMISSION");
                _trailRenderer.sharedMaterial.SetColor("_EmissionColor", lineModel.EmissionColor * lineModel.EmissionIntensity);
            }

            _number = lineModel.PhyllotaxisProperty.NumberStart;
            transform.localPosition = CalculatePhyllotaxis(lineModel.PhyllotaxisProperty.Degree, _currentScale, _number);
            if (lineModel.PhyllotaxisProperty.UseLerping)
            {
                _isLerping = true;
                SetLerpPositions();
            }
        }

        private void SetLerpPositions()
        {
            _phyllotaxisPosition = CalculatePhyllotaxis(_lineModel.PhyllotaxisProperty.Degree, _currentScale, _number);
            _startPosition = transform.localPosition;
            if (!float.IsNaN(_phyllotaxisPosition.x) && !float.IsNaN(_phyllotaxisPosition.y))
            {
                _endPosition = new Vector3(_phyllotaxisPosition.x, _phyllotaxisPosition.y, 0);
            }
        }


        private void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            if (_lineModel.PhyllotaxisProperty.UseScaling)
            {
                float valueScale;
                switch (_lineModel.PhyllotaxisProperty.ScaleFrequencyType)
                {
                    case AudioFrequencyType.Band:
                        valueScale = _audioPeer.GetAudioBand(_lineModel.PhyllotaxisProperty.ScaleAudioBand);
                        break;
                    case AudioFrequencyType.BandBuffer:
                        valueScale = _audioPeer.GetAudioBandBuffer(_lineModel.PhyllotaxisProperty.ScaleAudioBand);
                        break;
                    case AudioFrequencyType.Amplitude:
                        valueScale = _audioPeer.GetAmplitude();
                        break;
                    case AudioFrequencyType.AmplitudeBuffer:
                        valueScale = _audioPeer.GetAmplitudeBuffer();
                        break;
                    case AudioFrequencyType.Frequency:
                        valueScale = _audioPeer.GetFrequencyBand(_lineModel.PhyllotaxisProperty.ScaleAudioBand);
                        break;
                    case AudioFrequencyType.FrequencyBuffer:
                        valueScale = _audioPeer.GetFrequencyBandBuffer(_lineModel.PhyllotaxisProperty.ScaleAudioBand);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (_lineModel.PhyllotaxisProperty.UseScaleCurve)
                {
                    _scaleTimer += (_lineModel.PhyllotaxisProperty.InterpolationSpeed * valueScale) * Time.deltaTime;
                    if (_scaleTimer >= 1)
                    {
                        _scaleTimer -= 1;
                    }
                    _currentScale = Mathf.Lerp(_lineModel.PhyllotaxisProperty.ScaleMinMax.x, _lineModel.PhyllotaxisProperty.ScaleMinMax.y, _lineModel.PhyllotaxisProperty.ScaleInterpolationCurve.Evaluate(_scaleTimer));
                }
                else
                {
                    _currentScale = Mathf.Lerp(_lineModel.PhyllotaxisProperty.ScaleMinMax.x, _lineModel.PhyllotaxisProperty.ScaleMinMax.y, valueScale);
                }
            }


            if (_lineModel.PhyllotaxisProperty.UseLerping)
            {
                if (_isLerping)
                {
                    float valueLerping;
                    switch (_lineModel.PhyllotaxisProperty.LerpFrequencyType)
                    {
                        case AudioFrequencyType.Band:
                            valueLerping = _audioPeer.GetAudioBand(_lineModel.PhyllotaxisProperty.LerpAudioBand);
                            break;
                        case AudioFrequencyType.BandBuffer:
                            valueLerping = _audioPeer.GetAudioBandBuffer(_lineModel.PhyllotaxisProperty.LerpAudioBand);
                            break;
                        case AudioFrequencyType.Amplitude:
                            valueLerping = _audioPeer.GetAmplitude();
                            break;
                        case AudioFrequencyType.AmplitudeBuffer:
                            valueLerping = _audioPeer.GetAmplitudeBuffer();
                            break;
                        case AudioFrequencyType.Frequency:
                            valueLerping = _audioPeer.GetFrequencyBand(_lineModel.PhyllotaxisProperty.LerpAudioBand);
                            break;
                        case AudioFrequencyType.FrequencyBuffer:
                            valueLerping = _audioPeer.GetFrequencyBandBuffer(_lineModel.PhyllotaxisProperty.LerpAudioBand);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    _lerpPosSpeed = Mathf.Lerp(_lineModel.PhyllotaxisProperty.SpeedMinMax.x, _lineModel.PhyllotaxisProperty.SpeedMinMax.y, _lineModel.PhyllotaxisProperty.LerpInterpolationCurve.Evaluate(valueLerping));
                    _lerpPosTimer += Time.deltaTime * _lerpPosSpeed;
                    transform.localPosition = Vector3.Lerp(_startPosition, _endPosition, Mathf.Clamp01(_lerpPosTimer));
                    if (_lerpPosTimer >= 1)
                    {
                        _lerpPosTimer -= 1;
                        if (_forward)
                        {
                            _number += _lineModel.PhyllotaxisProperty.StepSize;
                            _currentIteration++;
                        }
                        else
                        {
                            _number -= _lineModel.PhyllotaxisProperty.StepSize;
                            _currentIteration--;
                        }
                        if ((_currentIteration >= 0) && (_currentIteration < _lineModel.PhyllotaxisProperty.MaxIterations))
                        {
                            SetLerpPositions();
                        }
                        else // current iteration has hit 0 or maxiteration
                        {
                            if (_lineModel.PhyllotaxisProperty.Repeat)
                            {
                                if (_lineModel.PhyllotaxisProperty.Invert)
                                {
                                    _forward = !_forward;
                                    SetLerpPositions();
                                }
                                else
                                {
                                    _number = _lineModel.PhyllotaxisProperty.NumberStart;
                                    _currentIteration = 0;
                                    SetLerpPositions();
                                }
                            }
                            else
                            {
                                _isLerping = false;
                            }
                        }
                    }

                }
            }
            if (!_lineModel.PhyllotaxisProperty.UseLerping)
            {
                _phyllotaxisPosition = CalculatePhyllotaxis(_lineModel.PhyllotaxisProperty.Degree, _currentScale, _number);
                transform.localPosition = new Vector3(_phyllotaxisPosition.x, _phyllotaxisPosition.y, 0);
                _number += _lineModel.PhyllotaxisProperty.StepSize;
                _currentIteration++;
            }
        }

        private Vector2 CalculatePhyllotaxis(float degree, float scale, int number)
        {
            double angle = number * (degree * Mathf.Deg2Rad);
            float r = scale * Mathf.Sqrt(number);
            float x = r * (float)System.Math.Cos(angle);
            float y = r * (float)System.Math.Sin(angle);
            Vector2 vec2 = new Vector2(x, y);
            return vec2;
        }
    }
}
