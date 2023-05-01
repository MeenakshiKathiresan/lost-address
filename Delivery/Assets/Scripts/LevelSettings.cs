using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelSettings : MonoBehaviour
{
    [SerializeField]
    List<Floor> floors = new List<Floor>();

    [SerializeField]
    int destinationFloor;

    [SerializeField]
    int destinationDoor;

    public float doorWaitTime = 1f;

    // should be withing 0 to total levels count
    [SerializeField]
    int playerStartFloor = 2;

    [SerializeField]
    float playerStartX = -15f;

    [SerializeField]
    int minEnemiesPerFloor = 1;

    // max - number of doors
    [SerializeField]
    int maxEnemiesPerFloor = 2;

    // max - number of floors
    [SerializeField]
    int floorsWithGroundEnemiesCount = 3;

    [SerializeField]
    int avoidDestinationNearPlayerBy = 1;

    [SerializeField]
    float levelBounds = 20;


    public static LevelSettings instance;

    public Vector2 GetPlayerStartPos()
    {
        Vector2 playerpos = new Vector2(playerStartX, floors[playerStartFloor].transform.position.y + 1f);
        return playerpos;
    }

    void StartLevel()
    {

        

        if (Random.Range(0, 2) == 0) 
        {
            int startIndex = Mathf.Clamp(playerStartFloor + avoidDestinationNearPlayerBy, 0, floors.Count - 1);
            destinationFloor = Random.Range(startIndex, floors.Count);
        }
        else 
        {
            int endIndex = Mathf.Clamp(playerStartFloor - avoidDestinationNearPlayerBy, 0, floors.Count - 1);
            destinationFloor = Random.Range(0, endIndex);
        }



        destinationDoor = Random.Range(0, floors[0].Doors.Count);

        // random floors selected to spawn ground enemies
      
        List<int> floorsWithGroundEnemies = GetRandomNumbersInRange(floorsWithGroundEnemiesCount, floors.Count);

        for (int i = 0; i < floors.Count; i++)
        {
            int enemyCount = Random.Range(minEnemiesPerFloor, maxEnemiesPerFloor + 1);

            List<int> enemyPositions = GetRandomNumbersInRange(enemyCount, floors[0].Doors.Count);


            if (floorsWithGroundEnemies.Contains(i))
            {
                float posX;
                //spawn ground enemy near ladder
                //if (floors[i].GetComponentInChildren<Ladder>())
                //{
                //    posX = floors[i].GetComponentInChildren<Ladder>().transform.position.x;
                //}
                //else
                {
                    float bound = levelBounds - 8;
                    posX = Random.Range(-bound, bound);
                }
                float posY = floors[i].transform.position.y + 2;

                GroundEnemy groundEnemy = (GroundEnemy)PoolManager.Instantiate("groundEnemy", new Vector2(posX, posY), transform.rotation);
                groundEnemy.CurrentFloor = i;
            }

            for (int j = 0; j < floors[i].Doors.Count; j++)
            {
                floors[i].CurrentFloor = i;
                Door currentDoor = floors[i].Doors[j];
                currentDoor.IsDestionation = false;

                if (destinationFloor > i)
                {
                    currentDoor.DoorDirection = Direction.up;
                }
                else if(destinationFloor < i)
                {
                    currentDoor.DoorDirection = Direction.down;
                }
                else
                {
                    // in the same floor
                    if (destinationDoor > j)
                    {
                        currentDoor.DoorDirection = Direction.right;
                    }
                    else if(destinationDoor < j)
                    {
                        currentDoor.DoorDirection = Direction.left;
                    }
                    else
                    {
                        currentDoor.IsDestionation = true;
                    }
                }

                currentDoor.currentFloor = i;
                currentDoor.currentDoor = j;

                
                //float spawnEnemyProbability = 1;//Random.Range(0f, 1f);

                //if (spawnEnemyProbability > 0.5f)
                //{
                //    currentDoor.HasEnemies = true;
                //}
                //else
                //{
                //    currentDoor.HasEnemies = false;
                //}

                if (enemyPositions.Contains(j))
                {
                    currentDoor.HasEnemies = true;
                }
                else
                {
                    currentDoor.HasEnemies = false;
                }

            }
        }

        //set random doors as enemy doors later
        
    }

    // per level enemy
    List<int> GetEnemyPositions()
    {
        int enemyCount = Random.Range(minEnemiesPerFloor, maxEnemiesPerFloor + 1);

        List<int> enemyPositions = new List<int>();
        List<int> numbers = Enumerable.Range(0, floors[0].Doors.Count).ToList();


        for (int i=0; i< enemyCount; i++)
        {
            int enemyIndex = Random.Range(0, numbers.Count);
            Debug.Log(numbers[enemyIndex]);
            enemyPositions.Add(numbers[enemyIndex]);
            numbers.RemoveAt(enemyIndex);
        }



        return enemyPositions;
    }

    List<int> GetRandomNumbersInRange(int n, int total)
    {
        
        List<int> randomNumbers = new List<int>();
        List<int> numbers = Enumerable.Range(0, total).ToList();

        for (int i = 0; i < n; i++)
        {
            int currentIndex = Random.Range(0, numbers.Count);
            randomNumbers.Add(numbers[currentIndex]);
            numbers.RemoveAt(currentIndex);
        }


        return randomNumbers;
    }




    private void OnEnable()
    {
        GameManager.OnGameStart += StartLevel;
        StartLevel();

    }


    void Update()
    {
        
    }
}

public enum Direction {left, right, up, down}
