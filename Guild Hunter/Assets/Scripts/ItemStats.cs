using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStats : MonoBehaviour
{
    // Start is called before the first frame update
    public int effect;
    public string type;

    private void Start()
    {
        if (tag == "HealthPotion")
        {
            type = "HealthPotion";
        }
        else
        {
            type = "Empty";
        }
    }
    // Update is called once per frame
    void Update()
    {
        SetStats();
    }

    void SetStats()
    {
        if (type == "HealthPotion")
        {
            effect = 50;
        }
    }
}
