using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHolster : MonoBehaviour
{

    public GameObject swordPrefab;

    public float swordDespawnTime;
    public bool swordLaunched = false;
    public bool swordAttached = false;
    public float swordMoveSpeed = 10f;

    public Vector3 holsterOffset;

    private Sword m_Sword;

    private Coroutine swordCoroutine;

    private void Awake()
    {
        //m_Sword = this.transform.GetChild(0).GetComponent<Sword>();
    }

    // Start is called before the first frame update
    void Start()
    {
        RespawnSword();
    }

    // Update is called once per frame
    void Update()
    {
        //this.transform.position = this.transform.parent.position + holsterOffset;
        //this.transform.localRotation = this.transform.parent.rotation;
        //Debug.Log(this.gameObject.name + " : " + GetSwordPos());
    }

    public void RespawnSword()
    {
        GameObject go = Instantiate(swordPrefab, this.transform.position, swordPrefab.transform.rotation, this.transform);

        Physics.IgnoreLayerCollision(9, 9);
        m_Sword = go.GetComponent<Sword>();
        m_Sword.hitUnattachableEnvironment.AddListener(OnHitUnattachableEnvironmentListener);
        swordLaunched = false;
    }

    public void LaunchSword(Vector3 hitInfo)
    {
        AkSoundEngine.PostEvent("Shoot", swordPrefab);
        swordLaunched = true;
        m_Sword.transform.LookAt(hitInfo);
        swordCoroutine = StartCoroutine(LaunchSwordCoroutine());
    }

    public bool IsSwordLaunched()
    {
        return swordLaunched;
    }

    public bool IsSwordAttached()
    {
        return swordAttached;
    }

    public void AttachSword()
    {
        swordAttached = true;
    }

    public void DetachSword()
    {
        swordAttached = false;
    }

    public IEnumerator LaunchSwordCoroutine()
    {
        m_Sword.changeMoveSpeed(swordMoveSpeed);
        yield return new WaitForSeconds(swordDespawnTime);

        DestroySword();
       
    }

    public void DestroySword()
    {
        if(swordCoroutine != null)
        {
            StopCoroutine(swordCoroutine);
        }
        DetachSword();
        Destroy(m_Sword.gameObject);
        RespawnSword();
    }

    public Vector3 GetSwordPos()
    {
        return m_Sword.transform.position;
    }

    public void OnHitUnattachableEnvironmentListener()
    {
        DestroySword();
    }
}
