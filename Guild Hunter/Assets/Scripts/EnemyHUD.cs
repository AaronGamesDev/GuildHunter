using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHUD : MonoBehaviour
{
    public Enemy enemy;
    public GameObject camera;
    public Image healthBar;
    public float enemyHp, enemyMaxHp, prevHealthFill, lerpSpeed;
    // Start is called before the first frame update
    void Start()
    {
        enemy = transform.parent.gameObject.GetComponent<Enemy>();
        lerpSpeed = 0.1f;
        camera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnemyStats();
        TurnToFaceCamera();
        prevHealthFill = healthBar.fillAmount;
        healthBar.fillAmount = Mathf.Lerp(prevHealthFill, enemyHp / enemyMaxHp, lerpSpeed);
    }

    void UpdateEnemyStats()
    {
        enemyHp = enemy.hp;
        enemyMaxHp = enemy.maxHp;
    }

    void TurnToFaceCamera()
    {
        Vector3 lookRotation = Quaternion.LookRotation(transform.position - camera.transform.position).eulerAngles;
        lookRotation.x = 0;
        lookRotation.z = 0;
        Quaternion lookAtRotation = Quaternion.Euler(lookRotation);
        Vector3 moveRotation = Quaternion.Slerp(transform.rotation, lookAtRotation, 5f * Time.deltaTime).eulerAngles;
        // Rotate y axis only
        moveRotation.x = 0;
        moveRotation.z = 0;
        // Apply rotation
        transform.rotation = Quaternion.Euler(moveRotation);
    }
}
