using Linyx.Services.Background;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Background
{
    public sealed class SetIntensityBackgroundCommand : Command
    {
        [Inject] public IBackgroundService BackgroundService { get; set; }
        [Inject] public float Value { get; set; }

        public override void Execute()
        {
            BackgroundService.SetBackgroundIntensity(Value);
        }
    }

    public sealed class SetIntensityBackgroundSignal : Signal<float> { }
}
