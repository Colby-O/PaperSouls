using System.Collections;
using System.Collections.Generic;
using Utils;
using UnityEngine;
using PaperSouls.Runtime.Helpers;

namespace PaperSouls.Runtime.DungeonGeneration
{
    /// <summary>
    /// Define what infomation lies on a grid tile
    /// </summary>
    public enum TileType : int
    {
        EMPTY = 0,
        ROOM = 1,
        HALLWAY = 2,
        HALLWAY_AND_ROOM = 3,
        ROOM_SPACING = 4
    }

    /// <summary>
    /// Defines the tile weights
    /// </summary>
    [System.Serializable]
    public class TileWeights
    {
        const int INF = 1000000;
        public float EMPTY = 10;
        public float ROOM = INF;
        public float HALLWAY = 5;
        public float HALLWAY_AND_ROOM = INF;
        public float ROOM_SPACING = 20;
        public float TURN_PENAILITY = 3;
    }

    public class DungeonGenerator : MonoBehaviour
    {
        public int gridSize = 50;
        public float tileSize = 30;
        public int maxNumberOfRooms = 15;
        public int minNumberOfRooms = 5;
        public int maxRoomSize = 10;
        public int minRoomSize = 5;
        public int minRoomSpacing = 2;
        public int hallwaySize = 1;
        public int hallwaySpacing = 1;
        [Range(0, 1)]
        public float loopProabilty = 0.2f;
        public bool regenerateDungeon = false;
        public bool useShortestPath = true;
        public bool useRandomRoomSizes = false;
        public bool allowGridExtensions = true;
        public string exitCompoentName = "Exits";
        public GameObject dungeonHolder;
        public List<RoomType> rawRoomTypes;
        public List<Hallway> rawHallwayTypes;
        public TileWeights tileWeights;

        [Header("Debug")]
        public bool drawDelaunayTriangulation = false;
        public bool drawMST = false;
        public bool drawFinalGraph = false;
        public bool drawHallwayPaths = false;

        private int gridExtension = 50;
        [SerializeField] private List<RoomType> roomTypes;
        [SerializeField] private List<RoomType> terminalRoomTypes;
        private TileType[,] grid;
        private DelaunayTriangulation delaunayTriangulation;
        private MST mst;
        private float[,] adjacencyMatrix;
        [SerializeField] private List<Room> roomList;

        public GameObject strightHallway;
        public GameObject curvedHallway;
        public GameObject threeWayHallway;
        public GameObject fourWayHallway;
        public GameObject enterenceHallway;

        [SerializeField] public Hallway[] hallwayTypeList;
        [SerializeField] private List<Hallway> hallwayList;
        private readonly int maxNumberOfRoomPlacementTries = 50;
        Dictionary<int, List<int>> mstEdges;
        private Vector3 maxSize;
        private Vector3 maxRoomScale;
        private int numberOfMainRooms;
        private List<Dictionary<Vector2Int, Vector2Int>> paths;
        private List<Vector2Int> pathStarts;
        private List<Vector2Int> pathEnds;
        [SerializeField] private int startRoomID;

        private static readonly Vector2Int[] DIRECTIONS = new[]
        {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0)
        };

        private readonly int roomBuffer = 0;

        /// <summary>
        /// Defines the a Hallway
        /// </summary>
        [System.Serializable]
        public class Hallway
        {
            public string name;
            public int id;
            public GameObject gameObject;
            public Vector3 size;

            public Hallway(GameObject gameObject, int id, string name)
            {
                this.id = id;
                this.name = name;
                this.gameObject = gameObject;
            }

            public Hallway(int id, string name)
            {
                this.id = id;
                this.name = name;
            }

            public Hallway(GameObject gameObject)
            {
                this.gameObject = gameObject;
            }
        }

        /// <summary>
        /// Defines the a Room Type
        /// </summary>
        [System.Serializable]
        public struct RoomType
        {
            public GameObject gameObject;
            [Range(0, 1)]
            public float proability;
            private Room room;

            /// <summary>
            /// Sets the game object
            /// </summary>
            public void SetGameObject(GameObject gameObject)
            {
                this.gameObject = gameObject;
            }

            /// <summary>
            /// Sets the spawn proability 
            /// </summary>
            public void SetProability(float proability)
            {
                this.proability = proability;
            }

            /// <summary>
            /// Sets the room
            /// </summary>
            public void SetRoom(Room room)
            {
                this.room = room;
            }

