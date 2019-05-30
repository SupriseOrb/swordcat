using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Saving : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI saveText;
    bool inside = false;
    bool saved = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!saved && inside && Input.GetButtonDown("Interact"))
        {
            GameManager.instance.Save();
            saveText.text = "Saved";
            saved = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            saveText.gameObject.SetActive(true);
            inside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            saveText.gameObject.SetActive(false);
            saveText.text = "Press button to save";
            inside = false;
            saved = false;
        }
    }
}
