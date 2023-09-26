using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public HitDetect hitDetect;
    public GameObject player, friendly;
    public float atkRange = 3;
    public float viewRange = 10;
    public bool canSeePlayer, canSeeFriendly;
    public GameObject activeTarget = null;
    public float distanceToPlayer, distanceToFriendly, distanceToTarget;
    public float speed = 4f;
    public bool playerIsWeak = false;
    public bool fleeing, failedToFlee, fleeSuccess;
    public int maxHp, fleeingHp;
    public Vector3 startPos;
    public Rigidbody rb;
    public Animator anim;
    public BoxCollider attackBox;
    public int baseDmg = 30;
    public int lvlMultiplier;
    public int dmg;

    // Start is called before the first frame update
    void Start()
    {
        name = this.gameObject.name;
        maxHp = 150;
        hitDetect = gameObject.GetComponent<HitDetect>();
        player = GameObject.FindGameObjectWithTag("Player");
        friendly = GameObject.FindGameObjectWithTag("Friendly");
        atkRange = 3;
        canSeePlayer = false;
        canSeeFriendly = false;
        speed = 4f;
        viewRange = 10;
        startPos = transform.position;
        failedToFlee = false;
        fleeing = false;
        fleeSuccess = false;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        baseDmg = 30;

        SetStats();
    }

    // Update is called once per frame
    void Update()
    {
        distanceToFriendly = Vector3.Distance(friendly.transform.position, transform.position);
        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        CheckHp();
        if (!fleeing || failedToFlee || fleeSuccess)
        {
            if (CanSeePlayer() || CanSeeFriendly())
            {
                CheckDistances();
                TurnToFace();
                if (!CheckForNearbyTargets())
                {
                    MoveTowardsTarget();
                }
                else
                {
                    Attack();
                }

            }
            else
            {
                Idle();
            }
        }
        else
        {
            Flee();
        }
    }

    void SetStats()
    {
        lvlMultiplier = player.GetComponent<Player>().level;
        dmg = 30 * lvlMultiplier;
        hp = maxHp * lvlMultiplier;
    }

    void CheckHp()
    {
        if (hp < (maxHp / 2) && !fleeing)//if below or half max hp and not already fleeing
        {
            fleeing = true;
            fleeingHp = hp;
            fleeSuccess = false;
        }
        else if (hp < fleeingHp)//if taken damage after fleeing
        {
            failedToFlee = true;
            fleeSuccess = false;
            fleeing = false;
        }
    }

    void Flee()
    {
        anim.ResetTrigger("Attack");
        anim.SetBool("isIdle", false);
        Debug.Log("Fleeing");
        activeTarget = null;
        //turn to face startPos
        Vector3 lookRotation = Quaternion.LookRotation(startPos - transform.position).eulerAngles;
        lookRotation.x = 0;
        lookRotation.z = 0;
        Quaternion lookAtRotation = Quaternion.Euler(lookRotation);
        Vector3 moveRotation = Quaternion.Slerp(transform.rotation, lookAtRotation, 2f * Time.deltaTime).eulerAngles;
        // Rotate y axis only
        moveRotation.x = 0;
        moveRotation.z = 0;
        // Apply rotation
        transform.rotation = Quaternion.Euler(moveRotation);

        transform.position = Vector3.MoveTowards(transform.position, startPos, speed * Time.deltaTime);
        anim.SetBool("isMoving", true);

        float distanceToStart = Vector3.Distance(transform.position, startPos);

        if (distanceToStart <= 1)
        {
            fleeSuccess = true;
            anim.SetBool("isMoving", false);
        }
    }

    void Attack()
    {
        anim.SetBool("isIdle", false);
        anim.SetBool("isMoving", false);
        Debug.Log("Attacking");
        anim.SetTrigger("Attack");
        //play attack animation which has animation events to enable box collider and detectHits
    }

    void MoveTowardsTarget()
    {
        anim.SetBool("isIdle", false);
        anim.ResetTrigger("Attack");
        anim.SetBool("isMoving", true);
        transform.position = Vector3.MoveTowards(transform.position, activeTarget.transform.position, speed * Time.deltaTime);
    }

    void Idle()
    {
        anim.ResetTrigger("Attack");
        anim.SetBool("isMoving", false);
        Debug.Log("Idle");
        anim.SetBool("isIdle", true);
        //do idle animations
    }

    void CheckDistances()
    {


        if (canSeePlayer && canSeeFriendly)
        {

            CheckTargetHealth();
            if (!playerIsWeak)
            {
                if (distanceToFriendly < distanceToPlayer)
                {
                    activeTarget = friendly;
                }
                else
                {
                    activeTarget = player;
                }
            }

        }
        else if (canSeePlayer && !canSeeFriendly)
        {
            activeTarget = player;
        }
        else if (!canSeePlayer && canSeeFriendly)
        {
            activeTarget = friendly;
        }
        else
        {
            activeTarget = null;
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

    void CheckTargetHealth()
    {
        if (player.GetComponent<Player>().hp <= (player.GetComponent<Player>().maxHp / 2))//if player health is lower than 50% always target Player
        {
            activeTarget = player;
            playerIsWeak = true;
        }
        else
        {
            playerIsWeak = false;
        }
    }

    bool CanSeePlayer()
    {
        //Debug.Log("running function");

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit) && distanceToPlayer < viewRange)//if the ray can see the player and is within the view distance
        {
            Debug.DrawRay(transform.position, (player.transform.position - transform.position), Color.red);
            if (hit.collider.tag == player.tag)
            {
                Debug.Log("Can see player");
                canSeePlayer = true;
                return true;
            }
            else
            {
                Debug.Log("Something obstructing view of player");
                canSeePlayer = false;
                return false;
            }
            
        }
        else
        {
            Debug.DrawRay(transform.position, (player.transform.position - transform.position), Color.red);
            Debug.Log("Can't see player");
            canSeePlayer = false;
            return false;
        }
    }

    bool CanSeeFriendly()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (friendly.transform.position - transform.position), out hit) && distanceToFriendly < viewRange)
        {
            Debug.DrawRay(transform.position, (friendly.transform.position - transform.position), Color.red);
            if (hit.collider.tag == friendly.tag)
            {
                Debug.Log("Can see friendly");
                canSeeFriendly = true;
                return true;
            }
            else
            {
                Debug.Log("Something obstructing view of friendly");
                canSeeFriendly = false;
                return false;
            }
        }
        else
        {
            Debug.DrawRay(transform.position, (friendly.transform.position - transform.position), Color.red);
            Debug.Log("Can't see friendly");
            canSeeFriendly = false;
            return false;
        }
    }

    bool CheckForNearbyTargets()
    {
        distanceToTarget = Vector3.Distance(activeTarget.transform.position, transform.position);
        if (distanceToTarget <= atkRange)
        {
            Debug.Log(activeTarget.tag + " within range");
            return true;
        }
        else
        {
            return false;
        }
    }
}
