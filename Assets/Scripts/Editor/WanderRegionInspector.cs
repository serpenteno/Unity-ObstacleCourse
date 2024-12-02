using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WanderRegion))]
public class WanderRegionInspector : Editor
{
    // Szybkie odwołanie do obiektu docelowego poprzez rzutowanie typu:
    private WanderRegion Target
    {
        get {return (WanderRegion)target;}
    }

    // Wysokość rysowanego obszaru.
    private const float BoxHeight = 10f;

    void OnSceneGUI()
    {
        // Biały kolor uchwytów:
        Handles.color = Color.white;

        // Rysuj szkielet sześcianu odpowiadający regionowi wędrowanie:
        Handles.DrawWireCube(Target.transform.position + (Vector3.up * BoxHeight * .5f),
            new Vector3(Target.size.x,BoxHeight,Target.size.z));
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
