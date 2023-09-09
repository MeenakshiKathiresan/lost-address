using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    [SerializeField]
    float speed;

    [SerializeField]
    float jumpPower;


    int spriteDirection = 1;
    int moveDirection = 1;

    float xMovement;

    Rigidbody2D rigidbody;

    [SerializeField]
    bool isGrounded;

    [HideInInspector]
    public bool isNearLadder;

    [HideInInspector]
    public bool canClimb;

    [HideInInspector]
    public bool isDownLadder;

    bool isJumpAttacking;



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

    bool isJumping = false;


    [SerializeField]
    ParticleSystem onHit;

    [SerializeField]
    Transform groundCheckPos;

    [SerializeField]
    LayerMask groundLayer;


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

    [SerializeField]
    float fallFactor = 10;

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
           

            if (isJumpAttacking)
            {
                // dont move in x axis if jump attacking
                xMovement /= 3;
            }
           

            if (Input.GetKeyDown(KeyCode.F) && isNearDoor)
            {
                currentDoor.OpenDoor();
            }


            HandleJumping();

            HandleFlipping();

            SetLadder();


            if (Time.time - lastAttackTime > coolDownTime && Input.GetKeyDown(KeyCode.Space))
            {
                lastAttackTime = Time.time;
                if(rigidbody.velocity.y > 0)
                {
                    isJumpAttacking = true;
                }
                Fire();
            }
        }
    }

    void SetLadder()
    {
        if (isNearLadder && rigidbody.velocity.y < 0.5f)
        {
            isDownLadder = true;
        }
        else
        {
            isDownLadder = false;
        }
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    void HandleFlipping()
    {
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

    void HandleJumping()
    {
        bool jumpKeyDown = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);


        if (jumpKeyDown && (IsGrounded() || (canClimb && rigidbody.velocity.x == 0)))
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpPower);

        }


        // come down faster
        if (rigidbody.velocity.y < 0f)
        {
            rigidbody.velocity -= Vector2.down * Physics2D.gravity * fallFactor * Time.deltaTime;
        }
       

        //set animations
        if (rigidbody.velocity.y > 0.5f)
        {
            isJumping = true;
        }
        else
        {
            isJumping = false;
            
        }
        animatorController.SetBool("jumping", isJumping);
    }



    private void FixedUpdate()
    {
        if (GameManager.instance.GetCurrentState() == GameManager.GameState.InGame)
        {
            //if (isGrounded || !isOnLadder)
            {
                rigidbody.velocity = new Vector2(xMovement * speed, rigidbody.velocity.y);

            }


            

            if (Mathf.Abs(rigidbody.velocity.x) > 0f && IsGrounded())
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

    public float getVelocityY()
    {
        return rigidbody.velocity.y;
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

    public Door GetCurrentDoor()
    {
       return currentDoor;
    }

    void Fire()
    {
        Bullet bullet = (Bullet)PoolManager.Instantiate(bulletString, fireTransform.position, fireTransform.rotation);
        bullet.transform.SetParent(bulletParent);
        bullet.SetDirection(moveDirection);
    }



    public bool IsGrounded()
    {
        Collider2D ground = Physics2D.OverlapBox(groundCheckPos.position, new Vector2(1f, 0.3f), 0, groundLayer);


        if (ground != null && ground.GetComponentInParent<Floor>())
        {
            Floor floor = ground.gameObject.GetComponentInParent<Floor>();
            CurrentFloor = floor.CurrentFloor;

            isGrounded = true;
            isJumpAttacking = false;

        }
        else
        {
            isGrounded = false;
        }


        return isGrounded;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.GetComponent<Enemy>())
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




        else if (collision.gameObject.GetComponent<Bullet>())
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            TakeDamage(bullet.Damage);
            bullet.PoolDestroy();
        }


    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        if (collision.gameObject.GetComponent<Enemy>())
        {
            enemyContact = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Ladder>())
        {
            isNearLadder = true;
            canClimb = true;


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
            isNearLadder = canClimb = false;
       

        }
        else if (collision.gameObject.GetComponent<Door>())
        {
            isNearDoor = false;
            // current door call disable
            if(currentDoor)
            currentDoor.DisableDoor();

            currentDoor = null;
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

