using OpenLib.Common;
using UnityEngine;
using UnityEngine.Rendering;
using static suitsTerminal.ModConfig;

namespace suitsTerminal;

internal class PictureInPicture
{
    internal static bool PiPCreated = false;
    internal static Camera PlayerCam = null!;
    internal static RenderTexture CamTexture = null!;
    internal static bool PipActive
    {
        get
        {
            if (Plugin.Terminal.terminalImage.texture == null)
                return false;

            return Plugin.Terminal.terminalImage.enabled;
        }
    }

    //camera modifiers
    internal static int HeightStep;
    internal static int ZoomStep;
    internal static int RotateStep;

    //player model modifiers
    internal static ShadowCastingMode ShadowDefault;
    internal static int ModelLayerDefault;


    private static void SetTerminalImageOpacity(float opacity)
    {
        Plugin.Terminal.terminalImage.color = new(1f, 1f, 1f, opacity);
    }

    private static void SetTerminalImage(bool toDefault, bool centered)
    {
        if (!toDefault)
        {
            Plugin.Terminal.terminalImage.enabled = true; //force this enabled to avoid base-game messing with displayText
            if (centered)
                SetRectDimesionsAndPos(Plugin.Terminal.terminalImage.rectTransform, new(225f, 190f), new(-10f, -4f));
            else
                SetRectDimesionsAndPos(Plugin.Terminal.terminalImage.rectTransform, new(225f, 190f), new(75f, -2f));

            SetTerminalImageOpacity(0.9f); //90%
        }
        else
        {
            Plugin.Terminal.terminalImage.enabled = false; //this will get updated by base-game but we want to disable it early to avoid jarring graphic size change
            SetRectDimesionsAndPos(Plugin.Terminal.terminalImage.rectTransform, new(400f, 300f), new(0f, 0.0001f)); //base-game defaults
            SetTerminalImageOpacity(1f); //100% opacity
        }
    }

    private static void SetRectDimesionsAndPos(RectTransform rectTransform, Vector2 sizeDelta, Vector2 anchorPos)
    {
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.anchoredPosition = anchorPos;
    }

    internal static void RotateCameraAroundPlayer(Transform playerTransform, Transform cameraTransform)
    {
        // Define the rotation increment
        Quaternion rotationIncrement = Quaternion.LookRotation(-playerTransform.right, playerTransform.up);

        // Calculate the new rotation by applying the rotation increment to the camera's current rotation
        Quaternion newRotation = rotationIncrement * cameraTransform.rotation;

        // Update the camera's rotation
        cameraTransform.rotation = newRotation;

        // Calculate the new position by offsetting from the player's position
        Vector3 offset = cameraTransform.position - playerTransform.position;
        Vector3 newPosition = playerTransform.position + rotationIncrement * offset;

        // Update the camera's position
        cameraTransform.position = newPosition;
    }


    internal static void ChangeCamZoom(Camera camera, ref int currentStep)
    {
        //float[] zoomVals = [1.2f, 0.28f, 0.35f, 0.45f];
        float[] zoomVals = [120f, 60f, 80f, 100f];

        // Increment the current step
        currentStep++;

        // If the current step exceeds the number of steps, circle back to the first step
        if (currentStep >= zoomVals.Length)
        {
            currentStep = 0;
        }

        camera.fieldOfView = zoomVals[currentStep];
        //camera.orthographicSize = zoomVals[currentStep];
    }

    internal static void MoveCamera(Transform cameraTransform, ref int currentStep)
    {
        // Define the heights for each step
        float[] stepHeights = [0f, 0.2f, -2.2f, -1.2f, -0.7f];

        // Increment the current step
        currentStep++;

        // If the current step exceeds the number of steps, circle back to the first step
        if (currentStep >= stepHeights.Length)
        {
            currentStep = 0;
        }

        // Get the target height for the current step
        float targetHeight = Plugin.Terminal.terminalImage.transform.position.y + stepHeights[currentStep];

        Vector3 newPosition = cameraTransform.position;

        // Update the y-coordinate of the new position to the target height
        newPosition.y = targetHeight;

        // Update the camera's position
        cameraTransform.position = newPosition;
    }

    internal static void TogglePicture(bool state, bool centered = false)
    {
        if (CamStyle.Value == PiP.Disabled)
            return;

        if (PipActive == state)
            return;

        if (state == false)
        {
            Menu.SuitsMenu.MenuNode.displayTexture = null!;
        }

        SetTerminalImage(!state, centered);

        Loggers.LogDebug($"TogglePiP: {state}");

        if (OpenLib.Plugin.instance.OpenBodyCamsMod && CamStyle.Value == PiP.OpenBodyCams)
        {
            Loggers.LogDebug($"OpenBodyCams detected, using OBC for Mirror (state:{state})");
            OpenLib.Compat.OpenBodyCamFuncs.OpenBodyCamsMirrorStatus(state, ObcResolution.Value, 0.1f, false, ref CamStuff.ObcCameraHolder);

            if (state == false)
                return;

            Camera Cam = OpenLib.Compat.OpenBodyCamFuncs.GetCam(OpenLib.Compat.OpenBodyCamFuncs.TerminalMirrorCam);
            Cam.fieldOfView = 100f;
            Loggers.LogDebug($"isActive [ Cam ] - {Cam.isActiveAndEnabled}");
            Menu.SuitsMenu.MenuNode.displayTexture = OpenLib.Compat.OpenBodyCamFuncs.GetTexture(OpenLib.Compat.OpenBodyCamFuncs.TerminalMirrorCam);
        }
        else
        {
            if (PlayerCam == null)
            {
                PlayerCam = CamStuff.HomebrewCam(ref CamTexture, ref CamStuff.MyCameraHolder);
                CamTexture.name = "suitsTerminal_CamText";
            }


            CamStuff.CamInitMirror(CamStuff.MyCameraHolder, PlayerCam, 0.1f, false);
            CamStuff.HomebrewCameraState(state, PlayerCam);

            if (state != false)
            {
                PlayerCam.fieldOfView = 100f;

                Menu.SuitsMenu.MenuNode.displayTexture = PlayerCam.targetTexture;
                //Plugin.Log.LogInfo($"MirrorTexture state: {state}\nplayerCam: {playerCam.gameObject.activeSelf}\nCameraHolder {CamStuff.CameraHolder.activeSelf}");
            }

            if (!OpenLib.Plugin.instance.ModelReplacement)
            {
                if (state)
                {
                    Plugin.LocalPlayer.thisPlayerModel.shadowCastingMode = ShadowCastingMode.Off;
                    Plugin.Log.LogInfo("Showing PlayerModel to player (player should be in terminal)");
                }
                else
                {
                    Plugin.LocalPlayer.thisPlayerModel.shadowCastingMode = ShadowDefault;
                    Plugin.Log.LogInfo("Hiding PlayerModel from player (PiP disabled or leaving suits menu)");
                }
            }

        }


        Loggers.LogDebug($"isActive [ PipActive ] - {PipActive}");
        //Plugin.Log.LogInfo($"pipGameObject: {pipGameObject.activeSelf}\npipRawImage: {pipRawImage.isActiveAndEnabled}\npipActive: {pipActive}");
    }
}
