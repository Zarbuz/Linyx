using System;
using Linyx.Services.ViewManager;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.ViewManager
{
    public sealed class ToggleViewCommand : Command
    {
        [Inject] public IViewManager ViewManager { get; set; }
        [Inject] public string Value { get; set; }
        public override void Execute()
        {
            ViewManager.ToggleView(Value);
        }
    }

    public sealed class ToggleViewSignal : Signal<string> { }
}
