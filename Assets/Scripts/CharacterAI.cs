using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAI : MonoBehaviour
{
    private static CharacterAI instance;
    public Character characterEnum;
    [SerializeField] private GameObject targetsParent;
    public List<GameObject> targets = new();
    private NavMeshAgent agent;
    private Animator animator;
    private bool haveTarget;
    private Vector3 targetTransform;
    private GameObject prevObject;
    [SerializeField] private GameObject stackObject;
    [SerializeField] private List<GameObject> bricks;
    [SerializeField] Transform[] bridges;
    Transform bridgeBeginning = null;
    private bool reachedLast;
    private int platform = 0;
    private int randomBridge;

    public static CharacterAI Instance { get => instance; set => instance = value; }
    public int Platform { get => platform; set => platform = value; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        prevObject = stackObject.transform.GetChild(0).gameObject;
        randomBridge = Random.Range(0, 3);
        for (int i = 0; i < targetsParent.transform.childCount; i++) //Filling up the targets list with bricks
        {
            targets.Add(targetsParent.transform.GetChild(i).gameObject);
        }
    }

    private void Update()
    {
        if (!haveTarget && targets.Count > 0)
        {
            ChooseTarget();
        }
        if (Platform == 1)
            randomBridge = Random.Range(3, 5);
    }

    void ChooseTarget()
    {
        if (bricks.Count > Random.Range(3, 7)) // at how many bricks the ai will leave them
        {
            targetTransform = bridges[randomBridge].GetChild(0).position; // choose bridge
        }
        else
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 10);
            List<Vector3> ourColors = new();

            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].tag.StartsWith(transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1)))
                {
                    ourColors.Add(hitColliders[i].transform.position);
                }
            }
            if (ourColors.Count > 0)
            {
                targetTransform = ourColors[0];
            }
            else
            {
                targetTransform = targets[randomBridge].transform.position;
            }
        }
        agent.SetDestination(targetTransform);
        haveTarget = true;

        if (!animator.GetBool("running")) //Animation
            animator.SetBool("running", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BridgeBeginning"))
        {
            bridgeBeginning = other.transform;
        }
        MeshRenderer otherMesh = other.transform.GetComponent<MeshRenderer>();
        Material myMaterial = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material;

        if (other.transform.tag.StartsWith(myMaterial.name.Substring(0, 1))) //Picking up bricks
        {
            haveTarget = false;
            other.transform.SetParent(stackObject.transform); //Changing brick's parent to stackObject
            Vector3 pos = prevObject.transform.localPosition; //Stacking
            pos.y += 0.2f;
            pos.x = 0;
            pos.z = 0;

            other.transform.localRotation = new Quaternion(0, 0.7071068f, 0, 0.7071068f); // making the bricks face the same way
            bricks.Add(other.gameObject);
            targets.Remove(other.gameObject);

            other.transform.DOLocalMove(pos, 0.2f);
            transform.LookAt(targetTransform);
            prevObject = other.gameObject;

            BrickSpawner.instance.GenerateCubes((int)characterEnum, this);
        }
        else if (other.tag.StartsWith("Bridge"))
        {
            if (bricks.Count > 0)
            {
                if (other.tag.StartsWith("Bridge") && !other.tag.StartsWith(("Bridge") + myMaterial.name.Substring(0, 1)))
                {
                    agent.enabled = false;
                    GameObject myObject = bricks[^1];
                    bricks.RemoveAt(bricks.Count - 1);
                    Destroy(myObject);
                    if (otherMesh != null)
                    {
                        otherMesh.material = myMaterial;
                        otherMesh.enabled = true;
                    }
                    else
                    {
                        other.GetComponent<BoxCollider>().isTrigger = true;
                        transform.position += Vector3.forward;
                    }
                    other.tag = "Bridge" + myMaterial.name.Substring(0, 1);
                }
                else if (other.tag.StartsWith("Bridge" + myMaterial.name.Substring(0, 1)))
                {
                    transform.position += Vector3.forward;
                }
                transform.DOMoveZ(transform.position.z + 0.5f, 0.2f);
            }
            else if (!reachedLast)
            {
                reachedLast = true;
                //transform.DORotate(new Vector3(0, transform.eulerAngles.y + 180, 0), 0.2f).OnComplete(() =>
                //{
                    transform.DOMove(bridgeBeginning.position, 1).OnComplete(() =>
                    {
                        agent.enabled = true;
                        ChooseTarget();
                    });
                //});
                prevObject = stackObject.transform.GetChild(0).gameObject;
            }
            reachedLast = false;
        }
        else if (other.CompareTag("End")) //When ai reaches the end of the bridge
        {
            Platform += 1; //change this to the platforms number so that the ai doesnt go back to the bridges on the previous platforms
            transform.DOMoveZ(23, 0.2f).OnComplete(() =>
            {
                agent.enabled = true; //reactivate navmeshagent
                prevObject = stackObject.transform.GetChild(0).gameObject; 
                targets.Clear(); //empty targets because it is filled with previous levels bricks 
                haveTarget = false;
                ChooseTarget(); //choose new targets 
            });
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Finish"))
        {
            GameManager.Instance.GameWin();
        }
    }
}
public enum Character
{
    Zero = 0,
    Two = 2
}
