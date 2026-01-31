using System.Collections;
using UnityEngine;

public class Frogger : MonoBehaviour
{
    // Stores the Sprite Renderer & sprites for the Leap coroutine & for the Death() method.
    private SpriteRenderer _spriteRenderer;
    public Sprite idleSprite;
    public Sprite leapSprite;
    public Sprite deadSprite;
    
    // Stores the starting position of the player for respawning.
    private Vector3 spawnPosition;

    // Gets needed components of the player GameObject once the game starts & obtains starting/spawning position.
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        spawnPosition = transform.position;
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
        // Calculates the destination the player will be moving to based on current position & the direction they're moving in.
        Vector3 destination = transform.position + direction;

        // Checks if there is a collider with a specific layer label where the destination is. 
        Collider2D barrier = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Barrier"));
        Collider2D platform = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Platform"));
        Collider2D obstacle = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Obstacle"));
        
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

        // If the player hits anything labeled as an obstacle, they will die unless they are on a platform.
        // Platforms are an exception because of the water being labeled as an obstacle.
        if (obstacle != null && platform == null)
        {
            // The player will still move onto the obstacle that kills them.
            transform.position = destination;
            Death();
        }
        else
        {
            StartCoroutine(Leap(destination));
        }
    }

    // Smoothly moves the player to give the illusion of animated movement using Lerp.
    private IEnumerator Leap(Vector3 destination)
    {
        // Starting position of the player before movement.
        Vector3 startPosition = transform.position;
        
        // Used to track the time that has passed (elapsed) and the duration of the movement.
        float elapsed = 0f;
        float duration = 0.125f;
        
        // While the player is "leaping", the sprite changes to give the illusion of animation.
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

    // When the player dies, this method will run.
    private void Death()
    {
        // Ensures the player cannot trigger Leap() while respawning.
        StopAllCoroutines();
        
        // Resets player rotation & switches the idle sprite to the dead sprite.
        transform.rotation = Quaternion.identity;
        _spriteRenderer.sprite = deadSprite;

        // If the player is currently on a moving obstacle, they will stop moving with it.
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }
        
        // Ensures the player cannot move.
        enabled = false;
        
        // Respawns the player after a 1-second delay.
        Invoke(nameof(Respawn), 1f);
    }
    
    // Allows the player to respawn after death.
    public void Respawn()
    {
        // Ensures the player cannot trigger Leap() while respawning.
        StopAllCoroutines();
        
        // Resets the player sprite & rotation, moves them to spawn, and then re-enables them.
        transform.rotation = Quaternion.identity;
        _spriteRenderer.sprite = idleSprite;
        transform.position = spawnPosition;
        
        // FIXME: uncomment this and modify if we're adding a celebration sprite/animation.
        // gameObject.SetActive(true);
        
        enabled = true;
    }

    // Activates when the player collides with other objects.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the player is still alive (enabled) and the object collided into is an obstacle, they will die.
        // Also checks if the player is on a platform (which will be their parent).
        if (enabled && other.gameObject.layer == LayerMask.NameToLayer("Obstacle") && transform.parent == null)
        {
            Death();
        }
    }
}
