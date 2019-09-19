using Linyx.Services.Background;
using Linyx.Services.Brush;
using Linyx.UI.Dialogs;
using MaterialUI;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace Linyx.Controllers.Background
{
    public sealed class SetBottomBackgroundColorCommand : Command
    {
        [Inject] public IBackgroundService BackgroundService { get; set; }
        [Inject] public BottomBackgroundColorSelectedSignal BottomBackgroundColorSelectedSignal { get; set; }
        public override void Execute()
        {
            DialogColorPicker dialogColorPicker = DialogManager.ShowDialogColorPicker();
            dialogColorPicker.Initialize(BackgroundService.GetBottomBackgroundColor(), OnColorSelected);
        }

        private void OnColorSelected(Color color)
        {
            BackgroundService.SetBottomBackgroundColor(color);
            BottomBackgroundColorSelectedSignal.Dispatch(color);
        }
    }

    public sealed class BottomBackgroundColorSelectedSignal : Signal<Color> { }
    public sealed class SetBottomBackgroundColorSignal : Signal { }
}