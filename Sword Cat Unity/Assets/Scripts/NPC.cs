﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] GameObject interactText;
    [SerializeField] CharacterDialogue dialogueManager;
    [SerializeField] NPCData characterData;

    [SerializeField] GameObject exclamationMarkRed;
    [SerializeField] GameObject exclamationMarkBlue;

    public Quest quest;

    bool interacted = false;
    bool inside = false;
    bool runningDialogue = false;

    public NPCState state;
    JToken script;
    JToken options;
    JObject rqDialogue;

    JObject jObj;

    public static List<MonoBehaviour> interactQueue = new List<MonoBehaviour>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        interacted = inside && !runningDialogue && interactQueue[0] == this && (interacted || Input.GetButtonDown("Interact"));
        if (exclamationMarkRed != null)
            exclamationMarkRed.SetActive(state.quest == NPCState.QuestState.AVAILABLE);
        if (exclamationMarkBlue != null)
            exclamationMarkBlue.SetActive(state.quest == NPCState.QuestState.ACTIVE && quest.IsComplete());
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
                    string[][] dialogue = NPCLite.getDialogueFromJson(rqDialogue["quest"]);

                    string[][] accept = NPCLite.getDialogueFromJson(rqDialogue["accept"]);
                    string[][] decline = NPCLite.getDialogueFromJson(rqDialogue["decline"]);
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
                    string[][] dialogue = NPCLite.getDialogueFromJson(script["quest"]);

                    string[][] accept = NPCLite.getDialogueFromJson(script["accept"]);
                    string[][] decline = NPCLite.getDialogueFromJson(script["decline"]);
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
                    string[][] dialogue;

                    if (state.randomQuest)
                    {
                        rqDialogue = JObject.Parse(state.data.randomQuestScripts[state.randomQuestDialogue].text);
                        dialogue = NPCLite.getDialogueFromJson(rqDialogue["complete"]);
                    }
                    else
                    {
                        dialogue = NPCLite.getDialogueFromJson(script["complete"]);
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
                        state.quest = NPCState.QuestState.NEXT;
                    }
                    else
                    {
                        state.quest = NPCState.QuestState.COMPLETE;
                    }
                }
                else
                {
                    string[][] dialogue;

                    if (state.randomQuest)
                    {
                        rqDialogue = JObject.Parse(state.data.randomQuestScripts[state.randomQuestDialogue].text);
                        dialogue = NPCLite.getDialogueFromJson(rqDialogue["incomplete"]);
                    }
                    else
                    {
                        dialogue = NPCLite.getDialogueFromJson(script["incomplete"]);
                    }

                    yield return dialogueManager.Dialogue(state.data.characterName, dialogue[Random.Range(0, dialogue.Length)], state.talked > 0, state.randomQuestType, state.randomQuestAmount);
                    state.talked++;
                }
            }
            else
            {
                string[][] dialogue = NPCLite.getDialogueFromJson(script["idle"]);
                yield return dialogueManager.Dialogue(state.data.characterName, dialogue[Random.Range(0, dialogue.Length)], state.talked > 0);
                state.talked++;
            }

            runningDialogue = false;

            if (options != null && options["mode"] != null && options["mode"].Value<string>() == "next" && state.state + 1 < state.data.dialogueScripts.Count)
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
        if (jObj != null)
        {
            script = jObj["scripts"];
            options = jObj["options"];
        }
    }

    public void LoadState()
    {
        if (characterData == null)
            return;

        state = GameManager.instance.data.npcs.Find(npc => npc.data == characterData);

        if (state == null)
        {
            state = new NPCState() { characterName = characterData.characterName };
            GameManager.instance.data.npcs.Add(state);
        }

        if (state.quest == NPCState.QuestState.NEXT)
        {
            state.state++;
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
        if (characterData == null || jObj == null)
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
    public int green;
    public int purple;

    public bool IsComplete()
    {
        int[] inventory = GameManager.instance.data.inventory;
        return inventory[0] >= red && inventory[1] >= green && inventory[2] >= purple;
    }

    public void TakeYarn()
    {
        int[] inventory = GameManager.instance.data.inventory;
        inventory[0] -= red;
        inventory[1] -= green;
        inventory[2] -= purple;
    }
}