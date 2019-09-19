using Linyx.Services.Camera;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Camera
{
    public sealed class SetCenterXValueCommand : Command
    {
        [Inject] public ICameraService CameraService { get; set; }
        [Inject] public float Value { get; set; }
        public override void Execute()
        {
            CameraService.SetCenterXValue(Value);
        }
    }

    public sealed class SetCenterXValueSignal : Signal<float> { }
}
