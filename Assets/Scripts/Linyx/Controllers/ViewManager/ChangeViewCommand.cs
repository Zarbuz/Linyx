using Linyx.Services.ViewManager;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.ViewManager
{
    public sealed class ChangeViewCommand : Command
    {
        [Inject] public IViewManager ViewManager { get; set; }
        [Inject] public string ViewName { get; set; }
        public override void Execute()
        {
            ViewManager.ChangeView(ViewName);
        }
    }

    public sealed class ChangeViewSignal : Signal<string> { }
}
