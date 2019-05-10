using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineFreeLook))]
public class CameraJoystickControls : MonoBehaviour
{

    public float m_YMin;
    public float m_YMax;

    private CinemachineFreeLook vcam;

    private void Awake()
    {
        vcam = this.GetComponent<CinemachineFreeLook>();
    }

    private void Start()
    {
#if UNITY_EDITOR_OSX
        vcam.m_XAxis.m_InputAxisName = "MacRightStickX";
        vcam.m_YAxis.m_InputAxisName = "MacRightStickY";
#endif

#if UNITY_EDITOR_WIN
        vcam.m_XAxis.m_InputAxisName = "WinRightStickX";
        vcam.m_YAxis.m_InputAxisName = "WinRightStickY";
#endif
    }

    private void Update()
    {
        vcam.m_YAxis.Value = Mathf.Clamp(vcam.m_YAxis.Value, m_YMin, m_YMax);
    }
}
