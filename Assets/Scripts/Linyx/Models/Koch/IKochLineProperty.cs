using OPS.Serialization.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace Linyx.Models.Koch
{
    public interface IKochLineProperty
    {
        /// Koch React
        bool IsKochEnabled { get; set; }
        int ShapePointAmount { get; set; }
        List<StartGen> ListStartGeneration { get; set; }
        AnimationCurve AnimationCurve { get; set; }
        bool UseBezierCurves { get; set; }
        int BezierVertexCount { get; set; }
        List<int> KochAudioBand { get; set; }
        Vector3[] OriginalPositions { get; set; }

        IKochLineProperty DeepCopy();
    }

    [SerializeAbleClass]
    public class StartGen
    {
        [SerializeAbleField(0)] public bool Outwards;
        [SerializeAbleField(1)] public float Scale;
        [SerializeAbleField(2)] public string Guid;

        public StartGen DeepCopy()
        {
            return new StartGen()
            {
                Guid = Guid,
                Scale = Scale,
                Outwards = Outwards
            };
        }
    }
}
