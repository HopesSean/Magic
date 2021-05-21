using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BossState {Walk, Sprint, Magic, Hit}
public class AIBossStateScript : StateMachineBehaviour
{
    BossState state;
    float endActionTime;
    AIBossControl AICtrl;
    NavMeshAgent agent;
    Transform transform;
    bool stateExitFlag;
    float AIToPlayerDist;
    bool isAttack;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (AICtrl == null)
            AICtrl = animator.GetComponent<AIBossControl>();
        if (agent == null)
            agent = animator.GetComponent<NavMeshAgent>();
        if (transform == null)
            transform = animator.transform;
        endActionTime = Time.time + 1.5f;
        stateExitFlag = false;
        isAttack = false;
        AICtrl.hitFlag = false;
        //agent.isStopped = false;
        AIToPlayerDist = Vector3.Distance(transform.position, AICtrl.attackTarget.position);
        if (AIToPlayerDist > AICtrl.sprintRange) {
            if (Random.Range(0, 100) % 4 == 0)
                state = BossState.Walk;
            else
                state = BossState.Magic;
        }
        else if (AIToPlayerDist <= AICtrl.sprintRange && AIToPlayerDist > AICtrl.hitRange) {
            if (Random.Range(0, 100) % 5 == 0)
                state = BossState.Walk;
            else {
                if (Random.Range(0, 100) % 2 == 0)
                    state = BossState.Sprint;
                else
                    state = BossState.Magic;
            }                
        }
        else {
            if (Random.Range(0, 100) % 2 == 0)
                state = BossState.Sprint;
            else
                state = BossState.Hit;
        }
        animator.SetFloat("Forward", 0);
        agent.speed = 0;

    }  

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (Time.time > endActionTime && !stateExitFlag && !animator.IsInTransition(0)) {
            animator.SetTrigger("StateExit");
            stateExitFlag = true;
        }
        else {
            if (state == BossState.Sprint || state == BossState.Magic || state == BossState.Hit) {
                agent.ResetPath();
                if (animator.IsInTransition(0))
                    return;
                Vector3 AIToPlayerDir = AICtrl.attackTarget.position - transform.position;
                Quaternion rot = Quaternion.LookRotation(AIToPlayerDir);
                Quaternion finalRot = Quaternion.Euler(0, rot.eulerAngles.y, 0);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, finalRot, 360 * Time.deltaTime);
                Vector3 faceDir = transform.forward;
                faceDir.y = 0;
                AIToPlayerDir.y = 0;
                if (!isAttack && Vector3.Angle(faceDir, AIToPlayerDir) < 0.01f && state == BossState.Magic) {
                    isAttack = true;
                    animator.SetTrigger("MagicTrigger");
                }
                if (!isAttack && Vector3.Angle(faceDir, AIToPlayerDir) < 0.01f && state == BossState.Sprint) {
                    isAttack = true;
                    animator.SetTrigger("SprintTrigger");
                }
                if (!isAttack && Vector3.Angle(faceDir, AIToPlayerDir) < 0.01f && state == BossState.Hit) {
                    isAttack = true;
                    animator.SetTrigger("HitTrigger");
                }
            }
            if (state == BossState.Walk) {
                agent.speed = 5;
                animator.SetFloat("Forward", 0.5f);
                agent.destination = AICtrl.attackTarget.position;
            }
            
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
