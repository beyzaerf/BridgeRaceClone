using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameManager instance;
    [SerializeField] private GameObject winScreen;


    private void Awake()
    {
        if (instance == null)
            instance = this;
    }


}
