using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
Changes the UI based on the state of SwordCat's wings
 */
public class InteractUI : MonoBehaviour
{
    /*UI elements for the wings*/
    [SerializeField]private Image leftWing;
    [SerializeField]private Image rightWing;
    /*In charge of manipulating cooldowns of the wings and update the UI*/
    [SerializeField]private float wingCooldown = 5f;
    private float leftWingCooldown;
    private float rightWingCooldown;
    private TMP_Text leftWingUIText;
    private TMP_Text rightWingUIText;

    // Start is called before the first frame update
    void Start()
    {
        leftWingCooldown = 0;
        rightWingCooldown = 0;
        leftWingUIText = leftWing.GetComponentInChildren<TMP_Text>();
        rightWingUIText = rightWing.GetComponentInChildren<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //left wing used
        if (Input.GetMouseButtonDown(0) && (leftWingCooldown == 0))
        {
            UsedWing(leftWing);
            SetWingCooldown("L", wingCooldown);
        }   
        //right wing used
        else if (Input.GetMouseButtonDown(1))
        {
            UsedWing(rightWing);
            SetWingCooldown("R", wingCooldown);
        }

    }

    void FixedUpdate()
    {
        //count down the cooldown
        if (OnCooldown(leftWingCooldown))
        {
            leftWingCooldown -= Time.fixedDeltaTime;
            leftWingUIText.text = string.Format("{0:0.00}", leftWingCooldown);
        }
        //wing is ready
        else
        {
            SetWingCooldown("L", 0);
            ResetWing(leftWing);
            leftWingUIText.text = "";
        }

        //count down the cooldown
        if (OnCooldown(rightWingCooldown))
        {
            rightWingCooldown -= Time.fixedDeltaTime;
            rightWingUIText.text = string.Format("{0:0.00}", rightWingCooldown);
        }
        //wing is ready
        else
        {
            SetWingCooldown("R", 0);
            ResetWing(rightWing);
            rightWingUIText.text = "";
        }
    }

    /*updates the UI to show the wing has been used. Greying out box.
    we save the current color hex of the image and grey it out a bit.*/
    void UsedWing(Image wing)
    {
        Color32 tempColor = wing.GetComponent<Image>().color;
        wing.GetComponent<Image>().color = new Color32(tempColor.r, tempColor.g, tempColor.b, 122);
    }

    /*updates the UI to show that the wing is ready to be used again. Brightening up box.
    we save the current hex of the image and brighten it up.*/
    void ResetWing(Image wing)
    {
        Color32 tempColor = wing.GetComponent<Image>().color;
        wing.GetComponent<Image>().color = new Color32(tempColor.r, tempColor.g, tempColor.b, 255);
    }

    /*when a wing is used, it will go on cooldown based on the set cooldown rate.
    we use a string to check which wing to update. We set that specific wing's 
    cooldown by a float that we also put into the function*/
    void SetWingCooldown(string wingType, float setTimer)
    {
        if (wingType.ToLower() == "l" || wingType.ToLower() == "left")
            leftWingCooldown = setTimer;
        else if (wingType.ToLower() == "r" || wingType.ToLower() == "right")
            rightWingCooldown = setTimer;
        else
            return;
    }

    /*checks if the wing is on cooldown.*/
    bool OnCooldown(float wingTime)
    {
        return (wingTime > 0.5);
    }
}
