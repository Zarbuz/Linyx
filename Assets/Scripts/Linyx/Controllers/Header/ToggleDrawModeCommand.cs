using Linyx.Services.Background;
using Linyx.Services.Camera;
using Linyx.Services.ViewManager;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Header
{
    public sealed class ToggleDrawModeCommand : Command
    {
        [Inject] public ICameraService CameraService { get; set; }
        [Inject] public IBackgroundService BackgroundService { get; set; }
        [Inject] public IViewManager ViewManager { get; set; }
        [Inject] public  bool DrawModeEnabled { get; set; }

        public override void Execute()
        {
            CameraService.ToggleDrawMode();
            if (!DrawModeEnabled)
            {
                BackgroundService.Refresh();
                ViewManager.ChangeView("RenderView");
            }
            else
            {
                BackgroundService.ResetBackgroundColor();
                ViewManager.ChangeView("BrushView");
            }
        }
    }

    public sealed class ToggleDrawModeSignal : Signal<bool> { }
}
