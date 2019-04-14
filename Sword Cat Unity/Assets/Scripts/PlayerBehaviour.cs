using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] float m_moveSpeed;
    [SerializeField] Vector3 m_direction;

    private Rigidbody rb;
    [SerializeField] private Camera cam;

    void Awake()
    {
        rb = this.transform.GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        m_direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        LookForward();

    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        //Debug.Log(m_direction.normalized * m_moveSpeed);
        //rb.velocity = m_direction.normalized * m_moveSpeed;
        rb.MovePosition(rb.position + transform.TransformDirection(m_direction.normalized) * m_moveSpeed * Time.deltaTime);

    }

    void LookForward()
    {
        Debug.Log("Looking Forward");
        if (cam == null)
            cam = Camera.main;

        float currentX = this.transform.rotation.x;
        float camY = cam.transform.eulerAngles.y;
        float currentZ = this.transform.rotation.z;
        //Debug.Log(camY);
        rb.MoveRotation(Quaternion.Euler(currentX, camY, currentZ));
    }
}
