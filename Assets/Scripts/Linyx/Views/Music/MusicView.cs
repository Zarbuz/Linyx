using System;
using Linyx.Root;
using MaterialUI;
using strange.extensions.signal.impl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Linyx.Views.Music
{
    public sealed class MusicView : BaseView
    {
        #region UI

        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private MaterialButton _previousButton;
        [SerializeField] private MaterialButton _playPauseButton;
        [SerializeField] private MaterialButton _stopButton;
        [SerializeField] private MaterialButton _nextButton;
        [SerializeField] private MaterialButton _loadSongButton;

        [SerializeField] private TextMeshProUGUI _songNameText;
        [SerializeField] private TextMeshProUGUI _actualPositionText;
        [SerializeField] private TextMeshProUGUI _songTotalDurationText;

        [SerializeField] private Image _songPlayerBar;
        [SerializeField] private Slider _songPlayerSlider;

        #endregion

        #region Local Signals

        public Signal LoadSongSignal = new Signal();

        #endregion

        #region Private Attributes

        private bool _isPlaying;
        private bool _active;
        private float _amount;

        #endregion

        #region Public Methods

        public override void Initialize()
        {
            base.Initialize();
            _previousButton.buttonObject.onClick.AddListener(OnPreviousClicked);
            _playPauseButton.buttonObject.onClick.AddListener(OnPlayOrPauseClicked);
            _nextButton.buttonObject.onClick.AddListener(OnNextClicked);
            _stopButton.buttonObject.onClick.AddListener(OnStopClicked);
            _loadSongButton.buttonObject.onClick.AddListener(OnLoadSongClicked);
            _songPlayerSlider.onValueChanged.AddListener(OnSongPlayerValueChanged);
        }

        public void PlaySound(AudioClip clip)
        {
            _songNameText.SetText(clip.name);
            _audioSource.clip = clip;
            DisplaySongDuration();
            OnStopClicked();
            _playPauseButton.iconVectorImageData = MaterialIconHelper.GetIcon("play_arrow").vectorImageData;
        }

        public void StopFromSignal()
        {
            OnStopClicked();
        }

        public void PlayFromSignal()
        {
            OnPlayOrPauseClicked();
        }

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (_active)
            {
                if (_isPlaying)
                {
                    if (_audioSource.isPlaying)
                    {
                        _amount = (_audioSource.time) / (_audioSource.clip.length);
                        _songPlayerBar.fillAmount = _amount;
                        _actualPositionText.SetText(Linyx.Utils.Utils.FromSecondsToMinutesAndSeconds(_audioSource.time));
                    }
                    else
                    {
                        OnStopClicked();
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private void DisplaySongDuration()
        {
            _songTotalDurationText.SetText(Linyx.Utils.Utils.FromSecondsToMinutesAndSeconds(_audioSource.clip.length));
        }

        private void OnStopClicked()
        {
            if (_audioSource.clip == null)
                return;

            _active = false;
            _isPlaying = false;

            _audioSource.Stop();
            _audioSource.time = 0;

            _playPauseButton.iconVectorImageData = MaterialIconHelper.GetIcon("pause").vectorImageData;
            _amount = 0f;
            _songPlayerSlider.value = 0f;
            _songPlayerBar.fillAmount = 0;
            _actualPositionText.SetText("00:00");

        }

        private void OnPlayOrPauseClicked()
        {
            if (_audioSource.clip == null)
                return;

            if (_isPlaying)
            {
                _active = false;
                _isPlaying = false;
                _audioSource.Pause();
                _playPauseButton.iconVectorImageData = MaterialIconHelper.GetIcon("play_arrow").vectorImageData;
            }
            else
            {
                _audioSource.Play();
                _playPauseButton.iconVectorImageData = MaterialIconHelper.GetIcon("pause").vectorImageData;
                _isPlaying = true;
                _active = true;
            }
        }

        private void OnPreviousClicked()
        {
            OnStopClicked();
        }

        private void OnNextClicked()
        {
            OnStopClicked();
        }

        private void OnLoadSongClicked()
        {
            LoadSongSignal.Dispatch();
        }

        private void OnSongPlayerValueChanged(float value)
        {
            if (_audioSource.clip == null)
                return;

            _active = false;
            if (value * _audioSource.clip.length < _audioSource.clip.length)
            {
                _audioSource.time = value * _audioSource.clip.length;
            }
            else if (value * _audioSource.clip.length >= _audioSource.clip.length)
                OnStopClicked();

            _actualPositionText.text = Linyx.Utils.Utils.FromSecondsToMinutesAndSeconds(_audioSource.time);
            _songPlayerBar.fillAmount = value;
            _active = true;
        }
        #endregion

    }
}
