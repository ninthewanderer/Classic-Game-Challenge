using UnityEngine;

public class MoveObstacles : MonoBehaviour
{
    // Default values; these will all be modified based on what object the script is attached to.
    public Vector2 direction = Vector2.right;
    public float speed = 1f;
    public int size = 1;

    // Used to store the edges of the screen.
    private Vector3 _leftEdge;
    private Vector3 _rightEdge;

    // Assigns the edges of the screen based on the main camera.
    private void Start()
    {
        _leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        _rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);
    }

    private void Update()
    {
        // If the object is moving and is out of bounds, it will be moved to the other side of the screen.
        if (direction.x > 0 && (transform.position.x - size) > _rightEdge.x)
        {
            Vector3 position = transform.position;
            position.x = _leftEdge.x - size;
            transform.position = position;
        }
        else if (direction.x < 0 && (transform.position.x + size) < _leftEdge.x)
        {
            Vector3 position = transform.position;
            position.x = _rightEdge.x + size;
            transform.position = position;
        }
        else // If it is not out of bounds, it will keep moving.
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }
}
