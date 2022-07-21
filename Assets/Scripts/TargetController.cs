using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField] private List<int> targets;
    private int randomBridge;

    public List<int> Targets { get => targets; set => targets = value; }

    public void AddRandom()
    {
        randomBridge = Random.Range(0, 3);
        if (Targets.Count == 0)
        {
            Targets.Add(randomBridge);
            return;
        }
        Targets.Add(randomBridge);
        for (int i = 0; i < Targets.Count - 1; i++)
        {
            if (randomBridge == Targets[i])
            {
                Targets.Remove(randomBridge);
                AddRandom();
            }
        }
    }
}
