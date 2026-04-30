using UnityEngine;
using UnityEngine.AI;

public class AlexisScript : MonoBehaviour
{
    private Animator animator;
    //public NavMeshAgent navMeshAgentAlexis;

    private SkinnedMeshRenderer alexisRenderer;

    void Start()
    {
        //navMeshAgentAlexis = GetComponent<NavMeshAgent>();
        alexisRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        animator = GetComponent<Animator>();
    }

    public void UpdateVisibility()
    {
        //navMeshAgentAlexis.enabled = true;
        alexisRenderer.enabled = true;
    }
}