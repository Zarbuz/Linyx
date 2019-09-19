using Linyx.Services.Brush;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Brush
{
    public sealed class SetBrushEmissionIntensityCommand : Command
    {
        [Inject] public IBrushService BrushService { get; set; }
        [Inject] public float Value { get; set; }

        public override void Execute()
        {
            BrushService.SetBrushEmissionIntensity(Value);
        }
    }

    public sealed class SetBrushEmissionIntensitySignal : Signal<float> { }
}
