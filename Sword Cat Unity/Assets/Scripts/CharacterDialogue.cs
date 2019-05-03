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

    IEnumerator RunDialogue(string[] dialogue, bool skipAtOnce)
    {
        bool localYes = false;
        bool localNo = false;

        string block = null;

        foreach (string line in dialogue)
        {
            bool readLine = true;

            if (block == "yes")
            {
                if (line == "@endyes")
                {
                    block = null;
                    readLine = false;
                }
                else
                {
                    readLine = localYes;
                }
            }
            else if (block == "no")
            {
                if (line == "@endno")
                {
                    block = null;
                    readLine = false;
                }
                else
                {
                    readLine = localNo;
                }
            }
            else if (block == "once")
            {
                if (line == "@endonce")
                {
                    block = null;
                    readLine = false;
                }
                else
                {
                    readLine = !skipAtOnce;
                }
            }
            else
            {
                if (line == "@ask")
                {
                    yield return Ask();
                    localYes = yesSelected;
                    localNo = noSelected;
                    readLine = false;
                }
                else if (line == "@yes")
                {
                    block = "yes";
                    readLine = false;
                }
                else if (line == "@no")
                {
                    block = "no";
                    readLine = false;
                }
                else if (line == "@once")
                {
                    block = "once";
                    readLine = false;
                }
            }

            if (readLine)
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
    }

    IEnumerator Ask()
    {
        yesSelected = false;
        noSelected = false;
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
        yield return new WaitUntil(() => yesSelected || noSelected);
        yesButton.gameObject.SetActive(false);
        noButton.gameObject.SetActive(false);
    }

    public IEnumerator Dialogue(string name, string[] dialogue, string[] onAccept, string[] onDecline, bool skipAtOnce)
    {
        uiName.text = name;
        uiDialogue.text = "";

        uiManager.SetActiveUI(1);
        uiManager.SetPlayerControlEnabled(false);

        yield return RunDialogue(dialogue, skipAtOnce);

        if (onAccept != null && onDecline != null)
        {
            yield return Ask();

            if (yesSelected)
            {
                yield return RunDialogue(onAccept, skipAtOnce);
            }
            else
            {
                yield return RunDialogue(onDecline, skipAtOnce);
            }
        }

        uiManager.SetActiveUI(0);
        uiManager.SetPlayerControlEnabled(true);
        uiName.text = "";
        uiDialogue.text = "";
    }

    public IEnumerator Dialogue(string name, string[] dialogue, bool skipAtOnce)
    {
        return Dialogue(name, dialogue, null, null, skipAtOnce);
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