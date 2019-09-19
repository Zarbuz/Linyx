using Linyx.Services.Brush;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Brush
{
    public sealed class SetBrushEmissionValueCommand :Command
    {
        [Inject] public IBrushService BrushService { get; set; }
        [Inject] public  bool Value { get; set; }
        public override void Execute()
        {
            BrushService.SetEmission(Value);
        }

    }

    public sealed class SetBrushEmissionValueSignal : Signal<bool> { }
}
