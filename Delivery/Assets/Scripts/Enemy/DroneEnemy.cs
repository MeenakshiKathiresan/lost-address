using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


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

    [SerializeField]
    ParticleSystem onAttack;

    [SerializeField]
    Transform spriteTransform;

    SpriteRenderer[] sprites;

    int spriteOrder = 2;


    Vector2 startPos;
    private void Start()
    {
        
    }
    protected override void OnEnable()
    {

        base.OnEnable();

        startPos = transform.position;

        healthBar.parent.gameObject.SetActive(false);

        sprites = GetComponentsInChildren<SpriteRenderer>();

        SetSpritesSortingOrder(2);
        // first 1 sec is stalling time, use 0.5sec of that to move up
        StartCoroutine(MovePlayerUp());
 
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.instance.GetCurrentState() == GameManager.GameState.InGame)
        {
            // start moving after initial stall time
            if (Time.time - startTime > startStallTime)
            {
                Vector2 playerPosition = GameManager.instance.player.GetPlayerPosition();
                float distanceToPlayer = Mathf.Abs(transform.position.x -  playerPosition.x);

                int playerFloor = GameManager.instance.player.GetCurrentFloor();
                bool inSameFloor = CurrentFloor == playerFloor;

                if (distanceToPlayer > damageDistance)
                {
                    // chase player if within follow dista
                    // nce and in same floor
                    if (distanceToPlayer <= maxFollowDistance && inSameFloor)
                    {

                        Vector2 moveDirection = playerPosition - (Vector2)transform.position;

                        //Clamp y value
                        //moveDirection.y = Random.Range(-1.5f, 1.5f);

                        // reflect if going towards floor or ladder
                        // include trigger layers
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, rigidbody.velocity.normalized, bounceOnBoundsDistance, Physics2D.DefaultRaycastLayers, 0, (int)QueryTriggerInteraction.Collide);


                        if ((hit.collider != null && hit.collider.GetComponent<Floor>()) || (hit.collider != null && hit.collider.GetComponent<Ladder>()))
                        {
                            Vector2 newDirection = Vector2.Reflect(rigidbody.velocity.normalized, hit.normal);
                            newDirection.y = 0;
                            rigidbody.velocity = newDirection.normalized * enemySpeed;
                        }
                        else
                        {
                            //chase!

                            moveDirection.y = 0;
                            rigidbody.velocity = moveDirection.normalized * enemySpeed;
                        }
                    }
                    else
                    {
                        // when no where near enemy
                        // go back to start pos
                        
                        Vector2 moveDirection = startPos - (Vector2)transform.position;
                        if(Mathf.Abs(transform.position.x - startPos.x) < 0.5f)
                        {
                            rigidbody.velocity = Vector2.zero;
                        }
                        else
                        {

                            moveDirection.y = 0;
                            rigidbody.velocity = moveDirection.normalized * enemySpeed;
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
                        onAttack.Play();

                        Vector2 direction = (GameManager.instance.player.transform.position - spriteTransform.transform.position).normalized;

                        spriteTransform.DOLocalMove(direction, 0.2f).SetLoops(2, LoopType.Yoyo);
                    }
                }
            }

            else
            {
                rigidbody.velocity = Vector2.zero;
               
            }


        }
    }



    IEnumerator MovePlayerUp()
    {
        yield return new WaitForSeconds(0.4f);

        SetSpritesSortingOrder(5);

        healthBar.parent.gameObject.SetActive(true);
        transform.DOMoveY(transform.position.y + 1.5f, 0.3f);
    }


    void SetSpritesSortingOrder(int order)
    {
        spriteOrder = order;
        sprites[0].sortingOrder = order;



        //for (int i = 0; i < sprites.Length; i++)
        //{
        //    sprites[i].sortingOrder = order;
        //}
    }
  


}
