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
        else
        {
            if (animator.GetBool("running"))
                animator.SetBool("running", false);
        }
    }

    private void Movement()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cam.transform.localPosition.z;

        Ray ray = cam.ScreenPointToRay(mousePos);

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
        {
            Vector3 hitVec = hit.point;

            transform.position = Vector3.MoveTowards(transform.position, Vector3.Lerp(transform.position, hitVec, lerpValue), Time.deltaTime * speed);
            Vector3 newMovePoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newMovePoint - transform.position), turnSpeed * Time.deltaTime);
            if (!animator.GetBool("running"))
                animator.SetBool("running", true);
        }
    }
}

////[SerializeField] private GameObject character;
////private Rigidbody rb;
//private Vector3 startPos, currentPos;
//private Vector3 pos;
//[Range(1, 50)] public float moveSpeed, rotSpeed; 
//private void Start()
//{
//    //rb = GetComponent<Rigidbody>();
//}
//private void Update()
//{
//    RaycastHit hit;
//    pos = Input.mousePosition;
//    Ray ray = Camera.main.ScreenPointToRay(pos);
//    Vector3 direction = (currentPos - transform.position).normalized;
//    if(Physics.Raycast(ray, out hit, 200))
//    {
//        currentPos = new Vector3(hit.point.x, 0.25f, hit.point.z);
//        if(Input.GetMouseButton(0))
//        {
//            transform.position = Vector3.Lerp(transform.position, currentPos, 0.5f);
//            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);
//        }
//    }
//    //if ( Input.GetMouseButtonDown(0))
//    //{
//    //    startPosition = Camera.main.WorldToScreenPoint(Input.mousePosition);
//    //}
//    //if (Input.GetMouseButton(0))
//    //{
//    //    pos = Camera.main.WorldToScreenPoint(Input.mousePosition);
//    //    Vector3 move = pos - startPosition;
//    //    rb.velocity = move;
//    //}
//}
//CharacterController characterController;
//private Vector3 startPos;
//void Start()
//{
//    characterController = GetComponent<CharacterController>();
//}
//void FixedUpdate()
//{
//    if (Input.GetMouseButtonDown(0))
//    {
//        startPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
//    }

//    if (Input.GetMouseButton(0))
//    {
//        Vector3 worldPosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
//        Vector3 move2 = worldPosition - startPos;
//        Vector3 move = new Vector3(move2.x, move2.z - 0.2f, move2.y);

//        Vector3.Lerp(transform.position, move, 0.5f);
//        //rb.velocity = move * Time.deltaTime * 5;
//        characterController.Move(move * Time.deltaTime * 5f);
//    }

//    if (Input.GetMouseButtonUp(0))
//    {
//        startPos = Vector3.zero;
//    }
//}
