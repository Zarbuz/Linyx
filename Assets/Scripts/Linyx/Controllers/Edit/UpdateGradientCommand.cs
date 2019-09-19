using Linyx.Models;
using Linyx.Models.Line;
using Linyx.UI.Dialogs;
using MaterialUI;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace Linyx.Controllers.Edit
{
    public sealed class UpdateGradientCommand : Command
    {
        [Inject] public LineModel LineModel { get; set; }
        [Inject] public UpdateLineSignal UpdateLineSignal { get; set; }
        [Inject] public UpdateGradientSelectedSignal UpdateGradientSelectedSignal { get; set; }
        public override void Execute()
        {
            DialogGradient dialogGradient = DialogManager.ShowDialogGradient();
            dialogGradient.Initialize(LineModel.Gradient, OnGradientSelected);
        }

        private void OnGradientSelected(Gradient gradient)
        {
            LineModel.Gradient = gradient;
            UpdateLineSignal.Dispatch(LineModel);
            UpdateGradientSelectedSignal.Dispatch(gradient);
        }
    }

    public sealed class UpdateGradientSignal : Signal<LineModel> { }

    public sealed class UpdateGradientSelectedSignal : Signal<Gradient> { }
}
