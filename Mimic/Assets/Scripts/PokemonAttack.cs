using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using System.Collections;

// Define the types and their effectiveness against each other.
public enum PokemonType
{
    Normal,
    Fire,
    Water,
    Grass,
    Fighting,
    Ice,
    Poison,
    Dark,
    Ghost,
    Ground,
    Rock,
    // Add more types here...
}

[System.Serializable]
public class TypeAdvantage
{
    public PokemonType attackerType;
    public List<PokemonType> strongAgainst = new List<PokemonType>();
    public List<PokemonType> weakAgainst = new List<PokemonType>();
}

public class PokemonAttack : MonoBehaviour
{
    public GameObject floatingText;
    private int damage;
    public PokemonType type;
    public float attackRange = 2.0f; // Adjust this value to set your attack range.
    public HealthSystem targetHealth; // Reference to the HealthSystem of the target enemy.
    public Animator animator; // Reference to the Animator component.
    public InputAction attackButton;

    public Material flashMaterial;  // The material to switch to for the flash effect
    private Material originalMaterial; // The original material of the object
    private Renderer objectRenderer;

    public Color normalColor;
    public Color fireColor;
    public Color waterColor;
    public Color grassColor;
    public Color fightingColor;
    public Color iceColor;
    public Color poisonColor;
    public Color darkColor;
    public Color ghostColor;
    public Color groundColor; 
    public Color rockColor;

    // Create a list of type advantages.
    public List<TypeAdvantage> typeAdvantages = new List<TypeAdvantage>();

    private Transform target; // The target enemy to attack.
    private IEnumerator flashCoroutine;

    void Start()
    {
        attackButton.Enable();

        objectRenderer = GetComponent<Renderer>();
        originalMaterial = objectRenderer.material;
        if(Gamepad.current != null)
        {
            var gamepad = DualShockGamepad.current;
            if (gamepad == null)
            return;
            gamepad.SetLightBarColor(Color.black);
        }
    }

    private void OnDestroy()
    {
        // Notify the quest manager when a monster of the tracked type is killed
        QuestManager.instance.MonsterKilled(type);
    }

    private void Update()
    {
        if(gameObject.CompareTag("Player") &&  Gamepad.current != null)
        {
            // Perform actions based on the player's type.
            HandlePlayerTypeActions();
        }

        if (Input.GetKeyDown(KeyCode.Space) && gameObject.CompareTag("Player") || attackButton.WasPressedThisFrame() && gameObject.CompareTag("Player"))
        {
            // Trigger the attack animation whenever you press Space and have the "Player" tag.
            animator.SetBool("isAttacking", true);

            if (target != null)
            {
                // Calculate the distance to the target.
                float distance = Vector3.Distance(transform.position, target.position);

                if (distance <= attackRange)
                {
                    int damage = CalculateDamage(target.GetComponent<PokemonAttack>().type);
                    Debug.Log("Dealt " + damage + " damage to the enemy.");

                    // Apply damage to the target's health.
                    targetHealth.TakeDamage(damage);

                    // Stop the existing coroutine if it is running
                    if (flashCoroutine != null)
                    {
                        StopCoroutine(flashCoroutine);
                    }

                    // Start a new flash coroutine
                    flashCoroutine = FlashCoroutine();
                    StartCoroutine(flashCoroutine);
                
                    SoundManager.PlaySound("hurt 2");

                    if(floatingText)
                    {   
                        ShowFloatingText();
                    }
                }
            }
        }
        else
        {
            // Ensure the "isAttacking" parameter is set to false if not attacking.
            if(gameObject.CompareTag("Player"))
            {
                animator.SetBool("isAttacking", false);
            }
        }
    }

