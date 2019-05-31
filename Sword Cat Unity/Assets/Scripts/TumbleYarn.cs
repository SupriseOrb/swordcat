using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//[RequireComponent(typeof())]
public class TumbleYarn : MonoBehaviour
{
    public enum YarnType { RED, GREEN, PURPLE }

    public UnityEvent collected;

    public YarnType yarnType;
    [SerializeField]private float tumbleSpeed = 1f;
    [SerializeField]private float tumbleHeight = 1f;
    [SerializeField]private float tumbleHopRate = 1f;
    [SerializeField]private string groundTag;
    private Rigidbody rb;
    private bool onGround; //set true if on ground
    private float timeToHop; //changable tumbleHopRate value
    
    // Start is called before the first frame update
    void Start()
    {
        //find the component and save it for rb, if no rigidbody then attach a new one.
        rb = (GetComponent<Rigidbody>() != null) ? GetComponent<Rigidbody>() : gameObject.AddComponent<Rigidbody>();
        timeToHop = 0;
        onGround = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (NotInAir() && readyToHop())
        {
            rb.velocity += new Vector3(0, tumbleHeight, 0);
            yarnSetCooldown(tumbleHopRate);
        }

        Vector3 vec;

        var rand = Random.value;

        if(rand < 0.25)
        {
            vec = new Vector3(tumbleSpeed, 0, 0);
        }
        else if (rand > 0.25 && rand <= 0.5)
        {
            vec = new Vector3(-tumbleSpeed, 0, 0);
        }
        else if (rand > 0.5 && rand <= 0.75)
        {
            vec = new Vector3(0, 0, tumbleSpeed);
        }
        else
        {
            vec = new Vector3(0, 0, -tumbleSpeed);
        }

        rb.velocity += vec;

        //checks for in air yarn
        if (yarnOnCooldown())
            yarnSetCooldown(timeToHop - Time.fixedDeltaTime);
        else
            yarnSetCooldown(0);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == groundTag)
            onGround = true;

        if (collision.gameObject.CompareTag("Player"))
        {
            collected.Invoke();
        }
    }

    //a bool to check if the yarn is on ground and not in air
    bool NotInAir()
    {
        return (onGround == true);
    }

    //a bool to check if the yearn is in the air, not touching ground
    bool IsInAir()
    {
        return (onGround == false);
    }

    //a bool to check if the yarn should hop now
    bool readyToHop()
    {   
        return (timeToHop == 0);
    }

    //a bool to check if the yarn is not ready to hop yet
    bool yarnOnCooldown()
    {
        return (timeToHop > 0);
    }

    //setting the timeInAir by a set amount of time, so that we know when the tumbleweed should hop again.
    void yarnSetCooldown(float cooldown)
    {
        timeToHop = cooldown;
    }
}
