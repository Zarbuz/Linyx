using Linyx.Services.Brush;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Brush
{
    public sealed class ResetBrushSettingsCommand : Command
    {
        [Inject] public IBrushService BrushService { get; set; }

        public override void Execute()
        {
            BrushService.ResetBrushSettings();
        }
    }

    public sealed class ResetBrushSettingsSignal : Signal { }
}
