using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour, Attachable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void attach(GameObject sword)
    {
        sword.transform.SetParent(this.gameObject.transform);
    }
}
