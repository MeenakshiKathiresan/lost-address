using System.Collections;
using System.Collections.Generic;
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


    public static LevelSettings instance;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < floors.Count; i++)
        {
            for (int j = 0; j < floors[i].Doors.Count; j++)
            {
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

            }
        }

        //set random doors as enemy doors later
        
    }

    private void OnEnable()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

    }

    void Update()
    {
        
    }
}

public enum Direction {left, right, up, down}
