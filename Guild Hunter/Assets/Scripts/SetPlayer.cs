using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetPlayer : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject knight, ranger;
    public bool characterIsOn = false;
    void Start()
    {
        knight = GameObject.Find("Knight");
        ranger = GameObject.Find("Ranger");
        knight.SetActive(false);
        ranger.SetActive(false);//set inactive both playable characters until saved/loaded data type has been checked
        characterIsOn = false;



        PlayerData data = SaveSystem.LoadPlayer();

        if (data.type == "Knight")//check type saved/loaded within data
        {
            knight.SetActive(true);//set player gameObject of that type to active
            characterIsOn = true;
        }
        else if (data.type == "Ranger")
        {
            ranger.SetActive(true);
            characterIsOn = true;
        }
    }
}
