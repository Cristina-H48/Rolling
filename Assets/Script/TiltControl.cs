//// TiltControl.cs
//using UnityEngine;

//public class TiltControl : MonoBehaviour
//{
//    public GameObject maze;     // Maze component

//    private Rigidbody playerRb; // Player rigid body
//    public float sensitivity = 9.8f;

//    private Vector3 rotation;         // current Euler angle of the maze
//    // Start is called before the first frame update
//    private void Start()
//    {
//        playerRb = GetComponent<Rigidbody>();
//        Reset();
//    }

//    // FixedUpdate is called at a fixed interval. This is useful for physics
//    // simulation and also for the Rigidbody update.
//    private void FixedUpdate()
//    {
//        if (SystemInfo.deviceType == DeviceType.Handheld)
//        {
//            // For mobile devices, we add the force to the player based on
//            // the acceleration from the accelerometer
//            if (playerRb.position.y <= 5)
//                Reset();
//            playerRb.AddForce(
//                new Vector3(Input.acceleration.x, 0, Input.acceleration.y)
//                    * sensitivity);
//        }
//        else
//        {
//            Vector3 movement = new Vector3(
//                Input.GetAxis("Vertical"), 0f, -Input.GetAxis("Horizontal"));
//            rotation += movement;
//            maze.transform.rotation = Quaternion.Euler(rotation);
//        }
//    }

//    // Resets the state. This is called manually.
//    public void Reset()
//    {
//        rotation = Vector3.zero;
//        maze.transform.rotation = Quaternion.Euler(rotation);
//        playerRb.transform.position = new Vector3(-0.5f, 0.5f, 2.5f);
//        playerRb.velocity = Vector3.zero;
//        playerRb.angularVelocity = Vector3.zero;
//    }
//}