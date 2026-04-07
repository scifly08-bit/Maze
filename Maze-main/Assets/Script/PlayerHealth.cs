using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Respawn Settings")]
    [SerializeField] private Vector3 respawnPosition = new Vector3(182, 79, -409);

    [Header("UI References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private DamageOverLay damageOverLay;

    private CharacterController characterController;

    void Start()
    {
        currentHealth = maxHealth;
        characterController = GetComponent<CharacterController>();
        
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void Update()
    {
        // Testing damage logic
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10f);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        UpdateUI();
        if (damageOverLay != null)
        {
            damageOverLay.FlashDamage();
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (damageOverLay != null)
        {
            damageOverLay.SetOverLayIntesity(currentHealth / maxHealth);
        }
    }

    private void Die()
    {
        Debug.Log("Player has died! Respawning...");

        // Disable CharacterController to allow teleportation
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        // Teleport to respawn position
        transform.position = respawnPosition;

        // Re-enable CharacterController
        if (characterController != null)
        {
            characterController.enabled = true;
        }

        // Reset health
        Heal(maxHealth);
    }

    public void SetRespawnPosition(Vector3 newPosition)
    {
        respawnPosition = newPosition;
        Debug.Log("Checkpoint Reached! New spawn point set: " + respawnPosition);
    }
}