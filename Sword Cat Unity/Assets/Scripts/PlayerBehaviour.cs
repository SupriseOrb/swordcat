using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] float m_MoveSpeed;
    [SerializeField] float m_TurnSpeed;
    [SerializeField] Vector3 m_direction;
    [SerializeField] float attackRange = 1000f;

    private Rigidbody rb;
    private Camera cam;
    private CharacterController controller;

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

    private RaycastHit hit;

    //For checking to see if the footstep sounds are playing.
    private bool footstepsPlaying = false;

    void Awake()
    {
        rb = this.transform.GetComponent<Rigidbody>();
        cam = Camera.main;

        CameraJoystickControls vcam = GameObject.FindGameObjectWithTag("vcam").GetComponent<CameraJoystickControls>();
        vcam.RotatePlayer.AddListener(OnCameraRotate);

        controller = this.GetComponent<CharacterController>();

        m_leftHolster = m_lHolster.GetComponent<SwordHolster>();
        m_rightHolster = m_rHolster.GetComponent<SwordHolster>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        leftTriggerName = "LeftTrigger";
        rightTriggerName = "RightTrigger";
        leftBumperName = "LeftBumper";
        rightBumperName = "RightBumper";
        rightStickX = "WinRightStickX";
        rightStickY = "WinRightStickY";
#endif

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
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
        Ray camRay = cam.ViewportPointToRay(Vector3.one * 0.5f);
        Debug.DrawRay(camRay.origin, camRay.direction * 100, Color.red, 2f);
        if (Physics.Raycast(camRay, out hit, attackRange, playerMask))
        {
            Rock hasAttach = hit.transform.GetComponent<Rock>();
            if (hasAttach)
            {
                hasAttach.TurnOnOutline();
            }
        }



        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        m_direction = new Vector3(x, 0, z);

        //controller.SimpleMove(m_direction * Time.deltaTime * m_MoveSpeed);

        transform.Rotate(Vector3.up, x * Time.deltaTime * m_TurnSpeed);

        if(z != 0)
        {
            if (!footstepsPlaying) //Trigger footstep sounds if player is moving
            {
                AkSoundEngine.PostEvent("Play_Footstep", gameObject);
                footstepsPlaying = true;
            }
            controller.SimpleMove(transform.forward * m_MoveSpeed * z);
        }
        else if (footstepsPlaying) //Stop footsteps if player is not moving
        {
            AkSoundEngine.PostEvent("Stop_Footsteps", gameObject);
            footstepsPlaying = false;
        }

        //LookForward();

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

    public void OnCameraRotate()
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, cam.transform.localEulerAngles.y, transform.localEulerAngles.z);
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
            var objectHit = holster.GetObjectHit();

            var objectCenter = objectHit.GetComponent<Collider>().bounds.center;

            //var objectCenter = this.transform.position;

            var yLevel = objectHit.GetComponent<Collider>().bounds.extents.y;

            this.transform.position = objectCenter + new Vector3(0, yLevel + 0.1f, 0);

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
