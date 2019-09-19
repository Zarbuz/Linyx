using Linyx.Models;
using Linyx.Models.Line;
using Linyx.Services.Project;

namespace Linyx.Services.Shortcut
{
    public interface IShortcutService : IServiceBase
    {
        void Undo();
        void Redo();

        void AddLine(ActionType actionType, LineModel line);
        
        bool IsUndoEmpty();
        bool IsRedoEmpty();
    }
}
