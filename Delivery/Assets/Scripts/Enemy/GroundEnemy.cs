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

    //[SerializeField]
    //float randomJumpTime = 1f;

    //[SerializeField]
    //float jumpPower = 5;

    //float lastJump = 0;

    [SerializeField]
    Transform aboveCollider;

    private void Start()
    {
        // count as half done
        startPos = transform.position;
        startPos.x -= moveRange / 2;

        //randomJumpTime = Random.Range(2, 4);
    }


    int dir;

    int defaultMovementdir = 1;
    // Update is called once per frame
    void FixedUpdate()
    {

        int playerFloor = GameManager.instance.player.GetCurrentFloor();
        bool inSameFloor = CurrentFloor == playerFloor;

        playerPosition = GameManager.instance.player.transform.position;
        float playerDistance = Mathf.Abs(playerPosition.x - transform.position.x);

        // jump 
        //if (Time.time - lastJump > randomJumpTime) 
        //{
        //    lastJump = Time.time;

        //    RaycastHit2D hit = Physics2D.Raycast(aboveCollider.position, Vector2.up, 4);
        //    if (hit.collider == null || !hit.collider.GetComponent<Enemy>())
        //    {
        //        rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpPower);
        //        Debug.Log("jumps");
        //    }
        //}


        if (playerDistance < shootingDistanceFromPlayer && inSameFloor)
        {
            rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);

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
            rigidbody.velocity = new Vector2(dir * enemySpeed, rigidbody.velocity.y);
            CheckAndShoot();


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
            rigidbody.velocity = new Vector2(enemySpeed * defaultMovementdir, rigidbody.velocity.y);

        }

       




}
    void CheckAndShoot()
    {
        bool outOfBounds = (transform.position.x > rightBound || transform.position.x < leftBound);

        if (outOfBounds)
        {
            rigidbody.velocity = Vector2.zero;
        }

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

    //protected override void OnCollisionEnter2D(Collision2D collision)
    //{
    //    base.OnCollisionEnter2D(collision);


    //    if (collision.gameObject.GetComponent<Player>())
    //    {
    //        foreach (ContactPoint2D contact in collision.contacts)
    //        {
    //            if (contact.normal.y < 0f)
    //            {
    //                TakeDamage(currentHealth);
    //            }
    //        }
    //    }
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.up * 4);
    }

}
