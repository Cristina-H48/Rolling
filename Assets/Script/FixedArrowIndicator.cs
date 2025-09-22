//using UnityEngine;
//using UnityEngine.UI;

//public class FixedArrowIndicator : MonoBehaviour
//{
//    [Header("References")]
//    public Transform player;      // The player's transform
//    public Transform endPoint;    // The EndPoint's transform
//    public Camera mainCamera;     // The main camera
//    public Image arrowImage;      // The UI arrow image (this Image)

//    void Update()
//    {
//        if (!endPoint || !player || !mainCamera || !arrowImage)
//            return;

//        // 1) Check if EndPoint is on-screen
//        Vector3 screenPos = mainCamera.WorldToScreenPoint(endPoint.position);
//        bool isOnScreen = (screenPos.z > 0 &&
//                           screenPos.x >= 0 && screenPos.x <= Screen.width &&
//                           screenPos.y >= 0 && screenPos.y <= Screen.height);

//        // Show arrow only if off-screen
//        arrowImage.enabled = !isOnScreen;

//        if (!isOnScreen)
//        {
//            // 2) Calculate direction on the XZ plane (ignore camera tilt)
//            Vector3 direction = (endPoint.position - player.position);
//            direction.y = 0; // Flatten so we only rotate around Y

//            // 3) Calculate how the camera is oriented on the XZ plane
//            Vector3 cameraForward = mainCamera.transform.forward;
//            cameraForward.y = 0; // Flatten

//            // 4) Compute signed angle between camera forward and direction
//            float angle = Vector3.SignedAngle(cameraForward.normalized, direction.normalized, Vector3.up);

//            // 5) Rotate arrow around Z to reflect angle
//            // If arrow points up by default, we use angle directly or offset as needed
//            arrowImage.rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
//        }
//    }
//}
