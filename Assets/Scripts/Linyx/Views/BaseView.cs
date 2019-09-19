using System;
using Linyx.UI;
using MaterialUI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace Linyx.Views
{
    public abstract class BaseView : View
    {
        [Header("Base View")]
        [SerializeField] protected GameObject _panelRoot;
        [SerializeField] protected GameObject _panelReduced;

        [SerializeField] private MaterialButton _togglePanelButton;
        [SerializeField] private MaterialButton _expandPanelButton;

        public bool IsPanelOpen { get; set; } = true;

        protected RectTransform _rectTransform;

        public Signal<string> TogglePanelSignal = new Signal<string>();
        public Signal<string> PointerInfoSignal = new Signal<string>();

        public virtual void Initialize()
        {
            _rectTransform = GetComponent<RectTransform>();
            _togglePanelButton?.buttonObject.onClick.AddListener(OnTogglePanelClicked);
            _expandPanelButton?.buttonObject.onClick.AddListener(OnTogglePanelClicked);
            InfoPointer[] pointers = GetComponentsInChildren<InfoPointer>(true);
            foreach (InfoPointer infoPointer in pointers)
            {
                infoPointer.Initialize(OnInfoPointer);
            }
        }
        public void Show()
        {
            _panelRoot.SetActive(true);
        }

        public void Hide()
        {
            _panelRoot.SetActive(false);
        }

        private void OnTogglePanelClicked()
        {
            IsPanelOpen = !IsPanelOpen;
            _panelRoot.SetActive(IsPanelOpen);
            _panelReduced.SetActive(!IsPanelOpen);
            TogglePanelSignal.Dispatch(GetType().Name);
        }

        private void OnInfoPointer(string text)
        {
            PointerInfoSignal.Dispatch(text);
        }
    }
}
