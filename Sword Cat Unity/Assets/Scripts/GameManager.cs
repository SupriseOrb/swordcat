using Newtonsoft.Json.Linq;
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
            instance.ReloadNPCs();
            return;
        }

        data = data ?? new GameData();
        DontDestroyOnLoad(gameObject);
        instance = this;

        ReloadNPCs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReloadNPCs()
    {
        List<NPC> giveQuest = new List<NPC>();
        List<NPC> chanceFailed = new List<NPC>();

        foreach (NPC npc in FindObjectsOfType<NPC>())
        {
            npc.LoadState();
            switch (npc.RollQuestCondition())
            {
                case 2:
                    chanceFailed.Add(npc);
                    break;
                case 4:
                    giveQuest.Add(npc);
                    break;
            }
        }

        if (giveQuest.Count == 0 && chanceFailed.Count > 0)
        {
            giveQuest.Add(chanceFailed[Random.Range(0, chanceFailed.Count)]);
        }

        foreach (NPC npc in giveQuest)
        {
            npc.state.quest = NPCState.QuestState.AVAILABLE;

            if (npc.state.data.randomQuestScripts.Count > 0 && npc.state.GetJsonData()["quest"]["random"] != null)
            {
                if (npc.state.randomQuestChance == -1)
                    npc.state.randomQuestChance = npc.state.GetJsonData()["quest"]["random"]["chance"].Value<float>();
                npc.state.randomQuest = Random.value < npc.state.randomQuestChance;

                if (npc.state.randomQuest)
                {
                    npc.state.randomQuestDialogue = Random.Range(0, npc.state.data.randomQuestScripts.Count);
                    npc.state.randomQuestType = (TumbleYarn.YarnType) Random.Range(0, 3);
                    switch (npc.state.randomQuestType)
                    {
                        case TumbleYarn.YarnType.RED:
                            npc.state.randomQuestAmount = Random.Range(8, 21);
                            break;
                        case TumbleYarn.YarnType.GREEN:
                            npc.state.randomQuestAmount = Random.Range(6, 16);
                            break;
                        case TumbleYarn.YarnType.PURPLE:
                            npc.state.randomQuestAmount = Random.Range(5, 11);
                            break;
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class GameData
{
    public int[] inventory = new int[3];
    public List<NPCState> npcs = new List<NPCState>();
}

[System.Serializable]
public class NPCState
{
    public enum QuestState { NONE, AVAILABLE, ACTIVE, COMPLETE };
    public NPCData data;
    public int state
    {
        get
        {
            return index;
        }

        set
        {
            quest = QuestState.NONE;
            talked = 0;
            randomQuest = false;
            randomQuestChance = GetJsonData()["quest"] != null && GetJsonData()["quest"]["random"] != null ? GetJsonData()["quest"]["random"]["chance"].Value<float>() : 0;
            index = value;
        }
    }
    [SerializeField] int index;
    public QuestState quest;
    public int talked;

    public float randomQuestChance = -1;
    public bool randomQuest;
    public int randomQuestDialogue;
    public TumbleYarn.YarnType randomQuestType;
    public int randomQuestAmount;

    public JObject GetJsonData()
    {
        return JObject.Parse(data.dialogueScripts[state].text);
    }
}