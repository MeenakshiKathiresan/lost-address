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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenDoor()
    {
        if (!doorOpened)
        {
            doorOpened = true;
            if (isDestination)
            {
                //level up
            }
            else if (hasEnemies)
            {
                enemy = (Enemy)PoolManager.Instantiate("enemy", transform.position, transform.rotation);
                //enemy = (Enemy)PoolManager.Instantiate("enemy", transform.position, transform.rotation);
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
        }
    }

    IEnumerator CloseDoor()
    {
        yield return new WaitForSeconds(LevelSettings.instance.doorWaitTime);
        transform.localScale = Vector3.one;

        if (isDestination) {

        }
        else if (hasEnemies) {

        }
        else
        {
            civilian.PoolDestroy();
        }
    }
}
