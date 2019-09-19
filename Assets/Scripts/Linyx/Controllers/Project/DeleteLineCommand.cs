using Linyx.Models;
using Linyx.Models.Line;
using Linyx.Services.Curve;
using Linyx.Services.Project;
using Linyx.Services.Shortcut;
using Linyx.Services.ViewManager;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace Linyx.Controllers.Project
{
    public sealed class DeleteLineCommand : Command
    {
        [Inject] public IProjectService ProjectService { get; set; }
        [Inject] public IShortcutService ShortcutService { get; set; }
        [Inject] public ICurveService CurveService { get; set; }
        [Inject] public IViewManager ViewManager { get; set; }

        [Inject] public string Guid { get; set; }

        public override void Execute()
        {
            ShortcutService.AddLine(ActionType.Delete, (LineModel)ProjectService.GetAllLines()[Guid]);
            CurveService.Close();
            ProjectService.DeleteLine(Guid);
            ViewManager.ChangeView("HideEdit");
        }
    }

    public sealed class LineDeletedSignal : Signal<string> { }

    public sealed class DeleteLineSignal : Signal<string> { }
}

