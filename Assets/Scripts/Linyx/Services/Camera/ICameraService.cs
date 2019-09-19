using UnityEngine;

namespace Linyx.Services.Camera
{
    public interface ICameraService : IServiceBase
    {
        void SetKaleidoscope(bool value);
        void SetRenderMode(int mode);
        void SetNumberValue(int value);
        void SetAngleValue(float value);
        void SetRadiusValue(float value);
        void SetChromaticValue(float value);
        void SetBloomValue(float value);
        void SetVignetteValue(float value);
        void SetCenterXValue(float value);
        void SetCenterYValue(float value);
        void ToggleDrawMode();
        void CaptureScreenshot(string title);

        void SetCameraRotation(bool enabled);
        void SetCameraPosition(Vector2 position);
        void SetCameraRotationSpeed(float speed);

        void RecenterCamera();
    }
}
