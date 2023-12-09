using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed = 1.19f;
    public Transform pointA;
    public Transform pointB;
    private Vector3 lastPosition;
    private bool facingLeft = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastPosition = transform.position;
    }

    void Update()
    {
        // Move between point A and B
        float time = Mathf.PingPong(Time.time * speed, 1);
        transform.position = Vector3.Lerp(pointA.position, pointB.position, time);

        // Determine the direction of movement
        CheckDirection();
    }

    void CheckDirection()
    {
        if (transform.position.x > lastPosition.x && facingLeft)
        {
            // Moving right, flip if currently facing left
            Flip();
        }
        else if (transform.position.x < lastPosition.x && !facingLeft)
        {
            // Moving left, flip if currently facing right
            Flip();
        }
        lastPosition = transform.position;
    }

    void Flip()
    {
        // Flip the enemy by rotating 180 degrees on the Z axis
        transform.Rotate(0, 180, 0);
        facingLeft = !facingLeft;
    }
}
