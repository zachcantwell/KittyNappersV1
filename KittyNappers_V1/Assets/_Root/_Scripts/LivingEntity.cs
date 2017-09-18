using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float m_startingHealth; 
    protected float m_health;
    protected bool m_dead;
    public event System.Action m_OnDeath; 

    protected virtual void Start()
    {
        m_health = m_startingHealth;
    }

    public void TakeHit(float damage, RaycastHit hit)
    {
        m_health -= damage;

        if(m_health <= 0)
        {
            Die();
        }
    }

    protected void Die()
    {
        m_dead = true;

        if(m_OnDeath != null)
        {
            m_OnDeath();
        }

        Destroy(gameObject);
    }
	
}
