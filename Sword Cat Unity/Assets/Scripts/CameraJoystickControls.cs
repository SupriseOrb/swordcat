using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class CameraJoystickControls : MonoBehaviour
{

    public float m_YMin;
    public float m_YMax;

    public float m_XMin;
    public float m_XMax;

    [Range(0, 10)]
    [SerializeField] float sensitivityY = 1f;

    [Range(0, 10)]
    [SerializeField] float sensitivityX = 1f;

    public UnityEvent RotatePlayer;

    private string rightX;
    private string rightY;

    private CinemachineVirtualCamera vcam;
    private CinemachineComposer composer;

    private void Awake()
    {
        vcam = this.GetComponent<CinemachineVirtualCamera>();
        composer = vcam.GetCinemachineComponent<CinemachineComposer>();

    }

    private void Start()
    {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        rightX = "MacRightStickX";
        rightY = "MacRightStickY";
#endif

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        rightX = "WinRightStickX";
        rightY = "WinRightStickY";
#endif
    }

    private void Update()
    {
        float vertical = -Input.GetAxis("Mouse Y");
        float horizontal = Input.GetAxis("Mouse X");

        float joystickY = Input.GetAxis(rightY);
        float joystickX = Input.GetAxis(rightX);

        float inputX = (horizontal + joystickX) * sensitivityX;
        float inputY = (vertical + joystickY) * sensitivityY;

        if(inputY != 0f)
        {
            composer.m_TrackedObjectOffset.y -= inputY;
            composer.m_TrackedObjectOffset.y = Mathf.Clamp(composer.m_TrackedObjectOffset.y, m_YMin, m_YMax);
        }
        if(inputX != 0f)
        {
            composer.m_TrackedObjectOffset.x += inputX;
            composer.m_TrackedObjectOffset.x = Mathf.Clamp(composer.m_TrackedObjectOffset.x, m_XMin, m_XMax);
            RotatePlayer.Invoke();
        }
        else
        {
            composer.m_TrackedObjectOffset.x = 1;
        }
    }
}
