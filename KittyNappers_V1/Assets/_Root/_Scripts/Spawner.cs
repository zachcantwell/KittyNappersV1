using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Wave
{
    public int m_enemyCount;
    public float m_timeBetweenSpawns;
}

public class Spawner : MonoBehaviour {

    public Wave[] m_Waves;
    public Enemy m_Enemy;

    private Wave m_currentWave;
    private int m_currentWaveNumber;

    private int m_enemiesRemainingToSpawn;
    private int m_enemiesRemainingAlive; 
    private float m_nextSpawnTime;

	void Start () {
        NextWave();
	}

    void Update()
    {
        if(m_enemiesRemainingToSpawn > 0 && Time.time > m_nextSpawnTime)
        {
            m_enemiesRemainingToSpawn--;
            m_nextSpawnTime = Time.time + m_currentWave.m_timeBetweenSpawns;

            Enemy spawnedEnemy = Instantiate(m_Enemy, transform.position, Quaternion.identity) as Enemy;
            spawnedEnemy.m_OnDeath += OnEnemyDeath;
        }
    }
	
    void NextWave()
    {
        m_currentWaveNumber++;

        if(m_currentWaveNumber - 1 < m_Waves.Length)
        {
            m_currentWave = m_Waves[m_currentWaveNumber - 1];
            m_enemiesRemainingToSpawn = m_currentWave.m_enemyCount;
            m_enemiesRemainingAlive = m_enemiesRemainingToSpawn;
        }
    }

    void OnEnemyDeath()
    {
        m_enemiesRemainingAlive--;

        if(m_enemiesRemainingAlive <= 0)
        {
            NextWave();
        }
    }
}
