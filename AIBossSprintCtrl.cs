using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBossSprintCtrl : StateMachineBehaviour
{
    float endActionTime;
    AIBossControl AICtrl;
    NavMeshAgent agent;
    Transform transform;
    bool stateExitFlag;
    float AIToPlayerDist;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (AICtrl == null)
            AICtrl = animator.GetComponent<AIBossControl>();
        if (agent == null)
            agent = animator.GetComponent<NavMeshAgent>();
        if (transform == null)
            transform = animator.transform;
        
        stateExitFlag = false;
        Vector3 AIToPlayerDir = AICtrl.attackTarget.position - transform.position;
        endActionTime = Time.time + Random.Range(7,10);
        //agent.destination = AIToPlayerDir * 5 + transform.position;
        //Debug.LogError(AICtrl.attackTarget.position * 2);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (Time.time > endActionTime && !stateExitFlag) {
            animator.SetTrigger("SprintEnd");
            stateExitFlag = true;
            //animator.SetFloat("Forward", 0);
        }
        else {
            //animator.SetFloat("Forward", 1);
            transform.Translate(Vector3.forward * Time.deltaTime * 15);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        
                
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
