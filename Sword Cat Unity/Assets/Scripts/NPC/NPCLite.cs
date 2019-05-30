using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCLite : MonoBehaviour
{
    [SerializeField] GameObject interactText;
    [SerializeField] CharacterDialogue dialogueManager;
    [SerializeField] TextAsset dialogue;

    bool interacted = false;
    bool inside = false;
    bool runningDialogue = false;

    string characterName;
    string[][] idleDialogue;

    // Start is called before the first frame update
    void Start()
    {
        JObject jObj = JObject.Parse(dialogue.text);
        characterName = jObj["name"] == null ? "Unnamed NPC" : jObj["name"].Value<string>();
        idleDialogue = jObj["scripts"]["idle"].ToObject<string[][]>(); ;
    }

    // Update is called once per frame
    void Update()
    {
        interacted = inside && !runningDialogue && NPC.interactQueue[0] == this && (interacted || Input.GetButtonDown("Interact"));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (NPC.interactQueue.Count == 0)
            {
                interactText.SetActive(true);
            }

            inside = true;
            NPC.interactQueue.Add(this);
        }
    }

    IEnumerator OnTriggerStay(Collider other)
    {
        if (!runningDialogue && interacted && NPC.interactQueue[0] == this)
        {
            interacted = false;
            runningDialogue = true;
            yield return dialogueManager.Dialogue(characterName, idleDialogue[Random.Range(0, idleDialogue.Length)]);
            runningDialogue = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            inside = false;
            NPC.interactQueue.Remove(this);

            if (NPC.interactQueue.Count == 0)
            {
                interactText.SetActive(false);
            }
        }
    }
}