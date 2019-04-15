using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDialogue : MonoBehaviour
{
    [SerializeField] TextAsset dialogueScript;

    [SerializeField] GameObject dialogueUI;

    [SerializeField] Text uiName;
    [SerializeField] Text uiDialogue;

    string characterName;
    JToken script;
    
    void Awake()
    {
        JObject jObj = JObject.Parse(dialogueScript.text);
        characterName = jObj["name"].Value<string>();
        script = jObj["scripts"];
    }

    public void RunDialogue(string key)
    {
        JToken next = script[key];
        
        if (next != null)
        {
            string[][] dialogue = script[key].ToObject<string[][]>();
            StartCoroutine(Dialogue(dialogue[Random.Range(0, dialogue.Length)]));
        }
    }

    IEnumerator Dialogue(string[] dialogue)
    {
        uiName.text = characterName;
        uiDialogue.text = "";

        dialogueUI.SetActive(true);

        foreach (string line in dialogue)
        {
            for (int i = 0; i < line.Length; i++)
            {
                uiDialogue.text = line.Substring(0, i + 1);

                yield return null;
                if (!Input.GetButton("Fire1"))
                {
                    yield return null;
                    yield return null;
                }
            }

            yield return new WaitUntil(() => Input.GetButtonDown("Fire1"));
        }

        dialogueUI.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}