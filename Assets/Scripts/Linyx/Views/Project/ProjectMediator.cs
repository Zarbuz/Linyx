using System;
using Linyx.Controllers.Bottom;
using Linyx.Controllers.Brush;
using Linyx.Controllers.Edit;
using Linyx.Controllers.Project;
using Linyx.Controllers.ViewManager;
using Linyx.Models;
using Linyx.Models.Line;
using Linyx.Services.Draw;
using strange.extensions.mediation.impl;

namespace Linyx.Views.Project
{
    public sealed class ProjectMediator : Mediator
    {
        [Inject] public ProjectView View { get; set; }
        [Inject] public AddLineSignal AddLineSignal { get; set; }
        [Inject] public SelectLineSignal SelectLineSignal { get; set; }
        [Inject] public DeleteLineSignal DeleteLineSignal { get; set; }
        [Inject] public UnselectLineSignal UnselectLineSignal { get; set; }
        [Inject] public UpdateLineSignal UpdateLineSignal { get; set; }
        [Inject] public ToggleViewSignal ToggleViewSignal { get; set; }
        [Inject] public SetInfoBarTextSignal SetInfoBarTextSignal { get; set; }
        [Inject] public LineDeletedSignal LineDeletedSignal { get; set; }
        [Inject] public LineAddedSignal LineAddedSignal { get; set; }
        [Inject] public FullCleanupSignal FullCleanupSignal { get; set; }
        [Inject] public SetBrushFromCopySignal SetBrushFromCopySignal { get; set; }
        [Inject] public ResetBrushSettingsSignal ResetBrushSettingsSignal { get; set; }
        

        public override void OnRegister()
        {
            base.OnRegister();
            View.PointerInfoSignal.AddListener(OnPointerInfoSignal);
            View.TogglePanelSignal.AddListener(OnTogglePanelSignal);
            View.LineSelectedSignal.AddListener(OnLineSelected);
            View.LineDeletedSignal.AddListener(OnLineDeletedRequested);
            View.LineUnSelectedSignal.AddListener(OnLineUnselected);
            View.LineCopiedSignal.AddListener(OnLineCopiedRequested);
            View.LineResetSignal.AddListener(OnLineResetRequested);

            FullCleanupSignal.AddListener(OnFullCleanupReceived);
            AddLineSignal.AddListener(OnLineAdded);
            LineAddedSignal.AddListener(OnLineAdded);
            UpdateLineSignal.AddListener(OnUpdateLineReceived);
            LineDeletedSignal.AddListener(OnLineDeletedReceived);
            View.Initialize();
        }

        public override void OnRemove()
        {
            base.OnRemove();
            View.PointerInfoSignal.RemoveListener(OnPointerInfoSignal);
            View.TogglePanelSignal.RemoveListener(OnTogglePanelSignal);
            View.LineSelectedSignal.RemoveListener(OnLineSelected);
            View.LineDeletedSignal.RemoveListener(OnLineDeletedRequested);
            View.LineUnSelectedSignal.RemoveListener(OnLineUnselected);
            View.LineCopiedSignal.RemoveListener(OnLineCopiedRequested);
            View.LineResetSignal.RemoveListener(OnLineResetRequested);


            FullCleanupSignal.RemoveListener(OnFullCleanupReceived);
            AddLineSignal.RemoveListener(OnLineAdded);
            LineAddedSignal.RemoveListener(OnLineAdded);

            UpdateLineSignal.RemoveListener(OnUpdateLineReceived);
            LineDeletedSignal.RemoveListener(OnLineDeletedReceived);
        }

        private void OnPointerInfoSignal(string value)
        {
            SetInfoBarTextSignal.Dispatch(value);
        }

        private void OnTogglePanelSignal(string value)
        {
            ToggleViewSignal.Dispatch(value);
        }

        private void OnLineSelected(ILineModel lineModel)
        {
            SelectLineSignal.Dispatch((LineModel) lineModel);
        }

        private void OnLineUnselected(string guid)
        {
            UnselectLineSignal.Dispatch(guid);
        }

        private void OnLineAdded(ILineModel lineModel)
        {
            View.CreateLineItem(lineModel);
        }

        private void OnLineDeletedRequested(ILineModel line)
        {
            DeleteLineSignal.Dispatch(line.Guid);
        }

        private void OnUpdateLineReceived(LineModel line)
        {
            View.UpdateLine(line);
        }

        private void OnLineDeletedReceived(string guid)
        {
            View.DeleteLine(guid);
        }

        private void OnFullCleanupReceived()
        {
            View.FullCleanup();
        }

        private void OnLineCopiedRequested(ILineModel linemodel)
        {
            SetBrushFromCopySignal.Dispatch((LineModel)linemodel);
        }

        private void OnLineResetRequested()
        {
            ResetBrushSettingsSignal.Dispatch();
        }
    }
}
