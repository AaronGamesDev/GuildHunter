using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public GameObject mainMenu, settingsMenu;
    public Resolution[] resolutions;
    public Dropdown resDropdown;

    public Toggle toggleFullscreen;

    public static int resIndex = 0;
    public static bool firstRun = true;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        settingsMenu.SetActive(false);

        resolutions = Screen.resolutions;
        resDropdown.ClearOptions();
        List<string> options = new List<string>();
        //int resIndex = 0;
        resIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string res = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(res);

            if (firstRun)
            {
                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    resIndex = i;
                    firstRun = false;
                }
            }
        }

        resDropdown.AddOptions(options);
        resDropdown.value = resIndex;
        resDropdown.RefreshShownValue();


        toggleFullscreen.isOn = Screen.fullScreen;
    }

    public void updateResolution(int resolutionIndex)
    {
        resIndex = resolutionIndex;
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        Debug.Log("resolution function ran");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Back()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("CreateCharacter");
    }

    public void Settings()
    {
        settingsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }
}
