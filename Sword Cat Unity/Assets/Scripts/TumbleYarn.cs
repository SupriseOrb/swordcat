using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TumbleYarn : MonoBehaviour
{
    [SerializeField]private float tumbleSpeed = 1f;
    [SerializeField]private float tumbleHeight = 1f;
    [SerializeField]private float tumbleHopRate = 1f;
    private Rigidbody rb;
    private float timeInAir; //changable tumbleHopRate value
    
    // Start is called before the first frame update
    void Start()
    {
        //find the component and save it for rb, if no rigidbody then attach a new one.
        rb = (GetComponent<Rigidbody>() != null) ? GetComponent<Rigidbody>() : gameObject.AddComponent<Rigidbody>();
        timeInAir = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (IsInAir())
        {
            rb.velocity += new Vector3(0, tumbleHeight, 0);
            setAirCooldown(tumbleHopRate);
        }

        rb.velocity += new Vector3(tumbleSpeed, 0, 0);

        //checks for in air yarn
        if (NotInAir())
            timeInAir = 0;
        else
            timeInAir -= Time.fixedDeltaTime;
    }

    //a bool to check if the timeInAir is less than or equal to 0
    bool NotInAir()
    {
        return (timeInAir <= 0);
    }

    //a bool to check if the timeInAir is 0
    bool IsInAir()
    {
        return (timeInAir == 0);
    }

    //setting the timeInAir by a set amount of time, so that we know when the tumbleweed should hop again.
    void setAirCooldown(float cooldown)
    {
        timeInAir = cooldown;
    }
}
