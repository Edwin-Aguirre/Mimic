using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.SceneManagement;

public class SwitchControl : MonoBehaviour
{
    public GameObject player;
    public float maxProximityDistance = 3f; // Maximum distance for the proximity check
    public string[] enemyScriptsToEnable;
    public string[] enemyScriptsToDisable;
    public InputAction transformButton;

    private GameObject currentEnemy;
    public bool playerControl = true;
    private NavMeshAgent enemyNavMeshAgent;
    private CharacterController enemyCharacterController;
    private Vector3 initialPlayerPosition;
    private Quaternion initialPlayerRotation;

    private EnemySpawnSystem enemySpawnSystem;

    public static SwitchControl instance;

    private bool isDying = false;

    private Material originalPlayerBloodMat;
    private ParticleSystem originalBloodParticles;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        enemySpawnSystem = FindAnyObjectByType<EnemySpawnSystem>();
        if (enemySpawnSystem == null)
        {
            Debug.LogError("EnemySpawnSystem not found in the scene.");
        }
    }

    private void Start()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
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
        // Check if the current scene is the Title scene
        if (SceneManager.GetActiveScene().name == "Title")
        {
            // Destroy the player object if it's in the Title scene
            Destroy(gameObject);
            return;
        }

        if (playerControl)
        {
            PlayerHealthCheck();

            if (transformButton.WasPerformedThisFrame())
            {
                // Attempt to switch to a new enemy if the player is within proximity
                if (CanSwitch())
                {
                    GameObject newEnemy = FindEnemyInProximity();
                    HealthSystem enemyHealth = newEnemy.GetComponent<HealthSystem>();
                    if(QuestManager.instance.quests[0].canTransformIntoAnyType)
                    {
                        QuestManager.instance.quests[0].targetMonsterType = newEnemy.GetComponent<PokemonAttack>().type;
                    }
                    if (newEnemy != null && enemyHealth.isStunned && enemyHealth.isAlive)
                    {
                        SwitchToEnemyCharacter(newEnemy);
                        enemyHealth.currentHealth += 30;
                        enemyHealth.currentHealth = Mathf.Clamp(enemyHealth.currentHealth, 0, enemyHealth.maxHealth);
                        StartCoroutine(enemyHealth.UpdateHealthBarSmoothly());
                        enemyHealth.UpdateHealthUI();
                    }
                    if (!enemyHealth.isStunned || !enemyHealth.isAlive)
                    {
                        CanvasShaker enemyCanvas = newEnemy.GetComponentInChildren<CanvasShaker>();
                        enemyCanvas.PlayCanvasShakerAnimation();
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
        //Changes player blood particles so that it plays monster particles
        HealthSystem playerBloodParticles = player.GetComponent<HealthSystem>();
        HealthSystem enemyDeathParticles = newEnemy.GetComponent<HealthSystem>();

        originalPlayerBloodMat = playerBloodParticles.bloodParticles.GetComponent<ParticleSystemRenderer>().material;

        playerBloodParticles.bloodParticles.GetComponent<ParticleSystemRenderer>().material 
        = enemyDeathParticles.deathParticles.GetComponent<ParticleSystemRenderer>().material;

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

        PokemonAttack pokemonAttack = newEnemy.GetComponent<PokemonAttack>();

        // Transfer PlayerScript to the new player
        PlayerScript.instance.TransferToNewPlayer(newEnemy);
        QuestManager.instance.MonsterTransformed(pokemonAttack.type);
    }

    private void SwitchBackToPlayerCharacter()
    {
        if (currentEnemy == null)
        {
            return;
        }

        StartCoroutine(RevertPlayerBlood());

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
        if (enemySpawnSystem != null)
        {
            enemySpawnSystem.EnemyDestroyed();
        }

        // Transfer PlayerScript to the old player
        PlayerScript.instance.TransferToNewPlayer(player);
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
        // Null check for currentEnemy
        if (currentEnemy == null)
        {
            return;
        }

        GameObject characterToCheck = playerControl ? player : currentEnemy;
        HealthSystem characterHealthSystem = characterToCheck.GetComponent<HealthSystem>();

        if (characterHealthSystem != null && characterHealthSystem.currentHealth <= 0 && gameObject.name != "Player" && !isDying)
        {
            Debug.Log("Character's health reached zero. Switching back to the player.");
            StartCoroutine(PlayerDeathAnimation());
            if (characterToCheck.name == "Dark Monster(Clone)")
            {
                StartCoroutine(FadeInMusic(1));
            }
        }
    }

    private IEnumerator PlayerDeathAnimation()
    {
        ThirdPersonController thirdPersonController = currentEnemy.GetComponent<ThirdPersonController>();
        HealthSystem healthSystem = currentEnemy.GetComponent<HealthSystem>();
        isDying = true;

        // Trigger death animation
        thirdPersonController.animator.SetTrigger("Die");
        thirdPersonController.moveSpeed = 0f;

        // Wait for the death animation to finish
        yield return new WaitForSeconds(4);

        // Apply death particles then respawn
        originalBloodParticles = healthSystem.bloodParticles;
        healthSystem.bloodParticles = null;
        healthSystem.deathParticles.gameObject.SetActive(true);
        healthSystem.deathParticles.Play();
        currentEnemy.GetComponent<SkinnedMeshRenderer>().enabled = false;
        yield return new WaitForSeconds(1f);

        // Switch back to the player
        SwitchBackToPlayerCharacter();

        thirdPersonController.animator.ResetTrigger("Die");

        isDying = false;

        healthSystem.bloodParticles = originalBloodParticles;
    }

    private IEnumerator RevertPlayerBlood()
    {
        yield return new WaitForSeconds(0.9f);

        HealthSystem playerBloodParticles = player.GetComponent<HealthSystem>();

        playerBloodParticles.bloodParticles.GetComponent<ParticleSystemRenderer>().material 
        = originalPlayerBloodMat;
    }
}
