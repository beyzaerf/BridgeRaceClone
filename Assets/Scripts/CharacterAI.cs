using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAI : MonoBehaviour
{
    public Character characterEnum;
    [SerializeField] private GameObject targetsParent;
    public List<GameObject> targets = new List<GameObject>();
    private NavMeshAgent agent;
    private Animator animator;
    public bool haveTarget;
    private Vector3 targetTransform;
    private GameObject prevObject;
    [SerializeField] private GameObject stackObject;
    [SerializeField] private List<GameObject> bricks;
    [SerializeField] Transform[] bridges;

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        prevObject = stackObject.transform.GetChild(0).gameObject;
        
        for(int i = 0; i < targetsParent.transform.childCount; i++) //Filling up the targets list with bricks
        {
            targets.Add(targetsParent.transform.GetChild(i).gameObject);
        }
    }
    private void Update()
    {
        if(!haveTarget && targets.Count > 0)
        {
            ChooseTarget();
        }
        else
        {
            if (animator.GetBool("running"))
                animator.SetBool("running", false);
        }
    }

    void ChooseTarget()
    {
        if (bricks.Count > 3) //NOT WORKING PROPERLY! If character has more than 3 bricks collected, it should go to random bridge 
        {
            int randomBridge = Random.Range(0, bridges.Length);
            targetTransform = bridges[randomBridge].GetChild(0).position;
        }
        else
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 20);
            List<Vector3> ourColors = new List<Vector3>();

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
                int random = Random.Range(0, targets.Count);
                targetTransform = targets[random].transform.position;
            }
        }
        agent.SetDestination(targetTransform);
        haveTarget = true;

        if (!animator.GetBool("running")) //Animation
            animator.SetBool("running", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.StartsWith(transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1))) //Picking up bricks
        {
            haveTarget = false;
            other.transform.SetParent(stackObject.transform); //Changing brick's parent to stackObject
            Vector3 pos = prevObject.transform.localPosition; //Stacking
            pos.y += 0.2f;
            pos.x = 0;
            pos.z = 0;

            bricks.Add(other.gameObject);
            targets.Remove(other.gameObject);

            other.transform.localRotation = new Quaternion(0, 0.7071068f, 0, 0.7071068f);
            other.transform.DOLocalMove(pos, 0.2f);
            prevObject = other.gameObject;

            BrickSpawner.instance.GenerateCubes((int)characterEnum, this);
        }
    }
}
public enum Character
{
    Zero = 0,
    Two = 2
}
