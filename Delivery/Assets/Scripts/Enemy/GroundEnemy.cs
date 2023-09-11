 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GroundEnemy : Enemy
{

    [SerializeField]
    float moveRange = 4;

    // if player is close to enemy - within range then start moving towards player
    [SerializeField]
    float shootingRange = 8;

    [SerializeField]
    float shootingDistanceFromPlayer = 4;

    [SerializeField]
    string bulletString = "bulletEnemy";

    [SerializeField]
    Transform fireTransform;

    Vector2 startPos;
    Vector3 playerPosition;

    public float leftBound;
    public float rightBound;


    int spriteDirection = 1;
    int moveDirection = 1;

    [SerializeField]
    Transform aboveCollider;

    SpriteRenderer sprite;

    private void Start()
    {
        // count as half done
        startPos = transform.position;
        startPos.x -= moveRange / 2;


        sprite = GetComponent<SpriteRenderer>();
    }


    int direction;

    int defaultMovementdir = 1;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.instance.GetCurrentState() == GameManager.GameState.InGame)
        {
            int playerFloor = GameManager.instance.player.GetCurrentFloor();
            bool inSameFloor = CurrentFloor == playerFloor;

            playerPosition = GameManager.instance.player.transform.position;
            float playerDistance = Mathf.Abs(playerPosition.x - transform.position.x);


            if (playerDistance < shootingDistanceFromPlayer && inSameFloor)
            {

                CheckAndShoot();
            }

            // 
            else if (playerDistance < shootingRange && inSameFloor)
            {

                if (playerPosition.x < transform.position.x)
                {
                    direction = -1;
                }
                else
                {
                    direction = 1;
                }

                transform.Translate(Vector2.right * direction * enemySpeed * Time.deltaTime);

            }

            // move left and right
            else
            {
                bool outOfBounds = (transform.position.x > rightBound || transform.position.x < leftBound);


                if (Mathf.Abs(transform.position.x - startPos.x) > moveRange || outOfBounds)
                {
                    defaultMovementdir *= -1;
                    direction = defaultMovementdir;
                    startPos.x = transform.position.x;

                }

                transform.Translate(Vector2.right * defaultMovementdir * enemySpeed * Time.deltaTime);

            }

            HandleFlipping();

        }
    }




    void CheckAndShoot()
    {

        if (Time.time - lastShot > shootInterval && sprite.isVisible)
        {
            lastShot = Time.time;
            Fire();
        }
    }

    void Fire()
    {
        Bullet bullet = (Bullet)PoolManager.Instantiate(bulletString, fireTransform.position, fireTransform.rotation);

        int moveDirection = 1;
        if (transform.position.x - playerPosition.x > 0)
        {
            moveDirection = -1;
        }
        direction = moveDirection;
        bullet.SetDirection(moveDirection);

    }

    void HandleFlipping()
    {
        if (direction > 0)
        {
            moveDirection = 1;
        }
        else if (direction < 0)
        {
            moveDirection = -1;
        }

        if (moveDirection != spriteDirection)
        {
            flip();
        }
    }

    void flip()
    {
        spriteDirection *= -1;
        transform.localScale = new Vector3(spriteDirection, 1, 1);

        // work around for health bar! change later
        if (moveDirection == -1)
        {

            Vector2 healthParentScale = healthBar.parent.localScale;
            healthParentScale.x = -1;
            healthBar.parent.localScale = healthParentScale;
        }
        else
        {
            Vector2 healthParentScale = healthBar.parent.localScale;
            healthParentScale.x = 1;
            healthBar.parent.localScale = healthParentScale;
        }
    }


}
