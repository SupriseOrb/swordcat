using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] float m_moveSpeed;
    [SerializeField] Vector3 m_direction;

    private Rigidbody rb;
    private Camera cam;

    private SwordHolster m_leftHolster;
    private SwordHolster m_rightHolster;

    private bool m_ltAxisInUse = false;
    private bool m_rtAxisInUse = false;

    void Awake()
    {
        rb = this.transform.GetComponent<Rigidbody>();
        cam = Camera.main;

        m_leftHolster = this.transform.GetChild(1).GetComponent<SwordHolster>();
        m_rightHolster = this.transform.GetChild(2).GetComponent<SwordHolster>();

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


        if (Input.GetButtonDown("Fire1"))// || Input.GetAxis("LeftTrigger") > 0)
            FireLeft();

        if (Input.GetButtonDown("Fire2"))// || Input.GetAxis("RightTrigger") > 0)
            FireRight();

        //Left Trigger
        if (Input.GetAxisRaw("LeftTrigger") != 0)
        {
            if (m_ltAxisInUse == false)
            {
                // Call your event function here.
                FireLeft();
                m_ltAxisInUse = true;
            }
        }
        if (Input.GetAxisRaw("LeftTrigger") == 0)
        {
            m_ltAxisInUse = false;
        }

        //Right Trigger
        if (Input.GetAxisRaw("RightTrigger") != 0)
        {
            if (m_rtAxisInUse == false)
            {
                // Call your event function here.
                FireRight();
                m_rtAxisInUse = true;
            }
        }
        if (Input.GetAxisRaw("RightTrigger") == 0)
        {
            m_rtAxisInUse = false;
        }

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
        //Debug.Log("Looking Forward");
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
        //Debug.Log("Fired left");
        if (!m_leftHolster.IsSwordLaunched())
        {
            m_leftHolster.LaunchSword();
        }

        else if (m_leftHolster.IsSwordLaunched())
        {

            Vector3 swordPosition = m_leftHolster.GetSwordPos();

            this.transform.position = new Vector3(swordPosition.x, this.transform.position.y, swordPosition.z);

            m_leftHolster.DestroySword();
        }
    }

    void FireRight()
    {
        //Debug.Log("Fired left");
        if (!m_rightHolster.IsSwordLaunched())
        {
            m_rightHolster.LaunchSword();
        }

        else if (m_rightHolster.IsSwordLaunched())
        {
            Vector3 swordPosition = m_rightHolster.GetSwordPos();

            this.transform.position = new Vector3(swordPosition.x, this.transform.position.y, swordPosition.z);

            m_rightHolster.DestroySword();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "yarn")
        {
            TumbleYarn yarn = collision.gameObject.GetComponent<TumbleYarn>();
            GameManager.instance.data.inventory[(int) yarn.yarnType] += 1;
            Destroy(collision.gameObject);
        }
    }

}
