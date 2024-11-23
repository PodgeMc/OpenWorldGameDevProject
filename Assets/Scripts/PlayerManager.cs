using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public Text healthText;

    void Start()
    {
        currentHealth = maxHealth; // Set health to max at the start
        UpdateHealthText(); // Update the health display
    }

    // Reduces player's health by the specified amount
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health stays within valid range
        UpdateHealthText();

        if (currentHealth <= 0)
        {
            Die(); // Trigger death if health reaches zero
        }
    }

    // Restores player's health by the specified amount
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health stays within valid range
        UpdateHealthText();
    }

    // Updates the health display in the UI
    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + currentHealth;
        }
        else
        {
            Debug.Log("Health Text UI is not assigned!");
        }
    }

    // Handles player's death and transitions to the Game Over screen
    private void Die()
    {
        SceneManager.LoadScene("GameOverScreen");
        Debug.Log("Player has died!");
    }
}
