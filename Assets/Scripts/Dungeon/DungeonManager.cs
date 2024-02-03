
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using UnityEngine.AI;

public class DungeonManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject LoadingPanel;
    //check if done
    public bool done;
    [Header("Object scatter setting")]
    [SerializeField] List<GameObject> objToScatter = new List<GameObject>();
    [Space] 
    [SerializeField] private LayerMask GroundToScatter;
    [Space]
    [SerializeField] private int minObjPerRoom;
    [SerializeField] private int maxObjPerRoom;
    
    [Header("Detail and prefab")] [Space] 
    [SerializeField] private float collaspBound;
    [SerializeField] private int maxRoomAndCorridor;
    [Space]
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject doorPrefab;
    [Space]
    [SerializeField] private List<GameObject> hallWays;
    [SerializeField] private List<GameObject> rooms;
    [Space]
    
    [Header("Seed")]
    [Space]
    [SerializeField] private int minSeed;
    [SerializeField] private int maxSeed;
    [SerializeField] private int thisSeed;
    [Space]
    [SerializeField] private bool seedInserted;
    [SerializeField] private int wantedSeed;
    [Space]
    
    [Header("Used List")]
    [Space]
    [SerializeField] private List<GameObject> existed = new List<GameObject>();
    
    private List<GenerateObject> _validRooms= new List<GenerateObject>();
    private List<GenerateObject> _validHallWays= new List<GenerateObject>();
    
    private int  cullingDistance=80;
    
    private void Start()
    {
        maxRoomAndCorridor = Setting.DungeonSize;
        seedInserted = Setting.UseSeed;
        wantedSeed = Setting.Seed;
        
        //added seed
        if (seedInserted)
        {
            thisSeed = wantedSeed;
            Random.InitState(wantedSeed);
        }
        else
        {
            thisSeed = Random.Range(minSeed, maxSeed);
            Random.InitState(thisSeed);
        }
        
        //add first room which is the main to both needed list
        if (this.GetComponent<GenerateObject>() != null)
        {
            existed.Add(this.gameObject);
            _validRooms.Add(this.GetComponent<GenerateObject>());
        }
        else
        {
            Debug.Log("cant find room");
        }
        //start generate if not done
        if(!done) StartCoroutine(GenerateCoroutine());
        
    }

    // ReSharper disable Unity.PerformanceAnalysis
    // Dungeon generating
    private IEnumerator GenerateCoroutine()
    {   
        //run until big enough
        while (existed.Count < maxRoomAndCorridor-1)
        {   
            //handle halls
            StartCoroutine(GenerateHallways());
            yield return new WaitForSeconds((float)maxRoomAndCorridor/50);
            
            //handle rooms
            StartCoroutine(GenerateRooms());
            yield return new WaitForSeconds((float)maxRoomAndCorridor/50);
            
        }
        CloseAllDoor();
        yield return new WaitForSeconds((float)maxRoomAndCorridor/100);
        BakeNavMesh();
        yield return new WaitForSeconds((float)maxRoomAndCorridor/100);
        ScatterObjectsInRooms();
        
        yield return new WaitForSeconds((float)maxRoomAndCorridor/100);
        done = true;
        player.GetComponent<PlayerController>().inventoryOn = false;
        LoadingPanel.SetActive(false);
        yield return null;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator GenerateHallways()
    {
        //loop backward through all hallways that can be generated (to remove items)
        for (int i = _validRooms.Count - 1; i >= 0; i--)
        {
            //check
            if (existed.Count > maxRoomAndCorridor-1)
            {
                yield break;
            }
            int rand = 0;
            //pick random door and random room type 
            if (_validRooms.Count > 0)
            {
                rand = Random.Range(0, _validRooms[i].Doors.Count);
            }
            int randRoom =Random.Range(0, hallWays.Count);
            //boolean for door
            bool doorConnected = _validRooms[i].Doors[rand].GetComponent<Door>().connected;
            
            //loop until there is an unconnected door
            while (doorConnected)
            {
                rand = Random.Range(0, _validRooms[i].Doors.Count);
                doorConnected = _validRooms[i].Doors[rand].GetComponent<Door>().connected;
            }
            
            //spawn guarantee hall
            SpawnObject(_validRooms[i].Doors[rand],hallWays[randRoom],existed,_validHallWays);
            yield return new WaitForSeconds(0.1f);
            
            //spawn random hall
            if(_validRooms.Count>0)
            foreach (var door in _validRooms[i].Doors)
            {
                //check
                if (existed.Count > maxRoomAndCorridor-1)
                {
                    yield break;
                }
                //check door
                if (!door.GetComponent<Door>().connected)
                {   
                    //random pecentage
                    int spawn = Random.Range(0, 4);
                    int randSpawn =Random.Range(0, hallWays.Count);
                    if (spawn == 1)
                    {
                        SpawnObject(door,hallWays[randSpawn],existed,_validHallWays);
                    }
                    else
                    {
                        //spawn wall
                        Instantiate(wallPrefab,  door.transform.position,door.transform.rotation,this.transform);
                        door.GetComponent<Door>().connected = true;
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
            
            //remove pass room
            if(_validRooms.Count>0) _validRooms.RemoveAt(i);
            yield return null; // Yield control back to Unity for one frame
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator GenerateRooms()
    {
        //loop backward through all rooms that can be generated (to remove items)
        for (int i = _validHallWays.Count - 1; i >= 0; i--)
        {
            //check
            if (existed.Count > maxRoomAndCorridor-1)
            {
                yield break;
            }

            int rand = 0;
            //pick random door and random room type 
            if (_validHallWays.Count > 0)
            {
                rand = Random.Range(0, _validHallWays[i].Doors.Count);
            }
            int randRoom =Random.Range(0, rooms.Count);
            //boolean for door
            bool doorConnected = _validHallWays[i].Doors[rand].GetComponent<Door>().connected;
            
            //loop until there is an unconnected door
            while (doorConnected)
            {
                rand = Random.Range(0, _validHallWays[i].Doors.Count);
                doorConnected = _validHallWays[i].Doors[rand].GetComponent<Door>().connected;
            }
            
            //spawn guarantee room
            SpawnObject(_validHallWays[i].Doors[rand], rooms[randRoom], existed, _validRooms);

            yield return new WaitForSeconds(0.1f);
            
            //spawn random room
            if(_validHallWays.Count>0)
            foreach (var door in _validHallWays[i].Doors)
            {
                //check
                if (existed.Count > maxRoomAndCorridor-1)
                {
                    yield break;
                }
                //check door
                if (!door.GetComponent<Door>().connected)
                {
                    //random chance
                    int spawn = Random.Range(0, 2);
                    int randSpawn =Random.Range(0, rooms.Count);
                    
                    if (spawn == 1)
                    {
                        SpawnObject(door, rooms[randSpawn], existed, _validRooms);
                    }
                    else
                    {
                        //spawn wall
                        Instantiate(wallPrefab,  door.transform.position,door.transform.rotation,this.transform);
                        door.GetComponent<Door>().connected = true;
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
            
            //remove pass hallway if there is still choice 
            if(_validHallWays.Count>0) _validHallWays.RemoveAt(i);
            yield return null; // Yield control back to Unity for one frame
        }
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    // Spawn hallways and rooms
    void SpawnObject(GameObject door, GameObject prefab , List<GameObject> existedObj, List<GenerateObject> valid)
    {   
        //check door to connect
        door.GetComponent<Door>().connected = true;
        //variable
        var dposition = door.transform.position;
        float rotateangleY;
        float doorRotate;
        //instantiate prefab
        GameObject instantiate = Instantiate(prefab, dposition, door.transform.rotation,this.transform);
        
        List<GameObject> rDoor = instantiate.GetComponent<GenerateObject>().Doors;
        //choose door to start
        int randDoor = Random.Range(0, rDoor.Count);

        rDoor[randDoor].GetComponent<Door>().connected = true;
        
        //variables
        rotateangleY = door.transform.eulerAngles.y;
        doorRotate = rDoor[randDoor].transform.localEulerAngles.y;
        
        //check what angle to rotate(rotate base on door gameobject angle and door chosen to spawn on angle)
        if (doorRotate == 90f)
        {
            rotateangleY += 270;
        }
        else if (doorRotate == 270f)
        {
            rotateangleY += 90;
        }
        else if (doorRotate == 180)
        {
            rotateangleY += 180;
        }

        //rotate
        instantiate.transform.rotation = Quaternion.Euler(0, rotateangleY, 0);
        GameObject doorPre = Instantiate(doorPrefab, door.transform.position,door.transform.rotation,this.transform);
        //move to offset
        ChangePos(instantiate, dposition, rDoor[randDoor].transform.position);
        //check door connected
        rDoor[randDoor].GetComponent<Door>().connected = true;
        
        //check if the instantiate is overlap with any if yes destroy
        foreach (GameObject existing in existedObj)
        {
            if (CheckOverlap(instantiate.transform.position, existing.transform.position,collaspBound))
            {
                
                // Destroy the new room if it overlaps with an existing room
                Destroy(doorPre);
                Instantiate(wallPrefab, door.transform.position,door.transform.rotation,this.transform);
                Destroy(instantiate);
                return; // Exit the method early to avoid further processing
            }
        }
        
        //if not destroy add to needed lists
        if (instantiate != null)
        {
            valid.Add(instantiate.GetComponent<GenerateObject>());
            existedObj.Add(instantiate);
        }
    }

    // Close all unconnected door after generating
    void CloseAllDoor()
    {
       
        //close rooms
        foreach (var room in _validRooms)
        {
            foreach (var door in room.Doors)
            {
                if (!door.GetComponent<Door>().connected)
                {
                    door.GetComponent<Door>().connected = true;
                    GameObject wall = Instantiate(wallPrefab, door.transform.position,door.transform.rotation,this.transform);
                }
            }
        }
        //close hallways
        foreach (var hallWay in _validHallWays)
        {
            foreach (var door in hallWay.Doors)
            {
                if (!door.GetComponent<Door>().connected)
                {
                    door.GetComponent<Door>().connected = true;
                    GameObject wall = Instantiate(wallPrefab, door.transform.position,door.transform.rotation,this.transform);
                }
            }
        }
        //clear unuse list
        _validHallWays.Clear();
        _validRooms.Clear();
    }
    
    // Change room and hallway position 
    void ChangePos(GameObject instantiate, Vector3 spawnDoor, Vector3 chosenDoorPos)
    {
        //variables
        var position = instantiate.transform.position;

        // Move the room to align the chosen door with the entrance position
        Vector3 offset = spawnDoor - chosenDoorPos;
        //Debug.Log(offset);
        position += offset;
        //position = rotation* position;
        instantiate.transform.position = position; 
        
    }
    
    // Check overlap with distance
    bool CheckOverlap(Vector3 a, Vector3 b, float distance)
    {
        //return distance between to object
        return Vector3.Distance(a, b) < distance;
    }
    
    // Scatter object randomly
    void ScatterObjectsInRooms()
    {
        foreach (GameObject exist in existed)
        {
            //determine the number of objects for this room
            int numberOfObjects = Random.Range(minObjPerRoom, maxObjPerRoom + 1); 

            //scatter objects in each room
            for (int i = 0; i < numberOfObjects; i++)
            {
                Vector3 randomPosition = GetRandomPositionInRoom(exist.transform.position);
                RaycastHit hit;

                //perform a raycast to check if the position is valid
                if (Physics.Raycast(randomPosition + Vector3.up * 10f, Vector3.down, out hit,Mathf.Infinity,GroundToScatter))
                {
                    //if the ray hits something, instantiate a random object at the hit point
                    GameObject obj=Instantiate(objToScatter[Random.Range(0, objToScatter.Count)], hit.point, Quaternion.identity);

                    if (obj.GetComponent<ItemObject>() != null)
                    {
                        obj.GetComponent<ItemObject>().amount = 1;
                    }
                }
            }
        }
    }
    
    
    Vector3 GetRandomPositionInRoom(Vector3 roomPosition)
    {
        //define the size of the scatter area within the room
        Vector3 scatterAreaSize = new Vector3(9f, 2f, 9f);

        //get a random position within the defined scatter area of the room
        Vector3 randomPosition = new Vector3(
            Random.Range(roomPosition.x - scatterAreaSize.x / 2, roomPosition.x + scatterAreaSize.x / 2),
            roomPosition.y, //ensure that objects are scattered at the floor level
            Random.Range(roomPosition.z - scatterAreaSize.z / 2, roomPosition.z + scatterAreaSize.z / 2)
        );

        return randomPosition;
    }
    
    //bake nav mesh
    void BakeNavMesh()
    {
        GetComponent<NavMeshSurface>().BuildNavMesh();
    }
    
    private void Update()
    {
        if (done)
        {
            manualCulling();
        }
    }

    void manualCulling()
    {
        
        MeshRenderer[] activeRenderer = GameObject.FindObjectsOfType<MeshRenderer>();
        foreach (var renderer in activeRenderer)
        {
            if (Vector3.Distance(renderer.transform.position, player.transform.position)>cullingDistance)
            {
                renderer.enabled = false;
            }
            else
            {
                renderer.enabled = true;
            }
        }
        
    }
}
