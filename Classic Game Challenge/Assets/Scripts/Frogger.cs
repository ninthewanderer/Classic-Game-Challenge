using System.Collections;
using UnityEngine;

public class Frogger : MonoBehaviour
{
    // Stores the Sprite Renderer & sprites for the Leap coroutine.
    private SpriteRenderer _spriteRenderer;
    public Sprite idleSprite;
    public Sprite leapSprite;

    // Gets needed components of the player GameObject once the game starts.
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    // Checks for player movement inputs every frame.
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            Move(Vector3.up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            Move(Vector3.down);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            Move(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            Move(Vector3.right);
        }
    }

    // Moves the player based on the provided value.
    private void Move(Vector3 direction)
    {
        Vector3 destination = transform.position + direction;

        // Checks if there is a collider with a specific layer label where the destination is. 
        Collider2D barrier = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Barrier"));
        Collider2D platform = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Platform"));
        
        // If there is a barrier, the player doesn't move.
        if (barrier != null)
        {
            return;
        }

        // If there is a platform, the player is set as a child object so that it moves with the platform.
        if (platform != null)
        {
            transform.SetParent(platform.transform);
        }
        else // If not on a platform, the player object is no longer a child object of the platform.
        {
            transform.SetParent(null);
        }
        
        StartCoroutine(Leap(destination));
    }

    // Smoothly moves the player to give the illusion of animated movement using Lerp.
    private IEnumerator Leap(Vector3 destination)
    {
        // Starting position of the player before movement.
        Vector3 startPosition = transform.position;
        
        // Used to track the time that has passed (elapsed) and the duration of the movement.
        float elapsed = 0f;
        float duration = 0.125f;
        
        _spriteRenderer.sprite = leapSprite;

        // Moves the player smoothly from frame to frame.
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPosition, destination, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Ensures the player is in the correct position at the end of the loop and resets the sprite.
        transform.position = destination;
        _spriteRenderer.sprite = idleSprite;
    }
}
