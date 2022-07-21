using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Stack : MonoBehaviour
{
    public static Stack instance;
    private GameObject prevObject;
    private int platform = 0;
    [SerializeField] private GameObject stackObject;
    [SerializeField] private List<GameObject> bricks;
    [SerializeField] private GameObject colliderPrefab;
    public GameObject collObject;

    public int Platform { get => platform; set => platform = value; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        prevObject = stackObject.transform.GetChild(0).gameObject; //Initially, previous object is set to an empty object that is the child of stackObject
        collObject = new GameObject();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("PinkBrick")) //Picking up bricks
        {
            other.transform.SetParent(stackObject.transform); //Changing brick's parent to stackObject
            Vector3 pos = prevObject.transform.localPosition; //Stacking
            pos.y += 0.2f;
            pos.x = 0;
            pos.z = 0;
            bricks.Add(other.gameObject);

            other.transform.localRotation = new Quaternion(0, 0.7071068f, 0, 0.7071068f);
            prevObject = other.gameObject;
            other.transform.DOLocalMove(pos, 0.2f);

            BrickSpawner.instance.GenerateCubes(1);
        }
        //Leaving bricks
        else if (other.transform.CompareTag("Bridge"))
        { 
            if (bricks.Count > 1)
            {
                GameObject myObject = bricks[^1];
                bricks.RemoveAt(bricks.Count - 1);
                Destroy(myObject);
                    
                other.GetComponent<MeshRenderer>().material = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material; //Changing the material of the bridge to our color
                other.GetComponent<MeshRenderer>().enabled = true; //Enabling the bricks on the bridge
                other.GetComponent<BoxCollider>().isTrigger = false;

                other.tag = "BridgeP"; //Changing the tag after leaving bricks
            }
            else if (bricks.Count <= 1) //If the bricks the player is carrying ends
            { 
                Vector3 playersPos = new(transform.position.x, transform.position.y, transform.position.z + 0.2f);
                if (collObject != null)
                {
                    GameObject ifObject = Instantiate(colliderPrefab, playersPos, Quaternion.Euler(0, 0, 0)) as GameObject;
                    ifObject.transform.parent = collObject.transform;
                }
                else
                    collObject = new GameObject();
            }
            prevObject = bricks[^1];
            if (collObject! && collObject.transform.childCount > 0)
            {
                collObject.tag = "Collider";
                collObject.transform.GetChild(0).tag = "Collider";
            }
        }
        else if (other.transform.CompareTag("End")) //not working, therefore the bricks spawning in the second platform is also not working
        {
            Platform += 1;
            Destroy(other.gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Collider"))
        {
            if (bricks.Count > 1)
            {
                if (collObject.transform.childCount > 0)
                {
                    Destroy(collObject);
                }
            }
        }
        else if (collision.transform.CompareTag("BridgeBeginning"))
        {
            Destroy(collision.gameObject);
        }
        else if (collision.transform.CompareTag("Finish"))
        {
            GameManager.Instance.GameWin();
        }
    }
}
