using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    [System.Serializable]
    public class Coord
    {
        public int m_x, m_y;
        public Coord(int x, int y)
        {
            m_x = x;
            m_y = y;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.m_x == c2.m_x && c1.m_y == c2.m_y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
    }

    [Range(0, 1)]
    public float m_outlinePercent;

    public Map[] m_Maps;
    public int m_mapIndex;
    Map m_CurrentMap;

    public Transform m_tilePrefab;
    public Transform m_obstaclePrefab;
    public Transform m_navMeshFloor;
    public Transform m_navMeshMaskPrefab; 

    public Vector2 m_maxMapSize;
    public List<Coord> m_allTileCoords;

    public float m_tileSize = 1f; 

    private Queue<Coord> m_shuffledTileCoords; 

    void Start () {
        GenerateMap();	
	}

    public Coord GetRandomCoord()
    {
        Coord randomCoord = m_shuffledTileCoords.Dequeue();
        m_shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public void GenerateMap()
    {
        m_CurrentMap = m_Maps[m_mapIndex];
        System.Random prng = new System.Random(m_CurrentMap.m_seed);
        GetComponent<BoxCollider>().size = new Vector3(m_CurrentMap.m_MapSize.m_x * m_tileSize, 0.05f, m_CurrentMap.m_MapSize.m_y * m_tileSize);

        //Generating Coords
        m_allTileCoords = new List<Coord>();

        for(int x = 0; x < m_CurrentMap.m_MapSize.m_x; x++)
        {
            for(int y = 0; y < m_CurrentMap.m_MapSize.m_y; y++)
            {
                m_allTileCoords.Add(new Coord(x, y));
            }
        }
        m_shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(m_allTileCoords.ToArray(), m_CurrentMap.m_seed));

        //Create mapholder object
        string holderName = "Generated Map";

        if(transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;


        //Spawning Tiles
        for(int x = 0; x < m_CurrentMap.m_MapSize.m_x; x++)
        {
            for(int y = 0; y < m_CurrentMap.m_MapSize.m_y; y++)
            {
                Vector3 tilePos = CoordToPosition(x, y);
                Transform newTile = Instantiate(m_tilePrefab, tilePos, Quaternion.Euler(Vector3.right * 90f), mapHolder) as Transform;

                newTile.localScale = Vector3.one * (1 - m_outlinePercent) * m_tileSize; 
            }
        }

        // Spawning Obstacles
        bool[,] obstacleMap = new bool[(int)m_CurrentMap.m_MapSize.m_x, (int)m_CurrentMap.m_MapSize.m_y];

        int obstacleCount = (int)(m_CurrentMap.m_MapSize.m_x * m_CurrentMap.m_MapSize.m_y * m_CurrentMap.m_obstaclePercent);
        int currentObstacleCount = 0;

        for(int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.m_x, randomCoord.m_y] = true;
            currentObstacleCount++;

            if(randomCoord != m_CurrentMap.m_MapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(m_CurrentMap.m_minObstacleHeight, m_CurrentMap.m_maxObstacleHeight, (float)prng.NextDouble());
                Vector3 obstaclePos = CoordToPosition(randomCoord.m_x, randomCoord.m_y);

                if(m_obstaclePrefab)
                {
                    Transform newObstacle = Instantiate(m_obstaclePrefab, obstaclePos + Vector3.up * obstacleHeight / 2f, Quaternion.identity) as Transform;
                    newObstacle.parent = mapHolder;
                    newObstacle.localScale = new Vector3((1 - m_outlinePercent) * m_tileSize, obstacleHeight, (1 - m_outlinePercent) * m_tileSize);

                    Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                    Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);

                    float colorPercent = randomCoord.m_y / (float)m_CurrentMap.m_MapSize.m_y;
                    obstacleMaterial.color = Color.Lerp(m_CurrentMap.m_foregroundColor, m_CurrentMap.m_backgroundColor, colorPercent);

                    obstacleRenderer.sharedMaterial = obstacleMaterial;

                }
            }
            else
            {
                obstacleMap[randomCoord.m_x, randomCoord.m_y] = false;
                currentObstacleCount--;
            }

        }

        //Creating navmesh mask
        Transform maskLeft = Instantiate(m_navMeshMaskPrefab, Vector3.left * (m_CurrentMap.m_MapSize.m_x + m_maxMapSize.x) / 4f * m_tileSize, Quaternion.Euler(0f,0f,90f)) as Transform;
        maskLeft.parent = mapHolder;
        maskLeft.localScale = new Vector3((m_maxMapSize.x - m_CurrentMap.m_MapSize.m_x) / 2f, 1, m_CurrentMap.m_MapSize.m_y) * m_tileSize;

        Transform maskRight = Instantiate(m_navMeshMaskPrefab, Vector3.right * (m_CurrentMap.m_MapSize.m_x + m_maxMapSize.x) / 4f * m_tileSize, Quaternion.Euler(0f, 0f, 90f)) as Transform;
        maskRight.parent = mapHolder;
        maskRight.localScale = new Vector3((m_maxMapSize.x - m_CurrentMap.m_MapSize.m_x) / 2f, 1, m_CurrentMap.m_MapSize.m_y) * m_tileSize;

        Transform maskTop = Instantiate(m_navMeshMaskPrefab, Vector3.forward * (m_CurrentMap.m_MapSize.m_y + m_maxMapSize.y) / 4f * m_tileSize, Quaternion.Euler(90f, 0f, 0f)) as Transform;
        maskTop.parent = mapHolder;
        maskTop.localScale = new Vector3(m_maxMapSize.x, 1, (m_maxMapSize.y - m_CurrentMap.m_MapSize.m_y) /2f) * m_tileSize;

        Transform maskBottom = Instantiate(m_navMeshMaskPrefab, Vector3.back * (m_CurrentMap.m_MapSize.m_y + m_maxMapSize.y) / 4f * m_tileSize, Quaternion.Euler(90f, 0f, 0f)) as Transform;
        maskBottom.parent = mapHolder;
        maskBottom.localScale = new Vector3(m_maxMapSize.x, 1, (m_maxMapSize.y - m_CurrentMap.m_MapSize.m_y) / 2f) * m_tileSize;

        m_navMeshFloor.localScale = new Vector3(m_maxMapSize.x, m_maxMapSize.y) * m_tileSize;

    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-m_CurrentMap.m_MapSize.m_x / 2f + 0.5f + x, 0f, -m_CurrentMap.m_MapSize.m_y / 2f + 0.5f + y) * m_tileSize; 
    }

    private bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(m_CurrentMap.m_MapCentre);
        mapFlags[m_CurrentMap.m_MapCentre.m_x, m_CurrentMap.m_MapCentre.m_y] = true;

        int accessibleTileCount = 1; 

        while(queue.Count > 0)
        {
            Coord tile = queue.Dequeue();

            for(int x = -1; x <= 1; x++)
            {
                for(int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.m_x + x;
                    int neighbourY = tile.m_y + y;

                    if(x == 0 || y == 0)
                    {
                        if(neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if(!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }
        int targetAccessibleTileCount = (int)(m_CurrentMap.m_MapSize.m_x * m_CurrentMap.m_MapSize.m_y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    [System.Serializable]
    public class Map
    {
        public Coord m_MapSize;
        [Range(0,1)]
        public float m_obstaclePercent;
        public int m_seed;
        public float m_minObstacleHeight;
        public float m_maxObstacleHeight;
        public Color m_foregroundColor;
        public Color m_backgroundColor;

        public Coord m_MapCentre
        {
            get
            {
                return new Coord(m_MapSize.m_x / 2, m_MapSize.m_y / 2);
            }
        }
    }
}
