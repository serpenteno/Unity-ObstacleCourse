using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    // Odwołania
    [Header("References")]
    public Transform trans;
    public Transform modelTrans;
    public CharacterController characterController;
    public GameObject cam;
    private bool paused = false;
    
    // Życie
    [Header("Stats")]
    public int maxHealth;
    [System.NonSerialized] public int currentHealth;

    // Ruch
    [Header("Movement")]
    [Tooltip("Liczba jednostek przesunięcia w ciągu sekundy przy maksymalnej prędkości.")]
    public float movespeed = 24;

    [Tooltip("Czas w sekundach potrzebny na osiągnięcie maksymalnej prędkości.")]
    public float timeToMaxSpeed = 0.26f;

    private float VelocityGainPerSecond { get {return movespeed/timeToMaxSpeed;} }

    [Tooltip("Czas w sekundach potrzebny na przejście od maksymalnej prędkości do zatrzymania.")]
    public float timeToLoseMaxSpeed = 0.2f;
    
    private float VelocityLossPerSecond { get {return movespeed/timeToLoseMaxSpeed;} }

    [Tooltip("Mnożnik dla prędkości przy ruchu w kierunku przeciwnym do aktualnego kierunku poruszania (np. przy próbie ruchu w prawo, gdy akutalnie poruszamy się w lewo).")]
    public float reverseMomentumMultiplier = 2.2f;

    private Vector3 movementVelocity = Vector3.zero;
    public Vector3 SetMovementVelocity(Vector3 newMovementVelocity) 
    {
        movementVelocity = newMovementVelocity;
        return movementVelocity;
    }

    // Zryw
    [Header("Dashing")]
    [Tooltip("Całkowita liczba jednostek pokonywanych przy wykonywaniu zrywu.")]
    public float dashDistance = 17;

    [Tooltip("Czas, jaki zajmuje zryw, w sekundach.")]
    public float dashTime = .26f;

    private bool IsDashing
    {
        get {return (Time.time < dashBeginTime + dashTime);}
    }

    private Vector3 dashDirection;
    private float dashBeginTime = Mathf.NegativeInfinity;

    [Tooltip("Czas po zakończeniu zrywu, zanim można go będzie wykonać ponownie.")]
    public float dashCooldown = 1.8f;

    private bool CanDashNow
    {
        get {return (Time.time > dashBeginTime + dashTime + dashCooldown);}
    }

    private void Movement()
    {
        // Wykonujemy ruch, tylko jeśli nie wykonujemy zrywu:
        if (!IsDashing)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                if (movementVelocity.z >= 0) // Jeśli już poruszamy się do przodu
                    // Zwiększamy prędkość Z o VelocityGainPerSecond,
                    // ale nie przekraczamy 'movespeed':
                    movementVelocity.z = Mathf.Min(movespeed,movementVelocity.z + VelocityGainPerSecond * Time.deltaTime);

                else // z kolei, jeśli poruszamy się do tyłu
                    // Zwiększamy prędkość Z o VelocityGainPerSecond, wykorzystując,
                    // reverseMomentumMultiplier, ale nie przekraczamy 0:
                    movementVelocity.z = Mathf.Min(0,movementVelocity.z + VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
            }

            // Jeśli przytrzymany jest klawisz S lub strzałka w dół
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                if (movementVelocity.z > 0) // Jeśli już poruszamy się do przodu
                    movementVelocity.z = Mathf.Max(0,movementVelocity.z - VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);

                else // Jeśli poruszamy się do tyłu albo nie poruszamy się w ogóle
                    movementVelocity.z = Mathf.Max(-movespeed,movementVelocity.z - VelocityGainPerSecond * Time.deltaTime);
            }
            
            else // Jeśli nie są wciśnięte klawisze ruchu do góry ani do dołu
            {
                // Musimy stopniowo przywrócić prędkość Z do 0.
                if (movementVelocity.z > 0) // Poruszamy się do góry
                    // Zmniejsz prędkość Z o VelocityLossPerSecond, ale nie poniżej 0:
                    movementVelocity.z = Mathf.Max(0,movementVelocity.z - VelocityLossPerSecond * Time.deltaTime);

                else // Poruszamy się w dół
                    // Zwiększ prędkość Z (z powrotem do 0) o VelocityLossPerSecond,
                    // ale nie powyżej 0:
                    movementVelocity.z = Mathf.Min(0,movementVelocity.z + VelocityLossPerSecond * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                if (movementVelocity.x >= 0) // Jeśli już poruszamy się w prawo
                    // Zwiększamy prędkość X o VelocityGainPerSecond,
                    // ale nie przekraczamy 'movespeed':
                    movementVelocity.x = Mathf.Min(movespeed,movementVelocity.x + VelocityGainPerSecond * Time.deltaTime);

                else // z kolei, jeśli poruszamy się w lewo
                    // Zwiększamy prędkość X o VelocityGainPerSecond, wykorzystując,
                    // reverseMomentumMultiplier, ale nie przekraczamy 0:
                    movementVelocity.x = Mathf.Min(0,movementVelocity.x + VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);
            }

            // Jeśli przytrzymany jest klawisz S lub strzałka w dół
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                if (movementVelocity.x > 0) // Jeśli już poruszamy się w prawo
                    movementVelocity.x = Mathf.Max(0,movementVelocity.x - VelocityGainPerSecond * reverseMomentumMultiplier * Time.deltaTime);

                else // Jeśli poruszamy się w lewo albo nie poruszamy się w ogóle
                    movementVelocity.x = Mathf.Max(-movespeed,movementVelocity.x - VelocityGainPerSecond * Time.deltaTime);
            }
            
            else // Jeśli nie są wciśnięte klawisze ruchu w prawo ani w lewo
            {
                // Musimy stopniowo przywrócić prędkość X do 0.
                if (movementVelocity.x > 0) // Poruszamy się w prawo
                    // Zmniejsz prędkość X o VelocityLossPerSecond, ale nie poniżej 0:
                    movementVelocity.x = Mathf.Max(0,movementVelocity.x - VelocityLossPerSecond * Time.deltaTime);

                else // Poruszamy się w lewo
                    // Zwiększ prędkość X (z powrotem do 0) o VelocityLossPerSecond,
                    // ale nie powyżej 0:
                    movementVelocity.x = Mathf.Min(0,movementVelocity.x + VelocityLossPerSecond * Time.deltaTime);
            }

            //Jeśli gracz porusza się w którymś z kierunków (lewy/prawy lub góra/dół):
            if (movementVelocity.x != 0 || movementVelocity.z != 0)
            {
                // Stosowanie wektora prędkości ruchu:
                characterController.Move(movementVelocity * Time.deltaTime);

                // Utrzymujemy obrót elementu przetrzymującego model
                // w kierunku ostatniego ruchu:
                modelTrans.rotation = Quaternion.Slerp(modelTrans.rotation,Quaternion.LookRotation(movementVelocity),0.18f);
            }
        }
    }

    private void Dashing() 
    {
        if (!IsDashing && CanDashNow) // Aktualnie nie wykonujemy zrywu 
        {
            // Jeśli zryw nie jest w czasie odnowienia i naciśnięty został klawisz spacji:
            if (Input.GetKey(KeyCode.Space))
            {
                // Znajdź kierunek określany przez wciśnięte klawisze ruchu:
                Vector3 movementDir = Vector3.zero;

                // Jeśli wciśnięty jest klawisz W lub strzałka w górę, ustaw z na 1:
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                    movementDir.z = 1;
                
                // W przeciwnym razie, jeśli wciśnięty jest klawisz S lub strzałka w dół, ustaw z na -1:
                else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                    movementDir.z = -1;

                // Jeśli wciśnięty jest klawisz D lub strzałka w prawo, ustaw x na 1:
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                    movementDir.x = 1;
                
                // W przeciwnym razie, jeśli wciśnięty jest klawisz A lub strzałka w lewo, ustaw x na -1:
                else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                    movementDir.x = -1;
                
                //Jeśli wciśnięty jest przynajmnie jeden klawisz ruchu:
                if (movementDir.x != 0 || movementDir.z != 0)
                {
                    // Rozpocznij zryw:
                    dashDirection = movementDir;
                    dashBeginTime = Time.time;
                    movementVelocity = dashDirection * movespeed;
                    modelTrans.forward = dashDirection;
                }
            }
        }
        if (IsDashing) // Wykonujemy zryw
        {
            characterController.Move(dashDirection * (dashDistance / dashTime) * Time.deltaTime);
        }
    }

    private void Pausing()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Przełącz stan wstrzymania:
            paused = !paused;
            // Jeśli gra jest wstrzymana, ustaw timeScale na 0:
            if (paused)
                Time.timeScale = 0;
            // W przeciwnym razie przywróć timeScale na 1:
            else 
                Time.timeScale = 1;
        }
    }

    void OnGUI()
    {
        if (paused)
        {
            float boxWidth = Screen.width * .4f;
            float boxHeight = Screen.height * .4f;

            GUILayout.BeginArea(new Rect(
                (Screen.width * .5f) - (boxWidth * .5f), 
                (Screen.height * .5f) - (boxHeight * .5f), 
                boxWidth, 
                boxHeight));

            if (GUILayout.Button("RESUME GAME",GUILayout.Height(boxHeight * .5f)))
            {
                paused = false;
                Time.timeScale = 1;
            }

            if (GUILayout.Button("RETURN TO MAIN MENU",GUILayout.Height(boxHeight * .5f)))
            {
                Time.timeScale = 1;
                SceneManager.LoadScene(0);
            }

            GUILayout.EndArea();
        }
    }
    
    // Śmierć i odradzanie
    [Header("Death and Respawning")]
    [Tooltip("Po jakim czasie (w sekundach) po śmierci gracz się odradza?")]
    public float respawnWaitTime = 2f;
    private bool dead = false;
    [System.NonSerialized] public Vector3 spawnPoint;

    private Quaternion spawnRotation;

    public void Die()
    {
        if (currentHealth <= 0)
            SceneManager.LoadScene(0);
        else
        {
            if (!dead)
            {
                dead = true;
                Invoke("Respawn",respawnWaitTime);
                movementVelocity = Vector3.zero;
                enabled = false;
                characterController.enabled = false;
                modelTrans.gameObject.SetActive(false);
                dashBeginTime = Mathf.NegativeInfinity;
            }
        }
    }

    public void Respawn()
    {
        dead = false;
        trans.position = spawnPoint;
        enabled = true;
        characterController.enabled = true;
        modelTrans.gameObject.SetActive(true);
        modelTrans.rotation = spawnRotation;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        spawnPoint = trans.position;
        spawnRotation = modelTrans.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {
            Movement();
            Dashing();
        }
        Pausing();
        if (Input.GetKey(KeyCode.T))
            Die();
    }
}
