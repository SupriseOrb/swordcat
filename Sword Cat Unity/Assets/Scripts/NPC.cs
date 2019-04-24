using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] GameObject interactText;
    [SerializeField] TextAsset dialogueScript;
    [SerializeField] CharacterDialogue dialogueManager;

    public Quest quest;
    
    public bool questAvailable;
    public bool questActive;

    bool interacted = false;
    bool inside = false;
    bool runningDialogue = false;

    string characterName;
    JToken script;

    JObject jObj;

    static List<NPC> interactQueue = new List<NPC>();

    void Awake()
    {
        jObj = JObject.Parse(dialogueScript.text);
        characterName = jObj["name"].Value<string>();
        script = jObj["scripts"];
    }

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

            if (questAvailable && jObj["quest"] != null && jObj["quest"]["type"].Value<string>() == "fetch")
            {
                string[][] dialogue = script["quest"].ToObject<string[][]>();

                string[][] accept = script["accept"].ToObject<string[][]>();
                string[][] decline = script["decline"].ToObject<string[][]>();
                yield return dialogueManager.Dialogue(characterName, dialogue[Random.Range(0, dialogue.Length)],
                    accept[Random.Range(0, accept.Length)],
                    decline[Random.Range(0, decline.Length)]);

                if (dialogueManager.yesSelected)
                {
                    quest = jObj["quest"]["fetch"].ToObject<Quest>();
                    questAvailable = false;
                    questActive = true;
                }
            }
            else if (questActive)
            {
                if (quest.IsComplete())
                {
                    quest.TakeYarn();
                    questActive = false;
                    string[][] dialogue = script["complete"].ToObject<string[][]>();
                    yield return dialogueManager.Dialogue(characterName, dialogue[Random.Range(0, dialogue.Length)]);
                }
                else
                {
                    string[][] dialogue = script["incomplete"].ToObject<string[][]>();
                    yield return dialogueManager.Dialogue(characterName, dialogue[Random.Range(0, dialogue.Length)]);
                }
            }
            else
            {
                string[][] dialogue = script["idle"].ToObject<string[][]>();
                yield return dialogueManager.Dialogue(characterName, dialogue[Random.Range(0, dialogue.Length)]);
            }

            runningDialogue = false;
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