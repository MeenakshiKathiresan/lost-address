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

    [SerializeField]
    bool isGrounded;

    [SerializeField]
    bool isOnLadder;

    [SerializeField]
    Transform healthFill;

    [Header("Shooting Settings")]
    [SerializeField]
    Transform fireTransform;

    [SerializeField]
    Transform bulletsParent;

    [SerializeField]
    float coolDownTime = 1f;

    float lastBulletTime = 0;

    [SerializeField]
    string bulletString = "playerBullet";

    [SerializeField]
    float totalHealth = 100;

    float currentHealth = 100;

    float CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = Mathf.Clamp(value, 0, 100); }
    }

    Door currentDoor;
    bool isNearDoor;

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


        if (Time.time - lastBulletTime > coolDownTime && Input.GetKeyDown(KeyCode.F))
        {
            lastBulletTime = Time.time;
            Fire();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isNearDoor)
        {
            currentDoor.OpenDoor();
        }


        if (jumpKeyDown && isOnLadder)
        {
            rigidbody.velocity = new Vector2(0, climbSpeed);
            //Vector3 currentPosition = transform.position;
            //currentPosition.x = ladderX;
            //transform.position = currentPosition;
        }


        if (jumpKeyDown && isGrounded)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpPower);
        }

        if (jumpKeyUp && rigidbody.velocity.y > 0)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y * 0.5f);
        }


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

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    private void FixedUpdate()
    {
        if (isGrounded || !isOnLadder)
            rigidbody.velocity = new Vector2(xMovement * speed, rigidbody.velocity.y);

    }

    void Fire()
    {
        Bullet bullet = (Bullet)PoolManager.Instantiate(bulletString, fireTransform.position, fireTransform.rotation);
        bullet.transform.SetParent(bulletsParent);
        bullet.SetDirection(moveDirection);
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
        else if (collision.gameObject.GetComponent<Door>())
        {
            isNearDoor = true;
            currentDoor = collision.gameObject.GetComponent<Door>();
        }
        else if (collision.gameObject.GetComponent<Bullet>())
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            if (bullet.bulletType == BulletType.Enemy)
            {
                CurrentHealth -= bullet.Damage;
                Vector3 updatedScale = healthFill.localScale;
                updatedScale.x = CurrentHealth / totalHealth;
                healthFill.localScale = updatedScale;

                bullet.PoolDestroy();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Ladder>())
        {
            isOnLadder = false;
        }
        else if (collision.gameObject.GetComponent<Door>())
        {
            isNearDoor = false;
        }
    }



    void flip()
    {
        spriteDirection *= -1;
        transform.localScale = new Vector3(spriteDirection, 1, 1);
    }

}
