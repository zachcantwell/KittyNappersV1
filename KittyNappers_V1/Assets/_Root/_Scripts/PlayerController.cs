using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    private Vector3 m_velocity; 
    private Rigidbody m_RigidBody;

    void Start () {
        m_RigidBody = GetComponent<Rigidbody>();
	}
	
    public void Move(Vector3 moveVelocity)
    {
        m_velocity = moveVelocity;    
    }

    void FixedUpdate()
    {
        m_RigidBody.MovePosition(m_RigidBody.position + m_velocity * Time.fixedDeltaTime);
           
    }

    public void LookAt(Vector3 point)
    {
        Vector3 heightCorrectedPoint = new Vector3(point.x, transform.position.y, point.z);
        transform.LookAt(heightCorrectedPoint);
    }
}
