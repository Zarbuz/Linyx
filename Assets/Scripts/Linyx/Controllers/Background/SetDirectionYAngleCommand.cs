using Linyx.Services.Background;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Background
{
    public sealed class SetDirectionYAngleCommand : Command
    {
        [Inject] public IBackgroundService BackgroundService { get; set; }
        [Inject] public int Value { get; set; }

        public override void Execute()
        {
            BackgroundService.SetDirectionYAngle(Value);
        }
    }

    public sealed class SetDirectionYAngleSignal : Signal<int> { }
}
