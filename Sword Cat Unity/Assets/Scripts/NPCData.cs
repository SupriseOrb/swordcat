using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPC.asset", menuName = "NPC Data")]
public class NPCData : ScriptableObject
{
    public string characterName;
    public List<TextAsset> dialogueScripts = new List<TextAsset>();
    public List<TextAsset> randomQuestScripts = new List<TextAsset>();
}
