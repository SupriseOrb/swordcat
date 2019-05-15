using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] float m_MoveSpeed;
    [SerializeField] float m_TurnSpeed;
    [SerializeField] Vector3 m_direction;
    [SerializeField] float m_RightHorizontal;
    [SerializeField] float attackRange = 100f;

    private Rigidbody rb;
    private Camera cam;

    private Ray forwardRay;
    private int playerMask = ~(1 << 9);

    [SerializeField] GameObject m_lHolster;
    [SerializeField] GameObject m_rHolster;

    private SwordHolster m_leftHolster;
    private SwordHolster m_rightHolster;

    private bool m_ltAxisInUse = false;
    private bool m_rtAxisInUse = false;

    private string leftTriggerName;
    private string rightTriggerName;

    private string leftBumperName;
    private string rightBumperName;

    private string rightStickX;
    private string rightStickY;

    void Awake()
    {
        rb = this.transform.GetComponent<Rigidbody>();
        cam = Camera.main;

        m_leftHolster = m_lHolster.GetComponent<SwordHolster>();
        m_rightHolster = m_rHolster.GetComponent<SwordHolster>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

#if UNITY_EDITOR_WIN
        leftTriggerName = "LeftTrigger";
        rightTriggerName = "RightTrigger";
        leftBumperName = "LeftBumper";
        rightBumperName = "RightBumper";
        rightStickX = "WinRightStickX";
        rightStickY = "WinRightStickY";
#endif

#if UNITY_EDITOR_OSX
        leftTriggerName = "MacLeftTrigger";
        rightTriggerName = "MacRightTrigger";
        leftBumperName = "MacLeftBumper";
        rightBumperName = "MacRightBumper";
        rightStickX = "MacRightStickX";
        rightStickY = "MacRightStickY";

#endif
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        m_direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        RotateDirection();

        if (Input.GetButtonDown("Fire1"))// || Input.GetAxis("LeftTrigger") > 0)
            FireSword(m_leftHolster);

        if (Input.GetButtonDown("Fire2"))// || Input.GetAxis("RightTrigger") > 0)
            FireSword(m_rightHolster);

        //Left Trigger
        if (Input.GetAxisRaw(leftTriggerName) > 0)
        {
            if (m_ltAxisInUse == false)
            {
                // Call your event function here.
                FireSword(m_leftHolster);
                m_ltAxisInUse = true;
            }
        }
        if (Input.GetAxisRaw(leftTriggerName) <= 0)
        {
            m_ltAxisInUse = false;
        }

        //Right Trigger
        if (Input.GetAxisRaw(rightTriggerName) > 0)
        {
            if (m_rtAxisInUse == false)
            {
                // Call your event function here.
                FireSword(m_rightHolster);
                m_rtAxisInUse = true;
            }
        }
        if (Input.GetAxisRaw(rightTriggerName) <= 0)
        {
            m_rtAxisInUse = false;
        }

        //Left Bumper
        if(Input.GetButtonDown(leftBumperName))
        {
            if (m_leftHolster.IsSwordLaunched())
            {
                m_leftHolster.DestroySword();
            }
        }

        //Right Bumper
        if (Input.GetButtonDown(rightBumperName))
        {
            if (m_rightHolster.IsSwordLaunched())
            {
                m_rightHolster.DestroySword();
            }
        }

    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        rb.velocity = m_direction.normalized * m_MoveSpeed;
    }


    void RotateDirection()
    {
        if (cam == null)
            cam = Camera.main;

        m_direction = cam.transform.TransformDirection(m_direction);
        m_direction.y = 0;
        //this.transform.Rotate(Vector3.up, Input.GetAxis("Horizontal") * m_TurnSpeed * Time.deltaTime);
    }

    void LookForward()
    {
        //Debug.Log("Looking Forward");
        if (cam == null)
            cam = Camera.main;

        float currentX = this.transform.rotation.x;
        float camY = cam.transform.eulerAngles.y;
        float currentZ = this.transform.rotation.z;

        Quaternion newRotation = Quaternion.Euler(new Vector3(currentX, camY, currentZ));
        //Debug.Log(camY);

    }

    void FireSword(SwordHolster holster)
    {
        if (!holster.IsSwordLaunched())
        {
            forwardRay = cam.ViewportPointToRay(Vector3.one * 0.5f);
            Debug.DrawRay(forwardRay.origin, forwardRay.direction * 100, Color.red, 2f);
            RaycastHit hitInfo;

            if (Physics.Raycast(forwardRay, out hitInfo, attackRange, playerMask))//if it raycast hits an object
            {
                holster.LaunchSword(hitInfo.point);
            }
            else //if raycast doesn't hit object, launch sword towards end of raycast
            {
                Vector3 end = forwardRay.origin + forwardRay.direction * attackRange;
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
