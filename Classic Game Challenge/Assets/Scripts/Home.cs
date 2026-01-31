using UnityEngine;

public class Home : MonoBehaviour
{
    // Reference to the frog in home.
    public GameObject frog;

    // If the Home script is enabled, the frog in home will be disabled.
    private void OnEnable()
    {
        frog.SetActive(false);
    }

    // If the Home script is disabled, the frog in home will be enabled. (Default behavior)
    private void OnDisable()
    {
        frog.SetActive(true);
    }
    
    // When the player enters home, the frog in home will be disabled.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            // First enables home then resets the player to the starting position using Respawn() after 1 second.
            enabled = true;
            Frogger player = other.gameObject.GetComponent<Frogger>();
            
            // FIXME: uncomment & fix this sprite if we add a celebration animation/sprite.
            // frogger.gameObject.SetActive(false);
            
            player.Invoke(nameof(player.Respawn), 1f);
        }
    }
}
