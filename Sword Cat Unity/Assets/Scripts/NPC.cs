using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] GameObject interactText;
    [SerializeField] TextAsset dialogueScript;
    [SerializeField] CharacterDialogue dialogueManager;

    // Placeholders for testing dialogue
    public bool questAvailable;
    public bool questInProgress;
    public bool questComplete;

    bool interacted = false;
    bool inside = false;
    bool runningDialogue = false;

    string characterName;
    JToken script;

    static List<NPC> interactQueue = new List<NPC>();

    void Awake()
    {
        JObject jObj = JObject.Parse(dialogueScript.text);
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
            string[][] dialogue = script["idle"].ToObject<string[][]>();

            if (questAvailable)
            {
                dialogue = script["quest"].ToObject<string[][]>();
            }
            else if (questInProgress)
            {
                dialogue = script["incomplete"].ToObject<string[][]>();
            }
            else if (questComplete)
            {
                dialogue = script["complete"].ToObject<string[][]>();
            }

            if (questAvailable)
            {
                string[][] accept = script["accept"].ToObject<string[][]>();
                string[][] decline = script["decline"].ToObject<string[][]>();
                yield return dialogueManager.Dialogue(characterName, dialogue[Random.Range(0, dialogue.Length)],
                    accept[Random.Range(0, accept.Length)],
                    decline[Random.Range(0, decline.Length)]);

                if (dialogueManager.yesSelected)
                {
                    // give quest
                }
            }
            else
            {
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
