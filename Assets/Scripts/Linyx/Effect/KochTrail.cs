using System;
using System.Collections.Generic;
using Linyx.Models.Koch;
using Linyx.Models.Line;
using Linyx.Services.Audio;
using Linyx.Services.Project;
using UnityEngine;

public class KochTrail : MonoBehaviour
{
    public class TrailObject
    {
        public GameObject Go;
        public TrailRenderer Trail;
        public int CurrentTargetNum;
        public Vector3 TargetPosition;
        public Color EmissionColor;
    }

    private List<TrailObject> _trails;
    private Color _startColor, _endColor;
    private bool _enabled;
    private float _lerpPosSpeed;
    private float _distanceSnap;
    private ILineModel _line;
    private IAudioPeerService _audioPeer;

    private Vector3[] _positions;
    private Vector3[] _targetPositions;
    private Vector3[] _bezierPositions;
    private Vector3[] _lerpPositions;
    private int _generationCount;
    private float[] _lerpAudio;
    private int _bezierVertexCount;

    public void Initialize(ILineModel line, Transform parent, GameObject trailPrefab, Material trailMaterial, AnimationCurve trailWidthCurve, IAudioPeerService audioPeer)
    {
        _line = line;
        _audioPeer = audioPeer;

        _trails = new List<TrailObject>();
        _startColor = new Color(0, 0, 0, 0);
        _endColor = new Color(0, 0, 0, 1);

        _targetPositions = new Vector3[line.KochLineProperty.OriginalPositions.Length];
        _targetPositions = line.KochLineProperty.OriginalPositions;
        _bezierVertexCount = line.KochLineProperty.BezierVertexCount;

        foreach (StartGen t in line.KochLineProperty.ListStartGeneration)
        {
            GenerateKochLine(line, _targetPositions, line.KochLineProperty.AnimationCurve.keys, t.Outwards, t.Scale);
            _generationCount++;
        }

        _lerpPositions = new Vector3[_positions.Length];

        for (int i = 0; i < line.KochLineProperty.ShapePointAmount; i++)
        {
            GameObject trailInstance = Instantiate(trailPrefab, transform.position, Quaternion.identity);
            trailInstance.transform.SetParent(parent);

            TrailObject trailObjectInstance = new TrailObject
            {
                Go = trailInstance,
                Trail = trailInstance.GetComponent<TrailRenderer>(),
                EmissionColor = line.Gradient.Evaluate(i * (1.0f / line.KochLineProperty.ShapePointAmount))
            };
            trailObjectInstance.Trail.material = new Material(trailMaterial);
            trailObjectInstance.Trail.numCapVertices = 8;
            trailObjectInstance.Trail.widthCurve = trailWidthCurve;
            Vector3 instantiatePosition;
            if (_generationCount > 0)
            {
                int step;
                if (line.KochLineProperty.UseBezierCurves)
                {
                    step = _bezierPositions.Length / line.KochLineProperty.ShapePointAmount;
                    instantiatePosition = _bezierPositions[i * step];
                    trailObjectInstance.CurrentTargetNum = (i * step) + 1;
                    trailObjectInstance.TargetPosition = _bezierPositions[trailObjectInstance.CurrentTargetNum];

                }
                else
                {
                    step = _positions.Length / line.KochLineProperty.ShapePointAmount;
                    instantiatePosition = _positions[i * step];
                    trailObjectInstance.CurrentTargetNum = (i * step) + 1;
                    trailObjectInstance.TargetPosition = _positions[trailObjectInstance.CurrentTargetNum];

                }
            }
            else
            {
                instantiatePosition = line.KochLineProperty.OriginalPositions[i];
                trailObjectInstance.CurrentTargetNum = i + 1;
                trailObjectInstance.TargetPosition = line.KochLineProperty.OriginalPositions[trailObjectInstance.CurrentTargetNum];
            }
            trailObjectInstance.Go.transform.localPosition = instantiatePosition;
            _trails.Add(trailObjectInstance);
        }

        _enabled = true;
    }

    private void Update()
    {
        if (_enabled)
        {
            HandleMovement();
            HandleAudio();
        }
    }

