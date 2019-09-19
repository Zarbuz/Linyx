using Linyx.Models;
using Linyx.Views.Project.Items;
using strange.extensions.signal.impl;
using System.Collections.Generic;
using System.Linq;
using Linyx.Models.Line;
using UnityEngine;

namespace Linyx.Views.Project
{
    public sealed class ProjectView : BaseView
    {
        #region UI

        [SerializeField] private ProjectItem _projectItemPrefab;
        [SerializeField] private Transform _parent;

        #endregion

        #region Local Signals

        public Signal<ILineModel> LineSelectedSignal = new Signal<ILineModel>();
        public Signal<string> LineUnSelectedSignal = new Signal<string>();
        public Signal<ILineModel> LineDeletedSignal = new Signal<ILineModel>();
        public Signal<ILineModel> LineCopiedSignal = new Signal<ILineModel>();
        public Signal LineResetSignal = new Signal();

        #endregion

        #region Private Attributes

        private List<ProjectItem> _itemLines = new List<ProjectItem>();

        #endregion

        #region Public Methods

        public void FullCleanup()
        {
            foreach (ProjectItem projectItem in _itemLines)
            {
                Destroy(projectItem.gameObject);
            }
            _itemLines.Clear();
        }

        public void CreateLineItem(ILineModel lineModel)
        {
            ProjectItem projectItem = Instantiate(_projectItemPrefab, _parent, false);
            projectItem.Initialize(lineModel, OnSelectItem, OnUnselectItem, OnDeleteItem, OnCopyLine);
            _itemLines.Add(projectItem);
        }


        public void UpdateLine(LineModel line)
        {
            ProjectItem projectItem = _itemLines.FirstOrDefault(t => t.LineModel.Guid == line.Guid);
            if (projectItem != null)
            {
                projectItem.UpdateLine(line);
                projectItem.UpdateTitle(line.DisplayName);
            }
        }

        public void DeleteLine(string guid)
        {
            ProjectItem projectItem = _itemLines.FirstOrDefault(t => t.LineModel.Guid == guid);
            if (projectItem != null)
            {
                Destroy(projectItem.gameObject);
                LineUnSelectedSignal.Dispatch(guid);
                _itemLines.RemoveAll(t => t.LineModel.Guid == guid);
            }
        }

        #endregion

        #region Private Methods

        private void OnSelectItem(ILineModel lineModel)
        {
            foreach (ProjectItem line in _itemLines)
            {
                if (line.LineModel.Guid != lineModel.Guid)
                {
                    line.UnSelect();
                    OnUnselectItem(line.LineModel);
                }
            }
            LineSelectedSignal.Dispatch(lineModel);
        }

        private void OnUnselectItem(ILineModel lineModel)
        {
            LineUnSelectedSignal.Dispatch(lineModel.Guid);
        }

        private void OnDeleteItem(ILineModel lineModel)
        {
            _itemLines.RemoveAll(t => t.LineModel.Guid == lineModel.Guid);
            LineDeletedSignal.Dispatch(lineModel);
        }

        private void OnCopyLine(ILineModel lineModel, bool selected)
        {
            foreach (ProjectItem projectItem in _itemLines)
            {
                if (projectItem.LineModel.Guid != lineModel.Guid)
                {
                    projectItem.DeselectCopy();
                }
            }

            if (selected)
            {
                LineCopiedSignal.Dispatch(lineModel);
            }
            else
            {
                LineResetSignal.Dispatch();
            }
        }

        #endregion

    }
}
