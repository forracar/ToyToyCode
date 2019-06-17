/* 
 * Oscar Forra Carbonell
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FMODUnity;

public class DistanceEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] TEnemyStates m_CurrentState = TEnemyStates.Idle;                                       // Current enemy state
    public ParticleSystem m_DieParticleSystem;
    public bool m_Active;                 
    [SerializeField] Transform m_BulletEmitter = null;                                                      // Bullet emitter, position where bullet will be instantated
    [SerializeField] Animator m_Animator = null;                                                            // Animator attached to enemy
    [SerializeField] AnimationClip m_HitAnimationClip= null;                                                // Hit animation clip
    [SerializeField] AnimationClip m_DieAnimationClip= null;                                                // Die animation clip
    [SerializeField] AnimationClip m_AttackAnimationClip = null;                                            // Attack animation clip
    [SerializeField] float m_BulletSpeed = 0;                                                               // Bullet speed
    private EnemyBlackboard m_EnemyBlackboard;                                                              // Component attached to enemy enemybalckboard
    private float m_ElapsedTime;                                                                            // Elapsed time since start
    private GameObject m_Target;                                                                            // target that have to kill
    public NavMeshAgent m_NavMeshAgent;                                                                    // NavmeshAgent component
    [HideInInspector] public FMOD.Studio.EventInstance m_CurrentAudioEvent;                                                  // Event instance that will contains the current audio instance playing
    private Vector3 m_StartPosition;
    private Transform m_EnemyTransform;


    public void OnAwake()
    {
        m_EnemyTransform = this.transform;
        m_StartPosition = m_EnemyTransform.position;
        m_Active = false;
        // Get enemy components
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_EnemyBlackboard = GetComponent<EnemyBlackboard>();
        m_Animator = GetComponent<Animator>();
    }
    public void OnInit(Vector3 position)
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
        // Set Audio
    }

    public void SetChasePlayer()
    {
        // Stop last audio
        //SoundController.Instance.StopEvent(m_CurrentAudioEvent);
        // Set state to ChasePlayer and start NavMesh
        m_CurrentState = TEnemyStates.ChasePlayer;
        m_ElapsedTime = 0.0f;
        m_NavMeshAgent.isStopped = false;
        // Set destination
        m_NavMeshAgent.SetDestination(GameController.Instance.m_PlayerBlackboard.m_PlayerTransform.position);
        DesactivateAnimations();
        SetChasingAnimation(true);
        // Set Audio
        //m_CurrentAudioEvent = SoundController.Instance.PlayEvent("event:/Helicoptero/Helices", this.transform,GetComponent<Rigidbody>());
    }

    public void SetDie()
    {
        // Create floating text
        FloatingTextController.CreateFloatingText(m_EnemyBlackboard.m_CoinsToDrop.ToString(),m_EnemyTransform);
        // Stop last audio
        SoundController.Instance.StopEvent(m_CurrentAudioEvent);
        // Set state  to Die and stop NavMesh
        m_CurrentState = TEnemyStates.Die;
        m_ElapsedTime = 0.0f;
        m_NavMeshAgent.isStopped = true;
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
        // Set state to Idle and stop NavMesh
        m_CurrentState = TEnemyStates.Idle;
        m_ElapsedTime = 0.0f;
        m_NavMeshAgent.isStopped = true;
        m_CurrentAudioEvent = SoundController.Instance.PlayEvent("event:/Helicopter/Helices",m_EnemyTransform,GetComponent<Rigidbody>());

    }

    public void UpdateAttackObjective()
    {
        m_EnemyTransform.LookAt(m_Target.transform);
        if ((m_Target.transform.position - transform.position).magnitude > m_EnemyBlackboard.m_MaxDistanceToAttack)
        {
            SetChaseObjective();
        }
        // Chase player
        if ((GameController.Instance.m_PlayerBlackboard.transform.position - m_EnemyTransform.position).magnitude < m_EnemyBlackboard.m_DistanceToDetectPlayer)
        {
            // Change state to chase
            SetChasePlayer();
        }
        // If target is destroyed chase player
        if(!m_Target.GetComponent<ProtectableObject>().m_EnabledInGame)
            SetChasePlayer();
        // If caan attack fire
        if (CanAttack()) {
           FireObjtive();
        }
        // If is dead die
        if (IsDead())
            SetDie();
    }

    public void UpdateAttackPlayer()
    {
        m_EnemyTransform.LookAt(GameController.Instance.m_PlayerBlackboard.m_PlayerTransform);
        // if is out of range chase player
        if ((GameController.Instance.m_PlayerBlackboard.transform.position - m_EnemyTransform.position).magnitude > m_EnemyBlackboard.m_MaxDistanceToAttack)
        {
            SetChasePlayer();
        }
        // If can attack fire
        if (CanAttack()) {
           FirePlayer();
        }
        // If is dead die
        if (IsDead())
            SetDie();
    }

    public void UpdateChaseObjective()
    {
        // Set navmeshAgent destination
        m_NavMeshAgent.SetDestination(m_Target.transform.position);
        if ((GameController.Instance.m_PlayerBlackboard.m_PlayerTransform.position - m_EnemyTransform.position).magnitude < m_EnemyBlackboard.m_DistanceToDetectPlayer)
        {
            // Change state to chase
            SetChasePlayer();
        }
        // If is in distance attack objective
        if ((m_Target.transform.position - m_EnemyTransform.position).magnitude < m_EnemyBlackboard.m_DistanceToAttackObjective)
        {
            SetAttackObjective();
        }
    }

    public void UpdateChasePlayer()
    {
        // Set navmeshAgent destination
        m_NavMeshAgent.SetDestination(GameController.Instance.m_PlayerBlackboard.m_PlayerTransform.position);
        // If in range attack player
        if ((GameController.Instance.m_PlayerBlackboard.transform.position - m_EnemyTransform.position).magnitude < m_EnemyBlackboard.m_MinDistanceToAttack)
        {
            SetAttackPlayer();
        }
    }
    // Update die state
    public void UpdateDie()
    {
        
    }
    public void UpdateHit()
    {
        // Set chase player if hit animation ended
        if(m_ElapsedTime > m_HitAnimationClip.length)
            SetChasePlayer();
        // Die
        if(IsDead())
            SetDie();
    }
    public void UpdateIdle()
    {
        if (m_Active)
        {
            if (m_ElapsedTime > 1f)
            {
                if (GameModeController.Instance)
                    if (GameModeController.Instance.m_CurrentGameMode == GameModeController.TGameMode.Protect)
                        SetChaseObjective();
                    else
                        SetChasePlayer();
            }
        }
    }
    // Return true if enemy is dead
    private bool IsDead()
    {
        if (m_EnemyBlackboard.m_CurrentHealth <= 0) {
            return true;
        }
        return false;
    }
    private bool CanAttack()
    {
         // If elapsed time is greather than attack cadence return true otherwise return false
        if (m_ElapsedTime > m_EnemyBlackboard.m_AttackCadence) 
        {
            SetAttackAnimation(false);
            m_ElapsedTime = 0;
            return true;
        }
        if(m_ElapsedTime > m_AttackAnimationClip.length)
            SetAttackAnimation(false);
        return false;
    }
    void FirePlayer()
    {
        DesactivateAnimations();
        SetAttackAnimation(true);
        m_ElapsedTime = 0.0f;
        GunBulletResponse currentBullet = GameController.Instance.GetHeliGunBulletInactive();
        Vector3 bulletDirection = m_EnemyTransform.forward - new Vector3(0,+0.1f,0);
        currentBullet.OnInit(m_EnemyBlackboard.m_Damage, m_BulletSpeed, bulletDirection,m_BulletEmitter.transform.position);
        // Set audio
        SoundController.Instance.PlayOneShootAudio("event:/Helicopter/Shot",m_EnemyTransform);
    }
    private void FireObjtive()
    {
        // Instantiate bullet
        GunBulletResponse currentBullet = GameController.Instance.GetHeliGunBulletInactive();
        // Commpute bullet direction
        Vector3 bulletDirection = (m_Target.transform.position - m_EnemyTransform.position).normalized;
        // Init bullet
        currentBullet.OnInit(m_EnemyBlackboard.m_Damage, m_BulletSpeed, bulletDirection,m_BulletEmitter.transform.position);
    }
    public void DealDamage(int damage)
    {
        if (m_EnemyBlackboard.m_CurrentHealth > 0)
        {
            // Substract current health
            m_EnemyBlackboard.m_CurrentHealth -= damage;
            if (m_EnemyBlackboard.m_CurrentHealth <= 0)
            {
                // Set die
                SetDie();
            }
            else
            {
                // Set hit
                SetHit();
            }
        }
    }
    // Return current state
    public TEnemyStates CurrentState()
    {
        return m_CurrentState;
    }
    // Die
    IEnumerator Die()
    {   
        
        m_DieParticleSystem.Play();
        yield return new WaitForSeconds(m_DieAnimationClip.length);
        // Add coins
        GameController.Instance.m_PlayerBlackboard.AddCoins(m_EnemyBlackboard.m_CoinsToDrop);
        // Update HUD
        HUDController.Instance.UpdateCurrentCoins();
        // Destroy
        DeasctiveEnemy();
    }
    // Set chase animation
    public void SetChasingAnimation(bool active)
    {
        m_Animator.SetBool("Chase",active);
    }
    // Active hit animation
    public void SetHitAnimation(bool active)
    {
        m_Animator.SetBool("Hit",active);
    }
    // Active die animation
    public void SetDieAnimation(bool active)
    {
        m_Animator.SetBool("Die",active);
    }
    // Active attack animation
    public void SetAttackAnimation(bool active)
    {
        m_Animator.SetBool("Attack",active);
    }
    // Desactive all animation
    private void DesactivateAnimations()
    {
        m_Animator.SetBool("Attack", false);
        m_Animator.SetBool("Die", false);
        m_Animator.SetBool("Hit", false);
        m_Animator.SetBool("Chase", false);
    }
    public void DeasctiveEnemy()
    {
        m_DieParticleSystem.Stop();
        SoundController.Instance.StopEvent(m_CurrentAudioEvent);
        DesactivateAnimations();
        m_CurrentState = TEnemyStates.Idle;
        m_EnemyTransform.position = m_StartPosition;
        m_Active = false;
        m_NavMeshAgent.enabled = false;
    }
    void ActiveEnemy()
    {
        m_EnemyBlackboard.StatsInit();
        m_Active = true;
        m_NavMeshAgent.enabled = true;
        SetIdle();
    }
}
