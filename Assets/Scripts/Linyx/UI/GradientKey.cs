using System;
using MaterialUI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Linyx.UI
{
    public sealed class GradientKey : MonoBehaviour
    {
        #region Serialize Fields
        [SerializeField] private VectorImage _icon;
        [SerializeField] private VectorImage _backgroundIcon;
        #endregion


        #region Public Attributes

        public string Guid;
        public bool IsAlpha;
        public float Alpha;
        public Color Color;
        public float Time;
        #endregion

        #region Private Attributes

        private RectTransform _parentRectTransform;
        private RectTransform _rectTransform;
        private FlexibleColorPicker _colorPicker;
        private Action<GradientKey> _onKeySelectedCallback;
        private Action<GradientKey> _onKeyDeletedCallback;
        private Action _onKeyUpdatedCallback;

        private bool _isSelected;
        #endregion

        public void Initialize(string guid, bool isAlpha, float time, FlexibleColorPicker colorPicker, Action<GradientKey> onKeySelectedCallback, Action<GradientKey> onKeyDeletedCallback, Action onKeyUpdatedCallback)
        {
            _parentRectTransform = transform.parent.GetComponent<RectTransform>();
            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.sizeDelta = new Vector2(10, 4);
            _colorPicker = colorPicker;

            Guid = guid;
            IsAlpha = isAlpha;
            Time = time;
            _onKeySelectedCallback = onKeySelectedCallback;
            _onKeyDeletedCallback = onKeyDeletedCallback;
            _onKeyUpdatedCallback = onKeyUpdatedCallback;
            _icon.vectorImageData = isAlpha
                ? MaterialUIIconHelper.GetIcon("CommunityMD", "arrow_down_bold").vectorImageData
                : MaterialUIIconHelper.GetIcon("CommunityMD", "arrow_up_bold").vectorImageData;

            _backgroundIcon.vectorImageData = isAlpha
                ? MaterialUIIconHelper.GetIcon("CommunityMD", "arrow_down_bold").vectorImageData
                : MaterialUIIconHelper.GetIcon("CommunityMD", "arrow_up_bold").vectorImageData;
            SetColor();
        }

        public void Select()
        {
            _isSelected = true;
            _rectTransform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            _onKeySelectedCallback.Invoke(this);
        }

        public void Deselect()
        {
            _isSelected = false;
            _rectTransform.localScale = Vector3.one;
        }

        public void SetAlpha(float alpha)
        {
            Alpha = alpha;
            Color color = _icon.color;
            color = new Color(color.r, color.g, color.b, alpha);
            _icon.color = color;
        }

        public void SetColor(Color color)
        {
            this.Color = color;
            _icon.color = color;
        }

        public void PointerUpdate(BaseEventData e)
        {
            Vector2 v = FlexibleColorPicker.GetNormalizedPointerPosition(_parentRectTransform, e);
            Time = v.x;
            UpdateMarker(v);
            _onKeyUpdatedCallback.Invoke();
        }

        private void UpdateMarker(Vector2 v)
        {
            Vector2 parentSize = _parentRectTransform.rect.size;
            Vector2 localPos = _rectTransform.localPosition;
            localPos.x = (v.x - _parentRectTransform.pivot.x) * parentSize.x;
            _rectTransform.localPosition = localPos;
        }

        private void Update()
        {
            if (_isSelected && Input.GetKeyDown(KeyCode.Delete))
            {
                DeleteKey();
            }
        }

        private void SetColor()
        {
            if (IsAlpha)
            {
                Color color = _icon.color;
                color = new Color(color.r, color.g, color.b, _colorPicker.Color.a);
                _icon.color = color;
                Alpha = color.a;
            }
            else
            {
                Color color = new Color(_colorPicker.Color.r, _colorPicker.Color.g, _colorPicker.Color.b, 255);
                _icon.color = color;
                Color = color;
            }
        }

        private void DeleteKey()
        {
            _onKeyDeletedCallback.Invoke(this);
        }
    }
}
