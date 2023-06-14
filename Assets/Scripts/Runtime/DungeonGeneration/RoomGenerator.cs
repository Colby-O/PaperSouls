using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaperSouls.Runtime.DungeonGeneration
{
    public class RoomGenerator : MonoBehaviour
    {
        public int seed;
        [Min(0)] public Vector2Int roomSize;
        [Min(1)] public Vector2Int minSubRoomSize;
        [Min(0)] public int numEnterences;
        [Range(0, 1)] public float proabilityForSplit = 0.5f;
        [Min(1)] public int test = 1;

        public List<DungeonAsset> wallObjects;
        public List<DungeonAsset> enterenceObjects;
        public List<DungeonAsset> floorObjects;
        public List<DungeonAsset> pillarObjects;

        private RoomTileType[,] grid;
        private Vector2 wallCount;
        private float wallSize;
        private Vector2 floorSize;
        private Vector2 floorCount;
        private List<GameObject> roomObjects;
        GameObject parnet;
        GameObject mesh;
        GameObject exits;

        /// <summary>
        /// Defines a generic dungeon asset
        /// </summary>
        [System.Serializable]
        public class DungeonAsset
        {
            public GameObject gameObject;
            [Range(0, 1)] public float proability = 1;

            public DungeonAsset(GameObject gameObject, float proability)
            {
                this.gameObject = gameObject;
                this.proability = proability;
            }
        }

        /// <summary>
        /// Defines room Tile Types
        /// </summary>
        public enum RoomTileType
        {
            Empty,
            Wall
        }

        /// <summary>
        /// Fetches the generate the locations of the room exits
        /// </summary>
        List<int> GetExterenceLocations(Vector2 wallCount)
        {
            List<int> enterenceIndices = new();

            if (numEnterences > (int)(2 * wallCount.x + 2 * wallCount.y)) numEnterences = (int)(2 * wallCount.x + 2 * wallCount.y);

            for (int i = 0; i < numEnterences; i++)
            {
                int rand = Random.Range(-2 * (int)wallCount.x, 2 * (int)wallCount.y + 1);

                while (rand == 0 || enterenceIndices.Contains(rand)) rand = Random.Range(-2 * (int)wallCount.x, 2 * (int)wallCount.y + 1);

                enterenceIndices.Add(rand);
            }

            return enterenceIndices;
        }

        /// <summary>
        /// Pick a random asset from a list
        /// </summary>
        DungeonAsset GetRandomAsset(List<DungeonAsset> objs)
        {
            bool assetFound = false;
            DungeonAsset obj = null;
            do
            {
                int rand = Random.Range(0, objs.Count);
                float prob = Random.value;

                if (prob <= objs[rand].proability)
                {
                    assetFound = true;
                    obj = objs[rand];
                }

            } while (!assetFound);

            return obj;
        }

        /// <summary>
        /// Create dungeon walls
        /// </summary>
        void CreateWalls()
        {
            wallCount = new(Mathf.Max(1, Mathf.FloorToInt(roomSize.x / wallSize)), Mathf.Max(1, Mathf.FloorToInt(roomSize.y / wallSize)));
            Vector2 scale = new((roomSize.x / wallCount.x) / wallSize, (roomSize.y / wallCount.y) / wallSize);

            List<int> enterenceIndices = GetExterenceLocations(wallCount);

            for (int i = 0; i < wallCount.x; i++)
            {
                Vector3 positionRight = transform.position + new Vector3(-roomSize.x / 2f + wallSize * scale.x / 2 + i * scale.x * wallSize, 0, roomSize.y / 2f);
                Vector3 positionLeft = transform.position + new Vector3(-roomSize.x / 2f + wallSize * scale.x / 2 + i * scale.x * wallSize, 0, -roomSize.y / 2f);

                Quaternion rotation = transform.rotation;
                Vector3 scaleVector = new(scale.x, 1, 1);

                DungeonAsset leftWallAsset = (enterenceIndices.Contains(-i - 1)) ? GetRandomAsset(enterenceObjects) : GetRandomAsset(wallObjects);
                DungeonAsset rightWallAsset = (enterenceIndices.Contains(-(int)wallCount.x - i - 1)) ? GetRandomAsset(enterenceObjects) : GetRandomAsset(wallObjects);

                if (enterenceIndices.Contains(-i - 1))
                {
                    GameObject exit = new("exit" + (exits.transform.childCount + 1));
                    exit.transform.position = positionRight;
                    exit.transform.parent = exits.transform;
                }

                if (enterenceIndices.Contains(-(int)wallCount.x - i - 1))
                {
                    GameObject exit = new("exit" + (exits.transform.childCount + 1));
                    exit.transform.position = positionLeft;
                    exit.transform.parent = exits.transform;
                }

                GameObject wallRight = GameObject.Instantiate(leftWallAsset.gameObject, positionRight, rotation * Quaternion.Euler(0f, 180f, 0f), mesh.transform);
                GameObject wallLeft = GameObject.Instantiate(rightWallAsset.gameObject, positionLeft, rotation * Quaternion.Euler(0f, 0f, 0f), mesh.transform);

                wallRight.transform.localScale = scaleVector;
                wallLeft.transform.localScale = scaleVector;

                roomObjects.Add(wallRight);
                roomObjects.Add(wallLeft);
            }

            for (int i = 0; i < wallCount.y; i++)
            {
                Vector3 positionDown = transform.position + new Vector3(roomSize.x / 2f, 0, -roomSize.y / 2f + wallSize * scale.y / 2f + i * scale.y * wallSize);
                Vector3 positionUp = transform.position + new Vector3(-roomSize.x / 2f, 0, -roomSize.y / 2f + wallSize * scale.y / 2f + i * scale.y * wallSize);

                Quaternion rotation = transform.rotation;
                Vector3 scaleVector = new(scale.y, 1, 1);

                DungeonAsset upWallAsset = (enterenceIndices.Contains(i + 1)) ? GetRandomAsset(enterenceObjects) : GetRandomAsset(wallObjects);
                DungeonAsset downWallAsset = (enterenceIndices.Contains((int)wallCount.y + i + 1)) ? GetRandomAsset(enterenceObjects) : GetRandomAsset(wallObjects);

                if (enterenceIndices.Contains(i + 1))
                {
                    GameObject exit = new("exit" + (exits.transform.childCount + 1));
                    exit.transform.position = positionDown;
                    exit.transform.parent = exits.transform;
                }

                if (enterenceIndices.Contains((int)wallCount.y + i + 1))
                {
                    GameObject exit = new("exit" + (exits.transform.childCount + 1));
                    exit.transform.position = positionUp;
                    exit.transform.parent = exits.transform;
                }

                GameObject wallDown = GameObject.Instantiate(upWallAsset.gameObject, positionDown, rotation * Quaternion.Euler(0f, 270f, 0f), mesh.transform);
                GameObject wallUp = GameObject.Instantiate(downWallAsset.gameObject, positionUp, rotation * Quaternion.Euler(0f, 90f, 0f), mesh.transform);

                wallDown.transform.localScale = scaleVector;
                wallUp.transform.localScale = scaleVector;

                roomObjects.Add(wallDown);
                roomObjects.Add(wallUp);
            }
        }

        /// <summary>
        /// Create dungeon floors
        /// </summary>
        void CreateFloors()
        {
            floorCount = new(Mathf.Max(1, Mathf.FloorToInt(roomSize.x / floorSize.x)), Mathf.Max(1, Mathf.FloorToInt(roomSize.y / floorSize.y)));
            Vector2 scale = new((roomSize.x / floorCount.x) / floorSize.x, (roomSize.y / floorCount.y) / floorSize.y);

            for (int i = 0; i < floorCount.x; i++)
            {
                for (int j = 0; j < floorCount.y; j++)
                {
                    Vector3 position = transform.position + new Vector3(-roomSize.x / 2f + floorSize.x * scale.x / 2 + i * scale.x * floorSize.x, 0, -roomSize.y / 2f + floorSize.y * scale.y / 2f + j * scale.y * floorSize.y);
                    Quaternion rotation = transform.rotation;

                    int rand = Random.Range(0, 4);
                    float rotate = 0;

                    if (rand == 0) rotate = 0;
                    else if (rand == 1) rotate = 90;
                    else if (rand == 2) rotate = 180;
                    else rotate = 270;

                    Vector3 scaleVector;
                    if (rand == 0 || rand == 2) scaleVector = new(scale.x, 1, scale.y);
                    else scaleVector = new(scale.y, 1, scale.x);

                    DungeonAsset floorObject = GetRandomAsset(floorObjects);

                    GameObject floor = GameObject.Instantiate(floorObject.gameObject, position, rotation * Quaternion.Euler(0f, rotate, 0f), mesh.transform);
                    floor.transform.localScale = scaleVector;
                    roomObjects.Add(floor);
                }
            }
        }

        /// <summary>
        /// Create dungeon pillar
        /// </summary>
        void CreatePillar()
        {
            if (pillarObjects.Count <= 0) return;

            DungeonAsset pillar = GetRandomAsset(pillarObjects);
            GameObject piller1 = GameObject.Instantiate(pillar.gameObject, new Vector3(-roomSize.x / 2, 0, -roomSize.y / 2), transform.rotation, mesh.transform);
            GameObject piller2 = GameObject.Instantiate(pillar.gameObject, new Vector3(-roomSize.x / 2, 0, roomSize.y / 2), transform.rotation, mesh.transform);
            GameObject piller3 = GameObject.Instantiate(pillar.gameObject, new Vector3(roomSize.x / 2, 0, -roomSize.y / 2), transform.rotation, mesh.transform);
            GameObject piller4 = GameObject.Instantiate(pillar.gameObject, new Vector3(roomSize.x / 2, 0, roomSize.y / 2), transform.rotation, mesh.transform);

            roomObjects.Add(piller1);
            roomObjects.Add(piller2);
            roomObjects.Add(piller3);
            roomObjects.Add(piller4);
        }

        /// <summary>
        /// Vertival split for BSP
        /// </summary>
        private void SplitRoomVertically(Bounds room, Queue<Bounds> roomsQueue, int minWidth)
        {
            //int xSplit = Mathf.Max(minWidth, room.size.x - minWidth);
            float xSplit = Random.Range(1, room.size.x);

            float centerOffsetRoom1 = room.center.x - (room.size.x - xSplit) / 2f;
            float centerOffsetRoom2 = room.center.x + xSplit / 2f;

            Bounds room1 = new(new Vector3(centerOffsetRoom1, room.center.y, room.center.z), new Vector3(xSplit, room.size.y, room.size.z));
            Bounds room2 = new(new Vector3(centerOffsetRoom2, room.center.y, room.center.z), new Vector3(room.size.x - xSplit, room.size.y, room.size.z));

            roomsQueue.Enqueue(room1);
            roomsQueue.Enqueue(room2);
        }

        /// <summary>
        /// Horizontally split for BSP
        /// </summary>
        private void SplitRoomHorizontally(Bounds room, Queue<Bounds> roomsQueue, int minHeight)
        {
            //int ySplit = Mathf.Max(minHeight, room.size.y - minHeight);
            float ySplit = Random.Range(1, room.size.y);

            float centerOffsetRoom1 = room.center.y - (room.size.y - ySplit) / 2f;
            float centerOffsetRoom2 = room.center.y + ySplit / 2f;

            Bounds room1 = new(new Vector3(room.center.x, centerOffsetRoom1, room.center.z), new Vector3(room.size.x, ySplit, room.size.z));
            Bounds room2 = new(new Vector3(room.center.x, centerOffsetRoom2, room.center.z), new Vector3(room.size.x, room.size.y - ySplit, room.size.z));

            roomsQueue.Enqueue(room1);
            roomsQueue.Enqueue(room2);
        }

        /// <summary>
        /// Uses Binary Space Partitioning (BSP) to partition a room
        /// </summary>
        private List<Bounds> BinarySpacePartitioning(Bounds mainRoom, int minWidth, int minHeight)
        {
            Queue<Bounds> roomsQueue = new();
            List<Bounds> subRooms = new();

            roomsQueue.Enqueue(mainRoom);

            while (roomsQueue.Count > 0)
            {
                Bounds room = roomsQueue.Dequeue();

                if (room.size.x < minWidth || room.size.y < minHeight) continue;

                if (Random.value < 0.5f)
                {
                    if (room.size.y >= 2 * minHeight) SplitRoomHorizontally(room, roomsQueue, minHeight);
                    else if (room.size.x >= 2 * minWidth) SplitRoomVertically(room, roomsQueue, minWidth);
                    else if (room.size.x >= minWidth && room.size.y >= minHeight) subRooms.Add(room);
                }
                else
                {
                    if (room.size.x >= 2 * minWidth) SplitRoomVertically(room, roomsQueue, minWidth);
                    else if (room.size.y >= 2 * minHeight) SplitRoomHorizontally(room, roomsQueue, minHeight);
                    else if (room.size.x >= minWidth && room.size.y >= minHeight) subRooms.Add(room);
                }
            }

            for (int i = subRooms.Count - 1; i > -1; i--)
            {
                if (Random.value >= proabilityForSplit) subRooms.RemoveAt(i);
            }

            return subRooms;
        }

        /// <summary>
        /// Adds a Wall to the Room Grid
        /// </summary>
        List<Vector2Int> AddWallToGrid(int start, int end)
        {
            List<Vector2Int> wallSegments = new();
            int cursor = -1;
            for (int i = start; i < end; i++)
            {
                if (grid[i, i + 1] == RoomTileType.Empty && cursor == -1) cursor = i;
                else if (grid[i, i + 1] == RoomTileType.Wall && cursor != -1)
                {
                    wallSegments.Add(new Vector2Int(cursor, i));
                    cursor = -1;
                }
                else if (i == end - 1 && grid[i, i + 1] == RoomTileType.Empty && cursor != -1) wallSegments.Add(new Vector2Int(cursor, end));
            }

            return wallSegments;
        }

        /// <summary>
        /// Generate sub rooms
        /// </summary>
        void CreateSubRooms(List<Bounds> subRooms)
        {
            foreach (Bounds room in subRooms)
            {
                Vector3 min = new(room.min.x, 0, room.min.y);
                Vector3 max = new(room.max.x, 0, room.max.y);

                Debug.DrawLine(max - new Vector3(0, 0, room.size.y), min, Color.green, 1);
                Debug.DrawLine(max - new Vector3(room.size.x, 0, 0), min, Color.green, 1);
                Debug.DrawLine(max, min + new Vector3(0, 0, room.size.y), Color.green, 1);
                Debug.DrawLine(max, min + new Vector3(room.size.x, 0, 0), Color.green, 1);

                wallCount = new(Mathf.Max(1, Mathf.FloorToInt(room.size.x / wallSize)), Mathf.Max(1, Mathf.FloorToInt(room.size.y / wallSize)));
                Vector2 scale = new((room.size.x / wallCount.x) / wallSize, (room.size.y / wallCount.y) / wallSize);

                List<DungeonAsset> subRoomObjects = new(wallObjects);
                subRoomObjects.AddRange(enterenceObjects);

                for (int i = 0; i < wallCount.x; i++)
                {
                    Vector3 positionRight = new Vector3(room.center.x, 0, room.center.y) + new Vector3(-room.size.x / 2f + wallSize * scale.x / 2 + i * scale.x * wallSize, 0, room.size.y / 2);
                    Vector3 positionLeft = new Vector3(room.center.x, 0, room.center.y) + new Vector3(-room.size.x / 2f + wallSize * scale.x / 2 + i * scale.x * wallSize, 0, -room.size.y / 2);

                    Quaternion rotation = transform.rotation;
                    Vector3 scaleVector = new(scale.x, 1, 1);

                    DungeonAsset leftWallAsset = GetRandomAsset(subRoomObjects);
                    DungeonAsset rightWallAsset = GetRandomAsset(subRoomObjects);

                    GameObject wallRight = GameObject.Instantiate(leftWallAsset.gameObject, positionRight, rotation * Quaternion.Euler(0f, 180f, 0f), mesh.transform);
                    GameObject wallLeft = GameObject.Instantiate(rightWallAsset.gameObject, positionLeft, rotation * Quaternion.Euler(0f, 0f, 0f), mesh.transform);

                    wallRight.transform.localScale = scaleVector;
                    wallLeft.transform.localScale = scaleVector;

                    if (Mathf.Abs(room.size.y / 2 + room.center.y - roomSize.y / 2) >= 1) roomObjects.Add(wallRight);
                    else GameObject.Destroy(wallRight);
                    if (Mathf.Abs(-room.size.y / 2 + room.center.y + roomSize.y / 2) >= 1) roomObjects.Add(wallLeft);
                    else GameObject.Destroy(wallLeft);
                }

                for (int i = 0; i < wallCount.y; i++)
                {
                    Vector3 positionDown = new Vector3(room.center.x, 0, room.center.y) + new Vector3(room.size.x / 2f, 0, -room.size.y / 2f + wallSize * scale.y / 2f + i * scale.y * wallSize);
                    Vector3 positionUp = new Vector3(room.center.x, 0, room.center.y) + new Vector3(-room.size.x / 2f, 0, -room.size.y / 2f + wallSize * scale.y / 2f + i * scale.y * wallSize);

                    Quaternion rotation = transform.rotation;
                    Vector3 scaleVector = new(scale.y, 1, 1);

                    DungeonAsset upWallAsset = GetRandomAsset(subRoomObjects);
                    DungeonAsset downWallAsset = GetRandomAsset(subRoomObjects);

                    GameObject wallDown = GameObject.Instantiate(upWallAsset.gameObject, positionDown, rotation * Quaternion.Euler(0f, 270f, 0f), mesh.transform);
                    GameObject wallUp = GameObject.Instantiate(downWallAsset.gameObject, positionUp, rotation * Quaternion.Euler(0f, 90f, 0f), mesh.transform);

                    wallDown.transform.localScale = scaleVector;
                    wallUp.transform.localScale = scaleVector;

                    if (Mathf.Abs(room.size.x / 2 + room.center.x - roomSize.x / 2) >= 1) roomObjects.Add(wallDown);
                    else GameObject.Destroy(wallDown);
                    if (Mathf.Abs(-room.size.x / 2 + room.center.x + roomSize.x / 2) >= 1) roomObjects.Add(wallUp);
                    else GameObject.Destroy(wallUp);
                }

            }
        }

        /// <summary>
        /// Creates a room
        /// </summary>
        void CreateRoom()
        {
            Random.InitState(seed);
            grid = new RoomTileType[roomSize.x, roomSize.y];

            for (int i = 0; i < roomSize.x; i++)
            {
                for (int j = 0; j < roomSize.y; j++)
                {
                    grid[i, j] = RoomTileType.Empty;
                }
            }

            roomObjects = new();
            parnet = new("room");
            mesh = new("mesh");
            exits = new("exits");
            mesh.transform.parent = parnet.transform;
            exits.transform.parent = parnet.transform;
            wallSize = Mathf.Max(wallObjects[0].gameObject.GetComponent<MeshRenderer>().bounds.size.x, wallObjects[0].gameObject.GetComponent<MeshRenderer>().bounds.size.z);
            floorSize = new Vector2(floorObjects[0].gameObject.GetComponent<MeshRenderer>().bounds.size.x, floorObjects[0].gameObject.GetComponent<MeshRenderer>().bounds.size.z);
            CreateWalls();
            CreateFloors();
            List<Bounds> subRooms = BinarySpacePartitioning(new(new Vector3(transform.position.x, transform.position.z, 0), new Vector3(roomSize.x, roomSize.y, 0)), minSubRoomSize.x, minSubRoomSize.y);
            CreateSubRooms(subRooms);
            CreatePillar();
        }

        /// <summary>
        /// CDestories a room
        /// </summary>
        void DestoryRoom()
        {
            foreach (GameObject obj in roomObjects)
            {
                GameObject.Destroy(obj);
            }

            GameObject.Destroy(parnet);
        }

        [ExecuteInEditMode]
        void OnValidate()
        {
            //minSubRoomSizePercentage.x = Mathf.Clamp01(minSubRoomSizePercentage.x);
            //minSubRoomSizePercentage.y = Mathf.Clamp01(minSubRoomSizePercentage.y);
        }

        void Start()
        {
            CreateRoom();
        }

        void Update()
        {
            DestoryRoom();
            CreateRoom();
        }
    }
}
