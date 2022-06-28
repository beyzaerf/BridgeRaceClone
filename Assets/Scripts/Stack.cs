using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Stack : MonoBehaviour
{
    private int brickCount;
    private GameObject prevObject;
    [SerializeField] private GameObject stackObject;
    
    private void Start()
    {
        prevObject = stackObject.transform.GetChild(0).gameObject; //Initially, previous object is set to an empty object that is the child of stackObject
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("PinkBrick")) //Picking up bricks
        {
            other.transform.SetParent(stackObject.transform); //Changing brick's parent to stackObject
            Vector3 pos = prevObject.transform.localPosition; //Stacking
            pos.y += 0.2f;
            pos.x = 0;
            pos.z = 0;

            other.transform.localRotation = new Quaternion(0, 0.7071068f, 0, 0.7071068f);
            prevObject = other.gameObject;
            other.transform.DOLocalMove(pos, 0.2f);

            brickCount++;
        }
    }
}
