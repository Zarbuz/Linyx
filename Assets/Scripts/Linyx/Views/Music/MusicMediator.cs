using Linyx.Controllers.Bottom;
using Linyx.Controllers.Music;
using Linyx.Services.Audio;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace Linyx.Views.Music
{
    public sealed class MusicMediator : Mediator
    {
        [Inject] public MusicView View { get; set; }
        [Inject] public LoadSongSignal LoadSongSignal { get; set; }
        [Inject] public SongLoadedSignal SongLoadedSignal { get; set; }
        [Inject] public SetInfoBarTextSignal SetInfoBarTextSignal { get; set; }
        [Inject] public SongStartedSignal SongStartedSignal { get; set; }
        [Inject] public SongStoppedSignal SongStoppedSignal { get; set; }

        public override void OnRegister()
        {
            base.OnRegister();
            View.PointerInfoSignal.AddListener(OnPointerInfoSignal);
            View.LoadSongSignal.AddListener(OnLoadSongRequested);
            SongLoadedSignal.AddListener(OnSongLoadedReceived);
            SongStartedSignal.AddListener(OnSongStartedReceived);
            SongStoppedSignal.AddListener(OnSongStoppedReceived);
            View.Initialize();
        }

        public override void OnRemove()
        {
            base.OnRemove();
            View.PointerInfoSignal.RemoveListener(OnPointerInfoSignal);
            View.LoadSongSignal.RemoveListener(OnLoadSongRequested);
            SongLoadedSignal.RemoveListener(OnSongLoadedReceived);
            SongStoppedSignal.RemoveListener(OnSongStoppedReceived);
            SongStartedSignal.RemoveListener(OnSongStartedReceived);
        }

        private void OnPointerInfoSignal(string value)
        {
            SetInfoBarTextSignal.Dispatch(value);
        }

        private void OnLoadSongRequested()
        {
            LoadSongSignal.Dispatch();
        }

        private void OnSongLoadedReceived(AudioClip clip)
        {
            View.PlaySound(clip);
        }

        private void OnSongStartedReceived()
        {
            View.PlayFromSignal();
        }

        private void OnSongStoppedReceived()
        {
            View.StopFromSignal();
        }
    }
}
