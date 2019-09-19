using System;
using System.Collections.Generic;
using System.Threading;
using Linyx.Services.Camera;
using SFB;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Bottom
{
    public sealed class ScreenshotCommand : Command
    {
        [Inject] public ICameraService CameraService { get; set; }

        public override void Execute()
        {
            string title = $"linyx{DateTime.Now:yyyy-mm-dd-hh-mm-ss}.png";
            string path = StandaloneFileBrowser.SaveFilePanel("Save File", "", title, "png");
            if (!string.IsNullOrEmpty(path))
            {
                CameraService.CaptureScreenshot(path);
            }
        }
    }

    public sealed class ScreenshotSignal : Signal { }
}
