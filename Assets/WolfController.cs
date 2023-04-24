using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.AI;

public class WolfController : MonoBehaviour
{
    private const int StoppedState = 0;
    private const int WalkingStateTo = 1;
    private const int RunningState = 2;
    private const int WalkingStateFrom = 3;

    private float speedMultiplier;

    private readonly float[] timeMultipliers =
    {
        1.0f,
        1.5f,
        4.0f,
        1.5f
    };

    private const float Stopped = 0.0f;
    private const float Walking = 0.5f;
    private const float Running = 1.0f;

    float currentSpeed = 0.0f;
    float currentTime = 0.0f;
    float timeThreshold = 0.0f;

    int currentState = StoppedState;

    NavMeshAgent navMeshAgent;
    Animator animator;

    Transform playerTransform;

    int life = 3;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        playerTransform = GameObject.Find("Player").transform;

        speedMultiplier = Random.Range(19.0f, 22.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (life <= 0)
        {
            navMeshAgent.isStopped = true;
        }

        if (Vector3.Distance(playerTransform.position, transform.position) > 1.0f)
        {
            navMeshAgent.destination = playerTransform.position;
        }
        else
        {
            animator.SetFloat("Speed", 0.0f);
            return;
        }

        if (currentTime >= timeThreshold)
        {
            currentState = (currentState + 1) % 4;

            currentTime = 0.0f;
            timeThreshold = Random.Range(5.0f, 10.0f) * timeMultipliers[currentState];

            if (currentState == StoppedState)
            {
                currentSpeed = Stopped;
            }
            else if (currentState == WalkingStateFrom || currentState == WalkingStateTo) 
            { 
                currentSpeed = Walking;
            }
            else if (currentSpeed == RunningState)
            {
                currentSpeed = Running;
            }

            navMeshAgent.speed = currentSpeed * speedMultiplier;
            animator.SetFloat("Speed", currentSpeed);
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            life--;

            if (life <= 0)
            {
                StartCoroutine(DieCoroutine());
            }
        }  
    }

    IEnumerator DieCoroutine()
    {
        animator.SetBool("IsDead", true);

        yield return new WaitForSeconds(2.0f);

        Destroy(gameObject);
    }
}
