using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] Button newGameButton;
    [SerializeField] Button continueButton;
    [SerializeField] Button quitButton;
    bool clicked = false;

    // Start is called before the first frame update
    void Start()
    {
        newGameButton.onClick.AddListener(NewGame);

        if (File.Exists(GameManager.instance.saveDataPath))
        {
            continueButton.onClick.AddListener(Continue);
        }
        else
        {
            continueButton.interactable = false;
        }
        quitButton.onClick.AddListener(Quit);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NewGame()
    {
        if (clicked)
            return;
        clicked = true;
        Fader.instance.FadeEffect(() => { return SceneManager.LoadSceneAsync("OasisTown"); });
        
    }

    public void Continue()
    {
        if (clicked)
            return;
        clicked = true;
        GameManager.instance.Load();
        Fader.instance.FadeEffect(() => { return SceneManager.LoadSceneAsync("OasisTown"); });
    }

    public void Quit()
    {
        if (clicked)
            return;
        clicked = true;
        Application.Quit();
    }
}
