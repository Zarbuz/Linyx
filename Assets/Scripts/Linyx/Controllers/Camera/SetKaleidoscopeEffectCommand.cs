using Linyx.Services.Camera;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Camera
{
    public sealed class SetKaleidoscopeEffectCommand : Command
    {
        [Inject] public ICameraService CameraService { get; set; }

        [Inject] public bool Value { get; set; }

        public override void Execute()
        {
            CameraService.SetKaleidoscope(Value);
        }
    }

    public sealed class SetKaleidoscopeEffectSignal : Signal<bool> { }
}
