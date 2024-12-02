using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            Player player = other.gameObject.GetComponent<Player>();

            player.spawnPoint = gameObject.transform.parent.transform.position;
            if (player.currentHealth < player.maxHealth)
                player.currentHealth++;

            Destroy(gameObject.transform.parent.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
