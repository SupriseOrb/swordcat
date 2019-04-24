using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameData data;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        data = new GameData();
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public class GameData
{
    public int[] inventory = new int[3];
}