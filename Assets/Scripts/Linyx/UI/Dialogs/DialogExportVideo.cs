using System;
using MaterialUI;
using SFB;
using UnityEngine;

namespace Linyx.UI.Dialogs
{
    public sealed class DialogExportVideo : MaterialDialog
    {
        [SerializeField] private MaterialButton _openPathButton;
        [SerializeField] private MaterialDropdown _videoTypeDropdown;
        [SerializeField] private MaterialDropdown _resolutionDropdown;
        [SerializeField] private MaterialDropdown _framerateDropdown;
        [SerializeField] private MaterialButton _affirmativeButton;
        [SerializeField] private MaterialButton _cancelButton;

        private Action<ExportVideoDTO> _onValidateCallback;

        private string _path;
        private VideoType _videoType = VideoType.MP4;
        private int _width = 720;
        private int _height = 480;
        private int _framerate = 30;

        #region Public Methods

        public void Initialize(Action<ExportVideoDTO> onValidateCallback)
        {
            _onValidateCallback = onValidateCallback;
            _openPathButton.buttonObject.onClick.AddListener(OnOpenPathClicked);
            _videoTypeDropdown.onItemSelected.AddListener(OnVideoTypeItemSelected);
            _resolutionDropdown.onItemSelected.AddListener(OnResolutionItemSelected);
            _framerateDropdown.onItemSelected.AddListener(OnFramerateItemSelected);

            _cancelButton.buttonObject.onClick.AddListener(OnCancelClicked);
            _affirmativeButton.buttonObject.onClick.AddListener(OnValidateClicked);
        }

        public void OnValidateClicked()
        {
            _onValidateCallback?.Invoke(new ExportVideoDTO(_path, _videoType, _width, _height, _framerate));
            Hide();
        }

        public void OnCancelClicked()
        {
            Hide();
        }

        #endregion

        #region Private Methods
        private void OnOpenPathClicked()
        {
            string title = $"linyx{DateTime.Now:yyyy-mm-dd-hh-mm-ss}.{_videoType.ToString().ToLower()}";
            string path = StandaloneFileBrowser.SaveFilePanel("Export Video", "", title, _videoType.ToString().ToLower());
            if (!string.IsNullOrEmpty(path))
            {
                _path = path;
                CheckValidation();
            }
        }

        private void OnVideoTypeItemSelected(int index)
        {
            _videoType = (VideoType) index;
        }

        private void OnResolutionItemSelected(int index)
        {
            VideoResolution resolution = (VideoResolution)index;
            switch (resolution)
            {
                case VideoResolution.Resolution480p:
                    _width = 720;
                    _height = 480;
                    break;
                case VideoResolution.Resolution720p:
                    _width = 1280;
                    _height = 720;
                    break;
                case VideoResolution.Resolution1080p:
                    _width = 1920;
                    _height = 1080;
                    break;
                case VideoResolution.Resolution2160p:
                    _width = 3840;
                    _height = 2160;
                    break;
                case VideoResolution.Resolution4320p:
                    _width = 7680;
                    _height = 4320;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void OnFramerateItemSelected(int index)
        {
            VideoFramerate framerate = (VideoFramerate) index;
            switch (framerate)
            {
                case VideoFramerate.Video30:
                    _framerate = 30;
                    break;
                case VideoFramerate.Video60:
                    _framerate = 60;
                    break;
            }
        }

        private void CheckValidation()
        {
            if (_width > 0 && _height > 0 && !string.IsNullOrEmpty(_path))
            {
                _affirmativeButton.interactable = true;
            }
            else
            {
                _affirmativeButton.interactable = false;
            }
        }

        #endregion
    }

    public enum VideoType
    {
        MP4 = 0,
        GIF = 1
    }

    public enum VideoFramerate
    {
        Video30 = 0,
        Video60 = 1
    }

    public enum VideoResolution
    {
        Resolution480p = 0,
        Resolution720p = 1,
        Resolution1080p = 2,
        Resolution2160p = 3,
        Resolution4320p = 4,
    }

    public class ExportVideoDTO
    {
        public string Path { get; }
        public VideoType VideoType { get; }
        public int Width { get; }
        public int Height { get; }
        public int Framerate { get; }

        public ExportVideoDTO(string path, VideoType videoType, int width, int height, int framerate)
        {
            Path = path;
            VideoType = videoType;
            Width = width;
            Height = height;
            Framerate = framerate;
        }

    }
}
