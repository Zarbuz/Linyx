using System;
using Linyx.Controllers.Edit;
using Linyx.Models;
using UnityEngine;

namespace Linyx.Services.Curve
{
    public sealed class CurveService : MonoBehaviour, ICurveService
    {
        #region Serialize Fields

        [SerializeField] private RTAnimationCurve _rtAnimationCurve;

        #endregion

        #region Injections


        #endregion

        #region Private Attributes

        private bool _checkForCurve;
        private bool _isEditorOpen;
        private AnimationCurve _lastAnimationCurve;
        #endregion

        #region Public Methods
        public void Initialize()
        {

        }

        public bool IsEditorOpen()
        {
            return !_rtAnimationCurve.IsCurveEditorClosed();
        }

        public void Close()
        {
            _rtAnimationCurve.CloseCurveEditor();
        }

        public RTAnimationCurve GetAnimationCurve()
        {
            return _rtAnimationCurve;
        }

        #endregion

    }
}