    private void GenerateKochLine(ILineModel lineModel, Vector3[] positions, Keyframe[] keys, bool outwards, float generatorMultiplier)
    {
        List<LineSegment> lineSegments = new List<LineSegment>();
        for (int i = 0; i < positions.Length - 1; i++)
        {
            LineSegment _line = new LineSegment
            {
                StartPosition = positions[i],
                EndPosition = i == positions.Length - 1 ? positions[0] : positions[i + 1],
            };
            _line.Direction = (_line.EndPosition - _line.StartPosition).normalized;
            _line.Length = Vector3.Distance(_line.EndPosition, _line.StartPosition);
            lineSegments.Add(_line);
        }

        List<Vector3> newPosition = new List<Vector3>();
        List<Vector3> targetPosition = new List<Vector3>();

        foreach (LineSegment lineSegment in lineSegments)
        {
            newPosition.Add(lineSegment.StartPosition);
            targetPosition.Add(lineSegment.StartPosition);

            for (int j = 1; j < keys.Length - 1; j++)
            {
                float moveAmount = lineSegment.Length * keys[j].time;
                float heightAmount = (lineSegment.Length * keys[j].value) * generatorMultiplier;
                Vector3 movePos = lineSegment.StartPosition + (lineSegment.Direction * moveAmount);
                Vector3 direction = outwards
                    ? Quaternion.AngleAxis(-90, new Vector3(0, 0, 1)) * lineSegment.Direction
                    : Quaternion.AngleAxis(90, new Vector3(0, 0, 1)) * lineSegment.Direction;
                newPosition.Add(movePos);
                targetPosition.Add(movePos + (direction * heightAmount));
            }
        }

        newPosition.Add(lineSegments[0].StartPosition);
        targetPosition.Add(lineSegments[0].StartPosition);
        _positions = new Vector3[newPosition.Count];
        _targetPositions = new Vector3[targetPosition.Count];
        _positions = newPosition.ToArray();
        _targetPositions = targetPosition.ToArray();
        _bezierPositions = BezierCurve(_targetPositions, _bezierVertexCount);
    }

    private Vector3[] BezierCurve(IReadOnlyList<Vector3> points, int vertexCount)
    {
        List<Vector3> pointList = new List<Vector3>();
        for (int i = 0; i < points.Count; i += 2)
        {
            if (i + 2 <= points.Count - 1)
            {
                for (float ratio = 0f; ratio <= 1f; ratio += 1.0f / vertexCount)
                {
                    Vector3 tangentLineVertex1 = Vector3.Lerp(points[i], points[i + 1], ratio);
                    Vector3 tangentLineVertex2 = Vector3.Lerp(points[i + 1], points[i + 2], ratio);
                    Vector3 bezierpoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);

                    pointList.Add(bezierpoint);
                }
            }
        }

        return pointList.ToArray();
    }

    private void HandleMovement()
    {
        _lerpPosSpeed = Mathf.Lerp(_line.KochTrailProperty.TrailSpeedMinMax.x, _line.KochTrailProperty.TrailSpeedMinMax.y, _audioPeer.GetAmplitude());
        foreach (TrailObject trail in _trails)
        {
            _distanceSnap = Vector3.Distance(trail.Go.transform.localPosition, trail.TargetPosition);
            if (_distanceSnap < 0.05f)
            {
                trail.Go.transform.localPosition = trail.TargetPosition;
                if (_line.KochLineProperty.UseBezierCurves && _generationCount > 0)
                {
                    if (trail.CurrentTargetNum < _bezierPositions.Length - 1)
                    {
                        trail.CurrentTargetNum++;
                    }
                    else
                    {
                        trail.CurrentTargetNum = 1;

                    }
                    trail.TargetPosition = _bezierPositions[trail.CurrentTargetNum];
                }
                else
                {
                    if (trail.CurrentTargetNum < _positions.Length - 1)
                    {
                        trail.CurrentTargetNum++;
                    }
                    else
                    {
                        trail.CurrentTargetNum = 1;
                    }
                    trail.TargetPosition = _targetPositions[trail.CurrentTargetNum];
                }
            }
            trail.Go.transform.localPosition = Vector3.MoveTowards(trail.Go.transform.localPosition, trail.TargetPosition, Time.deltaTime * _lerpPosSpeed);
        }
    }

    private void HandleAudio()
    {
        for (int i = 0; i < _line.KochLineProperty.ShapePointAmount; i++)
        {
            Color colorLerp = Color.Lerp(_startColor, _trails[i].EmissionColor * _line.EmissionIntensity, _audioPeer.GetAudioBand(_line.KochLineProperty.KochAudioBand[i]));
            _trails[i].Trail.material.SetColor("_EmissionColor", colorLerp);

            colorLerp = Color.Lerp(_startColor, _endColor, _audioPeer.GetAudioBand(_line.KochLineProperty.KochAudioBand[i]));
            _trails[i].Trail.material.SetColor("Color", colorLerp);

            float widthLerp = Mathf.Lerp(_line.KochTrailProperty.TrailWidthMinMax.x, _line.KochTrailProperty.TrailWidthMinMax.y, _audioPeer.GetAudioBandBuffer(_line.KochLineProperty.KochAudioBand[i]));
            _trails[i].Trail.widthMultiplier = widthLerp;

            float timeLerp = Mathf.Lerp(_line.KochTrailProperty.TrailTimeMinMax.x, _line.KochTrailProperty.TrailTimeMinMax.y, _audioPeer.GetAudioBandBuffer(_line.KochLineProperty.KochAudioBand[i]));
            _trails[i].Trail.time = timeLerp;

        }
    }
}
