using Linyx.Models.Save;
using Linyx.Services.SaveManager;
using OPS.Serialization.IO;
using SFB;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using System;
using System.IO;

namespace Linyx.Controllers.Header
{
    public sealed class SaveProjectCommand : Command
    {
        [Inject] public ISaveManager SaveManager { get; set; }
        [Inject] public SaveAvailableSignal SaveAvailableSignal { get; set; }
        [Inject] public Action SaveCallback { get; set; }

        public override void Execute()
        {
            if (string.IsNullOrEmpty(SaveManager.GetPathProject()))
            {
                string path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "", "linyx");
                if (!string.IsNullOrEmpty(path))
                {
                    SaveManager.SetPathProject(path);
                    Save(path);
                }
            }
            else
            {
                Save(SaveManager.GetPathProject());
            }

            SaveCallback?.Invoke();
        }

        private void Save(string path)
        {
            MasterSaveModel masterSave = SaveManager.GetSaveListModel();
            byte[] bytes = Serializer.Serialize(masterSave, false, false, "LinyxEncryption");
            File.WriteAllBytes(path, bytes);
            SaveAvailableSignal.Dispatch(false);
        }
    }

    public sealed class SaveAvailableSignal : Signal<bool> { }

    public sealed class SaveProjectSignal : Signal<Action> { }
}
