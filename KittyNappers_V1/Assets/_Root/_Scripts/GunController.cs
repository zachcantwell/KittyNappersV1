using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {

    private Gun m_equippedGun;

    public Gun m_startingGun;
    public Transform m_weaponHold;

    void Start()
    {
        if(m_startingGun != null)
        {
            EquipGun(m_startingGun);
        }
    }

    public void EquipGun(Gun gunToEquip)
    {
        if(m_equippedGun != null)
        {
            Destroy(m_equippedGun.gameObject);
        }
        m_equippedGun = Instantiate(gunToEquip, m_weaponHold.position, m_weaponHold.rotation) as Gun;
        m_equippedGun.transform.parent = m_weaponHold; 
    }

    public void Shoot()
    {
        if(m_equippedGun != null)
        {
            m_equippedGun.Shoot();
        }
    }
}
