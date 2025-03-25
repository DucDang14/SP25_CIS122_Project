using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public static bool paused = false;
    public GameObject Canvas;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(paused)
            {
                Play();
            }
            else
            {
                Stop();
            }
        }
    }
    void Stop()
    {
        Canvas.SetActive(true);
        Time.timeScale = 0f;
        paused = true;
    }

    public void Play()
    {
        Canvas.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
