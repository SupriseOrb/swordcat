using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterDialogue : MonoBehaviour
{
    [SerializeField] UIManager uiManager;

    [SerializeField] TextMeshProUGUI uiName;
    [SerializeField] TextMeshProUGUI uiDialogue;

    [SerializeField] Button yesButton;
    [SerializeField] Button noButton;

    public bool yesSelected { get; private set; } = false;
    public bool noSelected { get; private set; } = false;

    void OnEnable()
    {
        yesButton.onClick.AddListener(() => yesSelected = true);
        noButton.onClick.AddListener(() => noSelected = true);
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }

    IEnumerator RunDialogue(string[] dialogue)
    {
        foreach (string line in dialogue)
        {
            for (int i = 0; i < line.Length; i++)
            {
                while (i < line.Length && line[i] == '<')
                {
                    while (i < line.Length && line[i] != '>')
                    {
                        i++;
                    }

                    i++;
                }

                uiDialogue.text = line.Substring(0, Mathf.Min(i + 1, line.Length));

                yield return null;
                if (!Input.GetButton("Fire1"))
                {
                    yield return null;
                    yield return null;
                }
            }

            yield return new WaitUntil(() => Input.GetButtonDown("Fire1"));
        }
    }

    public IEnumerator Dialogue(string name, string[] dialogue, string[] onAccept, string[] onDecline)
    {
        uiName.text = name;
        uiDialogue.text = "";

        uiManager.SetActiveUI(1);
        uiManager.SetPlayerControlEnabled(false);

        yield return RunDialogue(dialogue);

        if (onAccept != null && onDecline != null)
        {
            yesSelected = false;
            noSelected = false;
            yesButton.gameObject.SetActive(true);
            noButton.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
            yield return new WaitUntil(() => yesSelected || noSelected);
            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);

            if (yesSelected)
            {
                yield return RunDialogue(onAccept);
            }
            else
            {
                yield return RunDialogue(onDecline);
            }
        }

        uiManager.SetActiveUI(0);
        uiManager.SetPlayerControlEnabled(true);
        uiName.text = "";
        uiDialogue.text = "";
    }

    public IEnumerator Dialogue(string name, string[] dialogue)
    {
        return Dialogue(name, dialogue, null, null);
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