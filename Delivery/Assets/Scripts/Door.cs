using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Direction doorDirection;

    Civilian civilian;
    Enemy enemy;

    bool doorOpened = false;

    public Direction DoorDirection
    {
        get
        {
            return doorDirection;
        }
        set
        {
            doorDirection = value;
        }
    }


    bool isDestination = false;

    public bool IsDestionation
    {
        get
        {
            return isDestination;
        }
        set
        {
            isDestination = value;
        }
    }

    bool hasEnemies = false;

    public bool HasEnemies
    {
        get
        {
            return hasEnemies;
        }
        set
        {
            hasEnemies = value;
        }
    }

    public int currentFloor;
    public int currentDoor;

    int enemyCount;
    float offsetEnemyDistance = 1f;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        // TODO put level design and spawn min max of that
        // for now 1 or 2 or 3

        enemyCount = Random.Range(1, 4);
        doorOpened = false;
    }

    public void OpenDoor()
    {


        if (isDestination)
        {
            //level up
        }
        else if (hasEnemies && !doorOpened)
        {
            Debug.Log(enemyCount);
            int dir = 1;
            for (int i = 0; i < enemyCount; i++)
            {
                // alternate positive and negative
                if (i % 2 == 0) dir = -1;

                int count = (int)(i + 1) / 2;

                Vector2 pos = new Vector2(transform.position.x + (offsetEnemyDistance * count * dir), transform.position.y + 2);
                enemy = (Enemy)PoolManager.Instantiate("enemy", pos, transform.rotation);
                enemy.maxFollowDistance = (i + 1) / enemyCount * enemy.maxFollowDistance;
            }

        }
        else
        {
            string directionToSpawn = "";
            switch (doorDirection)
            {
                case Direction.right:
                    directionToSpawn = "right";
                    break;
                case Direction.left:
                    directionToSpawn = "left";
                    break;
                case Direction.up:
                    directionToSpawn = "up";
                    break;
                case Direction.down:
                    directionToSpawn = "down";
                    break;
            }

            civilian = (Civilian)PoolManager.Instantiate(directionToSpawn, transform.position, transform.rotation);

        }

        transform.localScale = Vector3.zero;
        StartCoroutine(CloseDoor());
        doorOpened = true;
    }


    IEnumerator CloseDoor()
    {
        yield return new WaitForSeconds(LevelSettings.instance.doorWaitTime);
        transform.localScale = Vector3.one;

        if (isDestination)
        {

        }
        else if (hasEnemies)
        {

        }
        else
        {
            civilian.PoolDestroy();
        }
    }
}
