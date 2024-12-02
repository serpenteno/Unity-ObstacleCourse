using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("References")]
    public Transform trans;

    [Header("Stats")]
    [Tooltip("O ile jednostek na sekundę do przodu będzie poruszać się ten pocisk.")]
    public float speed = 34;

    [Tooltip("Odległość, na jaką przemieści się pocisk, zanim się zatrzyma.")]
    public float range = 70;

    private Vector3 spawnPoint; 

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = trans.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Przesuń pocisk wzdłuż jego lokalnj osi Z (do przodu):
        trans.Translate(0,0,speed*Time.deltaTime,Space.Self);

        // Zniszcz pocisk, jeśli przekroczył swój zakres:
        if (Vector3.Distance(trans.position,spawnPoint) >= range)
            Destroy(gameObject);
    }
}
