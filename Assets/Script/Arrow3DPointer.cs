using UnityEngine;

public class Arrow3DPointer : MonoBehaviour
{
    public Transform ball; 
    public Transform endPoint; 
    public Camera mainCamera;

    void Update()
    {
        if (ball == null || endPoint == null || mainCamera == null)
        {
            Debug.LogWarning("References are not set properly in Arrow3DPointer!");
            return;
        }

        // Convert the EndPoint world position to screen coordinates
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(endPoint.position);

        // Check if the endpoint is within the camera's visible viewport
        bool isOffScreen = screenPoint.z < 0 || screenPoint.x < 0 || screenPoint.x > Screen.width || screenPoint.y < 0 || screenPoint.y > Screen.height;
        Vector3 projectedEndPoint = new Vector3(endPoint.position.x, ball.position.y, endPoint.position.z);

        Vector3 directionToEndPoint = (projectedEndPoint - ball.position).normalized;//unit vector pointing from ball to endpoint

            // Position the pointer at a fixed offset from the ball along the direction
        transform.position = ball.position + directionToEndPoint;

            // Rotate the arrow to point towards the endpoint
        transform.rotation = Quaternion.LookRotation(Vector3.up, directionToEndPoint);
       
    }
}