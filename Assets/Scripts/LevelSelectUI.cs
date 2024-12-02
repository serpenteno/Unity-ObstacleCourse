using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectUI : MonoBehaviour
{
    // Indeks budowania aktualnie załadowanej sceny.
    private int currentScene = 0;

    // Kamera podglądu poziomu dla bieżącej sceny, o ile istnieje.
    private GameObject levelViewCamera;

    // Trwająca obecnie operacja ładowania sceny, o ile istnieje.
    private AsyncOperation curretLoadOperation;

    void OnGUI()
    {
        GUILayout.Label("OBSTACLE COURSE");

        // Jeśli nie jest to menu główne:
        if (currentScene != 0)
        {
            GUILayout.Label("Currently viewing Level " + currentScene);

            // Pokaż przycisk PLAY:
            if (GUILayout.Button("PLAY"))
            {
                // Jeśli przycisk został kliknięty, rozpocznij odtwarzanie poziomu:
                PlayCurrentLevel();
            }
        }
        else // Jeśli jest to główne menu
            GUILayout.Label("Select a level to preview it.");

        // Zaczynając od indeksu budowania sceny 1, 
        // przechodzimy w pętli przez pozostałe indeksy scen:
        for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            // Pokaż przycisk z tekstem "Level [numer poziomu]"
            if (GUILayout.Button("Level " + i))
            {
                // Jeśli ten przycisk zostanie naciśnięty i nie oczekujemy 
                // aktualnie na załadowanie sceny:
                if (curretLoadOperation == null)
                {
                    // Rozpocznij asynchroniczne ładowanie poziomu:
                    curretLoadOperation = SceneManager.LoadSceneAsync(i);

                    // Ustaw bieżącą scenę:
                    currentScene = i;
                }
            }
        }
    }

    private void PlayCurrentLevel()
    {
        // Dezaktywuj kamerę podglądu poziomu:
        levelViewCamera.SetActive(false);

        // Spróbuj znaleźć obiekt Player:
        var playerGobj = GameObject.Find("Player");

        // Zarejestruj błąd, jeśli nie można go znaleźć
        if (playerGobj == null)
            Debug.LogError("Couldn't find a Player in the level!");
        else // Jeśli znaleziono gracza:
        {
            // Uzyskaj dołączony skrypt Player i włącz go:
            var playerScript = playerGobj.GetComponent<Player>();
            playerScript.enabled = true;

            // Poprzez skrypt Player uzyskaj dostęp do obiektu kamery
            // i aktywuj ją:
            playerScript.cam.SetActive(true);

            // Zniszcz obiekt, do którego jest przypisany ten skrypt,
            // obiet pojawi się ponownie, gdy główna scena będzie znowu
            // ładowana:
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // Zapewniamy, aby ten obiekt nadal istniał, gdy scena się zmieni:
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Jeśli mamy aktualną operację ładowania i jest ona zakończona:
        if (curretLoadOperation != null && curretLoadOperation.isDone)
        {
            // Przypisz null do operacji ładowania:
            curretLoadOperation = null;

            // Znajdź kamerę podglądu poziomu na scenie:
            levelViewCamera = GameObject.Find("Level View Camera");

            // Zarejestruj błąd, jeśli nie mogliśmy znaleźć kamery:
            if (levelViewCamera == null)
                Debug.LogError("No level view camera was found in the scene!");
        }
    }
}
