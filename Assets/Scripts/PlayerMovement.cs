using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Camera cam;
    public float turnSpeed, speed, lerpValue;
    public LayerMask layer;
    private Animator animator;

    private void Start()
    {
        cam = Camera.main;
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            Movement();
        }
        else //Animation
        {
            if (animator.GetBool("running"))
                animator.SetBool("running", false);
        }
    }

    private void Movement() //Joystick-like movement
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cam.transform.localPosition.z;

        Ray ray = cam.ScreenPointToRay(mousePos);

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
        {
            Vector3 hitVec = hit.point;
            hitVec.y = transform.position.y;

            transform.position = Vector3.MoveTowards(transform.position, Vector3.Lerp(transform.position, hitVec, lerpValue), Time.deltaTime * speed);
            Vector3 newMovePoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newMovePoint - transform.position), turnSpeed * Time.deltaTime);
            
            if (!animator.GetBool("running")) //Animation
                animator.SetBool("running", true);
        }
    }
}