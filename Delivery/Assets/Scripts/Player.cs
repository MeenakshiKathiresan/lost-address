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
    float damage = 20f;

    [SerializeField]
    float coolDownTime = 1f;

    float lastAttackTime = 0;

    [SerializeField]
    float attackRadius = 1f;

    [SerializeField]
    float totalHealth = 100;

    float currentHealth = 100;

    bool enemyContact = false;
    Vector2 enemypos;

    int currentFloor;

    public int CurrentFloor
    {
        set { currentFloor = value; }
        get { return currentFloor; }
    }


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


        if (Time.time - lastAttackTime > coolDownTime && Input.GetKeyDown(KeyCode.F))
        {
            lastAttackTime = Time.time;
            Attack();
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
        if (isGrounded || !isOnLadder && !enemyContact)
        {
            rigidbody.velocity = new Vector2(xMovement * speed, rigidbody.velocity.y);
        }

        if (enemyContact)
        {
            float playerToEnemy = transform.position.x - enemypos.x;

            // avoid pushing enemies
            // enemy in the left and trying to move left or enemy in the right and trying to move right
            if ((playerToEnemy > 0 && xMovement < 0) || (playerToEnemy < 0 && xMovement > 0))

                rigidbody.velocity = Vector2.zero;
        }

    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        Vector3 scale = healthFill.localScale;
        scale.x = CurrentHealth / totalHealth;
        healthFill.localScale = scale;

        if (CurrentHealth <= 0)
        {
            Debug.Log("gameover");
        }
    }

    public int GetCurrentFloor()
    {
        Debug.Log(CurrentFloor);
        return CurrentFloor;
    }

    void Attack()
    {
        Vector2 forward = new Vector2(transform.position.x + moveDirection, transform.position.y);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(forward, attackRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.GetComponent<Enemy>())
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                enemy.TakeDamage(damage);
                     
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 forward = new Vector2(transform.position.x + moveDirection, transform.position.y);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(forward, attackRadius);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponentInParent<Floor>())
        {
            isGrounded = true;
            Floor floor = collision.gameObject.GetComponentInParent<Floor>();
            CurrentFloor = floor.CurrentFloor;
        }
        else if (collision.gameObject.GetComponent<Enemy>())
        {
            enemyContact = true;
            enemypos = collision.transform.position;
        }


    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponentInParent<Floor>())
        {
            isGrounded = false;
            
        }
        else if (collision.gameObject.GetComponent<Enemy>())
        {
            enemyContact = false;
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
