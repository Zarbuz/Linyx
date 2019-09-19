using System;
using MaterialUI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace Linyx.Views.Header
{
    public sealed class HeaderView : BaseView
    {
        #region UI

        [SerializeField] public MaterialSwitch _drawSwitch;

        [Header("Header Buttons")]
        [SerializeField] private MaterialButton _undoButton;
        [SerializeField] private MaterialButton _redoButton;
        [SerializeField] private MaterialButton _saveButton;
        [SerializeField] private MaterialButton _saveAsButton;
        [SerializeField] private MaterialButton _openButton;
        [SerializeField] private MaterialButton _newButton;

        [Header("Colors")]
        [SerializeField] private Color _saveAvailableColor;
        [SerializeField] private Color _saveDefaultColor;

        #endregion

        #region Local Signals
        public Signal<bool> ToggleDrawModeSignal = new Signal<bool>();
        public Signal UndoSignal = new Signal();
        public Signal RedoSignal = new Signal();
        public Signal SaveProjectSignal = new Signal();
        public Signal SaveProjectAsSignal = new Signal();
        public Signal LoadProjectSignal = new Signal();
        public Signal NewProjectSignal = new Signal();
        #endregion

        #region Public Methods
        public override void Initialize()
        {
            base.Initialize();
            _drawSwitch.toggle.onValueChanged.AddListener(OnDrawValueChanged);
            _saveButton.buttonObject.onClick.AddListener(OnSaveClicked);
            _saveAsButton.buttonObject.onClick.AddListener(OnSaveAsClicked);
            _undoButton.buttonObject.onClick.AddListener(OnUndoClicked);
            _redoButton.buttonObject.onClick.AddListener(OnRedoClicked);
            _openButton.buttonObject.onClick.AddListener(OnLoadClicked);
            _newButton.buttonObject.onClick.AddListener(OnNewClicked);
        }

        public void SetUndoAvailable(bool available)
        {
            _undoButton.interactable = available;
        }

        public void SetRedoAvailable(bool available)
        {
            _redoButton.interactable = available;   
        }

        public void SetSaveAvailable(bool available)
        {
            _saveButton.iconColor = available ? _saveAvailableColor : _saveDefaultColor;
        }
        #endregion


        #region Private Methods
        private void OnDrawValueChanged(bool enabled)
        {
            ToggleDrawModeSignal.Dispatch(!enabled);
        }

        private void OnSaveClicked()
        {
            SaveProjectSignal.Dispatch();
        }

        private void OnSaveAsClicked()
        {
            SaveProjectAsSignal.Dispatch();
        }
        private void OnLoadClicked()
        {
            LoadProjectSignal.Dispatch();
        }

        private void OnUndoClicked()
        {
            UndoSignal.Dispatch();
        }

        private void OnRedoClicked()
        {
            RedoSignal.Dispatch();
        }
        
        private void OnNewClicked()
        {
            NewProjectSignal.Dispatch();
        }
        #endregion

    }
}
