using Linyx.Services.Camera;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace Linyx.Controllers.Camera
{
    public sealed class SetCameraPositionCommand : Command
    {
        [Inject] public ICameraService CameraService { get; set; }
        [Inject] public Vector2 Value { get; set; }

        public override void Execute()
        {
            CameraService.SetCameraPosition(Value);
        }
    }

    public sealed class SetCameraPositionSignal : Signal<Vector2> { }
}