            /// <summary>
            /// Gets the current room
            /// </summary>
            public Room GetRoom()
            {
                return this.room;
            }
        }

        /// <summary>
        /// Defines the a room
        /// </summary>
        [System.Serializable]
        public class Room
        {
            public int roomID;
            public int numExits;
            public Vector3 size;
            public GameObject gameObject;
            public List<Transform> exits;
            public Stack<Transform> availableExits;
            public int exitsUsed = 0;
            public Room(GameObject gameObject, List<Transform> exits, Vector3 size, int roomID)
            {
                this.exits = new List<Transform>(exits);
                this.numExits = exits.Count;
                this.gameObject = gameObject;
                this.roomID = roomID;
                this.size = size;
                this.availableExits = new Stack<Transform>(exits);
            }

            public Room(int roomID)
            {
                this.numExits = 0;
                this.gameObject = null;
                this.roomID = roomID;
                this.size = new(0, 0, 0);
                this.exits = null;
                this.availableExits = null;
            }

            /// <summary>
            /// Sets the size of the room
            /// </summary>
            public void SetSize(Vector3 size)
            {
                this.size = size;
            }

            /// <summary>
            /// Sets the room object
            /// </summary>
            public void SetGameObject(GameObject gameObject)
            {
                this.gameObject = gameObject;
            }

            /// <summary>
            /// Sets the rooms exits
            /// </summary>
            public void SetExits(List<Transform> exits)
            {
                this.exits = new List<Transform>(exits);
                this.numExits = exits.Count;
                this.availableExits = new Stack<Transform>(exits);
            }
        }

        /// <summary>
        /// Gets the size of a room in world space 
        /// </summary>
        Vector3 GetSizeOfRoom(GameObject room)
        {
            Vector3 center = Vector3.zero;
            int childCount = 0;

            foreach (Transform child in room.transform)
            {
                foreach (Transform grandChild in child)
                {
                    Renderer renderer = grandChild.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        center += renderer.bounds.center;
                        childCount += 1;
                    }
                }
            }

            center /= childCount;


            Bounds bounds = new(center, Vector3.zero);

