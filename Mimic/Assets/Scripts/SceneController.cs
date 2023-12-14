using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }
    public static Vector3 playerPosition { get; set; }

    public float XOffset = -20f; // Desired x-coordinate for spawning

    private void Awake()
    {
        // Ensure only one instance of SceneController exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if returning to the original scene
        if (scene.name == "Mimic")
        {
            // Set the player's position to the stored position with offset
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                if (playerPosition != Vector3.zero)
                {
                    // Calculate the offset for the x-coordinate
                    float xOffsetDelta = XOffset - playerPosition.x;

                    // Apply the offset to the player's position
                    player.transform.position = new Vector3(playerPosition.x + xOffsetDelta, playerPosition.y, playerPosition.z);
                }
                else
                {
                    // Set the initial player position (e.g., at the start of the game)
                    player.transform.position = new Vector3(0f, 0f, -8.5f);
                }
            }
        }
    }
}
