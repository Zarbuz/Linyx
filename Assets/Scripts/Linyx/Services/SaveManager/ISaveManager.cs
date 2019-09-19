using Linyx.Models;
using Linyx.Models.Save;

namespace Linyx.Services.SaveManager
{
    public interface ISaveManager : IServiceBase
    {
        bool IsSaveAvailable();
        MasterSaveModel GetSaveListModel();
        void SetPathProject(string path);
        string GetPathProject();
        void LoadSave(MasterSaveModel masterSave);
    }
}
