using Linyx.Services.Brush;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Brush
{
    public sealed class SetBrushShapeInitSizeCommand : Command
    {
        [Inject] public IBrushService BrushService { get; set; }
        [Inject] public float Value { get; set; }

        public override void Execute()
        {
            BrushService.SetBrushShapeInitSize(Value);
        }
    }

    public sealed class SetBrushShapeInitSizeSignal: Signal<float> { }
}
