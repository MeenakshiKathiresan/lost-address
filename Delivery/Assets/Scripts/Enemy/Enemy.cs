using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IPoolable
{


    [Header("Shoot Settings")]
    [SerializeField]
    protected float shootInterval = 1.5f;

    [SerializeField]
    protected float totalHealth = 100;

    protected float currentHealth = 100;


    [SerializeField]
    protected float damage = 10f;


    [SerializeField]
    protected Transform healthBar;


    [Header("Movement Settings")]
    [SerializeField]
    protected float enemySpeed = 3f;

    protected int currentFloor;

    public int CurrentFloor
    {
        set { currentFloor = value; }
        get { return currentFloor; }
    }


    protected Rigidbody2D rigidbody;

    protected float startTime;

    protected float lastShot = 0;


    protected void OnEnable()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        SetToDefault();

        GameManager.OnGameStart += Reset;
    }

    protected void OnDisable()
    {
        GameManager.OnGameStart -= Reset;
    }

    protected void Reset()
    {
        SetToDefault();
        gameObject.SetActive(false);
    }

    protected void SetToDefault()
    {
        currentHealth = totalHealth;
        healthBar.localScale = Vector3.one;
        startTime = Time.time;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Bullet>())
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            if (bullet.bulletType == BulletType.Player)
            {
                TakeDamage(bullet.Damage);
                bullet.PoolDestroy();
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // done to reduce enemy's attack intensity when enemy is being attacked
        //lastShot = Time.time - (shootInterval / 2);
        //rigidbody.velocity = Vector3.zero;

        Vector3 scale = healthBar.localScale;
        scale.x = currentHealth / totalHealth;
        healthBar.localScale = scale;

        if (currentHealth <= 0)
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

}
