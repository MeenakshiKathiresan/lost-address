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

    private void Start()
    {
        startPos = transform.position;
    }


    // if in same floor, shoot
    //int playerFloor = GameManager.instance.player.GetCurrentFloor();
    //bool inSameFloor = CurrentFloor == playerFloor;
    // if in range move and shoot


    int dir;

    int defaultMovementdir = 1;
    // Update is called once per frame
    void FixedUpdate()
    {

        int playerFloor = GameManager.instance.player.GetCurrentFloor();
        bool inSameFloor = CurrentFloor == playerFloor;

        playerPosition = GameManager.instance.player.transform.position;
        float playerDistance = Mathf.Abs(playerPosition.x - transform.position.x);

        if (playerDistance < shootingDistanceFromPlayer && inSameFloor)
        {
            rigidbody.velocity = Vector2.zero;

            CheckAndShoot();
        }

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
            rigidbody.velocity = new Vector2(dir * enemySpeed, rigidbody.velocity.y);
            CheckAndShoot();


        }
        else
        {
           

            if (Mathf.Abs(transform.position.x - startPos.x) > moveRange)
            {
                defaultMovementdir *= -1;
                startPos.x = transform.position.x;

              
            }
            rigidbody.velocity = new Vector2(enemySpeed * defaultMovementdir, rigidbody.velocity.y);

        }

       




}
    void CheckAndShoot()
    {
        if (Time.time - lastShot > shootInterval)
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

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);


        if (collision.gameObject.GetComponent<Player>())
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < 0f)
                {
                    TakeDamage(currentHealth);
                }
            }
        }
    }

}
