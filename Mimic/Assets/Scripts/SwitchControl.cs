using UnityEngine;
using UnityEngine.AI;

public class SwitchControl : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    public GameObject enemy; // Reference to the enemy GameObject
    public string[] enemyScriptsToEnable; // Script names on the enemy to enable
    public string[] enemyScriptsToDisable; // Script names on the enemy to disable
    public float maxRaycastDistance = 3f; // Maximum distance for the raycast

    private bool playerControl = true;
    private NavMeshAgent enemyNavMeshAgent;
    private CharacterController enemyCharacterController;
    private Vector3 initialPlayerPosition;
    private Vector3 initialEnemyPosition;
    private Quaternion initialPlayerRotation;
    private Quaternion initialEnemyRotation;

    private void Start()
    {
        // Ensure the enemy's Nav Mesh Agent is enabled, and Character Controller is disabled at the start
        enemyNavMeshAgent = enemy.GetComponent<NavMeshAgent>();
        enemyCharacterController = enemy.GetComponent<CharacterController>();
        enemyNavMeshAgent.enabled = true;
        enemyCharacterController.enabled = false;

        // Store the initial positions and rotations
        initialPlayerPosition = player.transform.position;
        initialEnemyPosition = enemy.transform.position;
        initialPlayerRotation = player.transform.rotation;
        initialEnemyRotation = enemy.transform.rotation;
    }

    private void Update()
    {
        if (playerControl)
        {
            if ((Input.GetButtonDown("PS4_Triangle") || Input.GetKeyDown(KeyCode.E)) && CanSwitch())
            {
                // Toggle control between player and enemy
                playerControl = !playerControl;

                // Swap the visibility of player and enemy
                player.SetActive(playerControl);
                enemy.SetActive(!playerControl);

                // Enable or disable selected scripts only after the switch
                ToggleEnemyScripts();

                // Enable or disable the Nav Mesh Agent and Character Controller after the switch
                if (playerControl)
                {
                    enemyNavMeshAgent.enabled = true;
                    enemyCharacterController.enabled = false;
                }
                else
                {
                    enemyNavMeshAgent.enabled = false;
                    enemyCharacterController.enabled = true;
                }

                // Set the player (invisible) to be on top of the enemy
                player.transform.position = enemy.transform.position;
                player.transform.rotation = enemy.transform.rotation;
            }
        }
        else
        {
            // In this case, the enemy can switch without needing to look at anyone.
            if (Input.GetButtonDown("PS4_Triangle") || Input.GetKeyDown(KeyCode.E))
            {
                // Toggle control between player and enemy
                playerControl = !playerControl;

                // Swap the visibility of player and enemy
                player.SetActive(playerControl);
                enemy.SetActive(!playerControl);

                // Enable or disable selected scripts only after the switch
                ToggleEnemyScripts();

                // Enable or disable the Nav Mesh Agent and Character Controller after the switch
                if (playerControl)
                {
                    enemyNavMeshAgent.enabled = true;
                    enemyCharacterController.enabled = false;
                }
                else
                {
                    enemyNavMeshAgent.enabled = false;
                    enemyCharacterController.enabled = true;
                }

                // Set the player (invisible) to be on top of the enemy
                player.transform.position = enemy.transform.position;
                player.transform.rotation = enemy.transform.rotation;
            }
        }
    }

    private void ToggleEnemyScripts()
    {
        foreach (string scriptName in enemyScriptsToDisable)
        {
            MonoBehaviour script = enemy.GetComponent(scriptName) as MonoBehaviour;
            if (script != null)
            {
                script.enabled = false;
            }
        }

        foreach (string scriptName in enemyScriptsToEnable)
        {
            MonoBehaviour script = enemy.GetComponent(scriptName) as MonoBehaviour;
            if (script != null)
            {
                script.enabled = true;
            }
        }
    }

    private bool CanSwitch()
    {
        // Create a ray from the player's position forward
        Ray ray = new Ray(player.transform.position, player.transform.forward);
        RaycastHit hit;

        // Check if the ray hits the enemy
        if (Physics.Raycast(ray, out hit, maxRaycastDistance))
        {
            if (hit.collider.gameObject == enemy)
            {
                return true;
            }
        }

        return false;
    }
}
