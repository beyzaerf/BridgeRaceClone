using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    [SerializeField] private GameObject winScreen;

    public static GameManager Instance { get => instance; set => instance = value; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void GameWin()
    {
        Debug.Log("gamewon "); 
    }
}
