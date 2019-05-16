using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [SerializeField]private Transform respawnPoint;
    [SerializeField]private float deathTime = 2f;
    private GameObject player;

    void OnTriggerEnter(Collider collider)
    {
        player = collider.gameObject;
        player.SetActive(false);
        StartCoroutine(WaitForRespawn(deathTime));
    }

    IEnumerator WaitForRespawn(float spawnTime)
    {
        yield return new WaitForSecondsRealtime(spawnTime);
        respawnPlayer(player);
    }

    void respawnPlayer(GameObject other)
    {
        other.SetActive(true);
        spawnPlayerAtLocation(other);
    }

    void spawnPlayerAtLocation(GameObject other)
    {
        other.transform.position = respawnPoint.position;
    }
}
