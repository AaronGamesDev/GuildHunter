using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int hp;
    public int maxHp;
    public string type;
    public string name;
    public int baseDmg;
    public int maxStamina;
    public int maxMana;

    public PlayerData (Player player)
    {
        hp = player.hp;
        type = player.type;
        name = player.name;

        if (type == "Knight")//set maxHp and other stats based on character type
        {
            maxHp = 200;
            maxStamina = 100;
            maxMana = 100;
            baseDmg = 50;
        }
        else if (type == "Mage")
        {
            maxHp = 150;
            maxStamina = 75;
            maxMana = 200;
            baseDmg = 40;
        }
        else if (type == "Ranger")
        {
            maxHp = 100;
            maxStamina = 150;
            maxMana = 100;
            baseDmg = 35;
        }
    }
}
