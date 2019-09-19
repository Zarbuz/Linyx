using Linyx.Services.Project;
using Linyx.Services.Shortcut;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Header
{
    public sealed class RedoCommand : Command
    {
        [Inject] public IShortcutService ShortcutService { get; set; }

        public override void Execute()
        {
            ShortcutService.Redo();
        }
    }

    public sealed class RedoAvailableSignal : Signal<bool> { }
    public sealed class RedoSignal : Signal { }
}
