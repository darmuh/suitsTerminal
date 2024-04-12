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
        internal static float initCamHeight;
        internal static int heightStep;
        internal static int zoomStep;
        internal static int rotateStep;

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
            SetRawImageDimensionsAndPosition(pipRawImage.rectTransform, 0.45f, 0.33f, 90f, -12f);
            pipRawImage.color = new Color(1, 1, 1, 0.9f); //90% opacity
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

        internal static void RotateCameraAroundPlayer(Transform playerTransform, Camera camera)
        {
            // Define the rotation increment
            Quaternion rotationIncrement = Quaternion.LookRotation(-playerTransform.right, playerTransform.up);

            // Calculate the new rotation by applying the rotation increment to the camera's current rotation
            Quaternion newRotation = rotationIncrement * camera.transform.rotation;

            // Update the camera's rotation
            camera.transform.rotation = newRotation;

            // Calculate the new position by offsetting from the player's position
            Vector3 offset = camera.transform.position - playerTransform.position;
            Vector3 newPosition = playerTransform.position + rotationIncrement * offset;

            // Update the camera's position
            camera.transform.position = newPosition;
        }


        internal static void ChangeCamZoom(Camera camera, ref int currentStep)
        {
            float[] zoomVals = { 1.2f, 0.45f, 0.35f, 0.28f};

            // Increment the current step
            currentStep++;

            // If the current step exceeds the number of steps, circle back to the first step
            if (currentStep >= zoomVals.Length)
            {
                currentStep = 0;
            }

            camera.orthographicSize = zoomVals[currentStep];
        }

        internal static void MoveCamera(Camera camera, ref int currentStep)
        {
            // Define the heights for each step
            float[] stepHeights = { 0f, 1.3f, -9.499f, -7.749f, -5.499f, -2.49f };

            // Increment the current step
            currentStep++;

            // If the current step exceeds the number of steps, circle back to the first step
            if (currentStep >= stepHeights.Length)
            {
                currentStep = 0;
            }

            // Get the target height for the current step
            float targetHeight = initCamHeight + stepHeights[currentStep];

            Vector3 newPosition = camera.transform.localPosition;

            // Update the y-coordinate of the new position to the target height
            newPosition.y = targetHeight;

            // Update the camera's position
            camera.transform.localPosition = newPosition;
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
            initCamHeight = playerCam.transform.position.y;
            suitsTerminal.X($"initCamHeight: {initCamHeight}");

            playerCam.cullingMask = SConfig.setPiPCullingMask.Value; //9962313 old, ship security cam mask
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
