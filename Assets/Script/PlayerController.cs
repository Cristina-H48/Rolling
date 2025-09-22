using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Rigidbody and Movement
    private Rigidbody rb;
    private Vector2 movementInput;

    // Game Progress
    private int count = 0;
    private float timeElapsed;
    private bool isRunning;
    private Vector3 startPosition;

    // Movement and Jump Settings
    public float speed = 6;
    public float jumpForce = 5f;
    private bool isGrounded = true;
    private float marbleRadius;

    // Tilt Controls
    public GameObject maze;
    public float sensitivity = 9.8f;
    private Vector3 rotation;

    // UI Elements
    public GameObject winTextObject; 

    // In-game UI
    public TextMeshProUGUI countText;
    public TextMeshProUGUI timerText;
    public Button jumpButton;
    public Button restartButton;
    public Button returnButton;
    public Image arrowImage;
    public Camera mainCamera;

    // In game tapping interaction
    [Header("Tap-to-Collect Settings")]
    public float raycastRange = 50f; // How far the player can collect from
    public Camera playerCamera; // Camera for raycasting
    public float collectableRange = 1.6f;

    // In game picking up feedback
    [Header("Collectible Feedback")]
    public GameObject collectEffectPrefab; // Prefab for the particle effect
    public AudioClip collectSoundDiamond; // Sound for collection
    public AudioClip collectSoundchest;

    // In game proximity effect
    [Header("Approaching Feedback")]
    public GameObject approachingEffectPrefab; // Prefab for the particle effect
    public AudioClip approachingSound; // Sound for collection
    private Dictionary<GameObject, GameObject> activeProximityEffects = new Dictionary<GameObject, GameObject>();

    // Victory Screen UI
    public GameObject victoryScreen;
    public TextMeshProUGUI completionTimeText;
    public TextMeshProUGUI collectiblesText;
    public Button nextLevelButton;
    public Button restartLevelButton;
    public Button returnToMenuButton;

    // Obstacles
    public List<DoorController> doors = new List<DoorController>();

    // collectables
    private GameObject[] diamonds;
    private GameObject[] chests;

    
    [Header("Additional Effects & Sounds")]
    public AudioClip rollingSound;             // Ball rolling
    public GameObject rollingEffectPrefab;     // Rolling effect (e.g. dust)
    public AudioClip doorOpenSound;            // Door open
    public GameObject doorOpenEffectPrefab;
    public AudioClip jumpSound;                // Jump
    public GameObject jumpEffectPrefab;
    public AudioClip victorySound;             // Victory
    public GameObject victoryEffectPrefab;
    public AudioClip wallHitSound;             // Wall hit
    public GameObject wallHitEffectPrefab;

    // --- ADDED: Volume Controls ---
    [Header("Volume Controls")]
    [Range(0f, 1f)] public float rollingVolume = 5f;
    [Range(0f, 1f)] public float doorOpenVolume = 5f;
    [Range(0f, 1f)] public float jumpVolume = 5f;
    [Range(0f, 1f)] public float victoryVolume = 5f;
    [Range(0f, 1f)] public float wallHitVolume = 5f;

    // For ball rolling sound management
    private bool isRolling = false;
    private AudioSource rollingAudioSource;

    private void Awake()
    {
        //disable all ui
        if (jumpButton != null) jumpButton.onClick.AddListener(Jump);
        if (restartButton != null) restartButton.onClick.AddListener(RestartLevel);
        if (returnButton != null) returnButton.onClick.AddListener(ReturnToStartScreen);
        if (nextLevelButton != null) nextLevelButton.onClick.AddListener(GoToNextLevel);
        if (restartLevelButton != null) restartLevelButton.onClick.AddListener(RestartLevel);
        if (returnToMenuButton != null) returnToMenuButton.onClick.AddListener(ReturnToStartScreen);

        
        if (rollingSound != null)
        {
            rollingAudioSource = gameObject.AddComponent<AudioSource>();
            rollingAudioSource.clip = rollingSound;
            rollingAudioSource.loop = true;
            rollingAudioSource.playOnAwake = false;
            rollingAudioSource.volume = rollingVolume; // Use the rolling volume
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        marbleRadius = GetComponent<SphereCollider>().radius;
        timeElapsed = 0;
        isRunning = true;
        //start position of player
        startPosition = transform.position;

        if (arrowImage != null)
        {
            arrowImage.gameObject.SetActive(false);
        }

        SetCountText();
        winTextObject.SetActive(false);
        victoryScreen.SetActive(false);
        doors.AddRange(FindObjectsOfType<DoorController>());
        diamonds = GameObject.FindGameObjectsWithTag("Diamond");
        chests = GameObject.FindGameObjectsWithTag("Treasure");
        ResetMaze();
    }

    private void Update()
    {
        if (isRunning)
        {
            timeElapsed += Time.deltaTime;
            CheckProximityFeedback();
        }
        UpdateTimerDisplay();
        // Check for tap input on mobile
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            TryCollect();
        }

        // Check for mouse click input for testing in editor
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryCollect();
        }
    }

    private void FixedUpdate()
    {
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            Vector3 tilt = new Vector3(Input.acceleration.x, 0, Input.acceleration.y);
            float maxTilt = Mathf.Sin(30 * Mathf.Deg2Rad);
            tilt = Vector3.ClampMagnitude(tilt, maxTilt);

            // Ensure movement is based on gravity direction
            Vector3 gravityDir = Physics.gravity.normalized;
            Vector3 moveDirection = Vector3.ProjectOnPlane(tilt, gravityDir);
            rb.AddForce(moveDirection * sensitivity, ForceMode.Acceleration);
        }
        else
        {
            Vector3 movement = new Vector3(Input.GetAxis("Vertical"), 0f, -Input.GetAxis("Horizontal"));
            rotation += movement;
            maze.transform.rotation = Quaternion.Euler(rotation);
        }

        if (rb.position.y < -10f) RestartLevel();//restart the level when the player drops
        CheckBallRolling();
    }

    public void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * Mathf.Sqrt(2 * Physics.gravity.magnitude * marbleRadius) * jumpForce, ForceMode.Impulse);
            isGrounded = false;

            //jump soudn effect
            if (jumpSound) AudioSource.PlayClipAtPoint(jumpSound, transform.position, jumpVolume);
            if (jumpEffectPrefab) Instantiate(jumpEffectPrefab, transform.position, Quaternion.identity);
        }
    }
    bool isCollectable(GameObject other)
    {
        // check if the palyer is within certain range
        return (transform.position - other.transform.position).magnitude <= collectableRange; ;
    }
    void CheckProximityFeedback()
    {
        // Find all collectibles in the scene
        GameObject[] collectibles = GameObject.FindGameObjectsWithTag("Diamond");
        GameObject[] treasures = GameObject.FindGameObjectsWithTag("Treasure");

        foreach (GameObject collectible in collectibles)
        {
            HandleProximity(collectible);
        }

        foreach (GameObject treasure in treasures)
        {
            HandleProximity(treasure);
        }
    }

    void HandleProximity(GameObject collectible)
    {
        float distance = Vector3.Distance(transform.position, collectible.transform.position);

        // Trigger feedback when within proximity range 
        if (distance <= collectableRange && !activeProximityEffects.ContainsKey(collectible))
        {
            // Visual effect
            GameObject effect = Instantiate(approachingEffectPrefab, collectible.transform.position, Quaternion.identity);
            activeProximityEffects.Add(collectible, effect);

            // Play sound
            AudioSource.PlayClipAtPoint(approachingSound, collectible.transform.position);
        }
        // Remove the effect when the marble moves away
        else if (distance > collectableRange && activeProximityEffects.ContainsKey(collectible))
        {
            Destroy(activeProximityEffects[collectible]);
            activeProximityEffects.Remove(collectible);
        }
    }
    void TryCollect()
    {
        // Collect the collectables by raycasting
        Ray ray = playerCamera.ScreenPointToRay(
            Touchscreen.current != null ? Touchscreen.current.primaryTouch.position.ReadValue() : Mouse.current.position.ReadValue()
        );
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * raycastRange, Color.red, 1f); // Visualize the raycast

        if (Physics.Raycast(ray, out hit, raycastRange))
        {
            //Debug.Log("Hit: " + hit.collider.gameObject.name); // Log what object is hit
            //check if player is within collectable distance
            GameObject other = hit.collider.gameObject;

            if (isCollectable(other))
            {
                // Trigger visual effect
                Instantiate(collectEffectPrefab, other.transform.position, Quaternion.identity);

                if (hit.collider.CompareTag("Diamond"))
                {
                    // Play sound effect for diamond
                    AudioSource.PlayClipAtPoint(collectSoundDiamond, other.transform.position);
                    other.SetActive(false);
                    count++;
                    SetCountText();
                }
                else if (hit.collider.CompareTag("Treasure"))
                {
                    // Play sound effect for chest
                    AudioSource.PlayClipAtPoint(collectSoundchest, other.transform.position);
                    other.SetActive(false);
                    count += 2;
                    SetCountText();
                }
            }
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = true;

        // Wall hit effect & sound
        else if (collision.gameObject.CompareTag("Wall"))
        {
            //TODO hit voluem proportional to the velocity of the ball
            if (wallHitSound) AudioSource.PlayClipAtPoint(wallHitSound, transform.position, wallHitVolume);
            if (wallHitEffectPrefab) Instantiate(wallHitEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EndPoint"))
        {
            isRunning = false;
            winTextObject.SetActive(true); // Simple "You Win" text

            // Unlock next level
            int currentLevel = SceneManager.GetActiveScene().buildIndex - 1;
            Debug.Log("level" + currentLevel + "unlocked");
            PlayerPrefs.SetInt("Level" + (currentLevel + 1) + "Unlocked", 1);
            PlayerPrefs.Save();

            ShowVictoryScreen();

            // Win sound effect
            if (victorySound) AudioSource.PlayClipAtPoint(victorySound, transform.position, victoryVolume);
            if (victoryEffectPrefab) Instantiate(victoryEffectPrefab, transform.position, Quaternion.identity);
        }
 
        else if (other.gameObject.CompareTag("DoorTrigger"))
        {
            // Add sound effect when theh door trigger "button " is pressed
            if (doorOpenSound) AudioSource.PlayClipAtPoint(doorOpenSound, transform.position, doorOpenVolume);
            //if (doorOpenEffectPrefab)
            //{
            //    GameObject effect = Instantiate(doorOpenEffectPrefab, transform.position, Quaternion.identity);
            //    Destroy(effect, 1.5f);
            //}
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeElapsed / 60);
        int seconds = Mathf.FloorToInt(timeElapsed % 60);
        timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }

    void ShowVictoryScreen()
    {
        // Disable in-game UI elements
        countText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        jumpButton.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        returnButton.gameObject.SetActive(false);
        // Enable win screen
        victoryScreen.SetActive(true);
        completionTimeText.text = $"Completion Time: {timerText.text}";
        collectiblesText.text = $"Collectibles: {count}";

        // Hide "Next Level" button if this is the last level
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        if (currentLevel >= 4) // Assuming Level 3 has build index 4
        {
            nextLevelButton.gameObject.SetActive(false);
        }
    }

    void Respawn()
    {
        // Respawn the user to the initial point
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startPosition;
    }

    void RestartLevel()
    {
        Respawn();
        count = 0;
        timeElapsed = 0;
        SetCountText();
        winTextObject.SetActive(false);
        victoryScreen.SetActive(false);
        // Reactivate in-game UI elements
        countText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);
        jumpButton.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        returnButton.gameObject.SetActive(true);
        ResetMaze();
        isRunning = true;
    }

    public void ResetMaze()
    {
        rotation = Vector3.zero;
        maze.transform.rotation = Quaternion.Euler(rotation);
        rb.transform.position = startPosition;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        // reset doors

        foreach (DoorController door in doors)
        {
            door.ResetDoor();
        }
        ResetCollectables();
    }
    public void ResetCollectables()
    {
        foreach (GameObject d in diamonds)
        {
            d.SetActive(true);
        }
        foreach (GameObject c in chests)
        {
            c.SetActive(true);
        }
    }
    public void ReturnToStartScreen()
    {
        SceneManager.LoadScene("MazeSelection");
    }

    public void GoToNextLevel()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentLevel + 1);//+1 since there are two start screen
    }

    // check if ball is rolling and play audio clip
    private void CheckBallRolling()
    {
        float speed = rb.velocity.magnitude;

        if (rollingAudioSource != null)
        {
            if (speed > 0.2f && !isRolling)
            {
                rollingAudioSource.volume = rollingVolume; 
                rollingAudioSource.Play();
                isRolling = true;
            }
            else if (speed <= 0.2f && isRolling)
            {
                rollingAudioSource.Stop();
                isRolling = false;
            }
        }
    }

    //// --- ADDED: Volume Controls ---
    //[Header("Volume Controls")]
    //[Range(0f, 1f)] public float rollingVolume = 1f;
    //[Range(0f, 1f)] public float doorOpenVolume = 1f;
    //[Range(0f, 1f)] public float jumpVolume = 1f;
    //[Range(0f, 1f)] public float victoryVolume = 1f;
    //[Range(0f, 1f)] public float wallHitVolume = 1f;
}
