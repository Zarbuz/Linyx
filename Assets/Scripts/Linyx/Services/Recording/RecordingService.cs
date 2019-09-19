using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Linyx.Services.Audio;
using Linyx.UI.Dialogs;
using NatCorder;
using NatCorder.Clocks;
using NatCorder.Inputs;
using UnityEngine;

namespace Linyx.Services.Recording
{
    public sealed class RecordingService : MonoBehaviour, IRecordingService
    {
        #region Serialize Fields

        [SerializeField] private UnityEngine.Camera _camera;
        [SerializeField] private Canvas _mainCanvas;
        [SerializeField] private Canvas _recordCanvas;

        #endregion

        #region Injections
        [Inject] public IAudioPeerService AudioPeerService { get; set; }

        #endregion
        private IMediaRecorder _mediaRecorder;
        private IClock _clock;

        private RenderTexture _tmpRenderTexture;
        private CameraInput _cameraInput;
        private AudioInput _audioInput;
        private Coroutine _coroutine;

        private bool _record;
        private ExportVideoDTO _settings;

        #region Public Methods

        public void Initialize()
        {

        }

        public void StartRecording(ExportVideoDTO exportVideo)
        {
            _record = true;
            _settings = exportVideo;

            switch (exportVideo.VideoType)
            {
                case VideoType.MP4:
                    _mediaRecorder = new MP4Recorder(exportVideo.Width, exportVideo.Height, exportVideo.Framerate, AudioSettings.outputSampleRate, (int)AudioSettings.speakerMode, OnStopRecording);
                    break;
                case VideoType.GIF:
                    _mediaRecorder = new GIFRecorder(exportVideo.Width, exportVideo.Height, exportVideo.Framerate, OnStopRecording);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _clock = new RealtimeClock();
            _cameraInput = new CameraInput(_mediaRecorder, _clock, _camera);
            _audioInput = new AudioInput(_mediaRecorder, _clock, AudioPeerService.GetAudioListener());

            _mainCanvas.gameObject.SetActive(false);
            _recordCanvas.gameObject.SetActive(true);

            AudioPeerService.Stop();
            AudioPeerService.Play();
            _coroutine = StartCoroutine(_Play());
        }

        public void StopRecording()
        {
            _record = false;
            _mediaRecorder.Dispose();
            _cameraInput.Dispose();
            _audioInput.Dispose();

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _mainCanvas.gameObject.SetActive(true);
            _recordCanvas.gameObject.SetActive(false);
            AudioPeerService.Stop();
        }


        #endregion

        #region Private Methods
        private void OnStopRecording(string path)
        {
            Debug.Log(path);
            File.Move(path, _settings.Path);
        }

        private IEnumerator _Play()
        {
            yield return new WaitForSeconds(AudioPeerService.GetAudioClip().length);
            StopRecording();
        }

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (_record)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    StopRecording();
                }
            }
        }

        #endregion
    }
}
