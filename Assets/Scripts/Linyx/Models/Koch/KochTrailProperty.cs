using UnityEngine;

namespace Linyx.Models.Koch
{
    public class KochTrailProperty : IKochTrailProperty
    {
        public bool IsTrailEnabled { get; set; }
        public Vector2 TrailSpeedMinMax { get; set; }
        public Vector2 TrailWidthMinMax { get; set; }
        public Vector2 TrailTimeMinMax { get; set; }

        public IKochTrailProperty DeepCopy()
        {
            return new KochTrailProperty()
            {
                IsTrailEnabled = IsTrailEnabled,
                TrailSpeedMinMax = TrailSpeedMinMax,
                TrailWidthMinMax = TrailWidthMinMax,
                TrailTimeMinMax = TrailTimeMinMax
            };
        }
    }
}
