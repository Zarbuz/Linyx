using Linyx.Controllers.InitServices;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;
using UnityEngine;

namespace Linyx.Controllers.Start
{
    public sealed class StartCommand : Command
    {
        [Inject] public InitServicesSignal InitServicesSignal { get; set; }

        public override void Execute()
        {
            InitServicesSignal.Dispatch();
        }
    }

    public sealed class StartSignal : Signal { }

}
