using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IPoolable
{
    [SerializeField]
    float shootInterval = 1.5f;

    float lastShot = 0;

    [SerializeField]
    string bulletString = "enemyBullet";

    [SerializeField]
    Transform fireTransform;

    [SerializeField]
    float totalHealth = 100;

    float health = 100;
    Vector2 playerpos;

    [SerializeField]
    Transform healthBar;

    // Start is called before the first frame update
    void OnEnable()
    {
        playerpos = GameManager.instance.player.GetPlayerPosition();
        transform.position = new Vector3(transform.position.x, playerpos.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastShot > shootInterval)
        {
            lastShot = Time.time;
            Bullet bullet = (Bullet)PoolManager.Instantiate(bulletString, fireTransform.position, fireTransform.rotation);
            //bullet.transform.SetParent(bulletsParent);

            playerpos = GameManager.instance.player.GetPlayerPosition();
            int moveDirection = 1;
            if (transform.position.x - playerpos.x > 0)
            {
                moveDirection = -1;
            }

            bullet.SetDirection(moveDirection);
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
        if (collision.gameObject.GetComponent<Bullet>())
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();
            if (bullet.bulletType == BulletType.Player)
            {
                health -= bullet.Damage;
                Vector3 scale = healthBar.localScale;
                scale.x = health / totalHealth;
                healthBar.localScale = scale;

                bullet.PoolDestroy();
                if (health <= 0)
                {
                    PoolDestroy();
                }
            }
        }
    }
}
