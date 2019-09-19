using Linyx.Services.Recording;
using Linyx.UI.Dialogs;
using MaterialUI;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Bottom
{
    public sealed class ExportVideoCommand : Command
    {
        [Inject] public IRecordingService RecordingService { get; set; }
        public override void Execute()
        {
            DialogExportVideo dialog = DialogManager.ShowDialogExportVideo();
            dialog.Initialize(OnValidateClicked);
        }

        private void OnValidateClicked(ExportVideoDTO exportVideo)
        {
            RecordingService.StartRecording(exportVideo);
        }
    }

    public sealed class ExportVideoSignal : Signal { }
}
