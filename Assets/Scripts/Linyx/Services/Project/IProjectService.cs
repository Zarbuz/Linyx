using System.Collections.Generic;
using Linyx.Models;
using Linyx.Models.Line;

namespace Linyx.Services.Project
{
    public interface IProjectService : IServiceBase
    {
        void SelectLine(string guid);
        void UnselectLine(string guid);
        void CreateLine(ILineModel line);
        void DeleteLine(string guid);
        void UpdateLine(LineModel lineModel);
        Dictionary<string, ILineModel> GetAllLines();
    }
}