    private void ShowFloatingText()
    {
        var go = Instantiate(floatingText, target.transform.position, Quaternion.identity, target.transform);
        go.GetComponent<TextMesh>().text = damage.ToString();

        if(damage == 20)
        {
            go.GetComponent<TextMesh>().color = Color.green;
            SoundManager.audioSource.pitch = 2f;
        }
        if(damage == 10)
        {
            go.GetComponent<TextMesh>().color = Color.white;
            SoundManager.audioSource.pitch = 1.5f;
        }
        if(damage == 5)
        {
            go.GetComponent<TextMesh>().color = Color.red;
            SoundManager.audioSource.pitch = 1f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // Tag your enemy GameObject with "Enemy".
        {
            target = other.transform;
            targetHealth = other.GetComponent<HealthSystem>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            target = null;
            targetHealth = null;
        }
    }

    // Function to calculate damage based on type advantages.
    public int CalculateDamage(PokemonType target)
    {
        int baseDamage = 10; // You can set your own base damage value.
        damage = baseDamage;
    
        foreach (TypeAdvantage advantage in typeAdvantages)
        {
            if (advantage.attackerType == type)
            {
                if (advantage.strongAgainst.Contains(target))
                {
                    damage *= 2; // Double damage for strong type advantage.
                    PlayStrongTypeAdvantageEffect();
                    
                }
                else if (advantage.weakAgainst.Contains(target))
                {
                    damage /= 2; // Half damage for weak type advantage.
                }
            }
            else if (advantage.attackerType == target)
            {
                if (advantage.weakAgainst.Contains(type))
                {
                    damage /= 2; // Half damage for weak type advantage for the target.
                }
            }
        }

        return damage;
    }

    // Inside your combat script where you want to trigger screen shake
    private void PlayStrongTypeAdvantageEffect()
    {
        // Add visual effects for strong type advantage
        ScreenShake.Instance.ShakeScreen(1.0f, 1.0f); // Adjust amplitude and frequency as needed
    }

    IEnumerator FlashCoroutine()
    {
        if (target == null)
        {
            yield break; // Exit the coroutine if the target is null or destroyed.
        }

        // Store the original material of the target
        Material targetOriginalMaterial = target.GetComponent<Renderer>().material;

        // Change target's material emission color to indicate flash
        Color originalEmissionColor = targetOriginalMaterial.GetColor("_EmissionColor");
        targetOriginalMaterial.EnableKeyword("_EMISSION");
        targetOriginalMaterial.SetColor("_EmissionColor", flashMaterial.GetColor("_EmissionColor"));

        // Record the start time
        float startTime = Time.time;

        // Wait for 0.1 seconds
        while (Time.time - startTime < 0.1f)
        {
            // Check if the target is still valid before reverting the material
            if (target == null)
            {
                // If the target is destroyed during the wait, exit the coroutine
                targetOriginalMaterial.SetColor("_EmissionColor", originalEmissionColor);
                yield break;
            }

            // Check if the player left the trigger box during the wait
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > attackRange)
            {
                // Revert target's material emission color back to original
                targetOriginalMaterial.SetColor("_EmissionColor", originalEmissionColor);

                // Revert player's material back to originalMaterial
                objectRenderer.material = originalMaterial;

                yield break; // Exit the coroutine if the player left the trigger box
            }

            yield return null;
        }

        // Revert target's material emission color back to original
        targetOriginalMaterial.SetColor("_EmissionColor", originalEmissionColor);

        // Revert player's material back to originalMaterial
        objectRenderer.material = originalMaterial;
    }


    // Function to perform actions based on the player's type.
    private void HandlePlayerTypeActions()
    {
        var gamepad = DualShockGamepad.current;
        if (gamepad == null)
        return;
        // Check the player's type and perform actions accordingly.
        switch (type)
        {
            case PokemonType.Normal:
                // Perform actions specific to Normal type.
                gamepad = DualShockGamepad.current;
                gamepad.SetLightBarColor(normalColor);
                break; 
            case PokemonType.Fire:
                // Perform actions specific to Fire type.
                gamepad = DualShockGamepad.current;
                gamepad.SetLightBarColor(fireColor);
                break;
            case PokemonType.Grass:
                // Perform actions specific to Grass type.
                gamepad = DualShockGamepad.current;
                gamepad.SetLightBarColor(grassColor);
                break;
            case PokemonType.Water:
                // Perform actions specific to Water type.
                gamepad = DualShockGamepad.current;
                gamepad.SetLightBarColor(waterColor);
                break;
            case PokemonType.Fighting:
                // Perform actions specific to Fighting type.
                gamepad = DualShockGamepad.current;
                gamepad.SetLightBarColor(fightingColor);
                break;
            case PokemonType.Ice:
                // Perform actions specific to Ice type.
                gamepad = DualShockGamepad.current;
                gamepad.SetLightBarColor(iceColor);
                break; 
            case PokemonType.Poison:
                // Perform actions specific to Poison type.
                gamepad = DualShockGamepad.current;
                gamepad.SetLightBarColor(poisonColor);
                break;
            case PokemonType.Dark:
                // Perform actions specific to Dark type.
                gamepad = DualShockGamepad.current;
                gamepad.SetLightBarColor(darkColor);
                break;
            case PokemonType.Ghost:
                // Perform actions specific to Ghost type.
                gamepad = DualShockGamepad.current;
                gamepad.SetLightBarColor(ghostColor);
                break; 
            case PokemonType.Ground:
                // Perform actions specific to Ground type.
                gamepad = DualShockGamepad.current;
                gamepad.SetLightBarColor(groundColor);
                break;    
            case PokemonType.Rock:
                // Perform actions specific to Rock type.
                gamepad = DualShockGamepad.current;
                gamepad.SetLightBarColor(rockColor);
                break;                                          
        }
    }
}
