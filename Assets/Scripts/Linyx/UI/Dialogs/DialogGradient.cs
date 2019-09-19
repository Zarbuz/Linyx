using System;
using System.Collections.Generic;
using System.Linq;
using MaterialUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Linyx.UI.Dialogs
{
    public sealed class DialogGradient : MaterialDialog
    {
        #region Serialize Fields

        [SerializeField] private RectTransform _alphaKeysParent;
        [SerializeField] private RectTransform _colorKeysParent;
        [SerializeField] private GradientKey _gradientKeyItem;
        [SerializeField] private FlexibleColorPicker _flexibleColorPicker;
        [SerializeField] private RawImage _rawImage;
        
        #endregion

        #region Private Attributes

        private List<GradientKey> _alphasKeys = new List<GradientKey>();
        private List<GradientKey> _colorsKeys = new List<GradientKey>();
        private Action<Gradient> _onAffirmativeCallback;
        private Gradient _gradient;

        #endregion

        public void Initialize(Gradient gradient, Action<Gradient> onAffirmatedCallback)
        {
            foreach (GradientAlphaKey gradientAlphaKey in gradient.alphaKeys)
            {
                Vector2 parentSize = _alphaKeysParent.rect.size;
                Vector2 localPos = Vector2.zero;
                localPos.x = (gradientAlphaKey.time - _alphaKeysParent.pivot.x) * parentSize.x + (parentSize.x / 2);

                GradientKey alphaKey = InstantiateGradientKey(localPos, _alphaKeysParent, true, gradientAlphaKey.time);
                alphaKey.SetAlpha(gradientAlphaKey.alpha);
                _alphasKeys.Add(alphaKey);
            }

            foreach (GradientColorKey gradientColorKey in gradient.colorKeys)
            {
                Vector2 parentSize = _colorKeysParent.rect.size;
                Vector2 localPos = Vector2.zero;
                localPos.x = (gradientColorKey.time - _colorKeysParent.pivot.x) * parentSize.x + (parentSize.x / 2);

                GradientKey colorKey = InstantiateGradientKey(localPos, _colorKeysParent, false, gradientColorKey.time);
                colorKey.SetColor(gradientColorKey.color);
                _colorsKeys.Add(colorKey);
            }

            _onAffirmativeCallback = onAffirmatedCallback;
            _gradient = gradient;
            CreateTexture();
        }

        public void OnValidateClicked()
        {
            _onAffirmativeCallback.Invoke(_gradient);
            Hide();
        }

        public void OnCancelClicked()
        {
            Hide();
        }

        public void CreateAlphaKey(BaseEventData e)
        {
            if (_alphasKeys.Count < 8)
            {
                Vector2 v = FlexibleColorPicker.GetNormalizedPointerPosition(_alphaKeysParent, e);
                Vector2 parentSize = _alphaKeysParent.rect.size;
                Vector2 localPos = Vector2.zero;
                localPos.x = (v.x - _alphaKeysParent.pivot.x) * parentSize.x + (parentSize.x / 2);
                GradientKey gradientKey = InstantiateGradientKey(localPos, _alphaKeysParent, true, v.x);
                _alphasKeys.Add(gradientKey);
                UpdateGradient();
            }
        }

        public void CreateColorKey(BaseEventData e)
        {
            if (_colorsKeys.Count < 8)
            {
                Vector2 v = FlexibleColorPicker.GetNormalizedPointerPosition(_colorKeysParent, e);
                Vector2 parentSize = _colorKeysParent.rect.size;
                Vector2 localPos = Vector2.zero;
                localPos.x = (v.x - _colorKeysParent.pivot.x) * parentSize.x + (parentSize.x / 2);
                GradientKey gradientKey = InstantiateGradientKey(localPos, _colorKeysParent, false, v.x);
                _colorsKeys.Add(gradientKey);
                UpdateGradient();
            }
        }

        private GradientKey InstantiateGradientKey(Vector2 position, RectTransform parent, bool isAlpha, float time)
        {
            GradientKey gradientKey = (Instantiate(_gradientKeyItem, Vector3.zero, Quaternion.identity, parent));
            RectTransform rt = gradientKey.GetComponent<RectTransform>();
            rt.anchoredPosition = position;
            rt.SetParent(parent);
            gradientKey.Initialize(Guid.NewGuid().ToString("N"), isAlpha, time, _flexibleColorPicker, OnKeySelected, OnKeyDeleted, UpdateGradient);
            return gradientKey;
        }

        private void CreateTexture()
        {
            Vector2 size = _rawImage.rectTransform.rect.size;
            int width = (int)size.x;
            int height = (int)size.y;
            _rawImage.texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
            {
                name = _rawImage.name,
                wrapMode = TextureWrapMode.Clamp
            };
            UpdateGradient();
        }

        private void UpdateGradient()
        {
            Texture2D tex = (Texture2D) _rawImage.texture;
            int width = tex.width;
            int height = tex.height;
            Color[] pixels = new Color[width * height];

            GradientAlphaKey[] gradientAlphaKeys = new GradientAlphaKey[_alphasKeys.Count];
            GradientColorKey[] gradientColorKeys = new GradientColorKey[_colorsKeys.Count];

            for (int i = 0; i < _alphasKeys.Count; i++)
            {
                GradientKey alphaKey = _alphasKeys[i];
                gradientAlphaKeys[i].alpha = alphaKey.Alpha;
                gradientAlphaKeys[i].time = alphaKey.Time;
            }

            for (int i = 0; i < _colorsKeys.Count; i++)
            {
                GradientKey colorKey = _colorsKeys[i];
                gradientColorKeys[i].color = colorKey.Color;
                gradientColorKeys[i].time = colorKey.Time;
            }

            _gradient.alphaKeys = gradientAlphaKeys;
            _gradient.colorKeys = gradientColorKeys;

            for (int x = 0; x < width; x++)
            {
                float normX = (float) x / (width - 1);
                for (int y = 0; y < height; y++)
                {
                    Color color = _gradient.Evaluate(normX);
                    int pixelIndex = x + y * width;
                    pixels[pixelIndex] = color;
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
        }

        private void OnKeySelected(GradientKey selectedKey)
        {
            _alphasKeys.ForEach(t =>
            {
                if (t.Guid != selectedKey.Guid)
                    t.Deselect();
            });
            _colorsKeys.ForEach(t =>
            {
                if (t.Guid != selectedKey.Guid)
                    t.Deselect();
            });
        }

        private void OnKeyDeleted(GradientKey deletedKey)
        {
            if (deletedKey.IsAlpha)
            {
                if (_alphasKeys.Count > 1)
                {
                    _alphasKeys.RemoveAll(t => t.Guid == deletedKey.Guid);
                    Destroy(deletedKey.gameObject);
                    UpdateGradient();
                }
            }
            else
            {
                if (_colorsKeys.Count > 1)
                {
                    _colorsKeys.RemoveAll(t => t.Guid == deletedKey.Guid);
                    Destroy(deletedKey.gameObject);
                    UpdateGradient();
                }
            }
        }
    }
}
