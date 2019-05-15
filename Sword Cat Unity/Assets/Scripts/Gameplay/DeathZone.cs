using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [SerializeField]private Transform respawnPoint;
    [SerializeField]private float deathTime = 2f;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider collider)
    {
        player = collider.gameObject;
        player.SetActive(false);
        StartCoroutine("WaitForRespawn");
        respawnPlayer(player);
    }

    IEnumerator WaitForRespawn()
    {
        yield return new WaitForSecondsRealtime(deathTime);
    }

    void spawnPlayerAtLocation(GameObject other)
    {
        other.transform.position = respawnPoint.position;
    }

    void respawnPlayer(GameObject other)
    {
        other.SetActive(true);
        spawnPlayerAtLocation(other);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
