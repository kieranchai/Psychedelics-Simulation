using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject pauseMenu;
    public bool pauseIsActive = false;

    private void Awake()
    {
        pauseIsActive = false;
        pauseMenu.SetActive(pauseIsActive);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        pauseIsActive = !pauseIsActive;
        pauseMenu.SetActive(pauseIsActive);

        if (pauseIsActive == true)
        {
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f;
        }
    }

    public void ResumeGame()
    {
        TogglePauseMenu();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
