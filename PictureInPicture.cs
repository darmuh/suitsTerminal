using TerminalApi;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Object;

namespace suitsTerminal
{
    internal class PictureInPicture
    {
        internal static bool PiPCreated = false;
        internal static GameObject pipGameObject = null;
        internal static Camera playerCam = null;
        internal static RawImage pipRawImage = null;
        internal static Canvas terminalCanvas = null;
        internal static bool pipActive = false;

        internal static void InitPiP()
        {
            if (PiPCreated || !SConfig.enablePiPCamera.Value)
                return;

            terminalCanvas = GameObject.Find("Environment/HangarShip/Terminal/Canvas")?.GetComponent<Canvas>();

            if(terminalCanvas == null)
            {
                suitsTerminal.Log.LogError("Unable to find terminalCanvas, null.");
                return;
            }

            RawImage originalRawImage = GameObject.Find("Environment/HangarShip/Terminal/Canvas/MainContainer/ImageContainer/Image (1)")?.GetComponent<RawImage>();
            if (originalRawImage == null)
            {
                suitsTerminal.Log.LogError("Unable to find originalRawImage, null.");
                return;
            }

            pipGameObject = Instantiate(originalRawImage.gameObject, originalRawImage.transform.parent);
            pipGameObject.name = "suitsTerminal PiP (Clone)";

            pipRawImage = pipGameObject.GetComponent<RawImage>();

            // Set dimensions and position for radar image (rawImage1)
            SetRawImageDimensionsAndPosition(pipRawImage.rectTransform, 0.30f, 0.30f, 120f, -40f);
            pipGameObject.SetActive(false);
            PiPCreated = true;
        }

        private static void SetRawImageDimensionsAndPosition(RectTransform rectTransform, float heightPercentage, float widthPercentage, float anchoredPosX, float anchoredPosY)
        {
            RectTransform canvasRect = terminalCanvas.GetComponent<RectTransform>();
            float height = canvasRect.rect.height * heightPercentage;
            float width = canvasRect.rect.width * widthPercentage;
            rectTransform.sizeDelta = new Vector2(width, height);
            rectTransform.anchoredPosition = new Vector2(anchoredPosX, anchoredPosY);
        }

        internal static void PlayerCamSetup()
        {
            playerCam = Instantiate(StartOfRound.Instance.spectateCamera);
            playerCam.gameObject.SetActive(true);
            Transform termObject = GameObject.Find("Environment/HangarShip/Terminal").GetComponent<Transform>();
            if (termObject != null && termObject.gameObject.layer != 0)
            {
                termObject.gameObject.layer = 0;
                suitsTerminal.X("terminal layer changed");
            }
        }

        internal static void TogglePiP(bool state)
        {
            if (!SConfig.enablePiPCamera.Value)
                return;

            pipActive = state;
            pipRawImage.texture = MirrorTexture(state);
            pipRawImage.enabled = state;
            pipGameObject.SetActive(state);

        }

        private static Texture MirrorTexture(bool state)
        {
            if (playerCam == null)
            {
                PlayerCamSetup();
            }

            playerCam.enabled = state;
            playerCam.cameraType = CameraType.SceneView;

            Transform termTransform = suitsTerminal.Terminal.transform;
            Transform playerTransform = StartOfRound.Instance.localPlayerController.transform;
            suitsTerminal.X("camTransform assigned to terminal");

            // Calculate the opposite direction directly in local space
            Vector3 oppositeDirection = -playerTransform.forward;

            // Calculate the new rotation to look behind
            Quaternion newRotation = Quaternion.LookRotation(oppositeDirection, playerTransform.up);

            // Define the distance to back up the camera
            float distanceBehind = 1f;

            // Set camera's rotation and position
            playerCam.transform.rotation = newRotation;
            playerCam.transform.position = playerTransform.position - oppositeDirection * distanceBehind + playerTransform.up * 2f;

            playerCam.cullingMask = 9962313; //ship security camera culling mask, best for seeing yourself it seems
            playerCam.orthographic = true;
            playerCam.orthographicSize = 0.55f;
            playerCam.usePhysicalProperties = false;
            playerCam.farClipPlane = 30f;
            playerCam.nearClipPlane = 0.25f;
            playerCam.fieldOfView = 130f;
            playerCam.transform.SetParent(termTransform);

            Texture spectateTexture = playerCam.targetTexture;
            return spectateTexture;
        }
    }
}
