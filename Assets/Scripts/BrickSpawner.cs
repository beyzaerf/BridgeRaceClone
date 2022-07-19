using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    public static BrickSpawner instance;
    public GameObject redBrick, greenBrick, pinkBrick;
    public Transform redBrickParent, greenBrickParent, pinkBrickParent;
    public int minX, maxX, minZ, maxZ;
    public LayerMask layerMask;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    //0 red 1 pink 2 green
    public void GenerateCubes(int number, CharacterAI characterAI = null)
    {
        if(number == 0)
        {
            Generate(redBrick, redBrickParent, characterAI);
        }
        else if(number == 1)
        {
            Generate(pinkBrick, redBrickParent);
        }
        else if(number == 2)
        {
            Generate(greenBrick, greenBrickParent, characterAI);
        }
    }
    private void Generate(GameObject gameObject, Transform parent, CharacterAI characterAI = null)
    {
        GameObject g = Instantiate(gameObject);
        Vector3 pos = GiveRandomPosition();
        g.SetActive(false);

        Collider[] colliders = Physics.OverlapSphere(pos, 1, layerMask);
        while(colliders.Length != 0)
        {
            pos = GiveRandomPosition();
            colliders = Physics.OverlapSphere(pos, 1, layerMask);
        }
        g.SetActive(true);
        g.transform.position = pos;
        g.transform.parent = parent;

        if(characterAI!)
        {
            characterAI.targets.Add(g);
        }
    }

    private Vector3 GiveRandomPosition()
    {
        if (Stack.instance.Platform > 0)
        {
            minZ += 36;
            maxZ += 36;
            return new Vector3(Random.Range(minX, maxX), 0.33f, Random.Range(minZ, maxZ));
        }
        return new Vector3(Random.Range(minX, maxX), 0.33f, Random.Range(minZ, maxZ));
    }
}
