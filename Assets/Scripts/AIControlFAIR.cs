using DataStructures.RandomSelector;
using MonsterLove.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;

public class AIControlFAIR : MonoBehaviour {

    public enum States
    {
        Hold,
        WalkingToAttraction,
        FindAttraction,
        LeaveTheFair,
        WalkingToExit
    }
    StateMachine<States, StateDriverUnity> fsm;

    public GameObject[] locations;
    UnityEngine.AI.NavMeshAgent agent;
    Animator anim;
    float speedMult;
    float detectionRadius = 15;
    float fleeRadius = 5;
    float timeRemainingToAttraction = 0f;

    float time;

    private static CrowdManager crowdManager;

    private void Awake()
    {
        locations = GameObject.FindGameObjectsWithTag("attraction");
        agent = this.GetComponent<NavMeshAgent>();
        anim = this.GetComponent<Animator>();
        anim.SetFloat("wOffset", UnityEngine.Random.Range(0.0f, 1.0f));

        fsm = new StateMachine<States, StateDriverUnity>(this);
        fsm.ChangeState(States.FindAttraction, StateTransition.Safe);

        crowdManager = CrowdManager.Instance;
    }
    void Start() 
    {
       
    }
    void ResetAgent() 
    {
        speedMult = UnityEngine.Random.Range(0.9f, 1.5f);
        agent.speed = 2 * speedMult;
        agent.angularSpeed = 120.0f;
        anim.SetFloat("speedMult", speedMult);
        anim.SetBool("isWalking", true);
        anim.SetBool("isIdle", false);
        agent.ResetPath();
    }
    void HoldAgent()
    {
        speedMult = 0f;
        agent.speed = 2 * speedMult;
        agent.angularSpeed = 0;
        anim.SetBool("isIdle", true);
        anim.SetBool("isWalking", false);
        if (!isLocatedAtAttraction())
        {
            fsm.ChangeState(States.FindAttraction, StateTransition.Safe);
        }
    }
    bool isLocatedAtAttraction()
    {
        foreach(GameObject locs in locations)
        {
            if (Vector3.Distance(locs.transform.position, transform.position) < 2.5f)
            {
                return true;
            }
        }
        return false;
    }

    //In case of fire or something else
    public void DetectNewObstacle(Vector3 position)
    {
        if (Vector3.Distance(position, this.transform.position) < detectionRadius)
        {

            Vector3 fleeDirection = (this.transform.position - position).normalized;
            Vector3 newgoal = this.transform.position + fleeDirection * fleeRadius;

            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(newgoal, path);

            if (path.status != NavMeshPathStatus.PathInvalid)
            {

                agent.SetDestination(path.corners[path.corners.Length - 1]);
                anim.SetTrigger("isRunning");
                agent.speed = 6;
                agent.angularSpeed = 500;
            }
        }
    }
    public (float, List<(int, float)>) calculate_possibility_of_attraction()
    {
        float total_possibility = 0;
        List<(int, float)> list_of_probabilities = new List<(int, float)>();

        for (int i = 0; i < locations.Length; i++)
        {
            float probability = locations[i].GetComponent<Location>().CalculateInfluence(CrowdManager.Instance.TimeOfDay);
            total_possibility += probability;
            list_of_probabilities.Add((i, probability));
        }

        total_possibility = total_possibility > 100 ? 100 : total_possibility;
        total_possibility = float.IsNaN(total_possibility) ? 0 : total_possibility;
        
        return (total_possibility, list_of_probabilities);
    }
    void FindAttraction_Enter()
    {
        List<(int, float)> list_probabilities;

        (_, list_probabilities) = calculate_possibility_of_attraction();

        float prob_total = 0;
        DynamicRandomSelector<int> selector = new DynamicRandomSelector<int>();
        for (int i = 0; i < list_probabilities.Count; i++)
        {
            (int index, float prob) = list_probabilities[i];
            prob_total += prob;
            selector.Add(index, prob);
        }
        int idx;
        if(prob_total > 0)
        {
            selector.Build();
            idx = selector.SelectRandomItem();
        }
        else
        {
            idx = (int)UnityEngine.Random.Range(0f, locations.Length - 1);
            Debug.Log("Random choice: " + idx);
        }

        //resetAgent function should be called before setting the destination.
        ResetAgent();
        agent.SetDestination(locations[idx].transform.position);
        fsm.ChangeState(States.WalkingToAttraction, StateTransition.Safe);
        
    }
    
    void WalkingToAttraction_Update()
    {
        if (agent.remainingDistance < 1.8 && fsm.State!=States.WalkingToExit && fsm.State!=States.LeaveTheFair)
        {
            time = Time.time;

            (float min, float max) = CrowdManager.Instance.GetMinMaxTimeInAttraction();
            timeRemainingToAttraction = UnityEngine.Random.Range(min, max);
            fsm.ChangeState(States.Hold, StateTransition.Safe);
        }
    }

    void WalkingToExit_Enter()
    {
        ResetAgent();
        agent.SetDestination(GameObject.FindGameObjectWithTag("exit").transform.position);
    }

    void WalkingToExit_Update()
    {
        if (agent.remainingDistance < 1.2 && agent.hasPath)
        {
            CrowdManager.Instance.DecreaseCrowdByOne();
            Destroy(gameObject);
        }
    }

    void Hold_Enter()
    {
        HoldAgent();
    }
    void Hold_Update()
    {
        if (Time.time - time > timeRemainingToAttraction)
        {
            if (UnityEngine.Random.value < 0.12f)
            {
                fsm.ChangeState(States.WalkingToExit, StateTransition.Safe);
            }
            else
            {
                fsm.ChangeState(States.FindAttraction);
            }
            
        }
    }
    void Update()
    {
        fsm.Driver.Update.Invoke();
    }
}
