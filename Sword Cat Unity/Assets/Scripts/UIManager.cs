using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] List<GameObject> uiList = new List<GameObject>();
    [SerializeField] PlayerBehaviour playerController;

    // Start is called before the first frame update
    void Start()
    {
        SetActiveUI(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActiveUI(int index)
    {
        for (int i = 0; i < uiList.Count; i++)
        {
            uiList[i].SetActive(i == index);
        }
    }

    public void SetPlayerControlEnabled(bool enable)
    {
        playerController.enabled = enable;
    }
}
