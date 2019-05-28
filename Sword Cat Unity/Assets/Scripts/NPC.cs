using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] GameObject interactText;
    [SerializeField] CharacterDialogue dialogueManager;
    [SerializeField] NPCData characterData;

    public Quest quest;

    bool interacted = false;
    bool inside = false;
    bool runningDialogue = false;

    public NPCState state;
    JToken script;
    JToken options;
    JObject rqDialogue;

    JObject jObj;

    static List<NPC> interactQueue = new List<NPC>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        interacted = inside && !runningDialogue && interactQueue[0] == this && (interacted || Input.GetButtonDown("Fire1"));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (interactQueue.Count == 0)
            {
                interactText.SetActive(true);
            }

            inside = true;
            interactQueue.Add(this);
        }
    }

    IEnumerator OnTriggerStay(Collider other)
    {
        if (!runningDialogue && interacted && interactQueue[0] == this)
        {
            interacted = false;
            runningDialogue = true;

            if (state.quest == NPCState.QuestState.AVAILABLE)
            {
                if (state.randomQuest)
                {
                    rqDialogue = JObject.Parse(state.data.randomQuestScripts[state.randomQuestDialogue].text);
                    string[][] dialogue = rqDialogue["quest"].ToObject<string[][]>();

                    string[][] accept = rqDialogue["accept"].ToObject<string[][]>();
                    string[][] decline = rqDialogue["decline"].ToObject<string[][]>();
                    yield return dialogueManager.Dialogue(state.data.characterName, dialogue[Random.Range(0, dialogue.Length)],
                        accept[Random.Range(0, accept.Length)],
                        decline[Random.Range(0, decline.Length)], state.talked > 0, state.randomQuestType, state.randomQuestAmount);
                    state.talked++;

                    if (dialogueManager.yesSelected)
                    {
                        quest = new Quest();
                        switch (state.randomQuestType)
                        {
                            case TumbleYarn.YarnType.RED:
                                quest.red = state.randomQuestAmount;
                                break;
                            case TumbleYarn.YarnType.GREEN:
                                quest.green = state.randomQuestAmount;
                                break;
                            case TumbleYarn.YarnType.PURPLE:
                                quest.purple = state.randomQuestAmount;
                                break;
                        }
                        state.quest = NPCState.QuestState.ACTIVE;
                    }
                }
                else
                {
                    string[][] dialogue = script["quest"].ToObject<string[][]>();

                    string[][] accept = script["accept"].ToObject<string[][]>();
                    string[][] decline = script["decline"].ToObject<string[][]>();
                    yield return dialogueManager.Dialogue(state.data.characterName, dialogue[Random.Range(0, dialogue.Length)],
                        accept[Random.Range(0, accept.Length)],
                        decline[Random.Range(0, decline.Length)], state.talked > 0);
                    state.talked++;

                    if (dialogueManager.yesSelected)
                    {
                        quest = jObj["quest"]["fetch"].ToObject<Quest>();
                        state.quest = NPCState.QuestState.ACTIVE;
                    }
                }
            }
            else if (state.quest == NPCState.QuestState.ACTIVE)
            {
                if (quest.IsComplete())
                {
                    string[][] dialogue = script["complete"].ToObject<string[][]>();

                    if (state.randomQuest)
                    {
                        rqDialogue = JObject.Parse(state.data.randomQuestScripts[state.randomQuestDialogue].text);
                        dialogue = rqDialogue["complete"].ToObject<string[][]>();
                    }

                    yield return dialogueManager.Dialogue(state.data.characterName, dialogue[Random.Range(0, dialogue.Length)], state.talked > 0, state.randomQuestType, state.randomQuestAmount);
                    state.talked++;

                    quest.TakeYarn();

                    JToken token = jObj["quest"];

                    if (state.randomQuest)
                    {
                        state.quest = NPCState.QuestState.NONE;
                        state.randomQuestChance -= state.GetJsonData()["quest"]["random"]["modifier"] == null ? 0 : state.GetJsonData()["quest"]["random"]["modifier"].Value<float>();
                    }
                    else if (token["complete"] != null && token["complete"]["mode"].Value<string>() == "next")
                    {
                        state.state++;
                        ReloadScripts();
                    }
                    else
                    {
                        state.quest = NPCState.QuestState.COMPLETE;
                    }
                }
                else
                {
                    string[][] dialogue = script["incomplete"].ToObject<string[][]>();

                    if (state.randomQuest)
                    {
                        rqDialogue = JObject.Parse(state.data.randomQuestScripts[state.randomQuestDialogue].text);
                        dialogue = rqDialogue["incomplete"].ToObject<string[][]>();
                    }

                    yield return dialogueManager.Dialogue(state.data.characterName, dialogue[Random.Range(0, dialogue.Length)], state.talked > 0, state.randomQuestType, state.randomQuestAmount);
                    state.talked++;
                }
            }
            else
            {
                string[][] dialogue = script["idle"].ToObject<string[][]>();
                yield return dialogueManager.Dialogue(state.data.characterName, dialogue[Random.Range(0, dialogue.Length)], state.talked > 0);
                state.talked++;
            }

            runningDialogue = false;

            if (options != null && options["mode"].Value<string>() == "next" && state.state + 1 < state.data.dialogueScripts.Count)
            {
                state.state++;
                ReloadScripts();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            inside = false;
            interactQueue.Remove(this);

            if (interactQueue.Count == 0)
            {
                interactText.SetActive(false);
            }
        }
    }

    void ReloadScripts()
    {
        jObj = state.GetJsonData();
        script = jObj["scripts"];
        options = jObj["options"];
    }

    public void LoadState()
    {
        if (characterData == null)
            return;

        state = GameManager.instance.data.npcs.Find(npc => npc.data == characterData);

        if (state == null)
        {
            state = new NPCState() { data = characterData };
            GameManager.instance.data.npcs.Add(state);
        }

        ReloadScripts();

        if (options != null && options["mode"] != null && options["mode"].Value<string>() == "wait")
        {
            NPCState checkState = GameManager.instance.data.npcs.Find(npc => npc.data.characterName == options["wait"]["name"].Value<string>());
            if (checkState != null && checkState.state >= options["wait"]["state"].Value<int>())
            {
                state.state++;
                ReloadScripts();
            }
        }
        else if (state.quest == NPCState.QuestState.COMPLETE)
        {
            JToken token = jObj["quest"];
            if (token["complete"] != null && token["complete"]["mode"].Value<string>() == "wait")
            {
                NPCState checkState = GameManager.instance.data.npcs.Find(npc => npc.data.characterName == token["complete"]["wait"]["name"].Value<string>());
                if (checkState.state >= token["complete"]["wait"]["state"].Value<int>())
                {
                    state.state++;
                    ReloadScripts();
                }
            }
        }
        else if (state.quest == NPCState.QuestState.ACTIVE)
        {
            if (state.randomQuest)
            {
                quest = new Quest();
                switch (state.randomQuestType)
                {
                    case TumbleYarn.YarnType.RED:
                        quest.red = state.randomQuestAmount;
                        break;
                    case TumbleYarn.YarnType.GREEN:
                        quest.green = state.randomQuestAmount;
                        break;
                    case TumbleYarn.YarnType.PURPLE:
                        quest.purple = state.randomQuestAmount;
                        break;
                }
            }
            else
            {
                quest = jObj["quest"]["fetch"].ToObject<Quest>();
            }
        }
    }

    // 0: no quests available
    // 1: prereqs not met
    // 2: chance roll failed
    // 3: quest already active
    // 4: quest available
    public int RollQuestCondition()
    {
        if (characterData == null)
            return 0;

        JToken token = jObj["quest"];
        if (token == null || state.quest == NPCState.QuestState.COMPLETE)
            return 0;

        if (state.quest == NPCState.QuestState.ACTIVE)
            return 3;
        
        if (token["prereq"] != null)
        {
            string name = token["prereq"]["name"].Value<string>();

            NPCState npcState = GameManager.instance.data.npcs.Find(state => state.data.characterName == name);
            if (npcState == null || npcState.state < token["prereq"]["state"].Value<int>())
                return 1;
        }

        if (token["chance"] != null && Random.value > token["chance"].Value<float>())
            return 2;

        return 4;
    }
}

[System.Serializable]
public class Quest
{
    public int red;
    public int purple;
    public int green;

    public bool IsComplete()
    {
        int[] inventory = GameManager.instance.data.inventory;
        return inventory[0] >= red && inventory[1] >= purple && inventory[2] >= green;
    }

    public void TakeYarn()
    {
        int[] inventory = GameManager.instance.data.inventory;
        inventory[0] -= red;
        inventory[1] -= purple;
        inventory[2] -= green;
    }
}