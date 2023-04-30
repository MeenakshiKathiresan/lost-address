using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneEnemy : Enemy
{

    [SerializeField]
    float damageDistance = 2f;

    [SerializeField]
    float randomMovementInterval = 0.25f;

    float randomMovementRange = 1f;


    [SerializeField]
    public float maxFollowDistance = 8f;


    float lastDirectionChange = 0;


    [SerializeField]
    float bounceOnBoundsDistance = 2f;

    [SerializeField]
    float startStallTime = 1;


    // Update is called once per frame
    void FixedUpdate()
    {
        // start moving after initial stall time
        if (Time.time - startTime > startStallTime)
        {
            Vector2 playerPosition = GameManager.instance.player.GetPlayerPosition();
            playerPosition.y += 1.5f;
            float distanceToPlayer = Vector2.Distance(transform.position, playerPosition);

            int playerFloor = GameManager.instance.player.GetCurrentFloor();
            bool inSameFloor = CurrentFloor == playerFloor;

            if (distanceToPlayer > damageDistance)
            {
                // chase player if within follow distance and in same floor
                if (distanceToPlayer <= maxFollowDistance && inSameFloor)
                {

                    Vector2 moveDirection = playerPosition - (Vector2)transform.position;

                    //Clamp y value
                    //moveDirection.y = 0;

                    // reflect if going towards floor or ladder
                    // include trigger layers
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, rigidbody.velocity.normalized, bounceOnBoundsDistance, Physics2D.DefaultRaycastLayers, 0, (int)QueryTriggerInteraction.Collide);


                    if ((hit.collider != null && hit.collider.GetComponent<Floor>()) || (hit.collider != null  && hit.collider.GetComponent<Ladder>()))
                    {
                        Debug.Log("Reflecting even when within following distance");
                        Vector2 newDirection = Vector2.Reflect(rigidbody.velocity.normalized, hit.normal);
                        rigidbody.velocity = newDirection.normalized * enemySpeed;
                    }
                    else
                    {
                        //chase!
                        rigidbody.velocity = moveDirection.normalized * enemySpeed;
                    }
                }
                else
                {

                    if (Time.time - lastDirectionChange > randomMovementInterval)
                    {
                        lastDirectionChange = Time.time;
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, rigidbody.velocity.normalized, bounceOnBoundsDistance, Physics2D.DefaultRaycastLayers, 0, (int)QueryTriggerInteraction.Collide);

                        // reflect if going towards floor or ladder
                        if ((hit.collider != null && hit.collider.GetComponent<Floor>()) || hit.collider.GetComponent<Ladder>())
                        {
                            Vector2 newDirection = Vector2.Reflect(rigidbody.velocity.normalized, hit.normal);
                            rigidbody.velocity = newDirection.normalized * enemySpeed;
                        }
                        else
                        {
                            Vector2 randomDirection = new Vector2(Random.Range(-randomMovementRange, randomMovementRange), Random.Range(-randomMovementRange, randomMovementRange));
                            rigidbody.velocity = randomDirection.normalized * enemySpeed;
                        }
                    }

                }
            }
            else
            {
                rigidbody.velocity = Vector2.zero;
            }


            if (Time.time - lastShot > shootInterval && inSameFloor)
            {
                lastShot = Time.time;

                if (distanceToPlayer < damageDistance)
                {
                    //TODO Put claw in player direction
                    // Get player with collider overlap circle
                    GameManager.instance.player.TakeDamage(damage);
                }
            }
        }

        else
        {
            rigidbody.velocity = Vector2.zero;
        }

        
    }
  


}
