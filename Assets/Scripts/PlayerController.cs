using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    float horizontal;
    float vertical;
    public float runSpeed = 20.0f;
    public float slideSpeed = 5.0f;

    [SerializeField] private GameObject interactText;
    public bool canWalk;
    public float alertValue = 0.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        // Assuming the sprite is initially facing right, set the localScale accordingly
        transform.localScale = new Vector3(1f, 1f, 1f);

        interactText.SetActive(false);
        canWalk = true;
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        // Flip the sprite based on the movement direction
        FlipSprite();

        // Update the animator parameter
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        Vector2 movement = new Vector2(horizontal, vertical).normalized;
        rb.velocity = movement * runSpeed;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 normal = collision.contacts[0].normal;
            Vector2 slideDirection = Vector2.Reflect(rb.velocity, normal).normalized;
            rb.velocity = slideDirection * slideSpeed;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Interactable"))
        {
            collision.gameObject.GetComponent<Interactable>().OnInteract();
            interactText.SetActive(true);
            canWalk = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Interactable"))
        {
            collision.gameObject.GetComponent<Interactable>().OnInteractExit();
            interactText.SetActive(false);
        }
    }

    void FlipSprite()
    {
        if (horizontal > 0)
        {
            // Moving right, flip the sprite to face right
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (horizontal < 0)
        {
            // Moving left, flip the sprite to face left
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    void UpdateAnimator()
    {
        bool isRunning = (horizontal != 0f || vertical != 0f);
        animator.SetBool("isRunning", isRunning);
    }
}
