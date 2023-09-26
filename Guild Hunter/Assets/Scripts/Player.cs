using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    public int baseDmg;
    public int critMulti;
    public int critPercen;
    public int inventCap = 3;
    public int stamina;
    public int mana;
    public int maxStamina;
    public int maxMana;
    public string type;
    public int level = 1;
    public bool levelUp = false;
    public int lvlMultiplier;
    public int xp;
    public int maxXp;
    public int dmg;
    public int comboDmg, comboDmg2;

    private bool confirmed = false;
    private float walkSpeed = 4f;
    private float runSpeed = 8f;
    private float jumpHeight = 4f;
    private float rotateSpeed = 75f;
    public Rigidbody rb;

    GroundCheck groundCheck;
    public int jumpCount = 0;
    public bool isJumping = false;

    public Animator anim;

    public bool isWalking = false;
    public bool isRunning = false;

    public bool isAiming = false;
    public bool shot = false;

    public bool attack = false;
    public int chainAttack = 0;

    public bool startTimer = false;

    public int attackCount = 0;

    public int maxHp;

    public int targetsEliminated;

    public GameObject friendly;

    public int targetsEliminatedFriendly;

    public ItemStats[] inventory;

    public ItemStats itemInHand;

    public int inventoryCount = 0;

    public int itemTracker = 0;

    public GameObject[] ItemData;

    public int killTracker = 0;
    public bool secondPassed = false;
    public SetPlayer setPlayer;

    // Use this for initialization
    void Start() {
        setPlayer = GameObject.Find("SetPlayer").GetComponent<SetPlayer>();
        inventCap = 3;
        inventoryCount = 0;
        inventory = new ItemStats[3];
        ItemData = new GameObject[3];
        for (int i = 0; i < inventCap; i++)
        {
            ResetItem(i);
        }
        Debug.Log("Helo");
        if(inventory[2] == null)
         Debug.Log("ListItem2 is set");
        confirmed = false;
        rb = GetComponent<Rigidbody>();

        groundCheck = GetComponent<GroundCheck>();

        anim = GetComponent<Animator>();

        friendly = GameObject.FindGameObjectWithTag("Friendly");

        if (SceneManager.GetActiveScene().name != "CreateCharacter")//if not character creation scene, load player data.
        {
            PlayerData data = SaveSystem.LoadPlayer();

            name = data.name;
            hp = data.hp;
            type = data.type;
            maxHp = data.maxHp;
            maxStamina = data.maxStamina;
            maxMana = data.maxMana;
            baseDmg = data.baseDmg;
        }
        hp = maxHp;
        stamina = maxStamina;
        mana = maxMana;

        level = 1;
        lvlMultiplier = 1;
        levelUp = false;
        dmg = baseDmg * lvlMultiplier;
        if (type == "Knight")
        {
            comboDmg = dmg + 5;
            comboDmg2 = comboDmg + 5;
        }
        maxXp = 1000;
        itemInHand = inventory[0];
        itemTracker = 0;
        killTracker = 0;
    }

    // Update is called once per frame
    void Update() {
        if (setPlayer.characterIsOn)
        {
            StartCoroutine(Timer());//start timer
            setPlayer.characterIsOn = false;
        }
        UseInventory();
        XpToLevel();
        ApplyMultiplier();
        AnimMovement();
        MissionComplete();
        OnDeath();
    }

    void ResetItem(int i)
    {
        ItemData[i] = new GameObject("Empty");
        ItemData[i].transform.parent = this.gameObject.transform;
        ItemData[i].transform.position = Vector3.zero;
        ItemData[i].AddComponent<ItemStats>();
        inventory[i] = ItemData[i].GetComponent<ItemStats>();
    }

    void CloneItemStats (ItemStats ItemA, ItemStats ItemB)
    {
        ItemA.effect = ItemB.effect;
        ItemA.type = ItemB.type;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "HealthPotion" && inventoryCount != inventCap)//inventory.Count != inventCap)
        {
            for (int i = 0; i < inventCap; i++)
            {
                if (inventory[i].name == "Empty")
                {
                    CloneItemStats(ItemData[i].GetComponent<ItemStats>(), other.gameObject.GetComponent<ItemStats>());
                    inventory[i] = ItemData[i].GetComponent<ItemStats>();
                    ItemData[i].name = other.gameObject.name;
                    inventoryCount++;
                    i = 3;
                    Destroy(other.gameObject);
                }
            }

        }
        else if (inventoryCount == inventCap)
        {
            Debug.Log("Inventory is full");
        }
    }

    void UseInventory()
    {
        if (Input.GetKeyDown("e"))
        {
            itemTracker++;
            if (itemTracker > 2)
            {
                itemTracker = 0;
            }
        }
        else if (Input.GetKeyDown("q"))
        {
            itemTracker--;
            if (itemTracker < 0)
            {
                itemTracker = 2;
            }
        }
        itemInHand = inventory[itemTracker];


        if (Input.GetKeyDown("f"))
        {
            if (itemInHand.type == "HealthPotion")
            {
                hp += inventory[itemTracker].effect;
                Destroy(ItemData[itemTracker]);//destory game object holding script that
                ResetItem(itemTracker);//reset inventory slot
                inventoryCount--;
            }
            else
            {
                Debug.Log("This inventory slot is empty");
            }
        }
    }

    IEnumerator Timer()
    {
        if (secondPassed == false)
        {
            
            yield return new WaitForSeconds(1f);
            Debug.Log("Second Passed");
            secondPassed = true;
            //Debug.Log("set true for second passed");
            if (isRunning == true)//when running 
            {
                //Debug.Log("set true for second passed");
                stamina -= 5;//decrement stamina by 5 every second passed
            }
            RegenStamina();
            secondPassed = false;
            StartCoroutine(Timer());
        }
    }

    void UpdateStamina()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("jump"))
        {
            stamina -= (maxStamina / 10);//decrease stamina by 10%
        }

        if (type == "Knight")
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("atk 1"))
            {
                stamina -= 5;//decrease stamina by 5
            }
            else if (anim.GetCurrentAnimatorStateInfo(0).IsName("atk 2"))
            {
                stamina -= 10;//decrease stamina by 10
            }
            else if (anim.GetCurrentAnimatorStateInfo(0).IsName("atk 3"))
            {
                stamina -= 15;//decrease stamina by 15
            }
        }
        else if (type == "Ranger")
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("shoot"))
            {
                stamina -= 5;//decrease stamina by 5
            }
        }

        if (stamina <= 0)
        {
            stamina = 0;
        }
    }

    void RegenStamina()
    {
        if (type == "Knight")
        {
            if (anim.GetBool("isWalking") || anim.GetCurrentAnimatorStateInfo(0).IsName("idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("idle 1") || anim.GetCurrentAnimatorStateInfo(0).IsName("idle 2") || anim.GetCurrentAnimatorStateInfo(0).IsName("idle 3"))
            {
                if (secondPassed == true)
                {
                    if (stamina < maxStamina)
                    {
                        stamina += 10;
                    }
                    if (stamina >= maxStamina)
                    {
                        stamina = maxStamina;
                    }
                }
            }
        }
        else if (type == "Ranger")
        {

            if (anim.GetBool("isWalking") || anim.GetCurrentAnimatorStateInfo(0).IsName("idle 1") || anim.GetCurrentAnimatorStateInfo(0).IsName("idle 2") || anim.GetCurrentAnimatorStateInfo(0).IsName("idle 3"))
            {
                if (secondPassed == true)
                {
                    if (stamina < maxStamina)
                    {
                        stamina += 10;
                    }
                    if (stamina >= maxStamina)
                    {
                        stamina = maxStamina;
                    }
                }
            }
        }
    }

    void ApplyMultiplier()
    {
        if (levelUp)
        {
            levelUp = false;//reset
            lvlMultiplier = level;
            maxHp *= lvlMultiplier;
            dmg = baseDmg * lvlMultiplier;
            maxStamina *= lvlMultiplier;
            maxMana *= lvlMultiplier;
            if (type == "Knight")
            {
                comboDmg = dmg + 5;
                comboDmg2 = comboDmg + 5;
            }
        }
    }

    void AnimMovement()
    {
        float translationRun = Input.GetAxis("Vertical") * runSpeed;
        float translationWalk = Input.GetAxis("Vertical") * walkSpeed;
        float rotation = Input.GetAxis("Horizontal") * rotateSpeed;

        translationRun *= Time.deltaTime;
        translationWalk *= Time.deltaTime;
        rotation *= Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftShift) && groundCheck.isGrounded && stamina > 0)
        {
            isRunning = true;
            isWalking = false;
            transform.Rotate(0, rotation, 0);
            transform.Translate(0, 0, translationRun);
        }
        else
        {
            isWalking = true;
            isRunning = false;
            transform.Rotate(0, rotation, 0);
            transform.Translate(0, 0, translationWalk);
        }

        if (Input.GetKeyDown(KeyCode.Space) && stamina >= (maxStamina / 10))
        {
            if (jumpCount == 0 && groundCheck.isGrounded && !isJumping)
            {
                rb.AddForce(new Vector3(0, jumpHeight, 0), ForceMode.Impulse);
                jumpCount = 1;
                isJumping = true;
            }

        }
        else if (groundCheck.isGrounded && isJumping)
        {
            jumpCount = 0;
            isJumping = false;
        }

        if (translationWalk != 0)
        {
            if (isWalking)
            {
                anim.SetBool("isWalking", true);
                anim.SetBool("isRunning", false);
            }
            else if (isRunning)
            {
                anim.SetBool("isWalking", false);
                anim.SetBool("isRunning", true);
            }

            if (translationWalk > 0)
            {
                anim.SetBool("Forward", true);
                anim.SetBool("Backward", false);
            }
            else if (translationWalk < 0)
            {
                anim.SetBool("Backward", true);
                anim.SetBool("Forward", false);
            }

        }
        else
        {
            isWalking = false;
            isRunning = false;
            anim.SetBool("isWalking", false);
            anim.SetBool("Forward", false);
            anim.SetBool("Backward", false);
            anim.SetBool("isRunning", false);
        }

        if (isJumping)
        {
            if (jumpCount == 0)
            {
                anim.SetBool("Jumping", true);
            }
            else if (jumpCount == 1)
            {
                anim.SetBool("Jumping", true);
            }

        }
        else
        {
            anim.SetBool("Jumping", false);
        }

        if (type == "Ranger")
        {
            if (Input.GetMouseButton(1))//if right mouse button held
            {
                shot = false;
                isAiming = true;

                if (Input.GetMouseButtonDown(0) && stamina >= 10)
                {
                    shot = true;
                }
            }
            else if (Input.GetMouseButtonUp(1))//if right mouse button released
            {
                shot = false;
                isAiming = false;
            }

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("draw arrow") || anim.GetCurrentAnimatorStateInfo(0).IsName("aiming") || anim.GetCurrentAnimatorStateInfo(0).IsName("shoot"))
            {
                anim.SetBool("canDraw", false);
            }
            else
            {
                anim.SetBool("canDraw", true);
            }

            if (isAiming)
            {

                anim.SetBool("isAiming", true);
                if (shot)
                {
                    anim.SetBool("shot", true);
                }
                else
                {
                    anim.SetBool("shot", false);
                }
            }
            else
            {
                anim.SetBool("isAiming", false);
                anim.SetBool("shot", false);

            }
        }


        if (type == "Knight")//if knight character/class
        {
            if (Input.GetMouseButtonDown(0))
            {
                attack = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                attack = false;
            }

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("idle"))
            {
                anim.ResetTrigger("TriggerAtk1");
                anim.ResetTrigger("TriggerAtk2");
                anim.SetBool("ChainAttack", false);
                attackCount = 0;//reset attack count
            }

            if (attack && stamina >= 5)
            {
                anim.SetBool("Attack", true);
                attackCount = 0;//reset attack count
            }
            else
            {
                anim.SetBool("Attack", false);
            }

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("atk 1"))//if in first attack animation increment attack count
            {
                attackCount++;
            }

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("atk 1") && attack && stamina >= 10)//if in first attack animation and attack, move to next attack animation
            {
                attackCount++;
                anim.SetBool("ChainAttack", true);
                anim.SetTrigger("TriggerAtk1");
            }

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("atk 2") && attack && stamina >= 15)
            {
                attackCount++;
                anim.SetBool("ChainAttack", true);
                anim.SetTrigger("TriggerAtk2");
            }
        }
    }

    void XpToLevel()
    {
        targetsEliminated = gameObject.GetComponent<HitDetect>().targetsEliminated;
        targetsEliminatedFriendly = friendly.GetComponent<HitDetect>().targetsEliminated;
        if (targetsEliminated > 0)//if player eliminates a target
        {
            gameObject.GetComponent<HitDetect>().targetsEliminated = 0;
            Debug.Log("getting xp");
            xp += targetsEliminated * 100;//get 100 xp per target eliminated
            killTracker++;
        }

        if (targetsEliminatedFriendly > 0)//if friendly AI eliminates a target
        {
            friendly.GetComponent<HitDetect>().targetsEliminated = 0;
            Debug.Log("getting xp");
            xp += targetsEliminatedFriendly * 100;//get 100 xp per target eliminated
            killTracker++;
        }

        if (xp >= maxXp)
        {
            Debug.Log("Level up!");
            levelUp = true;
            level++;
            xp -= maxXp;
        }
    }

    void MissionComplete()
    {
        if (killTracker >= 15)
        {
            //switch scenes to end/missioncomplete screen
            SceneManager.LoadScene("End");
        }
    }

    void OnDeath()
    {
        if (hp <= 0)
        {
            SceneManager.LoadScene("Death");
        }
    }
}
