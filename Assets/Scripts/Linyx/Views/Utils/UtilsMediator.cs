using Linyx.Controllers.Bottom;
using Linyx.Controllers.Camera;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace Linyx.Views.Utils
{
    public sealed class UtilsMediator : Mediator
    {
        #region Injections

        [Inject] public UtilsView View { get; set; }
        [Inject] public ToggleRulerCameraSignal ToggleRulerCameraSignal { get; set; }
        [Inject] public NewCameraPositionSignal NewCameraPositionSignal { get; set; }
        [Inject] public SetCameraPositionSignal SetCameraPositionSignal { get; set; }

        #endregion

        public override void OnRegister()
        {
            View.CameraNewPositionSignal.AddListener(OnNewPositionCameraRequested);

            ToggleRulerCameraSignal.AddListener(OnToggleRulerCameraReceived);
            NewCameraPositionSignal.AddListener(OnNewPositionCameraReceived);
            View.Initialize();
        }

        public override void OnRemove()
        {
            View.CameraNewPositionSignal.RemoveListener(OnNewPositionCameraRequested);

            ToggleRulerCameraSignal.RemoveListener(OnToggleRulerCameraReceived);
            NewCameraPositionSignal.RemoveListener(OnNewPositionCameraReceived);
        }

        private void OnToggleRulerCameraReceived(bool show)
        {
            View.ShowRuler(show);
        }

        private void OnNewPositionCameraReceived(Vector2 position)
        {
            View.SetPositionCamera(position);
        }

        private void OnNewPositionCameraRequested(Vector2 position)
        {
            SetCameraPositionSignal.Dispatch(position);
        }
    }
}
