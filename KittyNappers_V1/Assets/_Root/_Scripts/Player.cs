using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (PlayerController))]
[RequireComponent(typeof (GunController))]
public class Player : LivingEntity {

    public float m_moveSpeed = 5f;
    private Camera m_viewCamera;
    private PlayerController m_PlayerController;
    private GunController m_GunController; 

	// Use this for initialization
	protected override void Start () {
        base.Start();
        m_viewCamera = Camera.main;
        m_GunController = GetComponent<GunController>();
        m_PlayerController = GetComponent<PlayerController>();	
	}
	
	// Update is called once per frame
	void Update () {

        //movement input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * m_moveSpeed;
        m_PlayerController.Move(moveVelocity);

        // look input
        Ray ray = m_viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if(groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Debug.DrawLine(ray.origin, point, Color.red);

            m_PlayerController.LookAt(point);
        }

        //weapon input
        if(Input.GetMouseButton(0))
        {
            m_GunController.Shoot();
        }
	}
}
