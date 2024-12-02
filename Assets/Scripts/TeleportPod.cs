using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPod : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip("Docelowe miejsce teleportacji.")]
    public Transform destination;

    [Tooltip("Czas po jakim gracz zostanie przeteleportowany.")]
    private float teleportationTime = 1.2f;

    [Header("References")]
    [Tooltip("Obiekt gracza")]
    GameObject playerObj;

    Player player;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            StartCoroutine("Teleport");
        }
    }

    IEnumerator Teleport()
    {
        PlayerEnableOrDisable(false);
        yield return new WaitForSeconds(teleportationTime);
        player.trans.position = destination.position;
        player.SetMovementVelocity(Vector3.zero);
        yield return new WaitForSeconds(.05f);
        PlayerEnableOrDisable(true);
    }

    void PlayerEnableOrDisable(bool can)
    {
        if (!can)
        {
        player.enabled = false;
        player.modelTrans.gameObject.SetActive(false);
        }
        if (can)
        {
        player.enabled = true;
        player.modelTrans.gameObject.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerObj = GameObject.Find("Player");
        player = playerObj.gameObject.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
