using System;
using MaterialUI;
using UnityEngine;

namespace Linyx.UI.Dialogs
{
    public sealed class DialogColorPicker : MaterialDialog
    {
        #region Serialize Fields
        [SerializeField] private FlexibleColorPicker _flexibleColorPicker;
        #endregion

        #region Private Attributes

        private Action<Color> _onAffirmativeCallback;

        #endregion

        #region Public Methods

        public void Initialize(Color color, Action<Color> onAffirmativeCallback)
        {
            _flexibleColorPicker.Color = color;
            _onAffirmativeCallback = onAffirmativeCallback;
        }


        public void OnValidateClicked()
        {
            _onAffirmativeCallback.Invoke(_flexibleColorPicker.Color);
            Hide();
        }

        public void OnCancelClicked()
        {
            Hide();
        }
        #endregion
    }
}
