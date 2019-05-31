using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC.asset", menuName = "NPC Data")]
public class NPCData : ScriptableObject
{
    public string characterName;
    public List<TextAsset> dialogueScripts = new List<TextAsset>();
    public List<TextAsset> randomQuestScripts = new List<TextAsset>();

    public static List<NPCData> dats = new List<NPCData>();

    void OnEnable()
    {
        foreach (TextAsset text in dialogueScripts)
        {
            try
            {
                JObject.Parse(text.text);
            }
            catch (JsonReaderException e)
            {
                Debug.LogError($"Error in json file {name}: {e.Message}" + e.Message, text);
            }
        }
        foreach (TextAsset text in randomQuestScripts)
        {
            try
            {
                JObject.Parse(text.text);
            }
            catch (JsonReaderException e)
            {
                Debug.LogError($"Error in json file {name}: {e.Message}", text);
            }
        }

        if (!dats.Contains(this))
            dats.Add(this);
    }
}
