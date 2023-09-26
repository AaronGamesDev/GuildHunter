using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friendly : Entity
{
    public GameObject player;
    public List<GameObject> targets;
    public float distanceToPlayer, distanceToEnemy;
    public List<float> distances;
    public GameObject activeTarget;
    public Animator anim;
    public int walkSpeed, runSpeed;
    public int stamina;
    public bool nearPlayer;
    public int maxHp, maxStamina, maxMana, baseDmg, mana, dmg;
    public GameObject closestEnemy;
    public bool prioritiseEnemy;
    public string type;
    public bool isAiming = false;
    public bool lowHp;
    public bool secondPassed = false;
    public bool slowFollow;
    public int lvlMultiplier;
    public bool levelUp;
    // Start is called before the first frame update
    void Start()
    {
        type = "Ranger";
        player = GameObject.FindGameObjectWithTag("Player");

        targets = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));

        anim = gameObject.GetComponent<Animator>();
        walkSpeed = 4;
        runSpeed = 8;

        maxHp = 100;
        maxStamina = 150;
        maxMana = 100;
        baseDmg = 35;
        hp = maxHp;
        stamina = maxStamina;
        mana = maxMana;
        dmg = baseDmg;

        prioritiseEnemy = false;
        isAiming = false;
        lowHp = false;

        secondPassed = false;
        slowFollow = false;
        levelUp = false;
        lvlMultiplier = player.GetComponent<Player>().lvlMultiplier;
        checks();
        StartCoroutine(Timer());
    }


    void checks()
    {
        targets = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        if (distances.Count >= targets.Count)
        {
            distances.Clear();
        }

        int totalEnemies = 0;
        foreach (GameObject enemy in targets)
        {
            distanceToEnemy = Vector3.Distance(enemy.transform.position, transform.position);
            distances.Add(distanceToEnemy);
            Debug.Log("set distances");
            totalEnemies++;
        }
        for (int i = 0; i < targets.Count; i++)
        {
            if (distanceToEnemy > distances[i])//is distanceToEnemy is bigger than distances[i]
            {
                distanceToEnemy = distances[i];//set new distance to closest Enemy
                closestEnemy = targets[i];//set new closest Enemy
            }
            else if (distanceToEnemy == distances[i])//if distanceToEnemy is equal to distances[i]
            {
                closestEnemy = targets[i];//set closestEnemy
            }
        }

        if (targets.Count == 0)
        {
            closestEnemy = null;
            distanceToEnemy = 0;
        }
    }
    // Update is called once per frame
    void Update()
    {
        ApplyMultiplier();
        checks();
        CanSeeEnemy();
        Controller();
        Shoot();
    }

    void UpdateStamina()//this function is called by animation
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("jump"))
        {
            stamina -= (maxStamina / 10);//decrease stamina by 10%
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("shoot"))
        {
            stamina -= 5;//decrease stamina by 5
        }
        if (stamina <= 0)
        {
            stamina = 0;
        }
    }

    void RegenStamina()
    {
        if (anim.GetBool("isWalking") || anim.GetCurrentAnimatorStateInfo(0).IsName("idle 1") || anim.GetCurrentAnimatorStateInfo(0).IsName("idle 2") || anim.GetCurrentAnimatorStateInfo(0).IsName("idle 3"))
        {
            if (secondPassed == true)
            {
                if (stamina < maxStamina)
                {
                    stamina += 10;
                }
                else if (stamina >= maxStamina)
                {
                    stamina = maxStamina;
                }
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
            if (anim.GetBool("isRunning"))//when running 
            {
                //Debug.Log("set true for second passed");
                stamina -= 5;//decrement stamina by 5 every second passed
            }
            RegenStamina();
            secondPassed = false;
            StartCoroutine(Timer());
        }
    }

    void ApplyMultiplier()
    {
        if (lvlMultiplier < player.GetComponent<Player>().lvlMultiplier)
        {
            lvlMultiplier = player.GetComponent<Player>().lvlMultiplier;
            levelUp = true;
        }

        if (levelUp)
        {
            levelUp = false;
            maxHp *= player.GetComponent<Player>().lvlMultiplier;
            dmg = baseDmg * player.GetComponent<Player>().lvlMultiplier;
            maxStamina *= player.GetComponent<Player>().lvlMultiplier;
            maxMana *= player.GetComponent<Player>().lvlMultiplier;
        }
    }

    void MoveAwayFromTarget()
    {
        anim.SetBool("Forward", false);
        anim.SetBool("isIdle", false);
        if (stamina > 0)//if has stamina
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", true);
            anim.SetBool("Backward", true);
            transform.position = Vector3.MoveTowards(transform.position, activeTarget.transform.position, -runSpeed * Time.deltaTime);//run
        }
        else if (stamina <= 0)//if no stamina
        {
            anim.SetBool("isRunning", false);
            anim.SetBool("isWalking", true);
            anim.SetBool("Backward", true);
            transform.position = Vector3.MoveTowards(transform.position, activeTarget.transform.position, -walkSpeed * Time.deltaTime);//walk
        }
    }

    void TurnToFaceOp()
    {
        Vector3 lookRotation = Quaternion.LookRotation(transform.position - activeTarget.transform.position).eulerAngles;
        lookRotation.x = 0;
        lookRotation.z = 0;
        Quaternion lookAtRotation = Quaternion.Euler(lookRotation);
        Vector3 moveRotation = Quaternion.Slerp(transform.rotation, lookAtRotation, 2f * Time.deltaTime).eulerAngles;
        // Rotate y axis only
        moveRotation.x = 0;
        moveRotation.z = 0;
        // Apply rotation
        transform.rotation = Quaternion.Euler(moveRotation);
    }

    void HealSelf()
    {
        hp += maxHp / 2;
    }

    void HealPlayer()
    {
        if (nearPlayer)//if players health is below half
        {
            player.GetComponent<Player>().hp += (player.GetComponent<Player>().maxHp / 2);//heal player by half hp
        }
    }

    void Controller()
    {
        if (distanceToPlayer > 5 && !prioritiseEnemy && !lowHp)
        {
            isAiming = false;
            Debug.Log("too far from player");
            nearPlayer = false;
            activeTarget = player;
            TurnToFace();
            MoveTowardsTarget();
            //move towards player
        }
        else if (distanceToPlayer <= 5 && !prioritiseEnemy && !lowHp)
        {
            isAiming = false;
            Debug.Log("near player");
            nearPlayer = true;
            activeTarget = player;
            TurnToFace();
            if (player.GetComponent<Player>().hp <= (player.GetComponent<Player>().maxHp / 2))//if players health is below half
            {
                HealPlayer();
                Debug.Log("Healing player");
            }

            if (player.GetComponent<Player>().type == "Ranger" && (player.GetComponent<Player>().anim.GetCurrentAnimatorStateInfo(0).IsName("idle 1") || player.GetComponent<Player>().anim.GetCurrentAnimatorStateInfo(0).IsName("idle 2") || player.GetComponent<Player>().anim.GetCurrentAnimatorStateInfo(0).IsName("idle 3")))
            {
                slowFollow = false;
                EndMovingAnimations();
            }
            else if (player.GetComponent<Player>().type == "Knight" && (player.GetComponent<Player>().anim.GetCurrentAnimatorStateInfo(0).IsName("idle") || player.GetComponent<Player>().anim.GetCurrentAnimatorStateInfo(0).IsName("idle 1") || player.GetComponent<Player>().anim.GetCurrentAnimatorStateInfo(0).IsName("idle 2") || player.GetComponent<Player>().anim.GetCurrentAnimatorStateInfo(0).IsName("idle 3")))
            {
                slowFollow = false;
                EndMovingAnimations();
            }
            else if (player.GetComponent<Player>().anim.GetBool("isWalking"))
            {
                slowFollow = true;
                if (distanceToPlayer > 3)
                {

                    MoveTowardsTarget();
                }
            }
            else if (player.GetComponent<Player>().anim.GetBool("isRunning"))
            {
                slowFollow = false;
                if (distanceToPlayer > 3)
                {

                    MoveTowardsTarget();
                }
            }
                //stop moving animations
                //idle
        }

        if ((distanceToEnemy < distanceToPlayer || nearPlayer) && stamina >= 5 && distanceToEnemy <= 10 && closestEnemy != null && CanSeeEnemy() && !(player.GetComponent<Player>().hp <= (player.GetComponent<Player>().maxHp / 2)) && !lowHp)//if enemy is closer to ai than ai is to player or is already near to player AND if closestEnemy exists AND cansee AND player isnt low health (and not low health themself)
        {
            //if enemy within 10 units and stamina is greater or equal to 5 (enough to shoot)
            isAiming = true;
            EndMovingAnimations();
            prioritiseEnemy = true;
            activeTarget = closestEnemy;
            Debug.Log("prioritising enemy over player");
            TurnToFace();
            //go after enemy
        }
        else if ((distanceToEnemy < distanceToPlayer || nearPlayer) && stamina != 0 && !(distanceToEnemy <= 10) && closestEnemy != null && CanSeeEnemy() && !(player.GetComponent<Player>().hp <= (player.GetComponent<Player>().maxHp / 2)) && !lowHp)//if enemy is closer to ai than ai is to player or is already near to player AND if closestEnemy exists AND cansee AND player isnt low health (and not low health themself)
        {
            //if enemy not within 10 units and stamina is not equal to 0
            isAiming = false;
            EndMovingAnimations();
            prioritiseEnemy = true;
            activeTarget = closestEnemy;
            Debug.Log("prioritising enemy over player");
            TurnToFace();
            MoveTowardsTarget();
            //go after enemy
        }
        else if ((!CanSeeEnemy() || player.GetComponent<Player>().hp <= (player.GetComponent<Player>().maxHp / 2) || closestEnemy == null || stamina == 0) && !lowHp)//if cant see enemy or player hp is below half or no enemies (and not low health)
        {
            //if stamina is 0
            isAiming = false;
            Debug.Log("player is priority now");
            prioritiseEnemy = false;//dont prioritise enemy
        }

        if (hp <= maxHp / 2)//if below half health
        {
            isAiming = false;
            prioritiseEnemy = false;
            lowHp = true;
            activeTarget = closestEnemy;
            Debug.Log("low health");
            TurnToFace();//turn to face enemy
            if (distanceToEnemy >= 5)
            {
                HealSelf();//heal self
            }
            else
            {
                MoveAwayFromTarget();//move backwards
            }
            //run away from enemy
            //if further than 5 units away from enemy then heal self
        }
        else
        {
            lowHp = false;
        }
    }

    void Shoot()
    {
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

            if (stamina >= 5)
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

    void EndShootingAnim()
    {
        anim.SetBool("canDraw", false);
        anim.SetBool("isAiming", false);
        anim.SetBool("shot", false);
    }
    void EndMovingAnimations()
    {
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("Forward", false);
    }

    bool CanSeeEnemy()
    {
        if (closestEnemy != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, closestEnemy.transform.position - transform.position, out hit))
            {
                
                if (hit.collider.tag == closestEnemy.tag)
                {
                    Debug.DrawRay(transform.position, closestEnemy.transform.position - transform.position, Color.green);
                    Debug.Log("Can see enemy");
                    return true;
                }
                else
                {
                    Debug.DrawRay(transform.position, closestEnemy.transform.position - transform.position, Color.yellow);
                    Debug.Log("Something obstructing view of enemy");
                    return false;
                }
            }
            else
            {
                Debug.DrawRay(transform.position, closestEnemy.transform.position - transform.position, Color.yellow);
                return false;
            }
        }
        else
        {
            Debug.Log("There are no enemies to look for");
            return false;
        }

    }

    void MoveTowardsTarget()
    {
        anim.SetBool("Backward", false);
        anim.SetBool("isIdle", false);
        if (stamina > 0 && !(nearPlayer && slowFollow))//if has stamina
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", true);
            anim.SetBool("Forward", true);
            transform.position = Vector3.MoveTowards(transform.position, activeTarget.transform.position, runSpeed * Time.deltaTime);//run
        }
        else if (stamina <= 0 || (nearPlayer && slowFollow))//if no stamina or near the player and player is walking
        {
            anim.SetBool("isRunning", false);
            anim.SetBool("isWalking", true);
            anim.SetBool("Forward", true);
            transform.position = Vector3.MoveTowards(transform.position, activeTarget.transform.position, walkSpeed * Time.deltaTime);//walk
        }
    }

    void TurnToFace()
    {
        Vector3 lookRotation = Quaternion.LookRotation(activeTarget.transform.position - transform.position).eulerAngles;
        lookRotation.x = 0;
        lookRotation.z = 0;
        Quaternion lookAtRotation = Quaternion.Euler(lookRotation);
        Vector3 moveRotation = Quaternion.Slerp(transform.rotation, lookAtRotation, 2f * Time.deltaTime).eulerAngles;
        // Rotate y axis only
        moveRotation.x = 0;
        moveRotation.z = 0;
        // Apply rotation
        transform.rotation = Quaternion.Euler(moveRotation);
    }
}
