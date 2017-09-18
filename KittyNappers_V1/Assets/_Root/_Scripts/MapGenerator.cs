using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public struct Coord
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
    [Range(0, 1)]
    public float m_obstaclePercent; 

    public Transform m_tilePrefab;
    public Transform m_obstaclePrefab; 

    public Vector2 m_mapSize;
    public List<Coord> m_allTileCoords;
    public int m_seed = 10;

    private Coord m_mapCentre; 
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
        m_allTileCoords = new List<Coord>();

        for(int x = 0; x < m_mapSize.x; x++)
        {
            for(int y = 0; y < m_mapSize.y; y++)
            {
                m_allTileCoords.Add(new Coord(x, y));
            }
        }
        m_shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(m_allTileCoords.ToArray(), m_seed));
        m_mapCentre = new Coord((int)m_mapSize.x / 2, (int)m_mapSize.y / 2);

        string holderName = "Generated Map";

        if(transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for(int x = 0; x < m_mapSize.x; x++)
        {
            for(int y = 0; y < m_mapSize.y; y++)
            {
                Vector3 tilePos = CoordToPosition(x, y);
                Transform newTile = Instantiate(m_tilePrefab, tilePos, Quaternion.Euler(Vector3.right * 90f), mapHolder) as Transform;

                newTile.localScale = Vector3.one * (1 - m_outlinePercent); 
            }
        }

        bool[,] obstacleMap = new bool[(int)m_mapSize.x, (int)m_mapSize.y];

        int obstacleCount = (int)(m_mapSize.x * m_mapSize.y * m_obstaclePercent);
        int currentObstacleCount = 0;

        for(int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.m_x, randomCoord.m_y] = true;
            currentObstacleCount++;

            if(randomCoord != m_mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                Vector3 obstaclePos = CoordToPosition(randomCoord.m_x, randomCoord.m_y);

                if(m_obstaclePrefab)
                {
                    Transform newObstacle = Instantiate(m_obstaclePrefab, obstaclePos + Vector3.up * 0.5f, Quaternion.identity) as Transform;
                    newObstacle.parent = mapHolder;
                }
            }

        }

    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-m_mapSize.x / 2 + 0.5f + x, 0f, -m_mapSize.y / 2 + 0.5f + y); 
    }

    private bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(m_mapCentre);
        mapFlags[m_mapCentre.m_x, m_mapCentre.m_y] = true;

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
        int targetAccessibleTileCount = (int)(m_mapSize.x * m_mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }
}
