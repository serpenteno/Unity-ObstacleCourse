using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wanderer : MonoBehaviour
{
    private enum State
    {
        Idle,
        Rotating,
        Moving
    }

    private State state = State.Idle;

    [HideInInspector] public WanderRegion region;

    [Header("References")]
    public Transform trans;
    public Transform modelTrans;

    [Header("Stats")]
    public float movespeed = 18f;
    
    [Tooltip("Minimalny czas oczekiwania przed ponownym wyznaczeniem celu.")]
    public float minRetargetInterval = 4.4f;

    [Tooltip("Maksymalny czas oczekiwania przed ponownym wyznaczeniem celu.")]
    public float maxRetargetInterval = 6.2f;

    [Tooltip("Czas w sekundach poświęcany na obrócenie się po wyznaczeniu celu, przed rozpoczęciem ruchu.")]
    public float rotationTime = .6f;

    [Tooltip("Czas w sekundach po zakończeniu obracania się, a przed rozpoczęciem ruchu.")]
    public float postRotationWaitTime = .3f;

    private Vector3 curretTarget;       // Położenie, które jest bieżącym wyznaczonym celem
    private Quaternion initalRotation;  // Nasz obrót w momencie wyznaczenia celu
    private Quaternion targetRotation;  // Obrót, jaki chcemy osiągnąć
    private float rotationStartTime;    // Wartość Time.time, kiedy rozpoczynamy obracanie

    // Wywoływana przy starcie i planowana do ponownego wywoływania przy każdym
    // swoim wywołaniu. Następne wywołanie będzie zaplanowane po losowym 
    // czasie z wyznaczonego zakresu.
    void Retarget()
    {
        // Ustaw bieżący cel na nowy, losowy punkt w regionie: 
        curretTarget = region.GetRandomPointWithin();

        // Zanotuj początkowy obrót: 
        initalRotation = modelTrans.rotation;

        // Zanotuj obrót potrzebny, aby patrzeć na cel: 
        targetRotation = Quaternion.LookRotation((curretTarget - trans.position).normalized);

        // Rozpocznij obracanie: 
        state = State.Rotating;
        rotationStartTime = Time.time;

        // Rozpocznij ruch po 'postRotationWaitTime' sekund
        // po zakończeniu obracania: 
        Invoke("BeginMoving",rotationTime + postRotationWaitTime);
    }

    // Wywołana przez Retarget w celu zainicjowania ruchu.
    void BeginMoving()
    {
        // Upewnij się, że patrzymy w kierunku targetRotation: 
        modelTrans.rotation = targetRotation;

        // Ustaw stan na Moving: 
        state = State.Moving;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Przy starcie natychmiast wywołaj Retarget().
        Retarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Moving)
        {
            // Zmierz odległość, na jaką przemieścimy się w tej klatce:
            float delta = movespeed * Time.deltaTime;

            // Wykonaj ruch w kierunku punktu docelowego o wartość delta: 
            trans.position = Vector3.MoveTowards(trans.position, curretTarget, delta);

            // Po osiągnięciu punktu docelowego zatrzymaj się i zaplanuj następne wywołanie Retarget: 
            if (trans.position == curretTarget)
            {
                state = State.Idle;
                Invoke("Retarget",Random.Range(minRetargetInterval,maxRetargetInterval));
            }
        }
        else if (state == State.Rotating)
        {
            // Zmierz czas dotychczasowego obracania się w sekundach: 
            float timeSpentRotating = Time.time - rotationStartTime;

            // Obracaj się od initialRotation do targetRotation:
            modelTrans.rotation = Quaternion.Slerp(initalRotation,targetRotation,timeSpentRotating / rotationTime);
        }
    }
}
