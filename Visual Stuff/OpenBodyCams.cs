using OpenBodyCams;
using OpenBodyCams.API;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using static OpenBodyCams.BodyCamComponent;
using static suitsTerminal.PictureInPicture;

namespace suitsTerminal
{
    internal class OpenBodyCams
    {
        internal static MonoBehaviour TerminalMirrorCam;

        internal static Vector2Int defaultRes;

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void CreateOpenBodyCamsMirror()
        {
            suitsTerminal.X("Getting ZaggyCam texture");
            if ((TerminalMirrorCam == null || TerminalMirrorCam.gameObject == null || ((BodyCamComponent)TerminalMirrorCam) == null))
                CreateTerminalMirror();

            suitsTerminal.X($"Attempting to grab targetTexture");
            SetBodyCamTexture(((BodyCamComponent)TerminalMirrorCam).GetCamera().targetTexture);
            ((BodyCamComponent)TerminalMirrorCam).ForceEnableCamera = true;

            Camera obcMirror = GetMirrorCam();
            obcMirror.orthographic = true;
            obcMirror.orthographicSize = 0.55f;
            obcMirror.usePhysicalProperties = false;
            obcMirror.farClipPlane = 30f;
            obcMirror.nearClipPlane = 0.05f;
            obcMirror.fieldOfView = 130f;
            CamInit(obcMirror);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void OpenBodyCamsMirror(bool state)
        {
            if (state)
                CreateOpenBodyCamsMirror();
            else
                DestroyBodyCamsMirror();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static Camera GetMirrorCam()
        {
            return ((BodyCamComponent)TerminalMirrorCam).GetCamera();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void DestroyBodyCamsMirror()
        {
            if (TerminalMirrorCam != null && TerminalMirrorCam.gameObject != null)
            {
                Camera cam2destroy = GetMirrorCam();
                Object.Destroy(cam2destroy);
            }
        }

        private static void SetBodyCamTexture(RenderTexture texture)
        {
            suitsTerminal.X("Setting mirror texture stuff");
            pipRawImage.texture = texture;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void CreateTerminalMirror()
        {
            if (!suitsTerminal.OpenBodyCams)
                return;

            if (TerminalMirrorCam != null && TerminalMirrorCam.gameObject != null)
                DestroyBodyCamsMirror();

            suitsTerminal.X("CreateTerminalMirror called");
            var terminalMirrorCam = BodyCam.CreateBodyCam(suitsTerminal.Terminal.gameObject, screenMaterial: null);

            TerminalMirrorCam = terminalMirrorCam;
            terminalMirrorCam.OnRenderTextureCreated += SetBodyCamTexture;
            terminalMirrorCam.OnRenderersToHideChanged += GetRenderersForOBC(((BodyCamComponent)TerminalMirrorCam).GetComponent<Renderer>());
            terminalMirrorCam.OnCameraCreated += ResetTransform;
            terminalMirrorCam.OnBlankedSet += CamIsBlanked;
            Camera cam = terminalMirrorCam.GetCamera();
            cam.gameObject.name = "suitsTerminal mirrorcam";

            terminalMirrorCam.Resolution = defaultRes;
            terminalMirrorCam.SetTargetToTransform(suitsTerminal.Terminal.transform);
            suitsTerminal.X("suitsTerminal mirrorcam created!");
        }

        private static GetRenderersToHide GetRenderersForOBC(Renderer original)
        {
            return original => [.. suitsTerminal.Terminal.GetComponentsInChildren<Renderer>()];
        }

        private static void CamIsBlanked(bool isBlanked)
        {
            suitsTerminal.X($"CamIsBlanked: {isBlanked}");
        }

        private static void ResetTransform(Camera cam)
        {
            suitsTerminal.X("ResetTransform Called!");
            CamInit(cam);
        }
    }

}
