using Linyx.Services.Camera;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Camera
{
    public sealed class SetVignetteValueCommand : Command
    {
        [Inject] public ICameraService CameraService { get; set; }
        [Inject] public float Value { get; set; }
        public override void Execute()
        {
            CameraService.SetVignetteValue(Value);
        }
    }

    public sealed class SetVignetteValueSignal : Signal<float> { }
}
