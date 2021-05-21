using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AIBossControl : MonoBehaviour {
    NavMeshAgent agent;
    public Transform attackTarget;
    public Transform shootMagicPoint_M, shootMagicPoint_R, shootMagicPoint_L;
    public GameObject sprintParticle;
    public GameObject shootRock;
    public float height;
    //public Text showBossHPText;
    Animator anim;
    AnimatorStateInfo animState;
    AnimatorTransitionInfo animTransState;
    bool isShowParticle;
    bool isHurt;
    public float sprintRange = 50f, hitRange = 6f;
    public Image currBossHpImage;
    [HideInInspector]public float currHP;
    [HideInInspector] public bool bossDefeat, hitFlag;
    public float maxHp = 500;
    // Start is called before the first frame update
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currHP = maxHp;
        isShowParticle = false;
        isHurt = false;
        bossDefeat = false;
        hitFlag = false;
        //agent.destination = target.position;

    }

    // Update is called once per frame
    void Update() {
        currBossHpImage.fillAmount = Mathf.Lerp(currBossHpImage.fillAmount, currHP / maxHp, 0.1f);
        animTransState = anim.GetAnimatorTransitionInfo(0);
        animState = anim.GetCurrentAnimatorStateInfo(0);
        if (animTransState.IsUserName("SprintStart") && !isShowParticle) {
            sprintParticle.SetActive(true);
            isShowParticle = true;
            Debug.LogError(1451);
        }
        if (animTransState.IsUserName("SprintEnd") && isShowParticle) {
            sprintParticle.SetActive(false);
            isShowParticle = false;
        }
        
        //transform.Translate(Vector3.forward * 10 * Time.deltaTime);
        //agent.destination = attackTarget.position;
        //transform.Translate(transform.forward * Time.deltaTime * 10);
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sprintRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitRange);
    }
    void ShootMagic(GameObject gameObject) {
        Instantiate(shootRock, shootMagicPoint_M.position, 
        Quaternion.LookRotation(attackTarget.position + new Vector3(0, height, 0) - shootMagicPoint_M.position));
        Instantiate(shootRock, shootMagicPoint_R.position,
        Quaternion.LookRotation(attackTarget.position + new Vector3(0, height, 0) - shootMagicPoint_R.position));
        Instantiate(shootRock, shootMagicPoint_L.position,
        Quaternion.LookRotation(attackTarget.position + new Vector3(0, height, 0) - shootMagicPoint_L.position));
    }
    private void OnCollisionEnter(Collision collision) {
        if (animState.IsName("Base Layer.Sprint") || animTransState.IsUserName("SprintStart"))
            if (collision.gameObject.tag == "Player") {
                Vector3 knockDir = (collision.transform.position - transform.position) * 7;
                knockDir.y = 0.1f;
                collision.gameObject.GetComponent<Rigidbody>().AddForce(knockDir / Time.deltaTime , ForceMode.Impulse);
                collision.gameObject.SendMessage("PlayerGetDamage", 15);
            }
    }
    private void OnTriggerStay(Collider other) {
        if (animState.IsName("Base Layer.HitAtk") && !hitFlag) {
            if (other.tag == "Player") {
                Vector3 knockDir = other.transform.position - transform.position;
                knockDir *= 1.5f;
                knockDir.y = 0.2f;
                other.gameObject.GetComponent<Rigidbody>().AddForce(knockDir / Time.deltaTime,ForceMode.Impulse);
                other.gameObject.SendMessage("PlayerGetDamage", 20);
                hitFlag = true;
            }
        }
    }
    void BossGetDamage(int damage) {
        currHP -= damage;
        if (currHP <= 0) {
            anim.SetTrigger("DeadTrigger");
            Destroy(gameObject, 3);
            agent.speed = 0;
            bossDefeat = true;
        }
        Debug.LogError("AI BOSS" + gameObject.name + currHP);
    }
}