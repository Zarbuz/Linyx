using System;
using Linyx.Models;
using Linyx.Models.Line;
using Linyx.Services.Project;
using Linyx.Services.Shortcut;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Edit
{
    public sealed class UpdateLineCommand : Command
    {
        [Inject] public IProjectService ProjectService { get; set; }
        [Inject] public IShortcutService ShortcutService { get; set; }
        [Inject] public LineModel Value { get; set; }

        public override void Execute()
        {
            ShortcutService.AddLine(ActionType.Edit, (LineModel)ProjectService.GetAllLines()[Value.Guid]);
            ProjectService.UpdateLine(Value);
        }
    }

    public sealed class LineUpdatedSignal : Signal<ILineModel> { }
    public sealed class UpdateLineSignal : Signal<LineModel> { }
}
