using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    int dir;

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
                    dir = -1;
                }
                else
                {
                    dir = 1;
                }

                transform.Translate(Vector2.right * dir * enemySpeed * Time.deltaTime);

            }

            // move left and right
            else
            {
                bool outOfBounds = (transform.position.x > rightBound || transform.position.x < leftBound);


                if (Mathf.Abs(transform.position.x - startPos.x) > moveRange || outOfBounds)
                {
                    defaultMovementdir *= -1;
                    startPos.x = transform.position.x;

                }

                transform.Translate(Vector2.right * defaultMovementdir * enemySpeed * Time.deltaTime);

            }

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
        bullet.SetDirection(moveDirection);

    }

   

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.up * 4);
    }

}
