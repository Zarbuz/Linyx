using UnityEngine;

namespace Linyx.Services.Background
{
    public interface IBackgroundService : IServiceBase
    {
        void SetTopBackgroundColor(Color color);
        void SetBottomBackgroundColor(Color color);
        void SetBackgroundIntensity(float value);
        void SetBackgroundExponent(float value);
        void SetDirectionXAngle(int value);
        void SetDirectionYAngle(int value);
        void ResetBackgroundColor();
        void Refresh();
        Color GetTopBackgroundColor();
        Color GetBottomBackgroundColor();
    }
}
