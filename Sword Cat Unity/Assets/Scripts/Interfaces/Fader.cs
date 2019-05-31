using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public static Fader instance;
    Image image;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        instance = this;

        image = GetComponentInChildren<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeEffect(System.Func<YieldInstruction> func)
    {
        AkSoundEngine.PostEvent("Stop_Footsteps", GameObject.Find("Player")); //Stop the footsteps from playing in between scenes

        IEnumerator Effect()
        {
            image.fillClockwise = true;
            for (int i = 0; i < 30; i++)
            {
                image.fillAmount = i / 29f;
                yield return null;
            }
            yield return func();
            image.fillClockwise = false;
            for (int i = 0; i < 30; i++)
            {
                image.fillAmount = 1 - i / 29f;
                yield return null;
            }
        }
        StartCoroutine(Effect());
    }
}
