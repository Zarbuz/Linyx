using Linyx.Models.Save;
using Linyx.Services.SaveManager;
using OPS.Serialization.IO;
using SFB;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using System.IO;

namespace Linyx.Controllers.Header
{
    public sealed class LoadProjectCommand : Command
    {
        [Inject] public ISaveManager SaveManager { get; set; }
        public override void Execute()
        {
            string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "linyx", false);
            if (!string.IsNullOrEmpty(paths[0]))
            {
                byte[] bytes = File.ReadAllBytes(paths[0]);
                MasterSaveModel masterSave = Serializer.DeSerialize<MasterSaveModel>(bytes, false, false, "LinyxEncryption");
                SaveManager.LoadSave(masterSave);
            }
        }
    }

    public sealed class LoadProjectSignal : Signal { }
}
