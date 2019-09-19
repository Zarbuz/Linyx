using System;
using Linyx.Services.Brush;
using strange.extensions.command.impl;
using strange.extensions.signal.impl;

namespace Linyx.Controllers.Brush
{
    public sealed class SetBrushShapeIndexCommand : Command
    {
        [Inject] public IBrushService BrushService { get; set; }
        [Inject] public int Value { get; set; }

        public override void Execute()
        {
            BrushService.SetBrushShape((Shape)Value);
        }
    }

    public sealed class SetBrushShapeIndexSignal : Signal<int> { }
}
