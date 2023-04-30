using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IPoolable
{
    

    float lastShot = 0;

    [Header("Shoot Settings")]
    [SerializeField]
    float shootInterval = 1.5f;

    [SerializeField]
    float totalHealth = 100;

    float health = 100;


    [SerializeField]
    float damage = 10f;


    [SerializeField]
    float damageDistance = 1f;

    [SerializeField]
    Transform healthBar;


    [Header("Movement Settings")]
    [SerializeField]
    float enemySpeed = 3f;

    [SerializeField]
    float randomMovementInterval = 0.25f;

    float randomMovementRange = 1f;

    int currentFloor;

    public int CurrentFloor
    {
        set { currentFloor = value; }
        get { return currentFloor; }
    }



    [SerializeField]
    public float maxFollowDistance = 8f;


    float lastDirectionChange = 0;

    float raycastDistance = 2f;


    Rigidbody2D rigidbody;

    float startTime;
    [SerializeField]
    float startStallTime = 1;

    // Start is called before the first frame update
    void OnEnable()
    {    
        rigidbody = GetComponent<Rigidbody2D>();
        health = totalHealth;
        healthBar.localScale = Vector3.one;
        startTime = Time.time;
    }


    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime > startStallTime)
        {
            Vector2 playerPosition = GameManager.instance.player.GetPlayerPosition();
            int playerFloor = GameManager.instance.player.GetCurrentFloor();
            float distanceToPlayer = Vector2.Distance(transform.position, playerPosition);

            // move only if away from player by atleast damage distance
            if (distanceToPlayer > damageDistance)
            {
                if (distanceToPlayer <= maxFollowDistance && CurrentFloor == playerFloor)
                {
                    Vector2 moveDirection = playerPosition - (Vector2)transform.position;
                    rigidbody.velocity = moveDirection.normalized * enemySpeed;
                }
                else
                {

                    if (Time.time - lastDirectionChange > randomMovementInterval)
                    {
                        lastDirectionChange = Time.time;
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, rigidbody.velocity.normalized, raycastDistance);
                        if (hit.collider != null && hit.collider.GetComponent<Floor>())
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


            if (Time.time - lastShot > shootInterval)
            {
                lastShot = Time.time;

                if (distanceToPlayer < damageDistance)
                {
                    GameManager.instance.player.TakeDamage(damage);
                }
            }
        }
        else
        {
            rigidbody.velocity = Vector2.zero;
        }

    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lastShot = Time.time - (shootInterval / 2);
        rigidbody.velocity = Vector3.zero;

        Vector3 scale = healthBar.localScale;
        scale.x = health / totalHealth;
        healthBar.localScale = scale;

        if (health <= 0)
        {
            PoolDestroy();
        }
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public bool IsAlive()
    {
        return gameObject.activeSelf;
    }

    public void PoolDestroy()
    {
        gameObject.SetActive(false);
    }

    public void PoolInstantiate(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        gameObject.SetActive(true);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
