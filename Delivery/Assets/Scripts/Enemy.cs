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

    [SerializeField]
    float randomMovementRange = 1f;


    [SerializeField]
    float followDistance = 5f;


    float lastDirectionChange = 0;

    float raycastDistance = 2f;


    Rigidbody2D rigidbody;

    // Start is called before the first frame update
    void OnEnable()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        health = totalHealth;
        healthBar.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 playerPosition = GameManager.instance.player.GetPlayerPosition();
        float distanceToPlayer = Vector2.Distance(transform.position, playerPosition);
        if (distanceToPlayer > damageDistance)
        {
            if (distanceToPlayer <= followDistance)
            {
                //TODO if enemy in same level - add check
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



                if (Time.time - lastDirectionChange > randomMovementInterval)
                {
                    lastDirectionChange = Time.time;
                    Vector2 randomDirection = new Vector2(Random.Range(-randomMovementRange, randomMovementRange), Random.Range(-randomMovementRange, randomMovementRange));
                    rigidbody.velocity = randomDirection.normalized * enemySpeed;
                }
            }
        }
        //else
        //{
        //    rigidbody.velocity = Vector2.zero;
        //}

        if (Time.time - lastShot > shootInterval)
        {
            lastShot = Time.time;
            
            if (distanceToPlayer < damageDistance)
            {
                GameManager.instance.player.TakeDamage(damage);
            }
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
