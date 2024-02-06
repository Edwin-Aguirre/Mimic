using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class SwitchControl : MonoBehaviour
{
    public GameObject player;
    public float maxProximityDistance = 3f; // Maximum distance for the proximity check
    public string[] enemyScriptsToEnable;
    public string[] enemyScriptsToDisable;
    public InputAction transformButton;

    private GameObject currentEnemy;
    private bool playerControl = true;
    private NavMeshAgent enemyNavMeshAgent;
    private CharacterController enemyCharacterController;
    private Vector3 initialPlayerPosition;
    private Quaternion initialPlayerRotation;

    private EnemySpawnSystem enemySpawnSystem;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemySpawnSystem = FindAnyObjectByType<EnemySpawnSystem>();

        if (player == null)
        {
            Debug.LogError("Player GameObject not found. Make sure to set the correct tag or find it using another method.");
            return;
        }

        initialPlayerPosition = player.transform.position;
        initialPlayerRotation = player.transform.rotation;
        currentEnemy = null;
        transformButton.Enable();
    }

    private void Update()
    {
        if (playerControl)
        {
            PlayerHealthCheck();

            if (transformButton.WasPerformedThisFrame())
            {
                // Attempt to switch to a new enemy if the player is within proximity
                if (CanSwitch())
                {
                    GameObject newEnemy = FindEnemyInProximity();
                    if (newEnemy != null)
                    {
                        SwitchToEnemyCharacter(newEnemy);
                    }
                    if (newEnemy.name == "Dark Monster(Clone)")
                    {
                        StartCoroutine(FadeOutMusic(1));
                    }
                }
            }
        }
        else
        {
            PlayerHealthCheck();

            if (transformButton.WasPerformedThisFrame())
            {
                if (currentEnemy != null)
                {
                    SwitchBackToPlayerCharacter();
                }
            }
        }
    }

    IEnumerator FadeOutMusic(float fadeTime)
    {
        float startPitch = MusicManager.audioSource.pitch;

        while (MusicManager.audioSource.pitch > -0.5f)
        {
            MusicManager.audioSource.pitch -= (startPitch + 0.5f) * Time.deltaTime / fadeTime;
            yield return null;
        }

        MusicManager.audioSource.pitch = -0.5f; // Set pitch to -0.5 to keep it there
    }


        IEnumerator FadeInMusic(float fadeTime)
    {
        float targetPitch = 1.0f;

        MusicManager.audioSource.pitch = 0.5f;
        MusicManager.audioSource.Play();

        while (MusicManager.audioSource.pitch < targetPitch)
        {
            MusicManager.audioSource.pitch += Time.deltaTime / fadeTime;
            yield return null;
        }

        MusicManager.audioSource.pitch = targetPitch; // Set pitch to 1 to keep it there
    }



    private GameObject FindEnemyInProximity()
    {
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, maxProximityDistance);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                return collider.gameObject;
            }
        }

        return null;
    }

    private void SwitchToEnemyCharacter(GameObject newEnemy)
    {
        playerControl = !playerControl;
        player.SetActive(playerControl);
        newEnemy.SetActive(!playerControl);
        newEnemy.tag = "Player";
        ToggleEnemyScripts(newEnemy);

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

        currentEnemy = newEnemy;

        AnimationControllerSwap animationControllerSwap = newEnemy.GetComponent<AnimationControllerSwap>();
        if (animationControllerSwap != null)
        {
            animationControllerSwap.SwapAnimatorController();
        }

        Debug.Log("New enemy assigned: " + newEnemy.name);
    }

    private void SwitchBackToPlayerCharacter()
    {
        Vector3 enemyPosition = currentEnemy.transform.position;
        Quaternion enemyRotation = currentEnemy.transform.rotation;

        playerControl = true;
        player.transform.position = enemyPosition;
        player.transform.rotation = enemyRotation;
        player.SetActive(true);

        if (currentEnemy.name == "Dark Monster(Clone)")
        {
            StartCoroutine(FadeInMusic(1));
        }
        
        currentEnemy.SetActive(false);
        currentEnemy.tag = "Enemy";
        Destroy(currentEnemy);
        ToggleEnemyScripts(player);
        currentEnemy = null;

        Debug.Log("Switched back to player");
        enemySpawnSystem.EnemyDestroyed();
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

    private bool CanSwitch()
    {
        return FindEnemyInProximity() != null;
    }

    private void PlayerHealthCheck()
    {
        GameObject characterToCheck = playerControl ? player : currentEnemy;
        HealthSystem characterHealthSystem = characterToCheck.GetComponent<HealthSystem>();

        if (characterHealthSystem != null && characterHealthSystem.currentHealth <= 0 && gameObject.name != "Player")
        {
            Debug.Log("Character's health reached zero. Switching back to the player.");
            SwitchBackToPlayerCharacter();
            StartCoroutine(FadeInMusic(1));
            enemySpawnSystem.EnemyDestroyed();
        }
    }
}
