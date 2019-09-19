using Linyx.Models;

namespace Linyx.Services.Curve
{
    public interface ICurveService : IServiceBase
    {
        RTAnimationCurve GetAnimationCurve();
        bool IsEditorOpen();
        void Close();
    }
}
