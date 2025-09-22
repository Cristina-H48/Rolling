using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    
    public GameObject player;
    private Vector3 offset;
    private bool followPlayer; 

    void Start()
    {
        offset = transform.position - player.transform.position;

        // Determine whether the camera should follow the player
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        followPlayer = (currentLevel == 4); // Level 3 has build index 4
    }

    void LateUpdate()
    {
        if (followPlayer)
        {
            // Follow player in Level 3
            transform.position = player.transform.position + offset;
        }
        // Else, the camera remains at its initial position (do nothing)
    }
}
