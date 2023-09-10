using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Door : MonoBehaviour
{
    Direction doorDirection;

    Civilian civilian;
    Enemy enemy;

    [SerializeField]
    bool doorOpened = false;

    [SerializeField]
    Transform doorBody;

    SpriteRenderer[] sprites;

    TextMeshPro doorNo;

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
        sprites = GetComponentsInChildren<SpriteRenderer>();

        doorNo = GetComponentInChildren<TextMeshPro>();

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

    public void SetCurrentFloorAndDoor(int floor, int door)
    {
        currentDoor = door;
        currentFloor = floor;


        doorNo.text = (((currentFloor + 1) * 10) + currentDoor + 1).ToString();
    }

    public void OpenDoor()
    {
        
        doorBody.DOLocalMoveX(-3, 0.5f);


        if (isDestination)
        {
            //level up
            civilian = (Civilian)PoolManager.Instantiate("civilian", transform.position, transform.rotation);
            GameManager.instance.OnPlayerReachedDestination();

        }
        else if (hasEnemies && !doorOpened)
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
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

        for(int i = 0; i < sprites.Length; i++)
        {

            sprites[i].DOColor(new Color(255, 255, 255, 1), 0.3f);
        }

        doorNo.alpha = 1;
    }

    public void DisableDoor()
    {
        if (!doorOpened)
        {
            for (int i = 0; i < sprites.Length; i++)
            {

                sprites[i].DOColor(new Color(255, 255, 255, 0.25f), 0.3f);
            }

            doorNo.alpha = 0.25f;
        }
        
    }

    IEnumerator CloseDoor()
    {
        yield return new WaitForSeconds(GameManager.instance.GetCurrentLevel().doorWaitTime);

        doorBody.DOLocalMoveX(0, 0.2f);
        //transform.localScale  Vector3.one;
        doorOpened = false;

        if(GameManager.instance.player.GetCurrentDoor() != this)
            DisableDoor();

   
    }
}
