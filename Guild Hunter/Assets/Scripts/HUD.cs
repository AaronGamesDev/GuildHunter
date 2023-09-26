using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    public Transform pos1, pos2, pos3;
    public Image healthBar, staminaBar, xpTracker, item1, item2, item3;
    public Text lvlText;
    public float playerHp, playerStamina, playerMaxHp, playerMaxStamina, playerXp, playerMaxXp, playerLevel, prevHealthFill, prevStaminaFill, prevXpFill, lerpSpeed;
    public int playerItemTracker;
    public Sprite emptySlot, healthPotion;
    public float transparent, opaque;
    public Color item1Color, item2Color, item3Color;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        lerpSpeed = 0.1f;
        transparent = 0.5f; 
        opaque = 1f;
        item1Color = item1.color;
        item2Color = item2.color;
        item3Color = item3.color;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerStats();
        SetInventoryImages();
        SetImagePositions();
        prevHealthFill = healthBar.fillAmount;
        healthBar.fillAmount = Mathf.Lerp(prevHealthFill, playerHp/playerMaxHp, lerpSpeed);
        prevStaminaFill = staminaBar.fillAmount;
        staminaBar.fillAmount = Mathf.Lerp(prevStaminaFill, playerStamina / playerMaxStamina, lerpSpeed);
        prevXpFill = xpTracker.fillAmount;
        xpTracker.fillAmount = Mathf.Lerp(prevXpFill, playerXp / playerMaxXp, lerpSpeed);
        lvlText.text = "Lvl. " + playerLevel;

    }

    void SetImagePositions()
    {
        if (playerItemTracker == 0)
        {
            item1.transform.position = Vector3.Lerp(item1.transform.position, pos1.position, lerpSpeed);
            item1.transform.localScale = Vector3.Lerp(item1.transform.localScale, pos1.localScale, lerpSpeed);

            item2.transform.position = Vector3.Lerp(item2.transform.position, pos2.position, lerpSpeed);
            item2.transform.localScale = Vector3.Lerp(item2.transform.localScale, pos2.localScale, lerpSpeed);

            item3.transform.position = Vector3.Lerp(item3.transform.position, pos3.position, lerpSpeed);
            item3.transform.localScale = Vector3.Lerp(item3.transform.localScale, pos3.localScale, lerpSpeed);
        }
        else if (playerItemTracker == 1)
        {
            item2.transform.position = Vector3.Lerp(item2.transform.position, pos1.position, lerpSpeed);
            item2.transform.localScale = Vector3.Lerp(item2.transform.localScale, pos1.localScale, lerpSpeed);

            item3.transform.position = Vector3.Lerp(item3.transform.position, pos2.position, lerpSpeed);
            item3.transform.localScale = Vector3.Lerp(item3.transform.localScale, pos2.localScale, lerpSpeed);

            item1.transform.position = Vector3.Lerp(item1.transform.position, pos3.position, lerpSpeed);
            item1.transform.localScale = Vector3.Lerp(item1.transform.localScale, pos3.localScale, lerpSpeed);
        }
        else if (playerItemTracker == 2)
        {
            item3.transform.position = Vector3.Lerp(item3.transform.position, pos1.position, lerpSpeed);
            item3.transform.localScale = Vector3.Lerp(item3.transform.localScale, pos1.localScale, lerpSpeed);

            item1.transform.position = Vector3.Lerp(item1.transform.position, pos2.position, lerpSpeed);
            item1.transform.localScale = Vector3.Lerp(item1.transform.localScale, pos2.localScale, lerpSpeed);

            item2.transform.position = Vector3.Lerp(item2.transform.position, pos3.position, lerpSpeed);
            item2.transform.localScale = Vector3.Lerp(item2.transform.localScale, pos3.localScale, lerpSpeed);
        }

    }

    void SetInventoryImages()
    {
        if (player.GetComponent<Player>().inventory[0].type == "HealthPotion")
        {
            item1.sprite = healthPotion;
        }
        else
        {
            item1.sprite = emptySlot;
        }

        if (player.GetComponent<Player>().inventory[1].type == "HealthPotion")
        {
            item2.sprite = healthPotion;
        }
        else
        {
            item2.sprite = emptySlot;
        }

        if (player.GetComponent<Player>().inventory[2].type == "HealthPotion")
        {
            item3.sprite = healthPotion;
        }
        else
        {
            item3.sprite = emptySlot;
        }

        if (playerItemTracker == 0)//if item is in position 1
        {
            Color prevItemColor = item1Color;
            item1Color.a = Mathf.Lerp(prevItemColor.a, opaque, lerpSpeed);//set image alpha to opaque
            item1.color = item1Color;
            Debug.Log("item1 is in pos1");
        }
        else
        {
            Color prevItemColor = item1Color;
            item1Color.a = Mathf.Lerp(prevItemColor.a, transparent, lerpSpeed);//else set to transparent
            item1.color = item1Color;
            Debug.Log("item1 is not in pos1");
        }

        if (playerItemTracker == 1)
        {
            Color prevItemColor = item2Color;
            item2Color.a = Mathf.Lerp(prevItemColor.a, opaque, lerpSpeed);
            item2.color = item2Color;
        }
        else
        {
            Color prevItemColor = item2Color;
            item2Color.a = Mathf.Lerp(prevItemColor.a, transparent, lerpSpeed);
            item2.color = item2Color;
        }

        if (playerItemTracker == 2)
        {
            Color prevItemColor = item3Color;
            item3Color.a = Mathf.Lerp(prevItemColor.a, opaque, lerpSpeed);
            item3.color = item3Color;
        }
        else
        {
            Color prevItemColor = item3Color;
            item3Color.a = Mathf.Lerp(prevItemColor.a, transparent, lerpSpeed);
            item3.color = item3Color;
        }
    }

    void UpdatePlayerStats()
    {
        playerHp = player.GetComponent<Player>().hp;
        playerMaxHp = player.GetComponent<Player>().maxHp;
        playerStamina = player.GetComponent<Player>().stamina;
        playerMaxStamina = player.GetComponent<Player>().maxStamina;
        playerXp = player.GetComponent<Player>().xp;
        playerMaxXp = player.GetComponent<Player>().maxXp;
        playerLevel = player.GetComponent<Player>().level;
        playerItemTracker = player.GetComponent<Player>().itemTracker;
    }
}
