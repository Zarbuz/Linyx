using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Linyx.UI
{
    public sealed class InfoPointer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private string _text;

        private Action<string> _onPointerInfoCallback;

        public void Initialize(Action<string> onPointerInfoCallback)
        {
            _onPointerInfoCallback = onPointerInfoCallback;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _onPointerInfoCallback?.Invoke(_text);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _onPointerInfoCallback?.Invoke(string.Empty);
        }
    }
}
