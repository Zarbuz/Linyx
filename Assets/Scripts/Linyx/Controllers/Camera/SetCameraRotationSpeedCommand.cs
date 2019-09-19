using Linyx.Services.Camera;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Camera
{
    public sealed class SetCameraRotationSpeedCommand : Command
    {
        [Inject] public ICameraService CameraService { get; set; }
        [Inject] public float Value { get; set; }

        public override void Execute()
        {
            CameraService.SetCameraRotationSpeed(Value);
        }
    }

    public sealed class SetCameraRotationSpeedSignal : Signal<float> { }
}
