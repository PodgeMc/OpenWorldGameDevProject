using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public int maxHealth = 100; // Maximum health of the player
    private int currentHealth; // Current health of the player

    public Text healthText; // Reference to the UI Text component for health display

    void Start()
    {
        // Initialize player health
        currentHealth = maxHealth;

        // Update the health text at the start
        UpdateHealthText();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthText();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth; // Update health text
        }
        else
        {
            Debug.LogWarning("Health Text UI is not assigned!");
        }
    }

    private void Die()
    {
        SceneManager.LoadScene("GameOverScreen");
        Debug.Log("Player has died!");
        // Add any death or respawn logic here
    }
}
