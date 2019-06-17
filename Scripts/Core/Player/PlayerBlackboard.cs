/* 
 * Oscar Forra Carbonell
*/
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerBlackboard : MonoBehaviour
{
    public int m_MaxHealth = 100;                                           // Max health that player can reach
    [Header("Movement parameters")]
    public float m_ForwardMovementSpeed;                                    // Forward speed
    public float m_BackWardsMovementSpeed;                                  // Backwards speed
    public float m_AimMovementSpeed;                                        // While aiming movement speed
    public float m_JumpSpeed;                                               // Jump force
    public bool m_CanJump = true;
    [Header("Buff damagers")]
    public int m_DamageMultiplayer;                                         // Integer used to know the buff damage mutliplier
    public int m_BuffDuration;                                              // Duration that damage boost will have
    [Header("Weapons")]
    public GameObject m_Nerf;                                               // Reference to nerf GO
    public GameObject m_Punch;                                              // Reference to punch GO
    public GameObject m_Croco;                                              // Reference to croco GO
    public WeaponCreator m_NerfSO;                                          // Nerf Scriptable object
    public WeaponCreator m_PunchSO;                                         // Punch Scriptable object
    public WeaponCreator m_CrocoSO;                                         // Croco Scriptable object
    [Header("Player and Weapon components")]
    public CharacterController m_CharacterController;                       // Character controller atached to player

    public Animator m_PlayerAnimator;                                       // Animator attached to mesh
    public Animation m_NerfAnimation;                                       // Animation attached to nerf (Used to fade in and out)
    public Animation m_PunchAnimation;                                      // Animation attached to punch (Used to fade out and in)
    public Animation m_CrocoAnimation;                                      // Animation attached to croco (Used to fade out and in)
    public AnimationClip m_FadeInAnimationNerf;                             // Fade in animation nerf
    public AnimationClip m_FadeOutAnimationNerf;                            // Fade out animation nerf
    public AnimationClip m_FadeInAnimationCroco;                            //...
    public AnimationClip m_FadeOutAnimationCroco;                           //...
    public AnimationClip m_FadeInAnimationPunch;                            //...
    public AnimationClip m_FadeOutAnimationPunch;                           //...
    public AnimationClip m_HitAnimationClip;                                // Animation clip when player is hit
    public ParticleSystem m_ChangeWeaponPS;                                 // Particle system used when weapon changes
    public PostProcessVolume m_PostProcessVolume;                           // Post process volume
    [HideInInspector] public float m_VignetteValue;                        // Vignette value
    public Animation m_PlayerAnimation;                                     // Player animation (Only used to feedback)
    private Vignette m_vignetteLayer;                                       // Vignette component in PPV
    public bool m_RecivedDamage;                                            // Bool used to know if recived damage
    [HideInInspector] public int m_CurrentHealth;                           // Current player health
    [HideInInspector] public int m_CurrentCoins;                            // Current coins that player have
    [HideInInspector] public int m_CurrentWeapon = 0;                       // Current weapon
    [HideInInspector] public int m_NerfCurrentAmmo;                         // Current nerf ammo
    [HideInInspector] public int m_CrocoCurrentAmmo;                        // Current croco ammo
    private bool m_BuffActive;                                              // Bool used to know if buff is active
    [HideInInspector]public Camera m_PlayerCamera;                          // Reference to camera behind the player
    [HideInInspector]public int m_NerfCurrentLevel;                         // Current level of nerf weapon
    [HideInInspector]public int m_PunchCurrentLevel;                        // Current level of punch
    [HideInInspector]public int m_CrocoCurrentLevel;                        // Current granade launcher level
    [HideInInspector]public TObjectCollected m_CurrentColelctableGameObjectType; // Current type of collected gameObject
    [HideInInspector]public bool m_AddingAmmo;                              // Boolean used to know is in minitable
    [HideInInspector]public bool m_AddingHp;                                // Bolean used to know if is in hp table
    [HideInInspector] public float m_ElapsedTimeBuff;                       // Float used to know time elapsed since buff is active
    private Vector3 m_StartPosition;                                        // Start Position
    private Quaternion m_StartRotation;                                     // Start rotation
    [HideInInspector] public Transform m_PlayerTransform;                   // Player transform
    void Awake()
    {
        PlayerInit();
    }
    private void Update()
    {
        // If buff is active update buff state
        UpdateDamageBuff();
        VignetteEffect();
        
    }
    void VignetteEffect()
    {
        // Set vignetteValue
        if(m_RecivedDamage)
            m_vignetteLayer.intensity.value = m_VignetteValue;
    }
    private void UpdateDamageBuff()
    {
        // If buff is active
        if (BuffActive())
        {
            // Update time since buff is active
            m_ElapsedTimeBuff -= Time.deltaTime;
            HUDController.Instance.m_BuffTableSprite.fillAmount = m_ElapsedTimeBuff / m_BuffDuration;
            // If time gone
            if (m_ElapsedTimeBuff <= 0)
            {
                SubstractBuff();
            }
        }
    }

    public void ChangeWeapon(int l_NewWeapon,bool left)
    {
        //Change weapon
        m_ChangeWeaponPS.Play();
        // Desactive current active weaapon
        switch(m_CurrentWeapon)
        {
            case (int)TWeaponTypes.Nerf:
                m_NerfAnimation.clip = m_FadeOutAnimationNerf;
                m_NerfAnimation.Play();
                break;
            case (int)TWeaponTypes.Croco:
                m_CrocoAnimation.clip = m_FadeOutAnimationCroco;
                m_CrocoAnimation.Play();
                break;
            case (int)TWeaponTypes.Punch:
                m_PunchAnimation.clip = m_FadeOutAnimationPunch;
                m_PunchAnimation.Play();
            break;
        } // End switch
        // Active new active weapon
        switch(l_NewWeapon)
        {
            case (int)TWeaponTypes.Nerf:
                m_NerfAnimation.clip = m_FadeInAnimationNerf;
                m_NerfAnimation.Play();
                break;
            case (int)TWeaponTypes.Croco:
                m_CrocoAnimation.clip = m_FadeInAnimationCroco;
                m_CrocoAnimation.Play();
                break;
            case (int)TWeaponTypes.Punch:
                m_PunchAnimation.clip = m_FadeInAnimationPunch;
                m_PunchAnimation.Play();
            break;
        } // End switch
        // Set new weapon as current
        m_CurrentWeapon = l_NewWeapon;
        HUDController.Instance.UpdateCurrentAmmo();
        HUDController.Instance.UpdateSelectedWeapon((TWeaponTypes)m_CurrentWeapon,left);
    }
    public void AddCoins(int l_Coins)
    {
        // Add coins to current count
        m_CurrentCoins += l_Coins;
    }
    public void SubstractCoins(int l_Coins)
    {
        // Substract coins to current count
        m_CurrentCoins -= l_Coins;    
    }
    public void SubstractAmmo(int l_Ammo)
    {
        //Set shoot anim
        StartCoroutine(SetShoot());
        // Substract ammo to current weapon active
        switch(m_CurrentWeapon)
        {
            case (int)TWeaponTypes.Nerf:
                m_NerfCurrentAmmo -= l_Ammo;
                // Clamp
                m_NerfCurrentAmmo = Mathf.Clamp(m_NerfCurrentAmmo,0,m_NerfSO.m_WeaponLevel[m_NerfCurrentLevel].m_Ammo);
                break;
            case (int)TWeaponTypes.Croco:
                m_CrocoCurrentAmmo -= l_Ammo;
                // Clamp
                m_CrocoCurrentAmmo = Mathf.Clamp(m_CrocoCurrentAmmo,0,m_CrocoSO.m_WeaponLevel[m_CrocoCurrentLevel].m_Ammo);
                break;
        } // End switch
        //Update HUD
        HUDController.Instance.UpdateCurrentAmmo();
    }
    public void AddAmmo(int l_Ammo)
    {
        // Add ammo to current weapon active
        switch(m_CurrentWeapon)
        {
            case (int)TWeaponTypes.Nerf:
                m_NerfCurrentAmmo += l_Ammo;
                // Clamp
                m_NerfCurrentAmmo = Mathf.Clamp(m_NerfCurrentAmmo,0,m_NerfSO.m_WeaponLevel[m_NerfCurrentLevel].m_Ammo);
                break;
            case (int)TWeaponTypes.Croco:
                m_CrocoCurrentAmmo += l_Ammo;
                // Clamp
                m_CrocoCurrentAmmo = Mathf.Clamp(m_CrocoCurrentAmmo,0,m_CrocoSO.m_WeaponLevel[m_CrocoCurrentLevel].m_Ammo);
                break;
        } // End switch
        //Update HUD
        HUDController.Instance.UpdateCurrentAmmo();
    }
    // Return true if weapon can add ammo
    public bool CanAddAmmo()
    {
        switch(m_CurrentWeapon)
        {
            case (int)TWeaponTypes.Nerf:
                if(m_NerfCurrentAmmo >= m_NerfSO.m_WeaponLevel[m_NerfCurrentLevel].m_Ammo)
                {
                    return false;
                }
                return true;
            case (int)TWeaponTypes.Croco:
                if(m_CrocoCurrentAmmo >= m_CrocoSO.m_WeaponLevel[m_CrocoCurrentLevel].m_Ammo)
                {
                    return false;
                }
                return true;
            case (int)TWeaponTypes.Punch:
                return false;
            default:
                return false;
        } // End switch
    }
    public void AddHealth(int l_Health)
    {
        // Add current health
        m_CurrentHealth += l_Health;
        // Clamp the current health to maximum
        m_CurrentHealth = Mathf.Clamp(m_CurrentHealth,0,m_MaxHealth);
        // Update HUD
        HUDController.Instance.UpdatePlayerHealthFillAmount();
    }
    public void DealDamage(int l_Damage)
    {
        //Set hit animation feedback
        if(!m_RecivedDamage)
            m_PlayerAnimation.Play();
        StartCoroutine(SetHit());
        // Recive damage
        m_CurrentHealth -= l_Damage;
        // Update HUD
        HUDController.Instance.UpdatePlayerHealthFillAmount();
        //Play sound
        SoundController.Instance.PlayOneShootAudio("event:/Character/Damage",m_PlayerTransform);
    }
    // Set animator integer to 0
    private IEnumerator SetHit()
    {
        // Set recive damage to true
        m_RecivedDamage = true;
        m_PlayerAnimator.SetInteger("Hit",Random.Range(0,4));
        // Wait for hit animation time
        yield return new WaitForSeconds(m_HitAnimationClip.length);
        // Set to false recive damage
        m_RecivedDamage = false;
        m_PlayerAnimator.SetInteger("Hit",-1);
    }

    // Set animator bool to false
    private IEnumerator SetShoot()
    {
        m_PlayerAnimator.SetBool("Shoot",true);
        yield return new WaitForSeconds(m_NerfSO.m_WeaponLevel[m_NerfCurrentLevel].m_Cadence);
        m_PlayerAnimator.SetBool("Shoot", false);
    }
    // Upgrade nerf level and clamp it to don't overpass the maximum level
    public void UpgradeNerf()
    {
        m_NerfCurrentLevel++;
        m_NerfCurrentLevel = Mathf.Clamp(m_NerfCurrentLevel, 0, m_NerfSO.m_WeaponLevel.Count);
        m_NerfCurrentAmmo += 1000;
        // Clamp
        m_NerfCurrentAmmo = Mathf.Clamp(m_NerfCurrentAmmo, 0, m_NerfSO.m_WeaponLevel[m_NerfCurrentLevel].m_Ammo);
    }
    // Upgrade punch level and calmp it to don't overpass the maximum level
    public void UpgradePunch() { m_PunchCurrentLevel++; m_PunchCurrentLevel = Mathf.Clamp(m_NerfCurrentLevel, 0, m_PunchSO.m_WeaponLevel.Count); }
    // Upgrade croco level and calmp it to don't overpass the maximum level
    public void UpgradeCroco()
    {
        m_CrocoCurrentLevel++;
        m_CrocoCurrentLevel = Mathf.Clamp(m_NerfCurrentLevel, 0, m_CrocoSO.m_WeaponLevel.Count);
        m_CrocoCurrentAmmo += 1000;
        // Clamp
        m_CrocoCurrentAmmo = Mathf.Clamp(m_CrocoCurrentAmmo, 0, m_CrocoSO.m_WeaponLevel[m_CrocoCurrentLevel].m_Ammo);
    }
    public void AddBuff()
    {
        HUDController.Instance.SetBuffTableHUD(true);
        // Set boolean to true 
        m_BuffActive = true;
        m_ElapsedTimeBuff = m_BuffDuration;
    }
    public void SubstractBuff()
    {
        HUDController.Instance.SetBuffTableHUD(false);
        // Set boolean to false
        m_BuffActive = false;
        m_ElapsedTimeBuff = m_BuffDuration;
    }
    public bool IsDead()
    {
        // Return true if player HP is less or equal to 0
        return m_CurrentHealth <= 0;
    }
    // Set dead animation to true
    public void SetDeadAnimation()
    {
        m_PlayerAnimator.SetBool("Die",true);
    }
    public bool CanShoot()
    {
        // Return true if current ammo of active weapon is greater than 0
        switch(m_CurrentWeapon)
        {
            case (int)TWeaponTypes.Nerf:
                return m_NerfCurrentAmmo > 0;
            case (int)TWeaponTypes.Croco:
                return m_CrocoCurrentAmmo > 0;
        }
        return true;
    }

    public bool BuffActive()
    {
        // Return true if buff is active
        return m_BuffActive;
    }
    // Return true if player is ammo minitable
    public bool AmmoBuffActive()
    {
        return m_AddingAmmo;
    }
    // Return true if player is in health minitable
    public bool HealthBuffActive()
    {
        return m_AddingHp;
    }
    // Retunr true if player can collect another object
    public bool CanCollectObject()
    {
        // Return true if no object is attached
        Debug.Log(m_CurrentColelctableGameObjectType == TObjectCollected.None);
        return m_CurrentColelctableGameObjectType == TObjectCollected.None;
    }
    // Init player
    private void PlayerInit()
    {
        // Get Transform
        m_PlayerTransform = this.transform;
        // Get PPV
        m_PostProcessVolume.profile.TryGetSettings(out m_vignetteLayer);
        // Get player camera
        m_PlayerCamera = Camera.main;
        // Set ammo to max ammo
        if(m_NerfSO && m_CrocoSO)
        {
            m_NerfCurrentAmmo = m_NerfSO.m_WeaponLevel[m_NerfCurrentLevel].m_Ammo;
            m_CrocoCurrentAmmo = m_CrocoSO.m_WeaponLevel[m_CrocoCurrentLevel].m_Ammo;
        }
        else
        {
            Debug.LogError("Drag to inspector the SO weapons in PlayerBlackboard");
        }
        m_CurrentColelctableGameObjectType = TObjectCollected.None;
        // Set start and start rrotation
        m_StartPosition = m_PlayerTransform.position;
        m_StartRotation = m_PlayerTransform.rotation;
        // Set max health
        m_CurrentHealth = m_MaxHealth;
    }
    public void RestartPlayerPosition()
    {
        // Disable charactercontroller to move player
        m_CharacterController.enabled = false;
        // Set position and rotation
        m_PlayerTransform.position = m_StartPosition;
        m_PlayerTransform.rotation = m_StartRotation;
        // Enalbe character controller
        m_CharacterController.enabled = true;
    }
    // Return current upgrade cost nerf
    public int GetNerfUpgradeCost()
    {
        return m_NerfSO.m_WeaponLevel[GameController.Instance.m_PlayerBlackboard.m_NerfCurrentLevel].m_UpgradeCost;
    }
    // Return current upgrade cost punch
    public int GetPunchUpgradeCost()
    {
        return m_PunchSO.m_WeaponLevel[GameController.Instance.m_PlayerBlackboard.m_PunchCurrentLevel].m_UpgradeCost;
    }
    // Return current upgrade cost croco
    public int GetCrocoUpgradeCost()
    {
        return m_CrocoSO.m_WeaponLevel[GameController.Instance.m_PlayerBlackboard.m_CrocoCurrentLevel].m_UpgradeCost;
    }
    
}
