using Linyx.Controllers.Project;
using Linyx.Services.Project;
using Linyx.Services.SaveManager;
using MaterialUI;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Header
{
    public sealed class NewProjectCommand : Command
    {
        [Inject] public FullCleanupSignal FullCleanupSignal { get; set; }
        [Inject] public SaveProjectSignal SaveProjectSignal { get; set; }

        [Inject] public ISaveManager SaveManager { get; set; }
        public override void Execute()
        {
            if (SaveManager.IsSaveAvailable())
            {
                DialogManager.ShowAlert("", OnAffirmativeClicked, "Yes", "Project unsaved, save changes to file?",
                    MaterialIconHelper.GetIcon(MaterialIconEnum.INFO_OUTLINE), OnDismissiveClicked, "No");
            }
            else
            {
                FullCleanupSignal.Dispatch();
            }
        }

        private void OnAffirmativeClicked()
        {
            SaveProjectSignal.Dispatch(OnDismissiveClicked);
        }

        private void OnDismissiveClicked()
        {
            FullCleanupSignal.Dispatch();
        }
    }

    public sealed class NewProjectSignal : Signal { }
}
