using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public Transform m_tilePrefab;
    public Vector2 m_mapSize;

    [Range(0, 1)]
    public float m_outlinePercent; 

	// Use this for initialization
	void Start () {
        GenerateMap();	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GenerateMap()
    {
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
                Vector3 tilePos = new Vector3(-m_mapSize.x / 2 + 0.5f + x, 0f, -m_mapSize.y/2 + 0.5f + y);
                Transform newTile = Instantiate(m_tilePrefab, tilePos, Quaternion.Euler(Vector3.right * 90f), mapHolder) as Transform;

                newTile.localScale = Vector3.one * (1 - m_outlinePercent); 
            }
        }
    }
}
