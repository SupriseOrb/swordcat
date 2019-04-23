using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineFreeLook))]
public class CameraJoystickControls : MonoBehaviour
{
    private CinemachineFreeLook vcam;

    private void Awake()
    {
        vcam = this.GetComponent<CinemachineFreeLook>();
    }

    private void Update()
    {
        vcam.m_XAxis.Value = Input.GetAxis("RightStickX");
        vcam.m_YAxis.Value= Input.GetAxis("RightStickY");
    }
}
