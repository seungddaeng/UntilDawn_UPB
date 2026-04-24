using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimationController : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent agent;
    public float movementThreshold = 0.05f;

    private string defaultStateName;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (agent == null)
            agent = GetComponentInParent<NavMeshAgent>();

        if (animator != null && animator.runtimeAnimatorController != null)
        {
            var clips = animator.runtimeAnimatorController.animationClips;
            if (clips.Length > 0)
            {
                defaultStateName = clips[0].name;
            }
        }
    }

    void Update()
    {
        if (animator == null || agent == null) return;

        bool isMoving = agent.velocity.magnitude > movementThreshold && !agent.isStopped;

        if (isMoving)
        {
            animator.speed = 1f;
        }
        else
        {
            animator.speed = 0f;

            if (!string.IsNullOrEmpty(defaultStateName))
            {
                animator.Play(defaultStateName, 0, 0f);
            }
        }
    }
}