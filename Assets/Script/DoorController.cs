using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Vector3 initialPosition;
    public Vector3 openPositionOffset = new Vector3(0, 5, 0);
    public float doorOpenSpeed = 2f;
    public AudioClip doorOpenSound;

    private bool isOpen = false;

    private void Awake()
    {
        // Awake runs before Start in other scripts so that the door is reset
        initialPosition = transform.position;
        //Debug.Log("Door Initial Position in Awake: " + initialPosition);
    }

    private void Update()
    {
        if (isOpen)
        {
            gameObject.SetActive(false);
            transform.position = Vector3.Lerp(transform.position, initialPosition + openPositionOffset, Time.deltaTime * doorOpenSpeed);
        }
    }

    public void OpenDoor()
    {
        //originally the door moves up, remove because of bug
        isOpen = true;
        
    }

    public void ResetDoor()
    {
        isOpen = false;
        gameObject.SetActive(true);
        transform.position = initialPosition;
    }
}
