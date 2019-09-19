using Linyx.Services.Background;
using Linyx.UI.Dialogs;
using MaterialUI;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace Linyx.Controllers.Background
{
    public sealed class SetTopBackgroundColorCommand : Command
    {
        [Inject] public IBackgroundService BackgroundService { get; set; }
        [Inject] public TopBackgroundColorSelectedSignal TopBackgroundColorSelectedSignal { get; set; }
        public override void Execute()
        {
            DialogColorPicker dialogColorPicker = DialogManager.ShowDialogColorPicker();
            dialogColorPicker.Initialize(BackgroundService.GetTopBackgroundColor(), OnColorSelected);
        }

        private void OnColorSelected(Color color)
        {
            BackgroundService.SetTopBackgroundColor(color);
            TopBackgroundColorSelectedSignal.Dispatch(color);
        }
    }

    public sealed class TopBackgroundColorSelectedSignal : Signal<Color> { }
    public sealed class SetTopBackgroundColorSignal : Signal { }
}
