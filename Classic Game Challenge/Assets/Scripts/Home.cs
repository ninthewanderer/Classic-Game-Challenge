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
            // Enables home.
            enabled = true;
            
            // Finds the Game Manager and calls the HomeCollected() method.
            FindObjectOfType<GameManager>().HomeCollected();
        }
    }
}
