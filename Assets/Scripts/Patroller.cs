using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Patroller : MonoBehaviour
{
    // Stałe:
    private const float rotationSlerpAmount = .68f;

    [Header("References")]
    public Transform trans;
    public Transform modelHolder;

    [Header("Stats")]
    public float movespeed = 30;

    // Zmienne prywatne:
    private int currentPointIndex;
    private Transform currentPoint;

    private Transform[] patrolPoints;

    // Zwraca listę zawierającą składniki Transform dla każdego obiektu 
    // podrzędnego o nazwie zaczynającej się od "Patrol Point (".
    private List<Transform> GetUnsortedPatrolPoints()
    {
        // Uzyskaj składnik Transform dla każdego obiektu podrzędnego
        // w Patroller:
        Transform[] children = gameObject.GetComponentsInChildren<Transform>();

        // Zadeklaruj lokalną listę do przechowania składników Transform:
        var points = new List<Transform>();

        // Przejdź w pętli przez składniki Transform obiektów podrzędnych:
        for (int i = 0; i < children.Length; i++)
        {
            // Sprawdź, czy nazwa obiektu podrzędnego zaczyna się od 
            // "Patrol Point (":
            if (children[i].gameObject.name.StartsWith("Patrol Point ("))
            {
                // Jeśli tak, dodaj ją do list 'points':
                points.Add(children[i]);
            }
        }

        // Zwróc listę points:
        return points;
    }

    private void SetCurrentPatrolPoint(int index)
    {
        currentPointIndex = index;
        currentPoint = patrolPoints[index];
    }

    // Start is called before the first frame update
    void Start()
    {
        // Uzyskaj nieposortowaną lisę punktów patrolowych:
        List<Transform> points = GetUnsortedPatrolPoints();

        // Kontynuuj, tylko jeśli znaleźliśmy conajmniej 1 punkt patrolowy:
        if (points.Count > 0)
        {
            // Przygotuj nasz tablicę punktów patrolowych:
            patrolPoints = new Transform[points.Count];

            for (int i = 0; i < points.Count; i++)
            {
                // Szybkie odwołanie do bieżącego punktu:
                Transform point = points[i];

                // Wyizoluj numer punktu patrolowego z jego nazwy:
                int closingParenthesisIndex = point.gameObject.name.IndexOf(')');

                string indexSubstring = point.gameObject.name.Substring(14,closingParenthesisIndex - 14);

                // Przekonwertuj ten numer z łańcucha znaków na liczbę
                // całkowitą:
                int index = Convert.ToInt32(indexSubstring);

                // Wstaw odwołanie do punktu do naszej tablicy partolPoints:
                patrolPoints[index] = point;

                // Odłącz każdy punkt patrolowy od obiektu nadrzędnego,
                // aby nie poruszał się wraz z przeszkodą:
                point.SetParent(null);

                // Ukryj punkt patrolowy w oknie Hierarchy:
                point.gameObject.hideFlags = HideFlags.HideInHierarchy;
            }

            // Zacznij patrolowanie od pierwszego punktu w tablicy:
            SetCurrentPatrolPoint(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Podejmujemy działanie, tylko jeśli mamy currentPoint:
        if (currentPoint != null)
        {
            // Przesuń główny element GameObject w kierunku bieżącego punktu:
            trans.position = Vector3.MoveTowards(trans.position,
            currentPoint.position,movespeed * Time.deltaTime);

            // Jeśli już dotarliśmy do tego punktu, zmieniamy bieżący punkt:
            if (trans.position == currentPoint.position)
            {
                // Jeśli jesteśmy w ostatnim punkcie patrolowym...:
                if (currentPointIndex >= patrolPoints.Length - 1)
                {
                    // ...ustawiamy jako bieżący pierwszy punkt patrolowy:
                    SetCurrentPatrolPoint(0);
                }
                else // W przeciwnym razie, jeśli nie jesteśmy w ostatnim
                     // punkcie patrolowym, idziemy do kolejnego indeksu.
                     SetCurrentPatrolPoint(currentPointIndex + 1);
            }
            // Jeśli nie jesteśmy jeszcze na bieżącym punkcie,
            // obracamy model w jego kierunku:
            else
            {
                Quaternion lookRotation = Quaternion.LookRotation((currentPoint
                    .position - trans.position).normalized);

                modelHolder.rotation = Quaternion.Slerp(modelHolder.rotation,
                    lookRotation,rotationSlerpAmount);
            }
        }
    }
}
