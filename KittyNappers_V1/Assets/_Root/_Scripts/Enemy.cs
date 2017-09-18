using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity {

    private NavMeshAgent m_pathFinder;
    private Transform m_target;
    private float m_attackDistanceThreshold = 1.5f;
    private float m_timeBetweenAttacks = 1f;
    private float m_nextAttackTime;

	protected override void Start () {
        base.Start();
        m_pathFinder = GetComponent<NavMeshAgent>();
        m_target = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(UpdatePath());

    }

    void Update()
    {
        if(Time.time > m_nextAttackTime)
        {
            float sqrDistToTarget = (m_target.position - transform.position).sqrMagnitude;

            if(sqrDistToTarget < Mathf.Pow(m_attackDistanceThreshold, 2))
            {
                m_nextAttackTime = Time.time + m_timeBetweenAttacks;
                StartCoroutine(Attack());
            }
        }

    }

    IEnumerator Attack()
    {
        yield return null;
    }


    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;

        while(m_target != null)
        {
            Vector3 targetPos = new Vector3(m_target.position.x, 0f, m_target.position.z);

            if(!m_dead)
            {
                m_pathFinder.SetDestination(targetPos);
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
