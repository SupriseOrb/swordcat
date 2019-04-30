using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private Rigidbody m_Rb;

    [SerializeField] float m_MoveSpeed = 0f;

    private bool m_IsLaunched = false;
    private bool m_IsAttached = false;

    private void Awake()
    {
        m_Rb = this.transform.GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_MoveSpeed = 0f;
        m_IsLaunched = false;
        m_IsAttached = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_IsLaunched)
        {
            this.transform.rotation = this.transform.parent.rotation * Quaternion.Euler(new Vector3(90f,0f,0f));
            this.transform.position = this.transform.parent.position;
        }
    }

    private void FixedUpdate()
    {
        m_Rb.MovePosition(m_Rb.position + transform.forward * m_MoveSpeed * Time.deltaTime);
    }

    public bool IsAttached()
    {
        return m_IsAttached;
    }

    public void changeMoveSpeed(float value)
    {
        m_MoveSpeed = value;
        m_IsLaunched = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collidedxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
        m_MoveSpeed = 0;
        try
        {
            if (m_IsLaunched)
            {
                collision.gameObject.GetComponent<Rock>().attach(this.gameObject);
                m_IsAttached = true;
            }

        }
        catch
        {
        }
    }
}
