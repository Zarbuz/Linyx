using Linyx.Controllers.Bottom;
using Linyx.Controllers.Camera;
using Linyx.Controllers.Music;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace Linyx.Views.Bottom
{
    public sealed class BottomMediator : Mediator
    {
        #region Injections
        [Inject] public BottomView View { get; set; }
        [Inject] public SetInfoBarTextSignal SetInfoBarTextSignal { get; set; }
        [Inject] public RecenterCameraSignal RecenterCameraSignal { get; set; }
        [Inject] public ToggleRulerCameraSignal ToggleRulerCameraSignal { get; set; }
        [Inject] public ScreenshotSignal ScreenshotSignal { get; set; }
        [Inject] public SongLoadedSignal SongLoadedSignal { get; set; }
        [Inject] public ExportVideoSignal ExportVideoSignal { get; set; }

        #endregion

        public override void OnRegister()
        {
            SetInfoBarTextSignal.AddListener(OnInfoBarTextReceived);
            SongLoadedSignal.AddListener(OnSongLoadedReceived);

            View.RecenterCameraSignal.AddListener(OnRecenterCameraReceived);
            View.PointerInfoSignal.AddListener(OnPointerInfoSignal);
            View.RulerCameraSignal.AddListener(OnRulerCameraRequested);
            View.ScreenshotSignal.AddListener(OnScreenshotRequested);
            View.ExportVideoSignal.AddListener(OnExportVideoReceived);
            View.Initialize();
        }

        public override void OnRemove()
        {
            View.RecenterCameraSignal.RemoveListener(OnRecenterCameraReceived);
            View.PointerInfoSignal.RemoveListener(OnPointerInfoSignal);
            View.RulerCameraSignal.RemoveListener(OnRulerCameraRequested);
            View.ScreenshotSignal.RemoveListener(OnScreenshotRequested);
            View.ExportVideoSignal.RemoveListener(OnExportVideoReceived);

            SetInfoBarTextSignal.RemoveListener(OnInfoBarTextReceived);
            SongLoadedSignal.RemoveListener(OnSongLoadedReceived);
        }

        private void OnPointerInfoSignal(string value)
        {
            SetInfoBarTextSignal.Dispatch(value);
        }

        private void OnRecenterCameraReceived()
        {
            RecenterCameraSignal.Dispatch();
        }

        private void OnInfoBarTextReceived(string text)
        {
            View.RefreshText(text);
        }

        private void OnRulerCameraRequested(bool value)
        {
            ToggleRulerCameraSignal.Dispatch(value);
        }

        private void OnScreenshotRequested()
        {
            ScreenshotSignal.Dispatch();
        }

        private void OnSongLoadedReceived(AudioClip clip)
        {
            View.SetExportVideoButtonEnabled();
        }

        private void OnExportVideoReceived()
        {
            ExportVideoSignal.Dispatch();
        }
    }
}
