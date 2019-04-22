using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] float m_moveSpeed;
    [SerializeField] Vector3 m_direction;

    private Rigidbody rb;
    private Camera cam;

    private SwordHolster leftHolster;
    private SwordHolster rightHolster;

    void Awake()
    {
        rb = this.transform.GetComponent<Rigidbody>();
        cam = Camera.main;

        leftHolster = this.transform.GetChild(1).GetComponent<SwordHolster>();
        rightHolster = this.transform.GetChild(2).GetComponent<SwordHolster>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        LookForward();

        m_direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        //m_direction = new Vector3(Input.GetAxis("LeftStickX"), 0f, Input.GetAxis("LeftStickY"));


        if (Input.GetButtonDown("Fire1") || Input.GetAxis("LeftTrigger") > 0)
            FireLeft();

        if (Input.GetButtonDown("Fire2") || Input.GetAxis("RightTrigger") > 0)
            FireRight();

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

    void FireLeft()
    {
        Debug.Log("Fired left");
        if (!leftHolster.IsSwordLaunched())
        {
            leftHolster.LaunchSword();
        }

        else if (leftHolster.IsSwordLaunched())
        {
            this.transform.position = leftHolster.GetSwordPos();

            leftHolster.DestroySword();
        }
    }

    void FireRight()
    {
        Debug.Log("Fired left");
        if (!rightHolster.IsSwordLaunched())
        {
            rightHolster.LaunchSword();
        }

        else if (rightHolster.IsSwordLaunched())
        {

            this.transform.position = rightHolster.GetSwordPos();

            rightHolster.DestroySword();
        }
    }

}
