using Linyx.Models.Koch;
using Linyx.Models.Line;
using Linyx.Services.Audio;
using Linyx.Services.Project;
using System.Collections.Generic;
using UnityEngine;

namespace Linyx.Effect
{
    public sealed class KochLine : MonoBehaviour
    {
        private ILineModel _line;
        private IAudioPeerService _audioPeer;

        private Vector3[] _positions;
        private Vector3[] _targetPositions;
        private Vector3[] _bezierPositions;
        private Vector3[] _lerpPositions;
        private int _generationCount;
        private float[] _lerpAudio;
        private int _bezierVertexCount;

        public void Initialize(ILineModel line, IAudioPeerService audioPeer)
        {
            _line = line;
            _audioPeer = audioPeer;

            _targetPositions = new Vector3[line.KochLineProperty.OriginalPositions.Length];
            _targetPositions = line.KochLineProperty.OriginalPositions;
            _bezierVertexCount = line.KochLineProperty.BezierVertexCount;
            _lerpAudio = new float[line.KochLineProperty.ShapePointAmount];

            foreach (StartGen t in line.KochLineProperty.ListStartGeneration)
            {
                GenerateKochLine(line, _targetPositions, line.KochLineProperty.AnimationCurve.keys, t.Outwards, t.Scale);
                _generationCount++;
            }

            _lerpPositions = new Vector3[_positions.Length];

        }


        private void Update()
        {
            if (_generationCount > 0)
            {
                KochReact();
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

        private void KochReact()
        {
            LineRenderer lineRenderer = _line.LineGameObject;

            int count = 0;
            for (int i = 0; i < _line.KochLineProperty.ShapePointAmount; i++)
            {
                _lerpAudio[i] = _audioPeer.GetAudioBandBuffer(_line.KochLineProperty.KochAudioBand[i]);
                for (int j = 0; j < (_positions.Length - 1) / _line.KochLineProperty.ShapePointAmount; j++)
                {
                    _lerpPositions[count] = Vector3.Lerp(_positions[count], _targetPositions[count], _lerpAudio[i]);
                    count++;
                }
            }

            _lerpPositions[count] = Vector3.Lerp(_positions[count], _targetPositions[count], _lerpAudio[_line.KochLineProperty.ShapePointAmount - 1]);

            if (_line.KochLineProperty.UseBezierCurves)
            {
                _bezierPositions = BezierCurve(_lerpPositions, _line.KochLineProperty.BezierVertexCount);
                lineRenderer.positionCount = _bezierPositions.Length;
                lineRenderer.SetPositions(_bezierPositions);
            }
            else
            {
                lineRenderer.positionCount = _lerpPositions.Length;
                lineRenderer.SetPositions(_lerpPositions);
            }
        }
    }
}
