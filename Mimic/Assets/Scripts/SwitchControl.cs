using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class SwitchControl : MonoBehaviour
{
    public GameObject player; // Reference to the player GameObject
    public float maxRaycastDistance = 3f; // Maximum distance for the raycast
    public string[] enemyScriptsToEnable; // Script names on the enemy to enable
    public string[] enemyScriptsToDisable; // Script names on the enemy to disable
    public InputAction transformButton;


    private GameObject currentEnemy; // The current enemy GameObject
    private bool playerControl = true;
    private NavMeshAgent enemyNavMeshAgent;
    private CharacterController enemyCharacterController;
    private Vector3 initialPlayerPosition;
    private Quaternion initialPlayerRotation;

    private void Start()
    {
        // Store the initial player position and rotation
        initialPlayerPosition = player.transform.position;
        initialPlayerRotation = player.transform.rotation;
        currentEnemy = null; // Initialize currentEnemy as null
        transformButton.Enable();
    }

    private void Update()
    {
        if (playerControl)
        {
            if (transformButton.WasPerformedThisFrame() || Input.GetKeyDown(KeyCode.E))
            {
                // Attempt to switch to a new enemy if the player is looking at one
                if (CanSwitch(out GameObject newEnemy))
                {
                    // Toggle control between player and enemy
                    playerControl = !playerControl;

                    // Swap the visibility of player and enemy
                    player.SetActive(playerControl);
                    newEnemy.SetActive(!playerControl);

                    // Change the enemy's tag to "Player"
                    newEnemy.tag = "Player";

                    // Enable or disable selected scripts only after the switch
                    ToggleEnemyScripts(newEnemy);

                    // Enable or disable the Nav Mesh Agent and Character Controller after the switch
                    enemyNavMeshAgent = newEnemy.GetComponent<NavMeshAgent>();
                    enemyCharacterController = newEnemy.GetComponent<CharacterController>();
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

                    // Set the player (invisible) to be on top of the new enemy
                    player.transform.position = newEnemy.transform.position;
                    player.transform.rotation = newEnemy.transform.rotation;

                    // Assign the new enemy as the current enemy
                    currentEnemy = newEnemy;

                    // Use raycast to find the AnimationControllerSwap script on the enemy
                    AnimationControllerSwap animationControllerSwap = newEnemy.GetComponent<AnimationControllerSwap>();
                    if (animationControllerSwap != null)
                    {
                        // Use the AnimationControllerSwap script to swap the Animator controller
                        animationControllerSwap.SwapAnimatorController();
                    }

                    // Debug log to confirm the new enemy assignment
                    Debug.Log("New enemy assigned: " + newEnemy.name);
                }
            }
        }
        //Switch back to player
        else
        {
            if (transformButton.WasPerformedThisFrame() || Input.GetKeyDown(KeyCode.E))
            {
                // Toggle control between player and enemy
                playerControl = !playerControl;

                // Swap the visibility of player and enemy
                player.SetActive(playerControl);
                if (currentEnemy != null)
                {
                    currentEnemy.SetActive(!playerControl);
                }

                // Enable or disable selected scripts only after the switch
                if (currentEnemy != null)
                {
                    ToggleEnemyScripts(currentEnemy);
                }

                // Enable or disable the Nav Mesh Agent and Character Controller after the switch
                if (currentEnemy != null)
                {
                    enemyNavMeshAgent = currentEnemy.GetComponent<NavMeshAgent>();
                    enemyCharacterController = currentEnemy.GetComponent<CharacterController>();
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
                }

                // Set the player (invisible) to be on top of the enemy
                if (currentEnemy != null)
                {
                    player.transform.position = currentEnemy.transform.position;
                    player.transform.rotation = currentEnemy.transform.rotation;
                }
            }
        }
    }

    private void ToggleEnemyScripts(GameObject enemy)
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

    private bool CanSwitch(out GameObject newEnemy)
    {
        newEnemy = null;

        // Create a ray from the player's position forward
        Ray ray = new Ray(player.transform.position, player.transform.forward);
        RaycastHit hit;

        // Check if the ray hits an object with the "Enemy" tag
        if (Physics.Raycast(ray, out hit, maxRaycastDistance) && hit.collider.CompareTag("Enemy"))
        {
            newEnemy = hit.collider.gameObject;
            return true;
        }

        return false;
    }
}
