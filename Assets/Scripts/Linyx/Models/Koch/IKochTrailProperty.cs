using UnityEngine;

namespace Linyx.Models.Koch
{
    public interface IKochTrailProperty
    {
        /// Trail
        bool IsTrailEnabled { get; set; }
        Vector2 TrailSpeedMinMax { get; set; }
        Vector2 TrailWidthMinMax { get; set; }
        Vector2 TrailTimeMinMax { get; set; }

        IKochTrailProperty DeepCopy();
    }
}
