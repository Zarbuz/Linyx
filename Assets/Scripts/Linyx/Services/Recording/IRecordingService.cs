using Linyx.UI.Dialogs;

namespace Linyx.Services.Recording
{
    public interface IRecordingService : IServiceBase
    {
        void StartRecording(ExportVideoDTO exportVideo);
        void StopRecording();
    }
}
