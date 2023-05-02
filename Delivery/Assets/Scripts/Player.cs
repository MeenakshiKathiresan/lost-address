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
    string bulletString = "bullet";

    [SerializeField]
    Transform bulletParent;

    [SerializeField]
    float coolDownTime = 1f;

    [SerializeField]
    float damageOnHittingEnemyFromBelow = 30;

    float lastAttackTime = 0;

    [SerializeField]
    float totalHealth = 100;


    [SerializeField]
    ParticleSystem onHit;


    float currentHealth = 100;

    bool enemyContact = false;

    Enemy enemyInContact;

    int currentFloor;

    public int CurrentFloor
    {
        set { currentFloor = value; }
        get { return currentFloor; }
    }


    float CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = Mathf.Clamp(value, 0, totalHealth); }
    }

    Door currentDoor;
    [SerializeField]
    bool isNearDoor;

    float ladderX;

    Animator animatorController;
    

    private void OnEnable()
    {
        GameManager.OnGameStart += Reset;
        Reset();

    }


    void Reset()
    {
        transform.position = GameManager.instance.GetPlayerStartPosition();
        CurrentHealth = totalHealth;
        healthFill.localScale = Vector3.one;
    }

    private void OnDisable()
    {
        GameManager.OnGameStart -= Reset;
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animatorController = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.GetCurrentState() == GameManager.GameState.InGame)
        {
            xMovement = Input.GetAxis("Horizontal");

            bool jumpKeyDown = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W);
            bool jumpKeyUp = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W);




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

            if (rigidbody.velocity.y > 0.5f)
            {
                animatorController.SetBool("jumping", true);
            }
            else
            {
                animatorController.SetBool("jumping", false);
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

            if (Time.time - lastAttackTime > coolDownTime && Input.GetKeyDown(KeyCode.F))
            {
                lastAttackTime = Time.time;
                Fire();
            }
        }
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.GetCurrentState() == GameManager.GameState.InGame)
        {
            //if (isGrounded || !isOnLadder)
            {
                rigidbody.velocity = new Vector2(xMovement * speed, rigidbody.velocity.y);

            }

            if (Mathf.Abs(rigidbody.velocity.x) > 0f && isGrounded)
            {
                animatorController.SetBool("running", true);
            }
            else
            {

                animatorController.SetBool("running", false);
            }

            if (enemyContact && enemyInContact.gameObject.activeInHierarchy)
            {
                float playerToEnemy = transform.position.x - enemyInContact.transform.position.x;

                // avoid pushing enemies
                // enemy in the left and trying to move left or enemy in the right and trying to move right
                if ((playerToEnemy > 0 && xMovement < 0) || (playerToEnemy < 0 && xMovement > 0) && (enemyInContact.CurrentFloor == currentFloor))

                {
                    rigidbody.velocity = Vector2.zero;
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {

        CurrentHealth -= damage;

        onHit.Play();

        Vector3 scale = healthFill.localScale;
        scale.x = CurrentHealth / totalHealth;
        healthFill.localScale = scale;



        if (CurrentHealth <= 0)
        {
            GameManager.instance.OnPlayerDead();
        }
    }

    public int GetCurrentFloor()
    {
        return CurrentFloor;
    }

    void Fire()
    {
        Bullet bullet = (Bullet)PoolManager.Instantiate(bulletString, fireTransform.position, fireTransform.rotation);
        bullet.transform.SetParent(bulletParent);
        bullet.SetDirection(moveDirection);
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
            enemyInContact = collision.gameObject.GetComponent<Enemy>();

            bool hitOnce = false;

            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < 0f && !hitOnce)
                {
                    hitOnce = true;
                    TakeDamage(damageOnHittingEnemyFromBelow);
                }
            }
        }


     

        else if (collision.gameObject.GetComponent<Bullet>()){
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            TakeDamage(bullet.Damage);
            bullet.PoolDestroy();
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
            //current door call enable
            currentDoor.EnableDoor();
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
            currentDoor = collision.gameObject.GetComponent<Door>();
            // current door call disable
            currentDoor.DisableDoor();
        }
    }



    void flip()
    {
        spriteDirection *= -1;
        transform.localScale = new Vector3(spriteDirection, 1, 1);

        // work around for health bar! change later
        if (moveDirection == -1)
        {

            Vector2 healthParentScale = healthFill.parent.localScale;
            healthParentScale.x = -1;
            healthFill.parent.localScale = healthParentScale;
        }
        else
        {
            Vector2 healthParentScale = healthFill.parent.localScale;
            healthParentScale.x = 1;
            healthFill.parent.localScale = healthParentScale;
        }
    }



}

