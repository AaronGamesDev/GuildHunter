using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterCreationMenu : MonoBehaviour {

    public Player player;
    public GameObject createScreen, proceedScreen;
    public Button knightBtn, mageBtn, rangerBtn;
    public int btnTracker = 0;
    public string nameHolder;
    public InputField inputField;
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        knightBtn = GameObject.Find("Knight").GetComponent<Button>();
        mageBtn = GameObject.Find("Mage").GetComponent<Button>();
        rangerBtn = GameObject.Find("Ranger").GetComponent<Button>();

        knightBtn.gameObject.SetActive(true);
        mageBtn.gameObject.SetActive(false);
        rangerBtn.gameObject.SetActive(false);

        proceedScreen.SetActive(false);

        inputField = GameObject.Find("InputField").GetComponent<InputField>();
    }

    void Update()
    {
        ShowCurrentBtn();
    }

    public void SetCharName(string name)
    {
        nameHolder = name;
    }

    public void ArrowRight()
    {
        btnTracker++;
        if (btnTracker > 2)
        {
            btnTracker = 0;
        }
    }

    public void ArrowLeft()
    {
        btnTracker--;
        if (btnTracker < 0)
        {
            btnTracker = 2;
        }
    }

    public void ShowCurrentBtn()
    {
        if (btnTracker == 0)
        {
            knightBtn.gameObject.SetActive(true);
            mageBtn.gameObject.SetActive(false);
            rangerBtn.gameObject.SetActive(false);
        }
        else if (btnTracker == 1)
        {
            knightBtn.gameObject.SetActive(false);
            mageBtn.gameObject.SetActive(true);
            rangerBtn.gameObject.SetActive(false);
        }
        else if (btnTracker == 2)
        {
            knightBtn.gameObject.SetActive(false);
            mageBtn.gameObject.SetActive(false);
            rangerBtn.gameObject.SetActive(true);
        }
    }

    public void ApplyToCharacter()
    {
        player.name = nameHolder;//apply name to character

        if (btnTracker == 0)//if 0
        {
            player.type = "Knight";//set characters type to Knight
        }
        else if (btnTracker == 1)//if 1
        {
            player.type = "Mage";//set characters type to Mage
        }
        else if (btnTracker == 2)//if 2
        {
            player.type = "Ranger";//set characters type to Ranger
        }
    }
    public void SaveCharacter()
    {
        //run save to file function
        SaveSystem.SavePlayer(player);
        createScreen.SetActive(false);
        proceedScreen.SetActive(true);
    }

    public void LoadCharacter()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        player.hp = data.hp;
        player.type = data.type;
        player.name = data.name;

        nameHolder = player.name;
        
        
        inputField.text = nameHolder;
        if (player.type == "Knight")
        {
            btnTracker = 0;
        }
        else if (player.type == "Mage")
        {
            btnTracker = 1;
        }
        else if (player.type == "Ranger")
        {
            btnTracker = 2;
        }

    }

    public void Confirm()
    {
        SceneManager.LoadScene("Game");
    }

    public void Decline()
    {
        proceedScreen.SetActive(false);
        createScreen.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Start");
    }
}
