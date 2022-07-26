using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAI : MonoBehaviour
{
    [SerializeField] private GameObject targetsParent;
    [SerializeField] private GameObject stackObject;
    [SerializeField] Transform[] bridges;
    [SerializeField] private bool haveTarget;
    public List<GameObject> bricks;
    public Character characterEnum;
    private static CharacterAI instance;
    private List<GameObject> targets = new();
    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 targetTransform;
    private GameObject prevObject;
    private Transform bridgeBeginning = null;
    private bool reachedLast;
    private int platform = 0;
    private int randomBridge;
    private TargetController targetController;
    private bool shouldTurn;

    public static CharacterAI Instance { get => instance; set => instance = value; }
    public int Platform { get => platform; set => platform = value; }
    public List<GameObject> Targets { get => targets; set => targets = value; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    private void Start()
    {
        // Variables for easier readability 
        targetController = ObjectManager.instance.TargetController.GetComponent<TargetController>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        prevObject = stackObject.transform.GetChild(0).gameObject;
        shouldTurn = false;

        // Make choosing bricks random
        targetController.AddRandom();
        randomBridge = targetController.Targets[^1];

        agent.updateRotation = false; // Turning off the rotation of navAgentMesh because it doesnt turn smoothly

        for (int i = 0; i < targetsParent.transform.childCount; i++) // Filling up the targets list with bricks
        {
            Targets.Add(targetsParent.transform.GetChild(i).gameObject);
        }
    }

    private void Update()
    {
        // Making the rotation of the navAgentMesh smooth
        if (!shouldTurn)
        {
            Vector3 targetDirection = targetTransform - transform.position;
            targetDirection.y = 0;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetDirection), Time.time * 0.5f);
        }

        if (!haveTarget && Targets.Count > 0)
        {
            ChooseTarget();
        }
        if (Platform == 1)
        {
            if (characterEnum == Character.Zero)
                randomBridge = 3;
            else if (characterEnum == Character.Two)
                randomBridge = 4;
        }
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
                targetTransform = Targets[Random.Range(0, targets.Count - 1)].transform.position;
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
            Debug.Log(pos);

            other.transform.localRotation = new Quaternion(0, 0.7071068f, 0, 0.7071068f); // making the bricks face the same way
            bricks.Add(other.gameObject);
            Targets.Remove(other.gameObject);

            other.transform.DOLocalMove(pos, 0.2f);
            prevObject = other.gameObject;

            BrickSpawner.instance.GenerateCubes((int)characterEnum, this);
        }
        else if (other.tag.StartsWith("Bridge"))
        {
            if (bricks.Count > 0)
            {
                if (other.tag.StartsWith("Bridge") && !other.tag.StartsWith(("Bridge") + myMaterial.name.Substring(0, 1)))
                {
                    shouldTurn = true;
                    transform.eulerAngles = Vector3.zero;

                    agent.enabled = false;
                    GameObject myObject = bricks[^1];
                    bricks.RemoveAt(bricks.Count - 1);
                    Destroy(myObject);

                    //agent.updateRotation = true;
                    if (bricks.Count == 1)
                    {
                        transform.rotation = new Quaternion(0, 180, 0, 0);
                    }

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
                transform.DOMoveZ(transform.position.z + 0.6f, 0.2f);
                shouldTurn = false;
            }
            else if (!reachedLast)
            {
                reachedLast = true;
                transform.DOMove(bridgeBeginning.position, 1).OnComplete(() =>
                {
                    agent.enabled = true;
                    haveTarget = false;
                    ChooseTarget();
                });
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
                Targets.Clear(); //empty targets because it is filled with previous levels bricks 
                haveTarget = false;
                ChooseTarget(); //choose new targets 
            });
        }
        else if (other.CompareTag("Finish")) //Ai winning the game 
        {
            Debug.Log("game was won by " + transform.name);
            GameManager.Instance.GameWin();
            agent.enabled = false;
            transform.position = new Vector3(0, 0.33f, 75);
            animator.SetBool("running", false);
        }
    }
}
public enum Character
{
    Zero = 0,
    Two = 2
}
