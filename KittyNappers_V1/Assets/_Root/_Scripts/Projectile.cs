using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public LayerMask m_collisionMask;
    float m_speed = 10f;
    float m_damage = 1f; 

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

        if(Physics.Raycast(ray, out hit, moveDistance, m_collisionMask, QueryTriggerInteraction.Collide))
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

}
