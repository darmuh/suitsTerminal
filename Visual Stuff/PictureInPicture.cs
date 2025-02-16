using OpenLib.Common;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Video;
using static UnityEngine.Object;

namespace suitsTerminal
{
    internal class PictureInPicture
    {
        internal static bool PiPCreated = false;
        internal static GameObject pipGameObject = null!;
        internal static int cullingMaskInt;
        internal static Camera playerCam = null!;
        internal static RenderTexture camTexture = null!;
        internal static RawImage pipRawImage = null!;
        internal static bool pipActive = false;

        //camera modifiers
        internal static int heightStep;
        internal static int zoomStep;
        internal static int rotateStep;

        //player model modifiers
        internal static ShadowCastingMode shadowDefault;
        internal static int modelLayerDefault;

        internal static void InitPiP()
        {
            if (PiPCreated || !SConfig.EnablePiPCamera.Value)
                return;

            if (Plugin.Terminal.terminalImage == null)
            {
                Plugin.WARNING("Original terminalImage not found");
                return;
            }

            if (pipGameObject != null && pipGameObject.gameObject != null)
                Object.Destroy(pipGameObject.gameObject);

            pipGameObject = Instantiate(Plugin.Terminal.terminalImage.gameObject, Plugin.Terminal.terminalImage.transform.parent);
            pipGameObject.name = "suitsTerminal PiP (Clone)";

            if (pipGameObject.GetComponent<VideoPlayer>() != null)
            {
                VideoPlayer extraPlayer = pipGameObject.GetComponent<VideoPlayer>();
                Destroy(extraPlayer);
                Plugin.X("extra videoPlayer deleted");
            }

            pipRawImage = pipGameObject.GetComponent<RawImage>();

            // Set dimensions and position for radar image (rawImage1)
            SetRawImageDimensionsAndPosition(pipRawImage.rectTransform, 0.45f, 0.33f, 95f, -20f);
            pipRawImage.color = new Color(1, 1, 1, 0.9f); //90% opacity
            pipGameObject.SetActive(false);
            PiPCreated = true;
        }

        private static void SetRawImageDimensionsAndPosition(RectTransform rectTransform, float heightPercentage, float widthPercentage, float anchoredPosX, float anchoredPosY)
        {
            RectTransform canvasRect = Plugin.Terminal.terminalUIScreen.GetComponent<RectTransform>();
            float height = canvasRect.rect.height * heightPercentage;
            float width = canvasRect.rect.width * widthPercentage;
            rectTransform.sizeDelta = new Vector2(width, height);
            rectTransform.anchoredPosition = new Vector2(anchoredPosX, anchoredPosY);
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

        internal static void TogglePiP(bool state)
        {
            if (!SConfig.EnablePiPCamera.Value)
                return;

            if (pipActive == state)
                return;

            Plugin.X($"TogglePiP: {state}");
            pipActive = state;
            pipRawImage.enabled = state;
            pipGameObject.SetActive(state);

            if (OpenLib.Plugin.instance.OpenBodyCamsMod && SConfig.UseOpenBodyCams.Value)
            {
                Plugin.X($"OpenBodyCams detected, using OBC for Mirror (state:{state})");
                OpenLib.Compat.OpenBodyCamFuncs.OpenBodyCamsMirrorStatus(state, SConfig.ObcResolution.Value, 0.1f, false, ref CamStuff.ObcCameraHolder);

                if (state == false)
                    return;

                Camera Cam = OpenLib.Compat.OpenBodyCamFuncs.GetCam(OpenLib.Compat.OpenBodyCamFuncs.TerminalMirrorCam);
                Cam.fieldOfView = 100f;
                Plugin.X($"isActive [ Cam ] - {Cam.isActiveAndEnabled}");
                pipRawImage.texture = OpenLib.Compat.OpenBodyCamFuncs.GetTexture(OpenLib.Compat.OpenBodyCamFuncs.TerminalMirrorCam);
            }
            else
            {
                if (playerCam == null)
                    playerCam = CamStuff.HomebrewCam(ref camTexture, ref CamStuff.MyCameraHolder);

                CamStuff.CamInitMirror(CamStuff.MyCameraHolder, playerCam, 0.1f, false);
                CamStuff.HomebrewCameraState(state, playerCam);

                if (state != false)
                {
                    playerCam.fieldOfView = 100f;

                    pipRawImage.texture = playerCam.targetTexture;
                    //Plugin.Log.LogInfo($"MirrorTexture state: {state}\nplayerCam: {playerCam.gameObject.activeSelf}\nCameraHolder {CamStuff.CameraHolder.activeSelf}");
                }

                if (!OpenLib.Plugin.instance.ModelReplacement)
                {
                    if (state)
                    {
                        StartOfRound.Instance.localPlayerController.thisPlayerModel.shadowCastingMode = ShadowCastingMode.Off;
                        Plugin.Log.LogInfo("Showing PlayerModel to player (player should be in terminal)");
                    }
                    else
                    {
                        StartOfRound.Instance.localPlayerController.thisPlayerModel.shadowCastingMode = shadowDefault;
                        Plugin.Log.LogInfo("Hiding PlayerModel from player (PiP disabled or leaving suits menu)");
                    }
                }

            }


            Plugin.X($"isActive [ pipGameObject ] - {pipGameObject.activeSelf}\nisActive [ pipRawImage ] {pipRawImage.isActiveAndEnabled}\nisActive [ pipActive ] - {pipActive}");
            //Plugin.Log.LogInfo($"pipGameObject: {pipGameObject.activeSelf}\npipRawImage: {pipRawImage.isActiveAndEnabled}\npipActive: {pipActive}");
        }
    }
}
