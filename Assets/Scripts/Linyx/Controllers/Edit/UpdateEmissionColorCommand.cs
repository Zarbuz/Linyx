using Linyx.Models;
using Linyx.Models.Line;
using Linyx.UI.Dialogs;
using MaterialUI;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace Linyx.Controllers.Edit
{
    public sealed class UpdateEmissionColorCommand : Command
    {
        [Inject] public UpdateLineSignal UpdateLineSignal { get; set; }
        [Inject] public UpdateEmissionColorSelectedSignal UpdateEmissionColorSelectedSignal { get; set; }
        [Inject] public LineModel Value { get; set; }

        public override void Execute()
        {
            DialogColorPicker dialogColorPicker = DialogManager.ShowDialogColorPicker();
            dialogColorPicker.Initialize(Value.EmissionColor, OnColorSelected);
        }

        private void OnColorSelected(Color color)
        {
            Value.EmissionColor = color;
            UpdateLineSignal.Dispatch(Value);
            UpdateEmissionColorSelectedSignal.Dispatch(color);
        }
    }

    public sealed class UpdateEmissionColorSelectedSignal : Signal<Color> { }
    public sealed class UpdateEmissionColorSignal : Signal<LineModel> { }
}
