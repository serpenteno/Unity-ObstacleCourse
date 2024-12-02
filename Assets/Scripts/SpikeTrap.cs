using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    private enum State
    {
        Lowered,
        Lowering,
        Raising,
        Raised
    }

    private State state = State.Lowered;

    [Header("Stats")]
    [Tooltip("Czas w sekundach po opuszczeniu kolców, zanim zostaną ponownie podniesione.")]
    public float interval = 2f;

    [Tooltip("Czas w sekundach po podniesieniu kolców, zanim zaczną być ponownie opuszczane.")]
    public float raiseWaitTime = .3f;

    [Tooltip("Czas w sekundach, ile zajmuje pełne opuszczenie kolców.")]
    private float lowerTime = .6f;

    [Tooltip("Czas w sekundach, ile zajmuje pełne podniesienie kolców.")]
    private float raiseTime = .08f;

    [Header("References")]
    [Tooltip("Odwołanie do obiektu nadrzędnego dla wszystkich kolców.")]
    public Transform spikeHolder;
    public GameObject hitboxGameObject;
    public GameObject colliderGameObject;

    private float lastSwitchTime = Mathf.NegativeInfinity;
    private const float SpikeHeight = 3.6f;
    private const float LoweredSpikeHeight = .08f;

    void StartRaising()
    {
        lastSwitchTime = Time.time;
        state = State.Raising;
        hitboxGameObject.SetActive(true);
    }
    void StartLowering()
    {
        lastSwitchTime = Time.time;
        state = State.Lowering;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Kolce domyślnie są opuszczone.
        // Rozpoczniemy ich podnoszenie po 'interval' sekund od metody Start.
        Invoke("StartRaising",interval);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Lowering)
        {
            // Uzyskaj lokalne skalowanie pojemnika na kolce:
            Vector3 scale = spikeHolder.localScale;

            // Zaktualizuj skalowanie Y przez liniową interpolację od wysokości
            // maksymalnej do minimalnej:
            scale.y = Mathf.Lerp(SpikeHeight,LoweredSpikeHeight,(Time.time - 
            lastSwitchTime) / lowerTime);

            // Zastosuj zaktualizowane skalowanie wobec pojemnika na kolce:
            spikeHolder.localScale = scale;

            // Jeśli kolce zakończyły opuszczanie:
            if (scale.y == LoweredSpikeHeight)
            {
                // Zaktualizuj stan i przygotuj wywołanie następnego podnoszenia
                // kolców za 'interval' sekund
                colliderGameObject.SetActive(false);
                Invoke("StartRaising",interval);
                state = State.Lowered;
            }
        }
        else if (state == State.Raising)
        {
            // Uzyskaj lokalne skalowanie pojemnika na kolce:
            Vector3 scale = spikeHolder.localScale;

            // Zakutalizuj skalowanie Y przez liniową interpolację od wysokości
            // minimalnej do maksymalnej:
            scale.y = Mathf.Lerp(LoweredSpikeHeight,SpikeHeight,(Time.time - 
            lastSwitchTime) / raiseTime);

            // Zastosuj zaktualizowane skalowanie wobec pojemnika na kolce:
            spikeHolder.localScale = scale;

            // Jeśli kolce zakończyły podnoszenie:
            if (scale.y == SpikeHeight)
            {
                // Zaktualizuj stan i przygotuj wywołanie następnego opuszczanie 
                // kolców za 'raiseWaitTime' sekund:
                Invoke("StartLowering",raiseWaitTime);
                state = State.Raised;

                // Aktywuj bryłę ograniczającą, która blokuje gracza:
                colliderGameObject.SetActive(true);

                // Dezaktywuj obszar trafienia, aby nie zabijał gracza:
                hitboxGameObject.SetActive(false);
            }
        }
    }
}
