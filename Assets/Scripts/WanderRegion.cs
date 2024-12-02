using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderRegion : MonoBehaviour
{
    [Tooltip("Rozmiar obszaru.")]
    public Vector3 size;

    public Vector3 GetRandomPointWithin()
    {
        float x = transform.position.x + Random.Range(size.x * -.5f,size.x * .5f);
        float z = transform.position.z + Random.Range(size.z * -.5f,size.z * .5f);

        return new Vector3(x,transform.position.y,z);
    }

    void Awake()
    {
        // Uzyskaj wszystkie obiekty podrzędne typu Wanderer:
        var wanderers = gameObject.GetComponentsInChildren<Wanderer>();

        // Przejdź po pętli przez te obiekty podrzędne:
        for (int i = 0; i < wanderers.Length; i++)
        {
            // Ustaw ich odwołanie .region na wystąpienie 'this' tego skryptu:
            wanderers[i].region = this;
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
