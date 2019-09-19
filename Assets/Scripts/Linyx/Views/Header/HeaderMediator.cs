using Linyx.Controllers.Bottom;
using Linyx.Controllers.Header;
using Linyx.Controllers.Project;
using strange.extensions.mediation.impl;

namespace Linyx.Views.Header
{
    public sealed class HeaderMediator : Mediator
    {
        [Inject] public HeaderView View { get; set; }
        [Inject] public ToggleDrawModeSignal ToggleDrawModeSignal { get; set; }
        [Inject] public SaveProjectSignal SaveProjectSignal { get; set; }
        [Inject] public SaveProjectAsSignal SaveProjectAsSignal { get; set; }
        [Inject] public SaveAvailableSignal SaveAvailableSignal { get; set; }
        [Inject] public UndoSignal UndoSignal { get; set; }
        [Inject] public UndoAvailableSignal UndoAvailableSignal { get; set; }
        [Inject] public SetInfoBarTextSignal SetInfoBarTextSignal { get; set; }
        [Inject] public RedoSignal RedoSignal { get; set; }
        [Inject] public RedoAvailableSignal RedoAvailableSignal { get; set; }
        [Inject] public LoadProjectSignal LoadProjectSignal { get; set; }
        [Inject] public NewProjectSignal NewProjectSignal { get; set; }
        [Inject] public FullCleanupSignal FullCleanupSignal { get; set; }

        public override void OnRegister()
        {
            base.OnRegister();

            UndoAvailableSignal.AddListener(OnUndoAvailableReceived);
            RedoAvailableSignal.AddListener(OnRedoAvailableReceived);
            SaveAvailableSignal.AddListener(OnSaveAvailableReceived);
            FullCleanupSignal.AddListener(OnFullCleanupReceived);

            View.PointerInfoSignal.AddListener(OnPointerSignal);
            View.UndoSignal.AddListener(OnUndoRequested);
            View.RedoSignal.AddListener(OnRedoRequested);
            View.ToggleDrawModeSignal.AddListener(OnToggleDrawMode);
            View.SaveProjectSignal.AddListener(OnSaveRequested);
            View.SaveProjectAsSignal.AddListener(OnSaveAsRequested);
            View.LoadProjectSignal.AddListener(OnLoadRequested);
            View.NewProjectSignal.AddListener(OnNewRequested);
            View.Initialize();
        }

        public override void OnRemove()
        {
            base.OnRemove();

            UndoAvailableSignal.RemoveListener(OnUndoAvailableReceived);
            RedoAvailableSignal.RemoveListener(OnRedoAvailableReceived);
            SaveAvailableSignal.RemoveListener(OnSaveAvailableReceived);
            FullCleanupSignal.RemoveListener(OnFullCleanupReceived);

            View.PointerInfoSignal.RemoveListener(OnPointerSignal);

            View.UndoSignal.RemoveListener(OnUndoRequested);
            View.RedoSignal.RemoveListener(OnRedoRequested);
            View.ToggleDrawModeSignal.RemoveListener(OnToggleDrawMode);
            View.SaveProjectSignal.RemoveListener(OnSaveRequested);
            View.SaveProjectAsSignal.RemoveListener(OnSaveAsRequested);
            View.LoadProjectSignal.RemoveListener(OnLoadRequested);
            View.NewProjectSignal.RemoveListener(OnNewRequested);
        }

        private void OnPointerSignal(string value)
        {
            SetInfoBarTextSignal.Dispatch(value);
        }

        private void OnUndoAvailableReceived(bool available)
        {
            View.SetUndoAvailable(available);
        }

        private void OnRedoAvailableReceived(bool available)
        {
            View.SetRedoAvailable(available);
        }

        private void OnSaveAvailableReceived(bool available)
        {
            View.SetSaveAvailable(available);
        }

        private void OnUndoRequested()
        {
            UndoSignal.Dispatch();
        }

        private void OnRedoRequested()
        {
            RedoSignal.Dispatch();
        }

        private void OnToggleDrawMode(bool enabled)
        {
            ToggleDrawModeSignal.Dispatch(enabled);
        }

        private void OnSaveRequested()
        {
            SaveProjectSignal.Dispatch(() => {});
        }

        private void OnSaveAsRequested()
        {
            SaveProjectAsSignal.Dispatch();
        }

        private void OnLoadRequested()
        {
            LoadProjectSignal.Dispatch();
        }

        private void OnNewRequested()
        {
            NewProjectSignal.Dispatch();
        }

        private void OnFullCleanupReceived()
        {
            View.SetUndoAvailable(false);
            View.SetRedoAvailable(false);
            View.SetSaveAvailable(false);
        }
    }
}
