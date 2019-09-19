using Linyx.Services.Camera;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Camera
{
    public sealed class SetAngleValueCommand : Command
    {
        [Inject] public ICameraService CameraService { get; set; }

        [Inject] public float Value { get; set; }
        public override void Execute()
        {
            CameraService.SetAngleValue(Value);
        }
    }

    public sealed class SetAngleValueSignal : Signal<float> { }
}
