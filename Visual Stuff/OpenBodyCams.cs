using OpenBodyCams;
using OpenBodyCams.API;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static suitsTerminal.Misc;
using static suitsTerminal.PictureInPicture;
using OpenLib.Common;

namespace suitsTerminal
{
    internal class OpenBodyCams
    {
        internal static MonoBehaviour TerminalMirrorCam;

        //internal static Vector2Int defaultRes;

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static Camera GetMirrorCam()
        {
            if ((TerminalMirrorCam == null || TerminalMirrorCam.gameObject == null || ((BodyCamComponent)TerminalMirrorCam) == null))
                OpenBodyCamsMirror();

            return ((BodyCamComponent)TerminalMirrorCam).GetCamera();
        }

        private static void SetBodyCamTexture(RenderTexture texture)
        {
            suitsTerminal.X("Setting mirror texture stuff");
            pipRawImage.texture = texture;
        }

        private static Renderer[] GetStuffToHide()
        {
            suitsTerminal.X("Getting renderers (stuff) to hide from mirrorcam!");

            Renderer termGameObject = suitsTerminal.Terminal.gameObject.GetComponent<MeshRenderer>();
            GameObject termCable = GetGameObject("Environment/HangarShip/Terminal/BezierCurve.001");
            GameObject termKeyboard = GetGameObject("Environment/HangarShip/Terminal/Terminal.003");
            Renderer termCableRender = termCable.GetComponent<MeshRenderer>();
            Renderer termKeyboardRender = termKeyboard.GetComponent<MeshRenderer>();
            Renderer[] allRenderers = [termGameObject, termCableRender, termKeyboardRender];
            return allRenderers;
        }

        private static void CamIsBlanked(bool isBlanked)
        {
            suitsTerminal.X($"CamIsBlanked: {isBlanked}");
            ResidualCamsCheck();
        }

        private static void ResetTransform(Camera cam)
        {
            suitsTerminal.X("ResetTransform Called!");
            CamInit(cam);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ResidualCamsCheck()
        {

            if (TerminalMirrorCam != null || ((BodyCamComponent)TerminalMirrorCam) != null)
            {
                Object.Destroy(((BodyCamComponent)TerminalMirrorCam));
                TerminalMirrorCam = null!;
                suitsTerminal.X("Attempting to destroy residual TerminalMirrorCam");
            }
        }

        // -------------------------------------------------------- //

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void TerminalMirrorStatus(bool enabled)
        {
            if (TerminalMirrorCam != null || ((BodyCamComponent)TerminalMirrorCam) != null)
            {
                Camera getCam = ((BodyCamComponent)TerminalMirrorCam).GetCamera();
                ToggleCamState(getCam, enabled);
                suitsTerminal.X($"OBC - Setting Mirror Status: [{enabled}]");
            }
            else if(enabled)
                OpenBodyCamsMirror();

        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void OpenBodyCamsMirror()
        {
            suitsTerminal.X("OBC - Getting ZaggyCam texture OpenBodyCamsMirror()");
            if ((TerminalMirrorCam == null || TerminalMirrorCam.gameObject == null || ((BodyCamComponent)TerminalMirrorCam) == null))
                CreateTerminalMirror();

            suitsTerminal.X($"OBC - Attempting to grab targetTexture");
            SetBodyCamTexture(((BodyCamComponent)TerminalMirrorCam).GetCamera().targetTexture);
            ((BodyCamComponent)TerminalMirrorCam).ForceEnableCamera = true;

        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void CreateTerminalMirror()
        {
            if (!suitsTerminal.OpenBodyCams)
                return;

            if (TerminalMirrorCam != null || ((BodyCamComponent)TerminalMirrorCam) != null)
            {
                suitsTerminal.X("OBC - MirrorCam already created and should be enabled, returning");
                return;
            }

            suitsTerminal.X("OBC - CreateTerminalMirror called");
            var terminalMirrorCam = BodyCam.CreateBodyCam(suitsTerminal.Terminal.gameObject, screenMaterial: null);

            TerminalMirrorCam = terminalMirrorCam;
            terminalMirrorCam.OnRenderTextureCreated += SetBodyCamTexture;
            Renderer[] termStuffToHide = GetStuffToHide();
            terminalMirrorCam.OnRenderersToHideChanged += originalRenderers => [.. termStuffToHide];
            terminalMirrorCam.OnCameraCreated += ResetTransform;
            terminalMirrorCam.OnBlankedSet += CamIsBlanked;

            terminalMirrorCam.Resolution = GetResolutionForOBC(SConfig.ObcResolution.Value);
            terminalMirrorCam.SetTargetToTransform(suitsTerminal.Terminal.transform);
            Camera cam = terminalMirrorCam.GetCamera();
            cam.gameObject.name = "TerminalStuff obc mirrorcam";
            SetBodyCamTexture(cam.targetTexture);

            cam.orthographic = true;
            cam.orthographicSize = 0.55f;
            cam.usePhysicalProperties = false;
            cam.farClipPlane = 30f;
            cam.nearClipPlane = 0.05f;
            cam.fieldOfView = 130f;
            CamInit(cam);
            suitsTerminal.X("OBC - TerminalStuff obc mirrorcam created!");
        }

        private static Vector2Int GetResolutionForOBC(string configItem)
        {
            Vector2Int resolution;
            List<string> resolutionStrings = CommonStringStuff.GetKeywordsPerConfigItem(configItem);
            List<int> resolutionList = StringStuff.GetNumberListFromStringList(resolutionStrings);
            if (resolutionList.Count == 2)
            {
                resolution = new Vector2Int(resolutionList[0], resolutionList[1]);
                suitsTerminal.Log.LogInfo($"OBC - Resolution set to {resolutionList[0]}x{resolutionList[1]}");
                return resolution;
            }
            else
            {
                resolution = new Vector2Int(1000, 700);
                suitsTerminal.WARNING($"OBC - Unable to set resolution to values provided in config: {configItem}\nUsing default of 1000x700");
                return resolution;
            }
        }

        internal static void ToggleCamState(Camera playerCam, bool state)
        {
            if (playerCam == null)
                return;

            playerCam.gameObject.SetActive(state);
            suitsTerminal.X($"{playerCam.name} game object set to state: {state}");

            if (state)
                SetBodyCamTexture(playerCam.targetTexture);
        }
    }

}
