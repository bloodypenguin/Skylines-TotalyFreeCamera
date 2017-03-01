using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using TotalyFreeCamera.Redirection;
using UnityEngine;

namespace TotalyFreeCamera.Detours
{
    [TargetType(typeof(CameraController))]
    public class CameraControllerDetour : CameraController
    {
        private static Dictionary<MethodInfo, RedirectCallsState> _redirects;
        private static bool initialized = false;
        private static bool cachedFreeCamera = false;
        private static Camera camera;
        private static FieldInfo cachedFreeCameraField = typeof(CameraController).GetField("m_cachedFreeCamera",
    BindingFlags.NonPublic | BindingFlags.Instance);

        public static void Deploy()
        {
            if (_redirects != null)
            {
                return;
            }
            _redirects = RedirectionUtil.RedirectType(typeof(CameraControllerDetour));
        }

        public static void Revert()
        {
            if (_redirects == null)
            {
                return;
            }
            foreach (var redirect in _redirects)
            {
                RedirectionHelper.RevertRedirect(redirect.Key, redirect.Value);
            }
            _redirects = null;
            initialized = false;
            camera = null;
            cachedFreeCamera = false;
        }

        [RedirectMethod]
        private void UpdateFreeCamera()
        {
            if (!initialized)
            {
                camera = this.GetComponent<Camera>();
                cachedFreeCamera = (bool) cachedFreeCameraField.GetValue(this);
                cachedFreeCameraField.SetValue(this, true);
                initialized = true;
            }

            if (this.m_freeCamera != cachedFreeCamera)
            {
                cachedFreeCamera = this.m_freeCamera;
                UIView.Show(!this.m_freeCamera);
                Singleton<NotificationManager>.instance.NotificationsVisible = !this.m_freeCamera;
                Singleton<GameAreaManager>.instance.BordersVisible = !this.m_freeCamera;
                Singleton<DistrictManager>.instance.NamesVisible = !this.m_freeCamera;
                Singleton<PropManager>.instance.MarkersVisible = !this.m_freeCamera;
                Singleton<GuideManager>.instance.TutorialDisabled = this.m_freeCamera;
            }


            if (cachedFreeCamera)
                camera.rect = new Rect(0.0f, 0.0f, 1f, 1f);
            else
                camera.rect = new Rect(0.0f, 0.105f, 1f, 0.895f);
        }
    }
}