using Linyx.Services.Camera;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Camera
{
    public sealed class RecenterCameraCommand : Command
    {
        [Inject] public ICameraService CameraService { get; set; }

        public override void Execute()
        {
            CameraService.RecenterCamera();
        }
    }

    public sealed class RecenterCameraSignal : Signal
    {

    }
}
