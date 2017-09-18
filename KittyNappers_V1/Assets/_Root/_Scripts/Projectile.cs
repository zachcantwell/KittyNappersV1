using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public LayerMask m_collisionMask;
    float m_speed = 10f;
    float m_damage = 1f;
    float m_lifeTime = 3f;
    float m_skinWidth = 0.1f; 

    void Start()
    {
        Destroy(gameObject, m_lifeTime);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, m_collisionMask);

        if(initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0]);
        }
    }

    public void SetSpeed(float newSpeed)
    {
        m_speed = newSpeed;
    }

	void Update () {
        float moveDistance = m_speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);	
	}

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, moveDistance + m_skinWidth, m_collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);   
        }
    }

    void OnHitObject(RaycastHit hit)
    {
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        if(damageableObject != null)
        {
            damageableObject.TakeHit(m_damage, hit);
        }   
        Destroy(gameObject);
    }

    void OnHitObject(Collider other)
    {
        IDamageable damageableObject = other.GetComponent<IDamageable>();
        if(damageableObject != null)
        {
            damageableObject.TakeDamage(m_damage);
        }
        Destroy(gameObject);
    }

}
