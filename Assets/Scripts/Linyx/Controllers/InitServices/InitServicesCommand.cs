using System.Threading;
using System.Threading.Tasks;
using Linyx.Services.Audio;
using Linyx.Services.Background;
using Linyx.Services.Brush;
using Linyx.Services.Camera;
using Linyx.Services.Draw;
using Linyx.Services.Project;
using Linyx.Services.SaveManager;
using Linyx.Services.Shortcut;
using Linyx.Services.ViewManager;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.InitServices
{
    public class InitServicesCommand : Command
    {
        #region Injections
        [Inject] public IBrushService BrushService { get; set; }
        [Inject] public IDrawService DrawService { get; set; }
        [Inject] public IViewManager ViewManager { get; set; }
        [Inject] public ICameraService CameraService { get; set; }
        [Inject] public IBackgroundService BackgroundService { get; set; }
        [Inject] public IProjectService ProjectService { get; set; }
        [Inject] public IAudioPeerService AudioPeerService { get; set; }
        [Inject] public ISaveManager SaveManager { get; set; }
        [Inject] public IShortcutService ShortcutService { get; set; }
        #endregion

        public override async void Execute()
        {
            await Task.Delay(1000);

            ViewManager.Initialize();
            SaveManager.Initialize();
            ShortcutService.Initialize();
            BrushService.Initialize();
            DrawService.Initialize();
            BackgroundService.Initialize();
            CameraService.Initialize();
            ProjectService.Initialize();
            AudioPeerService.Initialize();
        }
    }

    public class InitServicesSignal : Signal { }
}
