/* 
 * Oscar Forra Carbonell
*/
using System.Collections;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [Header("Weapons components")]
    public Transform m_BulletEmitter;                                               // Position where bullet will be instantiated
    public GunBulletResponse m_NerfBulletPrefab;                                    // Nerf bullet prefab
    public GranadeBulletResponse m_CrocoBulletPrefab;                               // Croco bullet prefab
    public Collider m_PunchCollider;                                                // Punch collider
    public LayerMask m_ShootLayerMask;                                              // Layer mask where nerf will affect
    public float m_BulletMaxDistance;                                               // Bullet distance
    [Header("Animations")]
    public Animator m_CrocoAnimator;                                                // Animator component attached to croco mesh
    public Animator m_PunchAnimator;                                                // Animator componenet attached to punch mesh
    public AnimationClip m_CrocoAnimationClip;                                      // Crocodile animaationClip played when fires
    public AnimationClip m_PunchAnimationClip;                                      // Punch animationClip played when fires
    [Header("Particles")]
    public ParticleSystem m_NerfShootParticleSystem;                                // Nerf canon PS
    public ParticleSystem m_NerfBulletParticleSystem;                               // Nerf bullet PS
    public ParticleSystem m_PunchParticleSystem;                                    // Punch PS
    private PlayerBlackboard m_PlayerBlackboard;                                    // Player blackboard
    private float m_ChangeWeaponCoolDown = 0.5f;                                    // Time between change weapon
    private float m_ElapsedTime;                                                    // Time elapsed since start
    private float m_ElapsedTimeFire;                                                // Time elapsed since last weapon shoot

    

    // Start is called before the first frame update
    void Awake()
    {
        // Get player blackboard
        if(m_PlayerBlackboard == null)
            m_PlayerBlackboard = GetComponent<PlayerBlackboard>();
    }
     // Update is called once per frame
    private void Update()
    {
        if (GameController.Instance.m_CurrentGameState == TGameState.Play)
        {
            // Compute elapsed times
            m_ElapsedTime += Time.deltaTime;
            m_ElapsedTimeFire += Time.deltaTime;
            // Create bumper float inputs
            float l_BumperLeft, l_BumperRight, l_TriggerRight;
            // Get bumper and trigger right inputs
            GetBumperAndTriggerInputs(out l_BumperLeft, out l_BumperRight, out l_TriggerRight);
            // Change weapon if player wants to
            SelectWeapon(l_BumperLeft, l_BumperRight);
            // Shoot if player wants to
            FireResponse(l_TriggerRight);
        }
    }
    // Get player inputs
    private void GetBumperAndTriggerInputs(out float l_BumperLeft, out float l_BumperRight,out float l_TriggerRight)
    {
        l_BumperLeft = Input.GetAxis("BumperLeft_WIN");
        l_BumperRight = Input.GetAxis("BumperRight_WIN");
        l_TriggerRight = Input.GetAxis("TriggerRight_WIN");
    }
    private void SelectWeapon(float l_BumperLeft, float l_BumperRight)
    {
        // If elapsed time is higher than CD can change weapon
        if(CanChangeWeapon())
        { 
            // Change weapon
            if(l_BumperRight > 0)
            {
                m_ElapsedTime = 0;
                if(m_PlayerBlackboard.m_CurrentWeapon == 2)
                {
                    m_PlayerBlackboard.ChangeWeapon(0,true);

                }
                else
                {
                    m_PlayerBlackboard.ChangeWeapon(m_PlayerBlackboard.m_CurrentWeapon+1,false);
                } 
            }
            if(l_BumperLeft > 0)
            {
                m_ElapsedTime = 0;
                if(m_PlayerBlackboard.m_CurrentWeapon == 0)
                {
                    m_PlayerBlackboard.ChangeWeapon(2,true);
                }
                else
                {
                    m_PlayerBlackboard.ChangeWeapon(m_PlayerBlackboard.m_CurrentWeapon-1,false);
                }   
            }
        }     
    }
    private void FireResponse(float l_TriggerRight)
    {
        // If player press trigger Fire
        if(l_TriggerRight > 0)
        {
            Fire();
            m_PlayerBlackboard.m_PlayerAnimator.SetBool("Shoot", true);
        }
        else
        {
            SetShootAnimationFalse();
        }
    }
    private void Fire()
    {
        // If player have enought Ammo
        if(m_PlayerBlackboard.CanShoot())
        {
            // If elapsed time is higher than current weapon candence fire
            switch(m_PlayerBlackboard.m_CurrentWeapon)
            {
                case (int)TWeaponTypes.Croco:
                    if(m_ElapsedTimeFire > m_PlayerBlackboard.m_CrocoSO.m_WeaponLevel[m_PlayerBlackboard.m_CrocoCurrentLevel].m_Cadence)
                        FireCroco();
                    break;
                case (int)TWeaponTypes.Punch:
                    if(m_ElapsedTimeFire > m_PlayerBlackboard.m_PunchSO.m_WeaponLevel[m_PlayerBlackboard.m_PunchCurrentLevel].m_Cadence)
                        StartCoroutine(FirePunch());
                    break;
                case (int)TWeaponTypes.Nerf:
                    if(m_ElapsedTimeFire > m_PlayerBlackboard.m_NerfSO.m_WeaponLevel[m_PlayerBlackboard.m_NerfCurrentLevel].m_Cadence)
                        FireNerf();
                    break;
            }
        }
        else
        {
            SoundController.Instance.PlayOneShootAudio("event:/Nerf/Recarga de municion",m_PlayerBlackboard.m_PlayerTransform);
        }
       
    }
    private void FireNerf()
    {
        // Instance PS
        if(m_NerfBulletParticleSystem)
            m_NerfBulletParticleSystem.Play();
        SoundController.Instance.PlayOneShootAudio("event:/Nerf/Shot",m_PlayerBlackboard.m_PlayerTransform);
        // Set elapsed time to 0
        m_ElapsedTimeFire = 0;
        //Substract Ammo
        m_PlayerBlackboard.SubstractAmmo(1);
        // Get damage depending if player have buff activated
        int l_Damage = m_PlayerBlackboard.BuffActive() ?
                        l_Damage = m_PlayerBlackboard.m_NerfSO.m_WeaponLevel[m_PlayerBlackboard.m_NerfCurrentLevel].m_Damage * m_PlayerBlackboard.m_DamageMultiplayer   : 
                        m_PlayerBlackboard.m_NerfSO.m_WeaponLevel[m_PlayerBlackboard.m_NerfCurrentLevel].m_Damage;
        Ray l_CameraRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_CameraRay, out l_RaycastHit, m_BulletMaxDistance, m_ShootLayerMask.value))
        {
            GameController.Instance.GetNerfProjectileCollisionParticle().Initialize(l_RaycastHit.point,l_RaycastHit.normal);
            if(l_RaycastHit.collider.GetComponent<MeleeEnemy>() != null)
            {
                l_RaycastHit.collider.GetComponent<MeleeEnemy>().DealDamage(l_Damage);
            }
            else if(l_RaycastHit.collider.GetComponent<DistanceEnemy>()!= null)
            {
                l_RaycastHit.collider.GetComponent<DistanceEnemy>().DealDamage(l_Damage);
            }
            else if (l_RaycastHit.collider.GetComponent<DestroyableObject>()!= null)
            {
                l_RaycastHit.collider.GetComponent<DestroyableObject>().DealDamage(l_Damage);
            }
            else if (l_RaycastHit.collider.tag == "BoxAmmo")
            {
                BoxAmmo l_Box = l_RaycastHit.collider.GetComponent<BoxAmmo>();
                l_Box.DropObject();
            }
            else
            {
                GameController.Instance.GetDecalNerfInactive().Initialize(l_RaycastHit.point, l_RaycastHit.normal);
            }
        }       
    }
    private void FireNerfAnitguo()
    {
        SoundController.Instance.PlayOneShootAudio("event:/Nerf/Shot",m_PlayerBlackboard.m_PlayerTransform);
        // Set elapsed time to 0
        m_ElapsedTimeFire = 0;
        //Substract Ammo
        m_PlayerBlackboard.SubstractAmmo(1);
        // Instantiate bullet
        GunBulletResponse l_CurrentBullet = GameController.Instance.GetGunBulletInactive();
        // Get damage depending if player have buff activated
        int l_Damage = m_PlayerBlackboard.BuffActive() ?
                        l_Damage = m_PlayerBlackboard.m_NerfSO.m_WeaponLevel[m_PlayerBlackboard.m_NerfCurrentLevel].m_Damage * m_PlayerBlackboard.m_DamageMultiplayer   : 
                        m_PlayerBlackboard.m_NerfSO.m_WeaponLevel[m_PlayerBlackboard.m_NerfCurrentLevel].m_Damage;
        // Init bullet with his damage, direction and cadence 
        l_CurrentBullet.OnInit(l_Damage,
                               m_PlayerBlackboard.m_NerfSO.m_WeaponLevel[m_PlayerBlackboard.m_NerfCurrentLevel].m_BulletForce,
                               m_PlayerBlackboard.m_PlayerCamera.transform.forward,m_BulletEmitter.transform.position);
        // Play PS
        m_NerfShootParticleSystem.Play();
    }
    private void FireCroco()
    {
        SoundController.Instance.PlayOneShootAudio("event:/Croco/Open",m_PlayerBlackboard.m_PlayerTransform);
        // Set elapsed time to 0
        m_ElapsedTimeFire = 0;
        //Substract Ammo
        m_PlayerBlackboard.SubstractAmmo(1);
        // Get damage depending if player have buff activated
        GranadeBulletResponse l_CurrentBullet = GameController.Instance.GetGranadeBulletInactive();
        int l_Damage = m_PlayerBlackboard.BuffActive() ?
                        l_Damage = m_PlayerBlackboard.m_CrocoSO.m_WeaponLevel[m_PlayerBlackboard.m_CrocoCurrentLevel].m_Damage * m_PlayerBlackboard.m_DamageMultiplayer   : 
                        m_PlayerBlackboard.m_CrocoSO.m_WeaponLevel[m_PlayerBlackboard.m_CrocoCurrentLevel].m_Damage;
        // Init bullet with his damage, direction and cadence
        l_CurrentBullet.OnInit(l_Damage,
                               m_PlayerBlackboard.m_CrocoSO.m_WeaponLevel[m_PlayerBlackboard.m_CrocoCurrentLevel].m_BulletForce,
                               m_PlayerBlackboard.m_PlayerCamera.transform.forward,m_BulletEmitter.transform.position);
        // Play animation
        m_CrocoAnimator.SetBool("Shoot",true);
        StartCoroutine(SetCrocoAnimatorBoolFlase());
    }
    // Set animator boolean to false
    private IEnumerator SetCrocoAnimatorBoolFlase()
    {
        yield return new WaitForSeconds(m_CrocoAnimationClip.length);
        m_CrocoAnimator.SetBool("Shoot",false);
        SoundController.Instance.PlayOneShootAudio("event:/Croco/Close",m_PlayerBlackboard.m_PlayerTransform);
    }
    // Fire punch
    private IEnumerator FirePunch()
    {
        //Instantaite PS
        m_PunchParticleSystem.Play();
        //Play Sound
        SoundController.Instance.PlayOneShootAudio("event:/Punch/Puño vuelta",m_PlayerBlackboard.m_PlayerTransform);
        // Set elapsed time to 0
        m_ElapsedTimeFire = 0;
        // Active punch collider and get visual feedback
        m_PunchAnimator.SetBool("Shoot",true);
        m_PunchCollider.enabled = true;
        // Wait half second
        yield return new WaitForSeconds(m_PunchAnimationClip.length);
        // Desactive punch collider and get visual feedback
        m_PunchAnimator.SetBool("Shoot",false);
        m_PunchCollider.enabled = false;
        m_PunchParticleSystem.Stop();
    }
    // Set animator boolean to false
    private void SetShootAnimationFalse()
    {
        m_NerfBulletParticleSystem.Stop();
        m_PlayerBlackboard.m_PlayerAnimator.SetBool("Shoot", false);
    }
    void OnTriggerEnter(Collider other) 
    {
        // Deal damage to enemy
        if(other.tag == "Enemy" && m_PlayerBlackboard.m_CurrentWeapon == (int)TWeaponTypes.Punch)
        {
            other.GetComponent<IEnemy>().DealDamage(m_PlayerBlackboard.m_PunchSO.m_WeaponLevel[m_PlayerBlackboard.m_PunchCurrentLevel].m_Damage);
        }
        // Deal damage to destroyable object
        if(other.tag == "DestroyableObject" && m_PlayerBlackboard.m_CurrentWeapon == (int)TWeaponTypes.Punch)
        {
            other.GetComponent<IDamageable>().DealDamage(m_PlayerBlackboard.m_PunchSO.m_WeaponLevel[m_PlayerBlackboard.m_PunchCurrentLevel].m_Damage);
        }
        // Desactive box ammo
        if(other.tag == "BoxAmmo")
        {
            BoxAmmo l_Box = other.GetComponent<BoxAmmo>();
            l_Box.DropObject();
        }
    }
    // Return true if can change weapon
    private bool CanChangeWeapon()
    {
        // Return true if elapsed Time is higher than change weapon CD
        return m_ElapsedTime > m_ChangeWeaponCoolDown;
    }
}
