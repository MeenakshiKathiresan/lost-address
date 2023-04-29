using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    float speed;

    [SerializeField]
    float jumpPower;

    [SerializeField]
    float climbSpeed;

    int spriteDirection = 1;
    int moveDirection = 1;

    float xMovement;

    Rigidbody2D rigidbody;

    bool isGrounded;

    bool isOnLadder;
    float ladderX;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        xMovement = Input.GetAxis("Horizontal");

        bool jumpKeyDown = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W);
        bool jumpKeyUp = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W);

        if (jumpKeyDown && isOnLadder)
        {

            rigidbody.velocity = new Vector2(rigidbody.velocity.x, climbSpeed);
            Vector3 currentPosition = transform.position;
            currentPosition.x = ladderX;
            transform.position = currentPosition;
        }


        if (jumpKeyDown && isGrounded)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpPower);
        }

        if (jumpKeyUp && rigidbody.velocity.y > 0)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y * 0.5f);
        }

        //transform.Translate(new Vector3(xMovement,0, 0) * Time.deltaTime * moveSpeed);

        if (xMovement > 0)
        {
            moveDirection = 1;
        }
        else if (xMovement < 0)
        {
            moveDirection = -1;
        }

        if (moveDirection != spriteDirection)
        {
            flip();
        }

    }

    private void FixedUpdate()
    {

        rigidbody.velocity = new Vector2(xMovement * speed, rigidbody.velocity.y);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponentInParent<Floor>())
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponentInParent<Floor>())
        {
            isGrounded = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Ladder>())
        {
            isOnLadder = true;
            ladderX = collision.transform.position.x;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Ladder>())
        {
            isOnLadder = false;
        }
    }

    void flip()
    {
        spriteDirection *= -1;
        transform.localScale = new Vector3(spriteDirection, 1, 1);
    }

}
