using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingZone : MonoBehaviour
{
    [SerializeField] string newSceneName;
    [SerializeField] PlayerBehaviour player;
    static bool triggered = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !triggered)
        {
            triggered = true;
            player.enabled = false;
            Fader.instance.FadeEffect(() => { triggered = false; return SceneManager.LoadSceneAsync(newSceneName); });
        }
    }
}
