/* 
 * Oscar Forra Carbonell
*/
using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] TEnemyStates m_CurrentState = TEnemyStates.Idle;                      // Current enemy state
    public Collider m_Collider;
    public bool m_Active;                                                                  // Used to know if enemy can we taken in the pull or not
    public GameObject m_ChaseParticleSystem;                                               // Particle system used when enemy is chasing
    [SerializeField] bool m_Teeth = false;                                                 // Bool used to know if it's teeth or not
    public EnemyBlackboard m_EnemyBlackboard;                                              // Reference to enemy blackboard attached to gameObject
    [SerializeField] float m_ElapsedTime;                                                  // Elapsed time since spawn
    private GameObject m_Target;                                                           // Protectable object that have to destroy
    public NavMeshAgent m_NavMeshAgent;                                                    // NavMeshAgent attached to gameObject
    [SerializeField] Animator m_Animator = null;                                                  // Animator attached to enemy
    [SerializeField] Animation m_Animation = null;
    [SerializeField] AnimationClip m_HitAnimationClip = null;                              // Hit animation
    [SerializeField] AnimationClip m_SpawnAnimationClip = null;
    [SerializeField] AnimationClip m_DieAnimationClip = null;                              // Die animation
    [SerializeField] AnimationClip m_AttackAnimationClip = null;                           // Attack animation
    private FMOD.Studio.EventInstance m_CurrentAudioEvent;                                 // Event instance that will contains the current audio instance playing
    private UnityEngine.Vector3 m_StartPosition;                                           // Enemy start position
    private Transform m_EnemyTransform;                                                    // Enemy transform
    public void OnAwake()
    {
        m_EnemyTransform = this.transform;
        m_StartPosition = m_EnemyTransform.position;
        m_Active = false;
        // Get enemy components
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
    }
    public void OnInit(UnityEngine.Vector3 position)
    {
        m_EnemyTransform.position = position;
        ActiveEnemy();
    }
    public void OnUpdate()
    {
        // Compute elapsed time
        m_ElapsedTime += Time.deltaTime;
    }
    public void SetAttackObjective()
    {
        // Set state to Chase Objective and stop NavMesh
        m_CurrentState = TEnemyStates.AttackObjective;
        m_NavMeshAgent.isStopped = true;
        m_ElapsedTime = 0.0f;
        DesactivateAnimations();
    }
    public void SetAttackPlayer()
    {
        // Set state to Attack Player and stop NavMesh
        m_CurrentState = TEnemyStates.AttackPlayer;
        m_NavMeshAgent.isStopped = true;
        m_ElapsedTime = 0.0f;
        DesactivateAnimations();
    }

    public void SetChaseObjective()
    {
        // Stop event if there is one playing
        SoundController.Instance.StopEvent(m_CurrentAudioEvent);
        // Set state to ChaseObjective and start NavMesh
        m_CurrentState = TEnemyStates.ChaseObjective;
        m_ElapsedTime = 0.0f;
        // Get Target
        m_Target = Utilities.FindNearestGameObjectOfTag(this.gameObject,GameModeController.Instance.m_ProtectableGameObjects);
        m_NavMeshAgent.isStopped = false;
        // Set destination
        m_NavMeshAgent.SetDestination(m_Target.transform.position);
        DesactivateAnimations();
        SetChasingAnimation(true);
        //Play PS
        m_ChaseParticleSystem.SetActive(true);
        // Play Sound
        if(m_Teeth)
            m_CurrentAudioEvent = SoundController.Instance.PlayEvent("event:/Teeth/TeethMovement",m_EnemyTransform,GetComponent<Rigidbody>());
        else
            m_CurrentAudioEvent = SoundController.Instance.PlayEvent("event:/Robot/RobotWheels", m_EnemyTransform, GetComponent<Rigidbody>());
    }

    public void SetChasePlayer()
    {
        // Stop event if there is one playing
        SoundController.Instance.StopEvent(m_CurrentAudioEvent);
        // Set state to ChasePlayer and start NavMesh
        m_CurrentState = TEnemyStates.ChasePlayer;
        m_ElapsedTime = 0.0f;
        m_NavMeshAgent.isStopped = false;
        // Set destination
        m_NavMeshAgent.SetDestination(GameController.Instance.m_PlayerBlackboard.transform.position);
        DesactivateAnimations();
        SetChasingAnimation(true);
        // Play Sound
        if(m_Teeth)
            m_CurrentAudioEvent = SoundController.Instance.PlayEvent("event:/Teeth/TeethMovement",m_EnemyTransform,GetComponent<Rigidbody>());
        else
            m_CurrentAudioEvent = SoundController.Instance.PlayEvent("event:/Robot/RobotWheels",m_EnemyTransform,GetComponent<Rigidbody>());

    }

    public void SetDie()
    {
         m_ChaseParticleSystem.SetActive(false);
        // Play Sound
        if(m_Teeth)
            SoundController.Instance.PlayOneShootAudio("event:/Teeth/TeethDie",m_EnemyTransform);
        else
            SoundController.Instance.PlayOneShootAudio("event:/Robot/RobotDie",m_EnemyTransform);
        m_Collider.enabled = false;
        // Create floating text
        FloatingTextController.CreateFloatingText(m_EnemyBlackboard.m_CoinsToDrop.ToString(),m_EnemyTransform);
        // Stop last audio
        SoundController.Instance.StopEvent(m_CurrentAudioEvent);
        // Set state  to Die and stop NavMesh
        m_CurrentState = TEnemyStates.Die;
        m_ElapsedTime = 0.0f;
        m_NavMeshAgent.isStopped = true;
        m_Animation.clip = m_DieAnimationClip;
        m_Animation.Play();
        DesactivateAnimations();
        SetDieAnimation(true);
        StartCoroutine(Die());  
    }

    public void SetHit()
    {
        // Set state to Hit and stop NavMesh
        m_CurrentState = TEnemyStates.Hit;
        m_ElapsedTime = 0.0f;
        m_NavMeshAgent.isStopped = true;
        DesactivateAnimations();
        SetHitAnimation(true);
    }

    public void SetIdle()
    {
        m_Animation.clip = m_SpawnAnimationClip;
        m_Animation.Play();
        DesactivateAnimations();
        // Set state to Idle and stop NavMesh
        m_CurrentState = TEnemyStates.Idle;
        m_ElapsedTime = 0.0f;
        m_NavMeshAgent.isStopped = true;
    }

    public void UpdateAttackObjective()
    {
        // Look at target
        transform.LookAt(m_Target.transform);
        // If distance is less than MAX distance to attack change state
        if ((m_Target.transform.position - transform.position).magnitude > m_EnemyBlackboard.m_MaxDistanceToAttack)
        {
            // Change state to chase
            SetChaseObjective();
        }
        // Chase player
        if ((GameController.Instance.m_PlayerBlackboard.transform.position - transform.position).magnitude < m_EnemyBlackboard.m_DistanceToDetectPlayer)
        {
            // Change state to chase
            SetChasePlayer();
        }
        if(!m_Target.GetComponent<ProtectableObject>().m_EnabledInGame)
            SetChasePlayer();
        // If can attack deal damage
        if (CanAttack()) {
            m_ElapsedTime = 0.0f;
            DesactivateAnimations();
            SetAttackAnimation(true);
            m_Target.GetComponent<IDamageable>().DealDamage(m_EnemyBlackboard.m_Damage);
        }
        // If is dead destroy
        if (IsDead())
            SetDie();
    }

    public void UpdateAttackPlayer()
    {
        // Look at target
        transform.LookAt(GameController.Instance.m_PlayerBlackboard.m_PlayerTransform);
        // If distance is less than MAX distance to attack change state
        if ((GameController.Instance.m_PlayerBlackboard.m_PlayerTransform.position - m_EnemyTransform.position).magnitude > m_EnemyBlackboard.m_MaxDistanceToAttack)
        {
            // Change state to chase
            SetChasePlayer();
        }
        // If can attack deal damage
        if (CanAttack()) {
            DesactivateAnimations();
            SetAttackAnimation(true);
            GameController.Instance.m_PlayerBlackboard.DealDamage(m_EnemyBlackboard.m_Damage);
            // Play audio if teeth
            if(m_Teeth)
                SoundController.Instance.PlayOneShootAudio("event:/Teeth/TeethBeat",m_EnemyTransform);
            //else
                //SoundController.Instance.PlayOneShootAudio("event:/Robot/RobotAttack",m_EnemyTransform);
        }
        // If is dead destroy
        if (IsDead())
            SetDie();
    }

    public void UpdateChaseObjective()
    {
        // Go to destination
        m_NavMeshAgent.SetDestination(m_Target.transform.position);
        // If distance is less than MinDistance set to attack
        // Chase player
        if ((GameController.Instance.m_PlayerBlackboard.m_PlayerTransform.position - m_EnemyTransform.position).magnitude < m_EnemyBlackboard.m_DistanceToDetectPlayer)
        {
            // Change state to chase
            SetChasePlayer();
        }
        if ((m_Target.transform.position - m_EnemyTransform.position).magnitude < m_EnemyBlackboard.m_DistanceToAttackObjective)
        {
            SetAttackObjective();
        }
    }

    public void UpdateChasePlayer()
    {
        // Go to destination
        m_NavMeshAgent.SetDestination(GameController.Instance.m_PlayerBlackboard.m_PlayerTransform.position);
        // If distance is less than MinDistance set to attack
        if ((GameController.Instance.m_PlayerBlackboard.transform.position - m_EnemyTransform.position).magnitude < m_EnemyBlackboard.m_MinDistanceToAttack)
        {
            SetAttackPlayer();
        }
    }

    public void UpdateDie()
    {
        //StartCoroutine(Die());
    }
    public void UpdateHit()
    {
        if(m_ElapsedTime > m_HitAnimationClip.length)
            SetChasePlayer();
        if(IsDead())
            SetDie();
    }
    public void UpdateIdle()
    {
        if (m_Active)
        {
            // If elapsedtime is greather than 1
            if (m_ElapsedTime > 1f)
            {
                // If is GameModeController
                if (GameModeController.Instance)
                    // GameModeState is protect set to chase objective
                    if (GameModeController.Instance.m_CurrentGameMode == GameModeController.TGameMode.Protect)
                        SetChaseObjective();
                    // Set chase player
                    else
                        SetChasePlayer();
            }
        }
    }
    private bool IsDead()
    {
        // If current health is 0 or less return true otherwise return false
        if (m_EnemyBlackboard.m_CurrentHealth <= 0) 
        {
            return true;
        }
        return false;
    }
    private bool CanAttack()
    {
        // If elapsed time is greather than attack cadence return true otherwise return false
        if (m_ElapsedTime > m_EnemyBlackboard.m_AttackCadence) 
        {
            m_ElapsedTime = 0;
            return true;
        }
        if(m_ElapsedTime > m_AttackAnimationClip.length)
            SetAttackAnimation(false);
        return false;
    }
    public void DealDamage(int damage)
    {
        if (m_EnemyBlackboard.m_CurrentHealth > 0)
        {
            // Play Sound
            SoundController.Instance.PlayOneShootAudio("event:/Teeth/TeethBeat", m_EnemyTransform);
            // Substract current health
            m_EnemyBlackboard.m_CurrentHealth -= damage;
            if (m_EnemyBlackboard.m_CurrentHealth <= 0)
            {
                SetDie();
            }
            else
            {
                //Set hit
                SetHit();
            }
        }
    }
    public TEnemyStates CurrentState()
    {
        // Return current state
        return m_CurrentState;
    }
    IEnumerator Die()
    {
        yield return new WaitForSeconds(m_DieAnimationClip.length);
        // Add coins
        GameController.Instance.m_PlayerBlackboard.AddCoins(m_EnemyBlackboard.m_CoinsToDrop);
        // Update HUD
        HUDController.Instance.UpdateCurrentCoins();
        // Destroy
        DeasctiveEnemy();
    }

    public void SetChasingAnimation(bool active)
    {
        m_Animator.SetBool("Chase",active);
    }

    public void SetHitAnimation(bool active)
    {
        m_Animator.SetBool("Hit",active);
    }

    public void SetDieAnimation(bool active)
    {
        m_Animator.SetBool("Die",active);
    }

    public void SetAttackAnimation(bool active)
    {
        m_Animator.SetBool("Attack",active);
    }

    private void DesactivateAnimations()
    {
        m_Animator.SetBool("Attack", false);
        m_Animator.SetBool("Die", false);
        m_Animator.SetBool("Hit", false);
        m_Animator.SetBool("Chase", false);
    }
    public void DeasctiveEnemy()
    {

        SoundController.Instance.StopEvent(m_CurrentAudioEvent);
        m_CurrentState = TEnemyStates.Idle;
        m_EnemyTransform.position = m_StartPosition;
        m_Active = false;
        m_NavMeshAgent.enabled = false;
        DesactivateAnimations();
    }
    void ActiveEnemy()
    {
        m_Collider.enabled= true;
        m_EnemyBlackboard.StatsInit();
        m_Active = true;
        m_NavMeshAgent.enabled = true;
        SetIdle();
    }
}
