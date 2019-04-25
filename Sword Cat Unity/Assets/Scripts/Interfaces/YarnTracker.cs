using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class YarnTracker : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI redYarnAmount;
    [SerializeField] TextMeshProUGUI greenYarnAmount;
    [SerializeField] TextMeshProUGUI purpleYarnAmount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        redYarnAmount.text = GameManager.instance.data.inventory[(int) TumbleYarn.YarnType.RED].ToString();
        greenYarnAmount.text = GameManager.instance.data.inventory[(int)TumbleYarn.YarnType.GREEN].ToString();
        purpleYarnAmount.text = GameManager.instance.data.inventory[(int)TumbleYarn.YarnType.PURPLE].ToString();
    }
}
