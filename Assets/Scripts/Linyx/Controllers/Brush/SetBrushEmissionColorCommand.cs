using Linyx.Services.Brush;
using Linyx.UI.Dialogs;
using MaterialUI;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace Linyx.Controllers.Brush
{
    public sealed class SetBrushEmissionColorCommand : Command
    {
        [Inject] public IBrushService BrushService { get; set; }
        [Inject] public BrushEmissionColorSelectedSignal BrushEmissionColorSelectedSignal { get; set; }

        public override void Execute()
        {
            DialogColorPicker dialogColorPicker = DialogManager.ShowDialogColorPicker();
            dialogColorPicker.Initialize(BrushService.GetBrushEmissionColor(), OnColorSelected);
        }

        private void OnColorSelected(Color color)
        {
            BrushService.SetBrushEmissionColor(color);
            BrushEmissionColorSelectedSignal.Dispatch(color);
        }
    }

    public sealed class SetBrushEmissionColorSignal : Signal { }

    public sealed class BrushEmissionColorSelectedSignal : Signal<Color> { }
}
