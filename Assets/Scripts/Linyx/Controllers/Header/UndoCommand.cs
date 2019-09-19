using Linyx.Services.Project;
using Linyx.Services.Shortcut;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Header
{
    public sealed class UndoCommand : Command
    {
        [Inject] public IShortcutService ShortcutService { get; set; }

        public override void Execute()
        {
            ShortcutService.Undo();
        }
    }

    public sealed class UndoAvailableSignal : Signal<bool> { }

    public sealed class UndoSignal : Signal { }
}
