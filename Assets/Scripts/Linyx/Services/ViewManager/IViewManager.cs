namespace Linyx.Services.ViewManager
{
    public  interface IViewManager : IServiceBase
    {
        void ChangeView(string view);
        void ToggleView(string view);
    }
}
