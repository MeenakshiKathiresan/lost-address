using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Door : MonoBehaviour
{
    Direction doorDirection;

    Civilian civilian;
    Enemy enemy;

    [SerializeField]
    bool doorOpened = false;

    SpriteRenderer sprite;

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
        sprite = GetComponentInChildren<SpriteRenderer>();
        DisableDoor();

        // TODO put level design and spawn min max of that
        // for now 1 or 2 or 3
        GameManager.OnGameStart += Reset;

        enemyCount = Random.Range(1, 4);
        doorOpened = false;
       // GameManager.OnGameStart += ResetGame;
    }

    private void OnDisable()
    {
        GameManager.OnGameStart -= Reset;
    }

    private void Reset()
    {
        doorOpened = false;
        enemyCount = Random.Range(1, 4);
    }

    public void OpenDoor()
    {
        
        transform.DOScaleX(0, 0.5f);

        if (isDestination)
        {
            //level up
        }
        else if (hasEnemies && !doorOpened)
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y + 1);
            enemy = (Enemy)PoolManager.Instantiate("enemy", pos, transform.rotation);
            enemy.CurrentFloor = currentFloor;
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

        //transform.localScale = Vector3.zero;
        StartCoroutine(CloseDoor());
        doorOpened = true;
    }

    public void EnableDoor()
    {
        sprite.DOColor(new Color(255, 255, 255, 1), 0.3f);
    }

    public void DisableDoor()
    {
        sprite.DOColor(new Color(255, 255, 255, 0.25f), 0.3f);
    }

    IEnumerator CloseDoor()
    {
        yield return new WaitForSeconds(GameManager.instance.GetCurrentLevel().doorWaitTime);

        transform.DOScaleX(1, 0.2f);
        //transform.localScale = Vector3.one;

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
