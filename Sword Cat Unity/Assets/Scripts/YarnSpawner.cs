using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YarnSpawner : MonoBehaviour
{

    public GameObject m_YarnObject;

    [SerializeField] float m_SpawnTime = 5f;
    [SerializeField] float m_ResumeTime = 10f;

    [SerializeField] bool m_IsSpawning = false;

    private IEnumerator spawn;

    private void Awake()
    {
        spawn = SpawnCoroutine();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartSpawning();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnYarn()
    {
        //Debug.Log("Yarn Spawned");

        GameObject go = Instantiate(m_YarnObject, this.transform.position, Quaternion.identity, this.transform);

        TumbleYarn yarn = go.GetComponent<TumbleYarn>();

        yarn.collected.AddListener(OnYarnCollected);
    }

    public void OnYarnCollected()
    {
        Debug.Log("Yarn Collected");

        StopSpawning();

        StartCoroutine(WaitCoroutine());
    }

    public void StartSpawning()
    {
        Debug.Log("Started Spawning");
        if(!m_IsSpawning)
        {
            m_IsSpawning = true;
            StartCoroutine(spawn);
        }

    }

    public void StopSpawning()
    {
        Debug.Log("Should stop spawning");
        StopCoroutine(spawn);
        m_IsSpawning = false;
    }

    private IEnumerator SpawnCoroutine()
    {
        for(; ; )
        {
            SpawnYarn();

            yield return new WaitForSeconds(m_SpawnTime);
        }
    }

    private IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(m_ResumeTime);

        StartSpawning();
    }
}
