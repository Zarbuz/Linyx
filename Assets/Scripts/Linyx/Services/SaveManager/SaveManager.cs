using System.Collections.Generic;
using Linyx.Controllers.Project;
using Linyx.Models.Line;
using Linyx.Models.Save;
using Linyx.Services.Draw;
using Linyx.Services.Project;
using Linyx.Services.Shortcut;
using UnityEngine;

namespace Linyx.Services.SaveManager
{
    public sealed class SaveManager : MonoBehaviour, ISaveManager
    {
        #region Injections
        [Inject] public IShortcutService ShortcutService { get; set; }
        [Inject] public IProjectService ProjectService { get; set; }
        [Inject] public FullCleanupSignal FullCleanupSignal { get; set; }
        [Inject] public LineAddedSignal LineAddedSignal { get; set; }


        #endregion

        #region Private Attributes

        private string _savePathProject;

        #endregion

        #region Public Methods
        public void Initialize()
        {
        }

        public bool IsSaveAvailable()
        {
            return !ShortcutService.IsUndoEmpty() || !ShortcutService.IsRedoEmpty();
        }

        public MasterSaveModel GetSaveListModel()
        {
            MasterSaveModel masterSave = new MasterSaveModel
            {
                AllLines = new List<SaveLineModel>()
            };

            foreach (KeyValuePair<string, ILineModel> line in ProjectService.GetAllLines())
            {
                LineModel lineModel = (LineModel)line.Value;
                SaveLineModel saveLineModel = (SaveLineModel)lineModel;
                masterSave.AllLines.Add(saveLineModel);
            }

            return masterSave;
        }

        public void SetPathProject(string path)
        {
            _savePathProject = path;
        }

        public string GetPathProject()
        {
            return _savePathProject;
        }

        public void LoadSave(MasterSaveModel masterSave)
        {
            FullCleanupSignal.Dispatch();
            
            foreach (SaveLineModel save in masterSave.AllLines)
            {
                LineModel lineModel = (LineModel)save;
                ProjectService.CreateLine(lineModel);
                LineAddedSignal.Dispatch(lineModel);
            }
        }

        #endregion
        
    }
}
