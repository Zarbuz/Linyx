using Linyx.Models.Save;
using Linyx.Services.SaveManager;
using OPS.Serialization.IO;
using SFB;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using System.IO;

namespace Linyx.Controllers.Header
{
    public sealed class SaveProjectAsCommand : Command
    {
        [Inject] public ISaveManager SaveManager { get; set; }
        [Inject] public SaveAvailableSignal SaveAvailableSignal { get; set; }

        public override void Execute()
        {
            string path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "", "linyx");
            if (!string.IsNullOrEmpty(path))
            {
                Save(path);
            }
        }

        private void Save(string path)
        {
            MasterSaveModel masterSave = SaveManager.GetSaveListModel();
            byte[] bytes = Serializer.Serialize(masterSave, false, false, "LinyxEncryption");
            File.WriteAllBytes(path, bytes);
            SaveAvailableSignal.Dispatch(false);
        }
    }

    public sealed class SaveProjectAsSignal : Signal { }
}
