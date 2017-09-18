using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity {

    public enum EnemyState
    {
        Idle,
        Chasing,
        Attacking
    }

    private EnemyState m_CURRENTSTATE; 
    private NavMeshAgent m_pathFinder;
    private Transform m_target;
    private Material m_skinMat;
    private Color m_originalColor;
    private LivingEntity m_targetEntity; 
    private float m_attackDistanceThreshold = .85f;
    private float m_timeBetweenAttacks = 1f;
    private float m_nextAttackTime;
    private float m_enemyCollisionRadius;
    private float m_targetCollisionRadius;
    private float m_damage = 1f;
    private bool m_hasTarget; 

	protected override void Start () {
        base.Start();
        m_skinMat = GetComponent<Renderer>().material;
        m_originalColor = m_skinMat.color;
        m_pathFinder = GetComponent<NavMeshAgent>();

        if(GameObject.FindGameObjectWithTag("Player"))
        {
            m_CURRENTSTATE = EnemyState.Chasing;
            m_hasTarget = true;

            m_target = GameObject.FindGameObjectWithTag("Player").transform;
            m_targetEntity = m_target.GetComponent<LivingEntity>();
            m_targetEntity.m_OnDeath += OnTargetDeath;

            m_enemyCollisionRadius = GetComponent<CapsuleCollider>().radius;
            m_targetCollisionRadius = m_target.GetComponent<CapsuleCollider>().radius;
            StartCoroutine(UpdatePath());
        }
    }

    void Update()
    {
        if(m_hasTarget)
        {
            if(Time.time > m_nextAttackTime)
            {
                float sqrDistToTarget = (m_target.position - transform.position).sqrMagnitude;

                if(sqrDistToTarget < Mathf.Pow(m_attackDistanceThreshold + m_enemyCollisionRadius + m_targetCollisionRadius, 2))
                {
                    m_nextAttackTime = Time.time + m_timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
    }

    void OnTargetDeath()
    {
        m_hasTarget = false;
        m_CURRENTSTATE = EnemyState.Idle;  
    }

    IEnumerator Attack()
    {
        m_CURRENTSTATE = EnemyState.Attacking;
        m_pathFinder.enabled = false; 
        Vector3 originalPos = transform.position;
        Vector3 dirToTarget = (m_target.position - transform.position).normalized;
        Vector3 attackPos = m_target.position - dirToTarget * (m_enemyCollisionRadius * 1.5f);

        float attackSpeed = 2.75f; 
        float percent = 0f;

        m_skinMat.color = Color.red;
        bool hasAppliedDamage = false;

        while(percent <= 1)
        {
            if(percent >= 0.5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                m_targetEntity.TakeDamage(m_damage);
            }

            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPos, attackPos, interpolation);
            yield return null;
        }

        m_skinMat.color = m_originalColor;
        m_CURRENTSTATE = EnemyState.Chasing;
        m_pathFinder.enabled = true;
    }


    IEnumerator UpdatePath()
    {
        float refreshRate = 0.2f;

        while(m_hasTarget)
        {
            if(m_CURRENTSTATE == EnemyState.Chasing)
            {
                Vector3 dirToTarget = (m_target.position - transform.position).normalized;
                Vector3 targetPos = m_target.position - dirToTarget * (m_enemyCollisionRadius + m_targetCollisionRadius + m_attackDistanceThreshold/2);

                if(!m_dead)
                {
                    m_pathFinder.SetDestination(targetPos);
                }
            }

            yield return new WaitForSeconds(refreshRate);
        }
    }
}
