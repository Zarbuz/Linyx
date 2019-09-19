using UnityEngine;

namespace Linyx.Services.Draw
{
    public interface IDrawService : IServiceBase
    {
        GameObject GetLinePrefab();
        GameObject GetRoot3D();
        Material GetLineMaterial();
    }
}
