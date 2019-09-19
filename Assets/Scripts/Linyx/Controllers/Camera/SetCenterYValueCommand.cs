using Linyx.Services.Camera;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Camera
{
    public sealed class SetCenterYValueCommand : Command
    {
        [Inject] public ICameraService CameraService { get; set; }
        [Inject] public float Value { get; set; }
        public override void Execute()
        {
            CameraService.SetCenterYValue(Value);
        }
    }

    public sealed class SetCenterYValueSignal : Signal<float> { }
}