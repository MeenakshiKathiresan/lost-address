using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable
{
    [SerializeField]
    float speed = 10;

    [SerializeField]
    public BulletType bulletType;



    [SerializeField]
    float damage = 2f;

    public float Damage
    {
        get { return damage; }
    }



    [SerializeField]
    float gameBounds = 25f;

    int xDirection = 1;

    Rigidbody2D rigidbody;

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
        rigidbody = GetComponent<Rigidbody2D>();
        gameObject.SetActive(true);
    }

    public void SetDirection(int xDirection)
    {
        Vector2 direction = new Vector2(xDirection, 0);
        rigidbody.velocity = direction * speed;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //destroy if out of bounds
        if (transform.position.x > gameBounds || transform.position.x < -gameBounds)
        {
            PoolDestroy();
        }
    }
}

public enum BulletType {Player, Enemy}
