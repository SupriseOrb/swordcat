using Newtonsoft.Json.Linq;
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

        data = data ?? new GameData();
        DontDestroyOnLoad(gameObject);
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
    public List<NPCState> npcs = new List<NPCState>();
}

[Serializable]
public class NPCState
{
    public enum QuestState { NONE, AVAILABLE, ACTIVE, COMPLETE };
    public NPCData data;
    public int state;
    public QuestState quest;

    public JObject GetJsonData()
    {
        return JObject.Parse(data.dialogueScripts[state].text);
    }
}