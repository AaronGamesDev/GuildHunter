using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetect : MonoBehaviour
{
    public GameObject attacker;
    public GameObject[] targets;
    public BoxCollider attackBox = null;
    public GameObject prefab;
    public Rigidbody rb = null;
    public float speed = 30;
    public ArrowHit arrowDetection;
    public int targetsEliminated;
    public string playerTag, friendlyTag, enemyTag;

    // Start is called before the first frame update
    void Start()
    {
        GameObject tempTarget;
        attacker = this.gameObject;

        if (attacker.tag == "Player")
        {
            targets = GameObject.FindGameObjectsWithTag("Enemy");
            enemyTag = targets[0].tag;
        }
        else if (attacker.tag == "Friendly")
        {
            targets = GameObject.FindGameObjectsWithTag("Enemy");
            enemyTag = targets[0].tag;
        }
        else if (attacker.tag == "Enemy")
        {
            targets = new GameObject[2];//set array size to 2 for player and friendly AI
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    if (GameObject.FindGameObjectWithTag("Player"))
                    {
                        tempTarget = GameObject.FindGameObjectWithTag("Player");
                        targets[i] = tempTarget;
                        playerTag = targets[i].tag;
                    }
                }
                else if (i == 1)
                {
                    if (GameObject.FindGameObjectWithTag("Friendly"))
                    {
                        tempTarget = GameObject.FindGameObjectWithTag("Friendly");
                        targets[i] = tempTarget;
                        friendlyTag = targets[i].tag;
                    }
                }
            }
        }
    }

        // Update is called once per frame
    void Update()
    {
        if (attacker.tag == "Player")
        {
            if (attacker.GetComponent<Player>().type == "Knight")
            {
                if (attackBox == null)
                {
                    //Debug.Log("Set the attackBox for Knight");
                    attackBox = attacker.GetComponent<BoxCollider>();
                    attackBox.enabled = false;
                }

            }
            else if (attacker.GetComponent<Player>().type == "Ranger")
            {
                if (rb == null)
                {
                    Debug.Log("this is a ranger type");
                    rb = prefab.GetComponent<Rigidbody>();
                }
            }
        }
        else if (attacker.tag == "Friendly")
        {
            if (attacker.GetComponent<Friendly>().type == "Ranger")
            {
                if (rb == null)
                {
                    Debug.Log("this is a ranger type");
                    rb = prefab.GetComponent<Rigidbody>();
                }
            }
        }
        else if (attacker.tag == "Enemy")
        {
            if (attackBox == null)
            {
                //Debug.Log("Set the attackBox for enemy");
                attackBox = attacker.GetComponent<Enemy>().attackBox;
                attackBox.enabled = false;
            }
        }
        if (arrowDetection != null)
        {
            if (arrowDetection.hitOther == true)//check if arrow hit something other than enemy
            {
                arrowDetection.hitOther = false;//reset
                Debug.Log("Event Unsubscribed");
                arrowDetection.CollisionDetected -= DetectArrowHit;//unsubscribe event
                Destroy(arrowDetection.gameObject);//destroy arrow
            }
            else if (arrowDetection.leftWorld == true)//check if arrow left world 
            {
                arrowDetection.leftWorld = false;//reset
                Debug.Log("Event Unsubscribed");
                arrowDetection.CollisionDetected -= DetectArrowHit;//unsubscribe event
                Destroy(arrowDetection.gameObject);//destroy arrow
            }
        }
    }

    public void EnableAttack ()
    {
        if (attacker.tag == "Player")
        {
            //Debug.Log("attacker is player or friendly");
            if (attacker.GetComponent<Player>().type == "Knight")
            {
                attackBox.enabled = true;
            }

            if (attacker.GetComponent<Player>().type == "Ranger")
            {

                Debug.Log("ranger attack");
                Rigidbody arrow = Instantiate(rb, gameObject.transform.Find("ArrowPos").transform.position, transform.rotation);
                StartCoroutine(killArrow(arrow.gameObject));
                arrow.velocity = attacker.transform.forward * speed;
                arrowDetection = arrow.gameObject.GetComponent<ArrowHit>();
                //for arrow event to call function
                Debug.Log("Event Subscribed");
                arrowDetection.CollisionDetected += DetectArrowHit;


            }
        }
        else if (attacker.tag == "Friendly")
        {
                Debug.Log("ranger attack");
                Rigidbody arrow = Instantiate(rb, gameObject.transform.Find("ArrowPos").transform.position, transform.rotation);
                StartCoroutine(killArrow(arrow.gameObject));
                arrow.velocity = attacker.transform.forward * speed;
                arrowDetection = arrow.gameObject.GetComponent<ArrowHit>();
                //for arrow event to call function
                Debug.Log("Event Subscribed");
                arrowDetection.CollisionDetected += DetectArrowHit;

        }
        else if (attacker.tag == "Enemy")
        {
            //Debug.Log("set true for enemy");
            attackBox.enabled = true;
        }
    }

    IEnumerator killArrow(GameObject tempArrow)//if arrow does not hit anything while attached to rigidbody variable, destroy after 10 seconds
    {
        yield return new WaitForSeconds(10f);
        if (tempArrow != null)
        {
            Debug.Log("Event Unsubscribed");
            arrowDetection.CollisionDetected -= DetectArrowHit;//unsubscribe event
            Destroy(tempArrow);
        }
    }

    public void DisableAttack ()
    {
        if (attacker.tag == "Player")
        {
            if (attacker.GetComponent<Player>().type == "Knight")
            {
                attackBox.enabled = false;
            }
        }
        else if (attacker.tag == "Enemy")/////////fix this because the box collider might reference the wrong one
        {
            attackBox.enabled = false;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (attacker.tag == "Player")
        {
            if (attacker.GetComponent<Player>().type == "Knight")
            {
                if (attackBox.enabled)
                {//while attackBox is enabled and in any attacking animation
                    if (other.gameObject.tag == enemyTag)
                    {
                        Debug.Log("Hit detected!");
                        if (attacker.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("atk 1"))//if player is performing first attack animation
                        {
                            other.GetComponent<Enemy>().hp -= attacker.GetComponent<Player>().dmg;//apply base damage to enemy hp
                            Debug.Log("Attack 1 damage: " + attacker.GetComponent<Player>().dmg + " applied");
                        }
                        else if (attacker.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("atk 2"))//if player is performing second attack animation
                        {
                            other.GetComponent<Enemy>().hp -= attacker.GetComponent<Player>().comboDmg;//apply combo damage to enemy hp
                            Debug.Log("Attack 2 damage: " + attacker.GetComponent<Player>().comboDmg + " applied");
                        }
                        else if (attacker.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("atk 3"))//if player is performing third attack animation
                        {
                            other.GetComponent<Enemy>().hp -= attacker.GetComponent<Player>().comboDmg2;//apply second combo damage to enemy hp
                            Debug.Log("Attack 3 damage: " + attacker.GetComponent<Player>().comboDmg2 + " applied");
                        }

                        if (other.GetComponent<Enemy>().hp <= 0)
                        {
                            targetsEliminated++;//tracks successful kill, this needs to be once target hp <= 0 and destroy target gameObject if so
                            Destroy(other.gameObject);
                        }

                        attackBox.enabled = false;
                    }
                }
            }
        }
        else if (attacker.tag == "Enemy")
        {
            if (attackBox.enabled)
            {//while attackBox is enabled and in any attacking animation
                if (other.gameObject.tag == playerTag)
                {
                    Debug.Log("Hit detected against player!");

                    other.GetComponent<Player>().hp -= attacker.GetComponent<Enemy>().dmg;//apply damage to player hp

                    attackBox.enabled = false;
                }
                else if (other.gameObject.tag == friendlyTag)
                {
                    Debug.Log("Hit detected against friendly!");
                    other.GetComponent<Friendly>().hp -= attacker.GetComponent<Enemy>().dmg;
                    attackBox.enabled = false;
                }
            }
        }
    }

    void DetectArrowHit()
    {
        Debug.Log("Arrow hit something m8");
        if (attacker.tag == "Player")
        {
            arrowDetection.other.GetComponent<Enemy>().hp -= attacker.GetComponent<Player>().dmg;
        }
        else if (attacker.tag == "Friendly")
        {
            arrowDetection.other.GetComponent<Enemy>().hp -= attacker.GetComponent<Friendly>().dmg;
        }

        if (arrowDetection.other.GetComponent<Enemy>().hp <= 0)
        {
            targetsEliminated++;//tracks successful kill, this needs to be once target hp <= 0 and destroy target gameObject if so
            Destroy(arrowDetection.other.gameObject);
        }
        Debug.Log("Targets Eliminated: " + targetsEliminated);
        Debug.Log("Event Unsubscribed");
        arrowDetection.CollisionDetected -= DetectArrowHit;//unsubscribe event
        Destroy(arrowDetection.gameObject);//destroy arrow
    }
}
