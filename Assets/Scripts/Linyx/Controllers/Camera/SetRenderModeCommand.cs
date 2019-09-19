using Linyx.Services.Camera;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Camera
{
    public sealed class SetRenderModeCommand : Command
    {
        [Inject] public ICameraService CameraService { get; set; }
        [Inject] public int RenderMode { get; set; }

        public override void Execute()
        {
            CameraService.SetRenderMode(RenderMode);
        }
    }

    public sealed class SetRenderModeSignal : Signal<int> { }
}
