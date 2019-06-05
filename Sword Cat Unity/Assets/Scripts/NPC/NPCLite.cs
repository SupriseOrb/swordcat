using Newtonsoft.Json;
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

    public static string[][] getDialogueFromJson(JToken jToken)
    {
        List<string[]> id = new List<string[]>();
        foreach (JToken a in jToken)
        {
            List<string> idd = new List<string>();
            foreach (string i in a.Values<string>())
            {
                idd.Add(i);
            }
            id.Add(idd.ToArray());
        }
        return id.ToArray();
    }

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            JObject jObj = JObject.Parse(dialogue.text);
            characterName = jObj["name"] == null ? "Unnamed NPC" : jObj["name"].Value<string>();
            idleDialogue = getDialogueFromJson(jObj["scripts"]["idle"]);
        }
        catch(JsonReaderException e)
        {
            Debug.LogError($"Error in json file {dialogue.name}: {e.Message}", dialogue);
        }
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