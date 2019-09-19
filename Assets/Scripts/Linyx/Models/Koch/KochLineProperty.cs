using System.Collections.Generic;
using Linyx.Models.Line;
using UnityEngine;

namespace Linyx.Models.Koch
{
    public class KochLineProperty : IKochLineProperty
    {
        public bool IsKochEnabled { get; set; }
        public int ShapePointAmount { get; set; }
        public List<StartGen> ListStartGeneration { get; set; }
        public AnimationCurve AnimationCurve { get; set; }
        public bool UseBezierCurves { get; set; }
        public int BezierVertexCount { get; set; }
        public List<int> KochAudioBand { get; set; }
        public Vector3[] OriginalPositions { get; set; }

        public IKochLineProperty DeepCopy()
        {
            return new KochLineProperty
            {
                IsKochEnabled = IsKochEnabled,
                ShapePointAmount = ShapePointAmount,
                ListStartGeneration = ListStartGeneration,
                AnimationCurve = AnimationCurve,
                UseBezierCurves = UseBezierCurves,
                BezierVertexCount = BezierVertexCount,
                KochAudioBand = KochAudioBand,
                OriginalPositions = OriginalPositions
            };
        }
    }
}
