using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAI : MonoBehaviour
{
    [SerializeField] private GameObject targetsParent;
    public List<GameObject> targets = new List<GameObject>();
    private NavMeshAgent agent;
    //private Animator animator;
    private bool haveTarget = false;
    private Vector3 targetTransform;
    private GameObject prevObject;
    [SerializeField] private GameObject stackObject;

    private void Start()
    {
        //animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        prevObject = stackObject.transform.GetChild(0).gameObject;
        for(int i = 0; i < targetsParent.transform.childCount; i++)
        {
            targets.Add(targetsParent.transform.GetChild(i).gameObject);
        }
    }
    private void Update()
    {
        if(!haveTarget && targets.Count >= 0)
        {
            ChooseTarget();
        }
    }
    void ChooseTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 20);
        List<Vector3> ourColors = new List<Vector3>();

        for(int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag.StartsWith(transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1)))
            {
                ourColors.Add(hitColliders[i].transform.position);

            }
        }
        
        if(ourColors.Count > 0)
        {
            targetTransform = ourColors[0];
        }
        else
        {
            int random = Random.Range(0, targets.Count);
            targetTransform = targets[random].transform.position;
        }
        agent.SetDestination(targetTransform);
        haveTarget = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.StartsWith(transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1))) //Picking up bricks
        {
            other.transform.SetParent(stackObject.transform); //Changing brick's parent to stackObject
            Vector3 pos = prevObject.transform.localPosition; //Stacking
            pos.y += 0.2f;
            //pos.x = 0;
            //pos.z = 0;

            other.transform.localRotation = new Quaternion(0, 0.7071068f, 0, 0.7071068f);
            prevObject = other.gameObject;
        }
    }
}
