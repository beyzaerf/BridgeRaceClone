using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] private Transform targetController;
    public static ObjectManager instance;

    public Transform TargetController { get => targetController; set => targetController = value; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

}
