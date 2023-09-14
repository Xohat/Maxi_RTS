using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CSelectable))]
public class UnitFSMBase : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent nmAgent;
    [HideInInspector]
    public CSelectable selectable;
    [HideInInspector]
    public CLife life;
    [HideInInspector]
    public CTeam team;

    [HideInInspector]
    public ArmyController army;

    public enum State
    {
        Idle,
        GoingTo,
        Attacking,
        Dying
    }

    public State currentState = State.Idle;
    public State lastState = State.Idle;

    [HideInInspector]
    public Vector3 screenPosition;

    protected Vector3 destiny;

    public float destinyThreshold = 0.5f;
    protected float destinyThreshold2;

    public float boundingRadius = 1f;
    [HideInInspector]
    public float boundingRadius2;

    public float visionSphereRadius = 10f;
    [HideInInspector]
    public float visionSphereRadius2;

    protected UnitFSMBase currentEnemy;
    public List<UnitFSMBase> enemiesInVisionSphere = new List<UnitFSMBase>();

    public float meleeAttack = 5f;
    public float attackCadence = 1.0f;
    protected float attackCadenceAux = 0f;

    public int populationCost = 1;
    public int resourcesCost = 100;

    private void Awake()
    {
        nmAgent = GetComponent<NavMeshAgent>();
        selectable = GetComponent<CSelectable>();
        team = GetComponent<CTeam>();
        life = GetComponent<CLife>();
    }

    // Start is called before the first frame update
    void Start()
    {
        destiny = transform.position;

        destinyThreshold2 = destinyThreshold * destinyThreshold;

        //boundingRadius = nmAgent.radius;
        boundingRadius2 = boundingRadius * boundingRadius;

        visionSphereRadius2 = visionSphereRadius * visionSphereRadius;

        nmAgent.stoppingDistance = destinyThreshold;
    }

    // Update is called once per frame
    void Update()
    {
        screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        switch (currentState)
        {
            case State.Idle:
                UpdateIdle();
                break;
            case State.GoingTo:
                UpdateGoingTo();
                break;
            case State.Attacking:
                UpdateAttacking();
                break;
            case State.Dying:
                UpdateDying();
                break;
            default:
                break;
        }
    }

    protected virtual void UpdateIdle()
    {

    }

    protected void UpdateGoingTo()
    {
        Vector3 offset = destiny - transform.position;
        float offsetSqrMagnitude = offset.sqrMagnitude;

        if (currentEnemy)
        {
            if (offsetSqrMagnitude <= boundingRadius2 + currentEnemy.boundingRadius2)
            {
                // close enough to attack!
                nmAgent.isStopped = true;

                lastState = currentState;
                currentState = State.Attacking;
            }
            else if (offsetSqrMagnitude <= destinyThreshold2)
            {
                // on destiny but the enemy is no on sight
                nmAgent.isStopped = true;

                lastState = currentState;
                currentState = State.Idle;
            }
        }
        else
        {
            if (offsetSqrMagnitude <= destinyThreshold2)
            {
                nmAgent.isStopped = true;

                lastState = currentState;
                currentState = State.Idle;
            }
        }
    }

    protected void UpdateAttacking()
    {
        if (currentEnemy)
        {
            attackCadenceAux += Time.deltaTime;

            Vector3 offset = currentEnemy.transform.position - transform.position;
            float offsetSqrMagnitude = offset.sqrMagnitude;

            if (offsetSqrMagnitude <= boundingRadius2 + currentEnemy.boundingRadius2)
            {
                if (attackCadenceAux >= attackCadence)
                {
                    // Attack
                    currentEnemy.life.Damage(meleeAttack);

                    attackCadenceAux = 0f;
                }
            }
            else if(offsetSqrMagnitude <= visionSphereRadius2)
            {
                destiny = currentEnemy.transform.position;
                nmAgent.SetDestination(destiny);
                nmAgent.isStopped = false;

                lastState = currentState;
                currentState = State.GoingTo;
            }
            else
            {
                nmAgent.isStopped = true;

                lastState = currentState;
                currentState = State.Idle;

                currentEnemy = null;
            }
        }
        else
        {
            lastState = currentState;
            currentState = State.Idle;
        }
    }

    protected void UpdateDying()
    {
        
    }

    public void RightClickOnFloor(Vector3 position)
    {
        currentEnemy = null;
        GoTo(position);
    }

    public void RightClickOnTransform(Transform destTransform)
    {
        CTeam otherTeam = destTransform.GetComponent<CTeam>();
        if (otherTeam)
        {
            if (otherTeam.teamNumber != team.teamNumber)
            {
                // click on enemy
                GoTo(destTransform.position);
                currentEnemy = destTransform.GetComponent<UnitFSMBase>();
            }
            else
                currentEnemy = null;
        }
    }

    public void GoTo(Vector3 destiny)
    {
        if (currentState != State.Dying)
        {
            this.destiny = destiny;
            nmAgent.isStopped = false;
            nmAgent.SetDestination(destiny);

            lastState = currentState;
            currentState = State.GoingTo;
        }
    }

    public virtual void EnemyEntersInVisionSphere(UnitFSMBase enemy)
    {
        enemiesInVisionSphere.Add(enemy);
    }

    public virtual void EnemyLeavesVisionSphere(UnitFSMBase enemy)
    {
        enemiesInVisionSphere.Remove(enemy);
    }

    public void UnitDied()
    {
        army?.UnitDied(this);
    }

    protected virtual void OnGUI()
    {
        GUI.Label(new Rect(screenPosition.x - 20f, Screen.height - screenPosition.y - 30f, 200f, 20f), currentState.ToString());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, visionSphereRadius);
    }
}
