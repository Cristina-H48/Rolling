using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    public GameObject orbitingCube; // Reference to the cube attached to the disc
    public float rotationSpeed = 10f; // Speed of disc rotation
    public float orbitDistance = 3.5f; // Distance of the cube from the disc's center

    void Update()
    {
        // Rotate the disc around its Y-axis
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

    }
}
