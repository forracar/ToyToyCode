/* 
 * Oscar Forra Carbonell
*/
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(EnemyBlackboard))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    private IEnemy m_EnemyIA;
    // Start is called before the first frame update
    void Start()
    {
        // Get enemy behaviour
        m_EnemyIA = GetComponent<IEnemy>();
        // Init enemy
        m_EnemyIA.OnAwake();
    }

    // Update is called once per frame
    void Update()
    {
        // Enemy states
        m_EnemyIA.OnUpdate();
        switch (m_EnemyIA.CurrentState())
        {
            case TEnemyStates.Idle:
                m_EnemyIA.UpdateIdle();
                break;
            case TEnemyStates.ChasePlayer:
                m_EnemyIA.UpdateChasePlayer();
                break;
            case TEnemyStates.ChaseObjective:
                m_EnemyIA.UpdateChaseObjective();
                break;
            case TEnemyStates.AttackPlayer:
                m_EnemyIA.UpdateAttackPlayer();
                break;
            case TEnemyStates.AttackObjective:
                m_EnemyIA.UpdateAttackObjective();
                break;
            case TEnemyStates.Hit:
                m_EnemyIA.UpdateHit();
                break;
            case TEnemyStates.Die:
                m_EnemyIA.UpdateDie();
                break;
        }
    }
}
