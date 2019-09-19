using Linyx.Models;
using Linyx.Models.Line;
using Linyx.Services.Brush;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Brush
{
    public sealed class SetBrushFromCopyCommand : Command
    {
        [Inject] public IBrushService BrushService { get; set; }
        [Inject] public LineModel LineModel { get; set; }
        [Inject] public SetBrushFromCopySelectedSignal SetBrushFromCopySelectedSignal { get; set; }

        public override void Execute()
        {
            BrushService.SetBrushFromCopy(LineModel);
            SetBrushFromCopySelectedSignal.Dispatch(LineModel);
        }
    }

    public sealed class SetBrushFromCopySelectedSignal : Signal<LineModel>
    {

    }

    public sealed class SetBrushFromCopySignal : Signal<LineModel>
    {

    }
}
