using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] float m_moveSpeed;
    [SerializeField] Vector3 m_direction;
    [SerializeField] float attackRange = 100f;

    private Rigidbody rb;
    private Camera cam;

    private Ray forwardRay;

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
            FireSword(m_leftHolster);

        if (Input.GetButtonDown("Fire2"))// || Input.GetAxis("RightTrigger") > 0)
            FireSword(m_rightHolster);

        //Left Trigger
        if (Input.GetAxisRaw("LeftTrigger") != 0)
        {
            if (m_ltAxisInUse == false)
            {
                // Call your event function here.
                FireSword(m_leftHolster);
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
                FireSword(m_rightHolster);
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

    void FireSword(SwordHolster holster)
    {
        if (!holster.IsSwordLaunched())
        {
            forwardRay = cam.ViewportPointToRay(Vector3.one * 0.5f);
            //Debug.DrawRay(forwardRay.origin, forwardRay.direction * 100, Color.red, 2f);
            RaycastHit hitInfo;

            if (Physics.Raycast(forwardRay, out hitInfo, attackRange))//if it raycast hits an object
            {
                holster.LaunchSword(hitInfo.point);
            }
            else //if raycast doesn't hit object, launch sword towards end of raycast
            {
                Vector3 end = forwardRay.direction * attackRange;
                holster.LaunchSword(end);
            }
        }

        else if (holster.IsSwordLaunched() && holster.IsSwordAttached())
        {
            this.transform.position = holster.GetSwordPos();

            holster.DestroySword();
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
