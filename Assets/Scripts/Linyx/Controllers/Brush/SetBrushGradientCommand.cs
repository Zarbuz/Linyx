using Linyx.Services.Brush;
using Linyx.UI.Dialogs;
using MaterialUI;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace Linyx.Controllers.Brush
{
    public sealed class SetBrushGradientCommand : Command
    {
        [Inject] public IBrushService BrushService { get; set; }
        [Inject] public BrushGradientSelectedSignal BrushGradientSelectedSignal { get; set; }

        public override void Execute()
        {
            DialogGradient dialogGradient = DialogManager.ShowDialogGradient();
            dialogGradient.Initialize(BrushService.GetBrushGradient(), OnBrushGradientSelected);
        }

        private void OnBrushGradientSelected(Gradient gradient)
        {
            BrushService.SetBrushGradient(gradient);
            BrushGradientSelectedSignal.Dispatch(gradient);
        }
    }

    public sealed class BrushGradientSelectedSignal : Signal<Gradient> { }
    public sealed class SetBrushGradientSignal : Signal { }
}
