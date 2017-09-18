using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public Transform m_muzzle;
    public Projectile m_projectile;
    public float m_msBetweenShots = 80f;
    public float m_muzzleVelocity = 25f;

    private float m_nextShotTime; 

    public void Shoot()
    {
        if(Time.time > m_nextShotTime)
        {
            m_nextShotTime = Time.time + m_msBetweenShots / 1000f;
            Projectile newProjectile = Instantiate(m_projectile, m_muzzle.transform.position, m_muzzle.transform.rotation) as Projectile;
            newProjectile.SetSpeed(m_muzzleVelocity);
        }
    }
}
