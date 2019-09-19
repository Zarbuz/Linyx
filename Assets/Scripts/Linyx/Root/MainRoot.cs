using strange.extensions.context.impl;

namespace Linyx.Root
{
    public class MainRoot : ContextView
    {
        private void Awake()
        {
            context = new MainContext(this, true);
            context.Start();
        }
    }
}
