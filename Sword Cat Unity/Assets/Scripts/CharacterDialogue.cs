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

    IEnumerator RunDialogue(string[] dialogue, bool skipAtOnce = false, TumbleYarn.YarnType color = TumbleYarn.YarnType.RED, int amount = 0)
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
                AkSoundEngine.PostEvent("Advance_text", gameObject);
                string readout = line;
                switch (color)
                {
                    case TumbleYarn.YarnType.RED:
                        readout = readout.Replace("{color}", "red");
                        readout = readout.Replace("{Color}", "Red");
                        break;
                    case TumbleYarn.YarnType.GREEN:
                        readout = readout.Replace("{color}", "green");
                        readout = readout.Replace("{Color}", "Green");
                        break;
                    case TumbleYarn.YarnType.PURPLE:
                        readout = readout.Replace("{color}", "purple");
                        readout = readout.Replace("{Color}", "Purple");
                        break;
                }

                readout = readout.Replace("{amount}", amount.ToString());
                for (int i = 0; i < readout.Length; i++)
                {
                    while (i < readout.Length && readout[i] == '<')
                    {
                        while (i < readout.Length && readout[i] != '>')
                        {
                            i++;
                        }

                        i++;
                    }

                    uiDialogue.text = readout.Substring(0, Mathf.Min(i + 1, readout.Length));

                    if (Input.GetButton("Interact"))
                    {
                        if (i % 2 == 0)
                            yield return null;
                    }
                    else
                    {
                        yield return new WaitForSeconds(0.05f);
                    }
                }

                yield return new WaitUntil(() => Input.GetButtonDown("Interact"));
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

    public IEnumerator Dialogue(string name, string[] dialogue, string[] onAccept, string[] onDecline, bool skipAtOnce = false, TumbleYarn.YarnType color = TumbleYarn.YarnType.RED, int amount = 0)
    {
        uiName.text = name;
        uiDialogue.text = "";

        uiManager.SetActiveUI(1);
        uiManager.SetPlayerControlEnabled(false);

        yield return RunDialogue(dialogue, skipAtOnce, color, amount);

        if (onAccept != null && onDecline != null)
        {
            yield return Ask();

            if (yesSelected)
            {
                yield return RunDialogue(onAccept, skipAtOnce, color, amount);
            }
            else
            {
                yield return RunDialogue(onDecline, skipAtOnce, color, amount);
            }
        }

        uiManager.SetActiveUI(0);
        uiManager.SetPlayerControlEnabled(true);
        uiName.text = "";
        uiDialogue.text = "";
    }

    public IEnumerator Dialogue(string name, string[] dialogue, bool skipAtOnce = false, TumbleYarn.YarnType color = TumbleYarn.YarnType.RED, int amount = 0)
    {
        return Dialogue(name, dialogue, null, null, skipAtOnce, color, amount);
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