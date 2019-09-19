using System;
using System.Collections.Generic;
using Linyx.Controllers.Edit;
using Linyx.Controllers.Header;
using Linyx.Controllers.Project;
using Linyx.Models;
using Linyx.Models.Line;
using Linyx.Services.Draw;
using Linyx.Services.Project;
using UnityEngine;

namespace Linyx.Services.Shortcut
{
    public sealed class ShortcutService : MonoBehaviour, IShortcutService
    {

        #region Injections
        [Inject] public IProjectService ProjectService { get; set; }
        [Inject] public AddLineSignal AddLineSignal { get; set; }
        [Inject] public UndoAvailableSignal UndoAvailableSignal { get; set; }
        [Inject] public RedoAvailableSignal RedoAvailableSignal { get; set; }
        [Inject] public SaveAvailableSignal SaveAvailableSignal { get; set; }
        [Inject] public FullCleanupSignal FullCleanupSignal { get; set; }
        [Inject] public LineAddedSignal LineAddedSignal { get; set; }
        [Inject] public LineDeletedSignal LineDeletedSignal { get; set; }
        [Inject] public LineUpdatedSignal LineUpdatedSignal { get; set; }


        #region Private Attributes

        private readonly Stack<Tuple<ActionType, ILineModel>> _undoLines = new Stack<Tuple<ActionType, ILineModel>>();
        private readonly Stack<Tuple<ActionType, ILineModel>> _redoLines = new Stack<Tuple<ActionType, ILineModel>>();

        #endregion

        #endregion

        #region Public Methods

        public void Initialize()
        {
            AddLineSignal.AddListener(OnAddLineReceived);
            FullCleanupSignal.AddListener(OnFullCleanup);
        }

        /// <summary>
        /// Return true if the element is the last in the queue
        /// </summary>
        /// <returns></returns>
        public void Undo()
        {
            if (_undoLines.Count != 0)
            {
                Tuple<ActionType, ILineModel> line = _undoLines.Pop();
                switch (line.Item1)
                {
                    case ActionType.Add:
                        DeleteLine(line.Item2, false);
                        LineDeletedSignal.Dispatch(line.Item2.Guid);
                        break;
                    case ActionType.Edit:
                        UpdateLine(line.Item2, false);
                        LineUpdatedSignal.Dispatch(line.Item2);
                        break;
                    case ActionType.Delete:
                        CreateLine(line.Item2, false);
                        LineAddedSignal.Dispatch(line.Item2);
                        break;
                }
            }
            SaveAvailableSignal.Dispatch(_undoLines.Count != 0);
            UndoAvailableSignal.Dispatch(_undoLines.Count != 0);
        }

        /// <summary>
        /// Return true if the element is the last in the queue
        /// </summary>
        /// <returns></returns>
        public void Redo()
        {
            if (_redoLines.Count != 0)
            {
                Tuple<ActionType, ILineModel> line = _redoLines.Pop();
                switch (line.Item1)
                {
                    case ActionType.Add:
                        CreateLine(line.Item2, true);
                        LineAddedSignal.Dispatch(line.Item2);
                        break;
                    case ActionType.Edit:
                        UpdateLine(line.Item2, true);
                        LineUpdatedSignal.Dispatch(line.Item2);
                        break;
                    case ActionType.Delete:
                        DeleteLine(line.Item2, true);
                        LineDeletedSignal.Dispatch(line.Item2.Guid);
                        break;
                }
            }
            SaveAvailableSignal.Dispatch(_redoLines.Count != 0);
            RedoAvailableSignal.Dispatch(_redoLines.Count != 0);
        }

        public void AddLine(ActionType actionType, LineModel line)
        {
            _undoLines.Push(new Tuple<ActionType, ILineModel>(actionType, line));
        }

        public bool IsUndoEmpty()
        {
            return _undoLines.Count == 0;
        }

        public bool IsRedoEmpty()
        {
            return _redoLines.Count == 0;
        }

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
            {
                Undo();
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y))
            {
                Redo();
            }
        }

        #endregion

        #region Private Methods

        private void OnFullCleanup()
        {
            _undoLines.Clear();
            _redoLines.Clear();
        }

        private void OnAddLineReceived(ILineModel line)
        {
            _undoLines.Push(new Tuple<ActionType, ILineModel>(ActionType.Add, line));
            UndoAvailableSignal.Dispatch(true);
            SaveAvailableSignal.Dispatch(true);
        }

        private void CreateLine(ILineModel line, bool undo)
        {
            ProjectService.CreateLine(line);
            if (undo)
            {
                _undoLines.Push(new Tuple<ActionType, ILineModel>(ActionType.Add, line));
                UndoAvailableSignal.Dispatch(true);
            }
            else
            {
                _redoLines.Push(new Tuple<ActionType, ILineModel>(ActionType.Delete, line));
                RedoAvailableSignal.Dispatch(true);
            }
        }

        private void UpdateLine(ILineModel line, bool undo)
        {
            ProjectService.UpdateLine((LineModel) line);
            if (undo)
            {
                _undoLines.Push(new Tuple<ActionType, ILineModel>(ActionType.Edit, line));
                UndoAvailableSignal.Dispatch(true);
            }
            else
            {
                _redoLines.Push(new Tuple<ActionType, ILineModel>(ActionType.Edit, line));
                RedoAvailableSignal.Dispatch(true);
            }
        }

        private void DeleteLine(ILineModel line, bool undo)
        {
            ProjectService.DeleteLine(line.Guid);
            if (undo)
            {
                _undoLines.Push(new Tuple<ActionType, ILineModel>(ActionType.Delete, line));
                UndoAvailableSignal.Dispatch(true);
            }
            else
            {
                _redoLines.Push(new Tuple<ActionType, ILineModel>(ActionType.Add, line));
                RedoAvailableSignal.Dispatch(true);
            }
        }

        #endregion
    }
}
