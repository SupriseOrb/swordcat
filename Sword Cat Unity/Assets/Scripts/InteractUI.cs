using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*Changes the UI based on the state of SwordCat's wings*/
public class InteractUI : MonoBehaviour
{
    /*UI elements for the wings*/
    [SerializeField]private Image leftWing;
    [SerializeField]private Image rightWing;
    /*In charge of manipulating cooldowns of the wings and update the UI*/
    [SerializeField]private GameObject leftSwordHolsterObject;
    [SerializeField]private GameObject rightSwordHolsterObject;
    private float leftSwordHolsterCooldown;
    private float rightSwordHolsterCooldown;
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
        leftSwordHolsterCooldown = leftSwordHolsterObject.GetComponent<SwordHolster>().swordDespawnTime;
        rightSwordHolsterCooldown = rightSwordHolsterObject.GetComponent<SwordHolster>().swordDespawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        //left wing controller
        if ((leftSwordHolsterObject.GetComponent<SwordHolster>().swordLaunched) && (leftWingCooldown == 0))
        {
            UsedWing(leftWing);
            SetWingCooldown("L", leftSwordHolsterCooldown);
        }   
        //right wing controller
        else if ((rightSwordHolsterObject.GetComponent<SwordHolster>().swordLaunched) && (rightWingCooldown == 0))
        {
            UsedWing(rightWing);
            SetWingCooldown("R", rightSwordHolsterCooldown);
        }

    }

    void FixedUpdate()
    {
        //LEFT WING
        //count down the cooldown, showing the float by 2 decimals
        if (leftSwordHolsterObject.GetComponent<SwordHolster>().swordLaunched)
        {
            leftWingCooldown -= Time.fixedDeltaTime;
            if (leftWingCooldown < 0)
                leftWingCooldown = 0;
            leftWingUIText.text = string.Format("{0:0.00}", leftWingCooldown);
            //leftWing.fillAmount -= 1f / leftSwordHolsterCooldown * Time.fixedDeltaTime;
        }
        //wing is ready
        else
        {
            SetWingCooldown("L", 0);
            ResetWing(leftWing);
            leftWingUIText.text = "";
        }

        //RIGHT WING
        //count down the cooldown, showing the float by 2 decimals
        if (rightSwordHolsterObject.GetComponent<SwordHolster>().swordLaunched)
        {
            rightWingCooldown -= Time.fixedDeltaTime;
            if (rightWingCooldown < 0)
                rightWingCooldown = 0;
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
//        Debug.Log(wing.GetComponent<Image>().color);
    }

    /*updates the UI to show that the wing is ready to be used again. Brightening up box.
    we save the current hex of the image and brighten it up.*/
    void ResetWing(Image wing)
    {
        Color32 tempColor = wing.GetComponent<Image>().color;
        wing.GetComponent<Image>().color = new Color32(tempColor.r, tempColor.g, tempColor.b, 255);
//        Debug.Log(wing.GetComponent<Image>().color);
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
    bool IsOnCooldown(float wingTime)
    {
        return (wingTime >= 0);
    }
}
