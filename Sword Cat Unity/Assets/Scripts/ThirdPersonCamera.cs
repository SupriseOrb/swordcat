using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Camera cam;

    [SerializeField] float m_MaxY = 40f;
    [SerializeField] float m_MinY = -50f;

    [SerializeField] float m_DistanceFromPlayer = 10.0f;
    [SerializeField] float m_CurrentX = 0f;
    [SerializeField] float m_CurrentY = 0f;
    [SerializeField] float m_SensitivityX = 4f;
    [SerializeField] float m_SensitivityY = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_CurrentX += Input.GetAxis("Mouse X") * m_SensitivityX;
        m_CurrentY += Input.GetAxis("Mouse Y") * m_SensitivityY;

        m_CurrentY = Mathf.Clamp(m_CurrentY, m_MinY, m_MaxY);

        CameraLook();


    }

    private void CameraLook()
    {
        Vector3 cameraOffset = new Vector3(0, 2f, -m_DistanceFromPlayer);
        Quaternion rotation = Quaternion.Euler(m_CurrentY, m_CurrentX, 0);
        this.transform.position = target.position + (rotation * cameraOffset);
        this.transform.LookAt(target.position);

    }
}
