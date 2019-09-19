using Linyx.Services.Camera;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Camera
{
    public sealed class SetNumberValueCommand : Command
    {
        [Inject] public ICameraService CameraService { get; set; }
        [Inject] public int Value { get; set; }
        public override void Execute()
        {
            CameraService.SetNumberValue(Value);
        }
    }

    public sealed class SetNumberValueSignal : Signal<int> { }
}
