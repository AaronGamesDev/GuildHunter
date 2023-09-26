using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public static bool paused = false;
    public GameObject pauseMenu, settingsMenu;

    Resolution[] resolutions;
    public Dropdown resDropdown;

    public bool inSettings = false;

    public Toggle toggleFullscreen;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        settingsMenu.SetActive(false);

        resolutions = Screen.resolutions;
        resDropdown.ClearOptions();
        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string res = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(res);
        }

        resDropdown.AddOptions(options);
        resDropdown.value = StartMenu.resIndex;
        resDropdown.RefreshShownValue();

        toggleFullscreen.isOn = Screen.fullScreen;
    }

    public void updateResolution(int resolutionIndex)
    {
        StartMenu.resIndex = resolutionIndex;
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        Debug.Log("resolution function ran");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused && !inSettings)
            {
                Resume();
            }
            else if (!paused)
            {
                Pause();
            }
            else if (paused && inSettings)
            {
                inSettings = false;
                Back();
            }
        }
    }

    public void Resume()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
    }

    public void Pause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        paused = true;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        paused = false;
        SceneManager.LoadScene("Start");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        paused = false;
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Settings()
    {
        inSettings = true;
        settingsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void Back()
    {
        pauseMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void ToggleFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }
}
