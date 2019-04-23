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
#if UNITY_EDITOR_OSX
        vcam.m_XAxis.Value = Input.GetAxis("MacRightStickX");
        vcam.m_YAxis.Value= Input.GetAxis("MacRightStickY");
#endif

#if UNITY_EDITOR_64
        vcam.m_XAxis.Value = Input.GetAxis("WinRightStickX");
        vcam.m_YAxis.Value = Input.GetAxis("WInRightStickY");
#endif
    }
}
