using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] float m_MoveSpeed = 0f;

    private bool m_IsLaunched = false;

    private void Awake()
    {
        rb = this.transform.GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_MoveSpeed = 0f;
        m_IsLaunched = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_IsLaunched)
        {
            this.transform.rotation = this.transform.parent.rotation;
            this.transform.position = this.transform.parent.position;
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.TransformDirection(transform.forward) * -m_MoveSpeed * Time.deltaTime);
    }

    public void changeMoveSpeed(float value)
    {
        m_MoveSpeed = value;
        m_IsLaunched = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided");

        m_MoveSpeed = 0;
    }
}
