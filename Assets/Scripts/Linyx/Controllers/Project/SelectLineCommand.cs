using Linyx.Models;
using Linyx.Models.Line;
using Linyx.Services.Curve;
using Linyx.Services.Project;
using Linyx.Services.ViewManager;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Project
{
    public sealed class SelectLineCommand : Command
    {
        [Inject] public IProjectService ProjectService { get; set; }
        [Inject] public IViewManager ViewManager { get; set; }
        [Inject] public ICurveService CurveService { get; set; }
        [Inject] public LineModel LineModel { get; set; }

        public override void Execute()
        {
            ProjectService.SelectLine(LineModel.Guid);
            CurveService.Close();
            ViewManager.ChangeView("EditView");
        }
    }

    public sealed class SelectLineSignal : Signal<LineModel> { }
}