            foreach (Transform child in room.transform)
            {
                foreach (Transform grandChild in child)
                {
                    Renderer renderer = grandChild.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        bounds.Encapsulate(renderer.bounds);
                    }
                }
            }

            return bounds.size;
        }

        /// <summary>
        /// Gets the numbers of exits a room prefab has
        /// </summary>
        List<Transform> GetRoomExits(GameObject roomObject)
        {
            List<Transform> exits = new();
            foreach (Transform child in roomObject.transform)
            {
                if (child.name == exitCompoentName)
                {
                    foreach (Transform exit in child)
                    {
                        exits.Add(exit);
                    }
                }
            }
            return exits;
        }

        /// <summary>
        /// Generate hallway varients for tilemap
        /// </summary>
        void GenerateHallwayVarients()
        {
            rawHallwayTypes = new();

            GameObject verticalHallway = Instantiate(strightHallway, strightHallway.transform.position, strightHallway.transform.rotation * Quaternion.Euler(0f, 0f, 0f));
            GameObject horzitionalHallway = Instantiate(strightHallway, strightHallway.transform.position, strightHallway.transform.rotation * Quaternion.Euler(0f, 90f, 0f));

            GameObject enterenceHallwayLeft = Instantiate(enterenceHallway, enterenceHallway.transform.position, enterenceHallway.transform.rotation * Quaternion.Euler(0f, 270f, 0f));
            GameObject enterenceHallwayRight = Instantiate(enterenceHallway, enterenceHallway.transform.position, enterenceHallway.transform.rotation * Quaternion.Euler(0f, 90f, 0f));
            GameObject enterenceHallwayUp = Instantiate(enterenceHallway, enterenceHallway.transform.position, enterenceHallway.transform.rotation * Quaternion.Euler(0f, 180f, 0f));
            GameObject enterenceHallwayDown = Instantiate(enterenceHallway, enterenceHallway.transform.position, enterenceHallway.transform.rotation * Quaternion.Euler(0f, 0f, 0f));


            GameObject curvedHallwayDownRight = Instantiate(curvedHallway, curvedHallway.transform.position, curvedHallway.transform.rotation * Quaternion.Euler(0f, 0f, 0f));
            GameObject curvedHallwayDownLeft = Instantiate(curvedHallway, curvedHallway.transform.position, curvedHallway.transform.rotation * Quaternion.Euler(0f, 90f, 0f));
            GameObject curvedHallwayUpRight = Instantiate(curvedHallway, curvedHallway.transform.position, curvedHallway.transform.rotation * Quaternion.Euler(0f, 270f, 0f));
            GameObject curvedHallwayUpLeft = Instantiate(curvedHallway, curvedHallway.transform.position, curvedHallway.transform.rotation * Quaternion.Euler(0f, 180f, 0f));

            GameObject threeWayHallwayUp = Instantiate(threeWayHallway, threeWayHallway.transform.position, threeWayHallway.transform.rotation * Quaternion.Euler(0f, 0f, 0f));
            GameObject threeWayHallwayDown = Instantiate(threeWayHallway, threeWayHallway.transform.position, threeWayHallway.transform.rotation * Quaternion.Euler(0f, 180f, 0f));
            GameObject threeWayHallwayLeft = Instantiate(threeWayHallway, threeWayHallway.transform.position, threeWayHallway.transform.rotation * Quaternion.Euler(0f, 270f, 0f));
            GameObject threeWayHallwayRight = Instantiate(threeWayHallway, threeWayHallway.transform.position, threeWayHallway.transform.rotation * Quaternion.Euler(0f, 90f, 0f));

            GameObject fourwayHallway = Instantiate(this.fourWayHallway, this.fourWayHallway.transform.position, this.fourWayHallway.transform.rotation * Quaternion.Euler(0f, 0f, 0f));


            rawHallwayTypes.Add(new Hallway(null, 0, "Invailed"));
            rawHallwayTypes.Add(new Hallway(enterenceHallwayUp, 1, ""));
            rawHallwayTypes.Add(new Hallway(enterenceHallwayRight, 2, ""));
            rawHallwayTypes.Add(new Hallway(curvedHallwayDownLeft, 3, ""));
            rawHallwayTypes.Add(new Hallway(enterenceHallwayDown, 4, ""));
            rawHallwayTypes.Add(new Hallway(verticalHallway, 5, ""));
            rawHallwayTypes.Add(new Hallway(curvedHallwayUpLeft, 6, ""));
            rawHallwayTypes.Add(new Hallway(threeWayHallwayUp, 7, ""));
            rawHallwayTypes.Add(new Hallway(enterenceHallwayLeft, 8, ""));
            rawHallwayTypes.Add(new Hallway(curvedHallwayDownRight, 9, ""));
            rawHallwayTypes.Add(new Hallway(horzitionalHallway, 10, ""));
            rawHallwayTypes.Add(new Hallway(threeWayHallwayLeft, 11, ""));
            rawHallwayTypes.Add(new Hallway(curvedHallwayUpRight, 12, ""));
            rawHallwayTypes.Add(new Hallway(threeWayHallwayDown, 13, ""));
            rawHallwayTypes.Add(new Hallway(threeWayHallwayRight, 14, ""));
            rawHallwayTypes.Add(new Hallway(fourwayHallway, 15, ""));

        }

        /// <summary>
        /// Sets up hallway and room prefabs
        /// </summary>
        void InitailzePrefabs()
        {
            roomTypes = new();
            terminalRoomTypes = new();

            maxSize = new(0, 0, 0);
            maxRoomScale = new(0, 0, 0);
            foreach (RoomType roomType in rawRoomTypes)
            {
                GameObject roomPrefab = roomType.gameObject;

                Vector3 size = GetSizeOfRoom(roomPrefab);

                if (maxRoomScale.x < size.x) maxRoomScale.x = size.x;
                if (maxRoomScale.y < size.y) maxRoomScale.y = size.y;
                if (maxRoomScale.z < size.z) maxRoomScale.z = size.z;

                List<Transform> exits = GetRoomExits(roomPrefab);

                Room room = new(-1);
                room.SetGameObject(roomPrefab);
                room.SetSize(size);
                room.SetExits(exits);

                roomType.SetRoom(room);

                if (exits.Count > 1) roomTypes.Add(roomType);
                else if (exits.Count > 0) terminalRoomTypes.Add(roomType);
            }

            foreach (Hallway hallwayType in rawHallwayTypes)
            {
                if (hallwayType.gameObject == null) continue;

                GameObject roomPrefab = hallwayType.gameObject;

                Vector3 size = GetSizeOfRoom(roomPrefab);

                //if (maxSize.x < size.x) maxSize.x = size.x;
                //if (maxSize.y < size.y) maxSize.y = size.y;
                //if (maxSize.z < size.z) maxSize.z = size.z;

                hallwayType.size = size;
            }

        }

        /// <summary>
        /// General Initailze Steps
        /// </summary>
        void InitailzeDungeon()
        {
            if (dungeonHolder == null) dungeonHolder = new GameObject("DungeonHolder");
            grid = new TileType[gridSize, gridSize];
            roomList = new();
            paths = new();
            pathStarts = new();
            pathEnds = new();
            hallwayTypeList = new Hallway[rawHallwayTypes.Count];

            for (int i = 0; i < rawHallwayTypes.Count; i++)
            {
                hallwayTypeList[i] = rawHallwayTypes[i];
            }

            for (int j = 0; j < gridSize; j++)
            {
                for (int k = 0; k < gridSize; k++)
                {
                    grid[j, k] = TileType.EMPTY;
                }
            }
        }

        /// <summary>
        /// Add a room to the grid
        /// </summary>
        bool UpdateRoomGrid(Vector2Int roomSize, Vector2Int roomPosition, int minRoomSpacing)
        {
            TileType[,] gridTemp = grid.Clone() as TileType[,];

            bool vaildRoom = true;
            for (int j = -(roomSize.x + minRoomSpacing) / 2; j < (roomSize.x + minRoomSpacing) / 2; j++)
            {
                for (int k = -(roomSize.y + minRoomSpacing) / 2; k < (roomSize.y + minRoomSpacing) / 2; k++)
                {
                    if (roomPosition.x + j >= gridSize || roomPosition.x + j < 0 || roomPosition.y + k >= gridSize || roomPosition.y + k < 0)
                    {
                        vaildRoom = false;
                        break;
                    }
                    if ((j < -roomSize.x / 2 || j > roomSize.x / 2 || k < -roomSize.y / 2 || k > roomSize.y / 2) && grid[roomPosition.x + j, roomPosition.y + k] == TileType.EMPTY) gridTemp[roomPosition.x + j, roomPosition.y + k] = TileType.ROOM_SPACING;
                    else if (grid[roomPosition.x + j, roomPosition.y + k] == TileType.EMPTY) gridTemp[roomPosition.x + j, roomPosition.y + k] = TileType.ROOM;
                    else
                    {
                        vaildRoom = false;
                        break;
                    }
                }

                if (!vaildRoom) break;
            }

            if (vaildRoom) grid = gridTemp.Clone() as TileType[,];

            return vaildRoom;
        }

        /// <summary>
        /// Extends the size of the grid
        /// </summary>
        void ExtendGridSize()
        {
            TileType[,] oldGrid = grid.Clone() as TileType[,];
            int oldGridSize = gridSize;
            gridSize += gridExtension;
            grid = new TileType[gridSize, gridSize];

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < oldGridSize; j++)
                {
                    grid[i, j] = TileType.EMPTY;
                }
            }

            for (int i = 0; i < oldGridSize; i++)
            {
                for (int j = 0; j < oldGridSize; j++)
                {
                    grid[i, j] = oldGrid[i, j];
                }
            }
        }

        /// <summary>
        /// Generate vaild random position in the grid
        /// </summary>
        bool GenerateRandomPosition(ref Vector2Int roomPosition, ref Vector2Int roomSizeVector)
        {
            int numberOfPlacementTries = 0;
            bool foundVaildRoom;
            do
            {
                foundVaildRoom = UpdateRoomGrid(roomSizeVector, roomPosition, minRoomSpacing);

                if (!foundVaildRoom)
                {
                    roomPosition = new(Random.Range(10, gridSize - 10), Random.Range(10, gridSize - 10));
                    int randRoomSize = (useRandomRoomSizes) ? Random.Range(minRoomSize, maxRoomSize) : maxRoomSize;
                    roomSizeVector = new(randRoomSize, randRoomSize); //new(roomSize, roomSize);
                    numberOfPlacementTries += 1;
                    if (maxNumberOfRoomPlacementTries < numberOfPlacementTries)
                    {
                        if (!allowGridExtensions) break;
                        ExtendGridSize();
                        numberOfPlacementTries = 0;
                    }
                }
            } while (!foundVaildRoom);

            return foundVaildRoom;
        }

        /// <summary>
        /// Instantiate A Dungeon Object
        /// </summary>
        GameObject InstantiateDungeonObject(GameObject prefab, Vector3 roomPosition, Vector3 roomSizeVector, int roomID)
        {
            GameObject newObject = GameObject.Instantiate(prefab.gameObject, roomPosition, Quaternion.identity);
            newObject.transform.localScale = roomSizeVector;
            newObject.name = "Room_" + roomID.ToString();
            newObject.transform.parent = dungeonHolder.transform;

            return newObject;
        }

        /// <summary>
        /// Create a new room given a prefab, position, scale, and id
        /// </summary>
        void CreateNewRoom(Room roomPrefab, Vector2Int roomPosition, Vector2Int roomSizeVector, int roomID)
        {
            Vector3 position = new Vector3(roomPosition.x, 0.0f, roomPosition.y) * tileSize;
            Vector3 scale = new Vector3(roomSizeVector.x, 1.0f, roomSizeVector.y);

            float scalingFactor = Mathf.Min(tileSize / maxRoomScale.x, tileSize / maxRoomScale.z);

            scale.x *= scalingFactor;
            //scale.y /= roomPrefab.size.y;
            scale.z *= scalingFactor;

            GameObject roomObject = InstantiateDungeonObject(roomPrefab.gameObject, position, scale, roomID);

            List<Transform> exits = GetRoomExits(roomObject);

            Room room = new(roomObject, exits, roomPrefab.size, roomID);

            roomList.Add(room);
        }

        /// <summary>
        /// Choses a random roomType
        /// </summary>
        Room GetRandomRoom(List<RoomType> roomTypes)
        {
            bool roomFound = false;
            Room room = null;
            do
            {
                float rand = Random.Range(0.0f, 1.0f);

                RoomType type = roomTypes[Random.Range(0, roomTypes.Count)];

                if (type.proability >= rand)
                {
                    roomFound = true;
                    room = type.GetRoom();
                }

            } while (!roomFound);

            return room;
        }

        /// <summary>
        /// Generate a dungeon room
        /// </summary>
        void GenerateDungeonRooms()
        {
            int numberOfRooms = Random.Range(minNumberOfRooms, maxNumberOfRooms + 1);

            for (int i = 0; i < numberOfRooms; i++)
            {
                Room roomPrefab = GetRandomRoom(roomTypes);
                Vector2Int roomPosition = new(Random.Range(10, gridSize - 10), Random.Range(10, gridSize - 10));
                int randRoomSize = (useRandomRoomSizes) ? Random.Range(minRoomSize, maxRoomSize) : maxRoomSize;
                Vector2Int roomSizeVector = new(randRoomSize, randRoomSize); //new(roomSize, roomSize);

                bool foundVaildRoom = GenerateRandomPosition(ref roomPosition, ref roomSizeVector);

                if (foundVaildRoom) CreateNewRoom(roomPrefab, roomPosition, roomSizeVector, i);
            }
        }

        /// <summary>
        /// Get A list of room center coordinates
        /// </summary>
        List<Vector2> GetVerticeList()
        {
            List<Vector2> vertices = new();

            foreach (Room room in roomList)
            {
                vertices.Add(new Vector2(room.gameObject.transform.position.x, room.gameObject.transform.position.z));
            }

            return vertices;
        }

        /// <summary>
        /// Calculate Delaunay Triangulation using the room centers
        /// </summary>
        void CalculateDelaunayTriangulation()
        {
            List<Vector2> vertices = GetVerticeList();

            delaunayTriangulation = new(vertices);
            adjacencyMatrix = delaunayTriangulation.CalculateDelaunayTriangulation();
        }

        /// <summary>
        /// Finds a room type with the least number of exits
        /// </summary>
        RoomType FindRoomWithLeastExits(int numExitsNedded)
        {
            RoomType bestRoom = roomTypes[0];
            foreach (RoomType type in roomTypes)
            {
                if (type.GetRoom().numExits >= numExitsNedded)
                {
                    if (bestRoom.GetRoom().numExits < numExitsNedded) bestRoom = type;
                    else if (bestRoom.GetRoom().numExits > type.GetRoom().numExits) bestRoom = type;
                }

            }
            return bestRoom;
        }

        /// <summary>
        /// Replace a room.
        /// Called when the original room has too little exits
        /// </summary>
        void ReplaceRoom(int roomID, int numberOfExitsNeeded)
        {
            Vector3 roomPos = roomList[roomID].gameObject.transform.position;
            Vector3 roomScale = roomList[roomID].gameObject.transform.localScale;

            RoomType type = FindRoomWithLeastExits(numberOfExitsNeeded);

            // Might be issue in future with repalcing rooms with a bigger/smaller rooms
            // If this issue arises check here first and make function to re-computed grid
            GameObject.Destroy(roomList[roomID].gameObject);
            GameObject newRoom = InstantiateDungeonObject(type.GetRoom().gameObject, roomPos, roomScale, roomID);
            roomList[roomID].gameObject = newRoom;
            roomList[roomID].SetExits(GetRoomExits(roomList[roomID].gameObject));

        }

        /// <summary>
        /// Finds the number of used exits on a room
        /// </summary>
        void FindNumberOUsedExits()
        {
            foreach (int vertex in mstEdges.Keys)
            {
                foreach (int adjVertex in mstEdges[vertex])
                {
                    roomList[vertex].exitsUsed += 1;
                    roomList[adjVertex].exitsUsed += 1;
                }
            }
        }

        /// <summary>
        /// Calcuate a MST on the trigulation
        /// </summary>
        void CalculateMST()
        {
            List<Vector2> vertices = GetVerticeList();
            mst = new MST(adjacencyMatrix, vertices);
            mstEdges = mst.GetMST();
            FindNumberOUsedExits();
        }

        /// <summary>
        /// Makes sure all room have enough exits 
        /// </summary>
        void CheckForRoomsWithInvaildNumberOfEdages()
        {
            for (int v = 0; v < roomList.Count; v++)
            {
                if (roomList[v].exitsUsed > roomList[v].exits.Count) ReplaceRoom(v, roomList[v].exitsUsed);
            }
        }

        /// <summary>
        /// Add back random edge to the MST 
        /// </summary>
        void AddRandomEdgeToMST()
        {
            int numberOfVertices = roomList.Count;

            for (int i = 0; i < numberOfVertices; i++)
            {
                for (int v = 0; v < numberOfVertices; v++)
                {
                    float rand = Random.Range(0.0f, 1.0f);
                    if (rand <= loopProabilty && adjacencyMatrix[i, v] != 0 && !(mstEdges[i].Contains(v) || mstEdges[v].Contains(i)) && roomList[v].exitsUsed < roomList[v].exits.Count && roomList[i].exitsUsed < roomList[i].exits.Count)
                    {
                        mstEdges[i].Add(v);
                        roomList[i].exitsUsed += 1;
                        roomList[v].exitsUsed += 1;
                    }

                }
            }
        }

        /// <summary>
        /// Converts world coordinates into grid space cooridnates 
        /// </summary>
        Vector2Int GetRoomdPosition(Vector3 start)
        {
            return new Vector2Int((int)Mathf.Ceil((start.x / tileSize)), (int)Mathf.Ceil((start.z / tileSize)));
        }

        /// <summary>
        /// Add hallway to the grid
        /// </summary>
        void AddHallwayToGrid(Vector2Int gridPos)
        {
            grid[gridPos.x, gridPos.y] = (grid[gridPos.x, gridPos.y] == TileType.ROOM) ? TileType.HALLWAY_AND_ROOM : TileType.HALLWAY;
        }

        /// <summary>
        /// Place a allway along a path
        /// </summary>
        void PlaceHallway(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int end)
        {
            Vector2Int gridPT = start;
            while (true)
            {
                AddHallwayToGrid(gridPT);

                if (!cameFrom.ContainsKey(gridPT)) break;
                else gridPT = cameFrom[gridPT];
            }
            AddHallwayToGrid(end);
        }

        /// <summary>
        /// Generates a path between rooms using A*
        /// </summary>
        void GenerateHallwayBetweenTwoRooms(int roomIDA, int roomIDB)
        {
            Transform startExit = roomList[roomIDA].availableExits.Pop();
            Transform endExit = roomList[roomIDB].availableExits.Pop();

            Vector2Int start = GetRoomdPosition(startExit.position);
            Vector2Int end = GetRoomdPosition(endExit.position);

            PathFinder pathFinder = new PathFinder(grid, tileWeights);
            Dictionary<Vector2Int, Vector2Int> path = pathFinder.FindOptimalPath(start, end);

            pathStarts.Add(start);
            pathEnds.Add(end);
            paths.Add(path);

            PlaceHallway(path, end, start);
        }

        /// <summary>
        /// Place all hallways in dungeon
        /// </summary>
        void GenerateHallways()
        {
            int numberOfRooms = roomList.Count;

            for (int i = 0; i < numberOfRooms; i++)
            {
                foreach (int roomID in mstEdges[i])
                {
                    GenerateHallwayBetweenTwoRooms(i, roomID);
                }
            }
        }

        /// <summary>
        /// Places a terminal room
        /// </summary>
        bool AddTerminalRoom(int roomID)
        {
            Room terminalRoomPrefab = GetRandomRoom(terminalRoomTypes);
            Vector2Int roomPosition = new(Random.Range(10, gridSize - 10), Random.Range(10, gridSize - 10));
            int randRoomSize = (useRandomRoomSizes) ? Random.Range(minRoomSize, maxRoomSize) : maxRoomSize;
            Vector2Int roomSizeVector = new(randRoomSize, randRoomSize);

            bool success = GenerateRandomPosition(ref roomPosition, ref roomSizeVector);

            if (success)
            {
                int newRoomID = roomList.Count;

                CreateNewRoom(terminalRoomPrefab, roomPosition, roomSizeVector, newRoomID);
                GenerateHallwayBetweenTwoRooms(roomID, newRoomID);
            }

            return success;
        }

        /// <summary>
        /// Add terminal rooms to cap off any open exits
        /// </summary>
        void CapOffOpenExits()
        {
            List<Room> roomCopy = new List<Room>(roomList);

            foreach (Room room in roomCopy)
            {
                int numberOfTries = 0;
                while (room.availableExits.Count != 0)
                {
                    if (numberOfTries > maxNumberOfRoomPlacementTries)
                    {
                        if (!allowGridExtensions) break;
                        numberOfTries = 0;
                        ExtendGridSize();
                    }

                    if (AddTerminalRoom(room.roomID)) roomList[room.roomID].exitsUsed += 1;
                    numberOfTries += 1;
                }
            }

        }

        /// <summary>
        /// Initailzation wrapper
        /// </summary>
        void Initailzation()
        {
            InitailzePrefabs();
            InitailzeDungeon();
        }

        /// <summary>
        /// Finds the hallway orention needed to be placed
        /// </summary>
        int GetHallwayOrention(TileType[,] tiles)
        {
            int key = 0;
            int i = 0;

            foreach (Vector2Int dir in DIRECTIONS)
            {
                if (tiles[dir.x + 1, dir.y + 1] == TileType.HALLWAY || tiles[dir.x + 1, dir.y + 1] == TileType.HALLWAY_AND_ROOM) key |= 1 << i;
                i += 1;
            }

            return key;
        }

        /// <summary>
        /// Finds the all surrounding tiles
        /// </summary>
        TileType[,] GetSurroundingTiles(Vector2Int pos)
        {
            TileType[,] tiles = new TileType[3, 3];

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (pos.x + i < gridSize && pos.y + j < gridSize && pos.x + i >= 0 && pos.y + j >= 0) tiles[i + 1, j + 1] = grid[pos.x + i, pos.y + j];
                    else tiles[i + 1, j + 1] = TileType.EMPTY;
                }
            }

            return tiles;
        }

        /// <summary>
        /// Instantiates A Hallway Tile
        /// </summary>
        GameObject InstantiateDungeonHallwayObject(GameObject prefab, Vector3 roomPosition, Vector3 roomSizeVector, int hallwayID)
        {
            GameObject newObject = GameObject.Instantiate(prefab.gameObject, roomPosition, prefab.gameObject.transform.localRotation);
            newObject.transform.localScale = roomSizeVector;
            newObject.name = "Hallway_" + hallwayID.ToString();
            newObject.transform.parent = dungeonHolder.transform;

            return newObject;
        }

        /// <summary>
        /// Generates the hallway mesh
        /// </summary>
        void CreateHallwayMesh(Vector2Int gridPos, int key, int hallwayID)
        {
            Debug.Log(key);
            Hallway hallway = hallwayTypeList[key];

            Vector3 position = new Vector3(gridPos.x, 0, gridPos.y) * tileSize;
            Vector3 scale = new Vector3(1, 1, 1);

            float scalingFactor = Mathf.Max(tileSize / 4, tileSize / 4);

            scale.x *= scalingFactor;
            //scale.y /= hallway.size.y;
            scale.z *= scalingFactor;

            GameObject hallwayObject = InstantiateDungeonHallwayObject(hallway.gameObject, position, scale, hallwayID);

            hallwayObject.gameObject.SetActive(true);

            Hallway hallwayInstance = new(hallwayID, "Hallway_" + hallwayID.ToString());
            hallwayInstance.gameObject = hallwayObject;
            hallwayList.Add(hallwayInstance);

        }

        /// <summary>
        /// Renderer all hallways
        /// </summary>
        void RenderHallway()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    //if (grid[i, j] == TileType.ROOM) continue;

                    Vector2Int gridPos = new Vector2Int(i, j);
                    TileType[,] tiles = GetSurroundingTiles(gridPos);

                    int key = GetHallwayOrention(tiles);

                    if (grid[i, j] == TileType.HALLWAY) CreateHallwayMesh(gridPos, key, i * gridSize + j);
                }
            }
        }

        /// <summary>
        /// Main Function for The Dungeon Generation
        /// </summary>
        void GenerateDungeon()
        {
            GenerateHallwayVarients();
            Initailzation();
            GenerateDungeonRooms();
            CalculateDelaunayTriangulation();
            CalculateMST();
            CheckForRoomsWithInvaildNumberOfEdages();
            AddRandomEdgeToMST();
            GenerateHallways();
            numberOfMainRooms = roomList.Count;
            CapOffOpenExits();
            RenderHallway();

            foreach (Hallway hallway in rawHallwayTypes)
            {
                if (hallway.gameObject != null) hallway.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Destroy The Dungeon And All Assets
        /// </summary>
        void DestroyDungeon()
        {
            foreach (Room room in roomList)
            {
                GameObject.Destroy(room.gameObject);
            }

            foreach (Hallway hallway in hallwayList)
            {
                GameObject.Destroy(hallway.gameObject);
            }
        }

        /// <summary>
        /// Draw path for debugging
        /// </summary>
        void DrawPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int end)
        {
            Vector2Int gridPT = start;
            while (true)
            {
                if (!cameFrom.ContainsKey(gridPT)) break;

                Vector3 worldPT = new Vector3(gridPT.x, 0, gridPT.y) * tileSize;
                Vector3 nextWorldPT = new Vector3(cameFrom[gridPT].x, 0, cameFrom[gridPT].y) * tileSize;

                if (grid[cameFrom[gridPT].x, cameFrom[gridPT].y] == TileType.ROOM || grid[cameFrom[gridPT].x, cameFrom[gridPT].y] == TileType.HALLWAY_AND_ROOM) Debug.DrawLine(worldPT, nextWorldPT, Color.red, Mathf.Infinity);
                else Debug.DrawLine(worldPT, nextWorldPT, Color.magenta, Mathf.Infinity);

                gridPT = cameFrom[gridPT];
            }
        }

        /// <summary>
        /// Draw grid for debugging
        /// </summary>
        void DrawGrid()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(i, 0, j) * tileSize;
                    cube.transform.localScale = new Vector3(tileSize, 1, tileSize);
                    if (grid[i, j] == TileType.ROOM) cube.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                    else if (grid[i, j] == TileType.ROOM_SPACING) cube.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                    else if (grid[i, j] == TileType.EMPTY) cube.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
                    else if (grid[i, j] == TileType.HALLWAY) cube.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                    else cube.GetComponent<Renderer>().material.SetColor("_Color", Color.grey);
                }
            }
        }

        void Awake()
        {
            regenerateDungeon = false;
            GenerateDungeon();
        }

        void Update()
        {
            if (regenerateDungeon)
            {
                regenerateDungeon = false;
                DestroyDungeon();
                GenerateDungeon();
            }

            if (drawDelaunayTriangulation)
            {
                drawDelaunayTriangulation = false;
                // Debug
                delaunayTriangulation.DrawDelaunayTriangulation();
            }

            if (drawMST)
            {
                drawMST = false;
                // Debug
                mst.DrawMST();
            }

            if (drawFinalGraph)
            {
                drawFinalGraph = false;
                // Debug
                for (int i = 0; i < numberOfMainRooms; i++)
                {
                    if (mstEdges[i].Count == 0) continue;
                    for (int j = 0; j < mstEdges[i].Count; j++)
                    {
                        Debug.DrawLine(roomList[mstEdges[i][j]].gameObject.transform.position, roomList[i].gameObject.transform.position, Color.blue, Mathf.Infinity);
                    }
                }
            }

            if (drawHallwayPaths)
            {
                drawHallwayPaths = false;
                //DrawGrid();
                for (int i = 0; i < paths.Count; i++)
                {
                    DrawPath(paths[i], pathEnds[i], pathStarts[i]);
                }
            }
        }
    }
}
