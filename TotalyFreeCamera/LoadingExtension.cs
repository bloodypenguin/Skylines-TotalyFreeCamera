using System.Reflection;
using ICities;
using TotalyFreeCamera.Detours;

namespace TotalyFreeCamera
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            CameraControllerDetour.Deploy();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            CameraControllerDetour.Revert();
        }
    }
}