using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            Player player = other.GetComponent<Player>();

            player.currentHealth--;
            if (player != null)
            {
                player.Die();
            }
        }

        if (other.gameObject.layer == 11)
        {
            Destroy(gameObject);
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
