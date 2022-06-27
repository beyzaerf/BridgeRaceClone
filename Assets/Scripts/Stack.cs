using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stack : MonoBehaviour
{
    private int brickCount;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Brick")) //Picking up bricks
        {
            collision.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            collision.transform.parent = transform; //Making it its parent
            brickCount++;
        }
    }
}
