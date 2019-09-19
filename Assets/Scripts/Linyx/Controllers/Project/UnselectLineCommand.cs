using Linyx.Services.Project;
using Linyx.Services.ViewManager;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Project
{
    public sealed class UnselectLineCommand : Command
    {
        [Inject] public IProjectService ProjectService { get; set; }
        [Inject] public IViewManager ViewManager { get; set; }
        [Inject] public string Guid { get; set; }

        public override void Execute()
        {
            ProjectService.UnselectLine(Guid);
            ViewManager.ChangeView("HideEdit");
        }
    }

    public sealed class UnselectLineSignal : Signal<string> { }
}
