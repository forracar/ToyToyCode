/* 
 * Oscar Forra Carbonell
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public enum TGameState {Play,Pause,Upgrade,GameOver,Win, None};                                             // Enum used to know the actual game state
public enum TEnemyTypes{Teeth,Helicopter,Robot};                                                            // Enum used to know witch enemy types are in game
public enum TEnemyStates{Idle,ChasePlayer,ChaseObjective,AttackPlayer,AttackObjective,Hit,Die};             // Enum used to know states of enemies
public enum TZones{Military,Student,Twin1,Twin2,Barbie}                                                     // Enum used to know witch talbes are in game
public enum TWeaponTypes{Nerf, Croco, Punch};                                                               // Enum u sed to know witch weapons are in game
public enum TObjectCollected{Crown,Lego,AidKit,None};                                                       // Enum used to know whitch type of collectable objetct is
public enum TSpawnID{One,Two,Three,Four,Five};
public enum TDifficulty{Easy,Normal,Hard};
public class GameController : Singleton<GameController>
{
    public TGameState m_CurrentGameState;                                                                   // Current game state
    public TDifficulty m_CurrentDifficulty;                                                                 // Curent game difficulty
    public int m_RoundToIncreaseDifficultyToNormal;                                                         // Round that game will increase to normal difficulty
    public int m_RoundToIncreaseDiffcultyToHard;                                                            // Round that game will increase to hard difficulty
    public int m_CurrentRound;                                                                              // Current round
    public PlayerBlackboard m_PlayerBlackboard;                                                             // Player blackboard reference
    public int m_CostToBuyAmmoTable;                                                                        // Cost to buy ammo table
    public int m_CostToBuyHealthTable;                                                                      // Cost to buy health table
    public int m_CostToBuyBuffTable;                                                                        // Cost to buy buff table
    public GameObject m_AmmoTableGameObject;                                                                // Reference to ammo table
    public GameObject m_HealthTableGameObject;                                                              // Reference to health table
    public GameObject m_BuffTableGameObject;                                                                // Reference to buff table
    public GameObject m_AmmoTablePS;                                                                        // AmmoTalbe PS
    public GameObject m_HealthTablePS;                                                                      // HealthTable PS
    public GameObject m_BuffTablePS;                                                                        //  bUFF TALBE ps
    public List<IRestartGameElement> m_RestartGameElements = new List<IRestartGameElement>();               // All restart elements
    public List<IActiveDuringGameState> m_ActiveDuringGameStateElements = new List<IActiveDuringGameState>();// All hud elements that will active and desactive depending on game state
    public GunBulletResponse[] m_GunBullets;                                                                 // Bullets PULL
    public GunBulletResponse[] m_HeliGunBullets;                                                             // Heli bullets PULL
    public GranadeBulletResponse[] m_GranadeBullets;                                                         // GranadeBullets PULL
    public MeleeEnemy[] m_TeethEnemies;                                                                      // Teeth PULL
    public MeleeEnemy[] m_RobotEnemies;                                                                      // Robot PULL
    public DistanceEnemy[] m_HelicotperEnemies;                                                              // Helicopter PULL
    public Decal [] m_NerfDecals;                                                                            // Nerf decals PULL
    public GranadeDecal[] m_GranadeDecals;                                                                  // Granade decals PULL
    public ProjectileNerfCollisionParticleSystem [] m_NerfProjectileParticleSystem;                         // Nerf projectile PULL
    public FloatingText m_FloatingTextPrefab;                                                               // Floating text prefab
    public float m_ElapsedTime;
   public bool m_Dead = false;

    private void Start()
    {
        m_Dead = false;
        // When game init change game state to play and start round
        InitGame();
        FloatingTextController.Initialize();
        // lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = Cursor.visible = false;
    }
    void Update()
    {
        m_ElapsedTime += Time.deltaTime;
        switch (m_CurrentGameState)
        {
            case TGameState.Play:
                // If player press start button and game state is play
                if (Input.GetAxisRaw("Pause_WIN") > 0 && GameController.Instance.m_CurrentGameState == TGameState.Play)
                {
                    // Change game mode to pause
                    ChangeGameStateToPause();
                }
                // Else player is dead
                else if (m_PlayerBlackboard.IsDead() && !m_Dead)
                {
                    m_Dead = true;
                    // Play Sound
                    SoundController.Instance.StopAudioSource();
                    SoundController.Instance.PlayOneShootAudio("event:/GamePlay/GameOver", GameController.Instance.m_PlayerBlackboard.m_PlayerCamera.transform);
                    // Change game mode to gameOver
                    StartCoroutine(ChangeGameStateToGameOver());
                }
                break;
            case TGameState.Upgrade:
                if(Input.GetAxisRaw("ButtonB_WIN") > 0)
                {
                    GameController.Instance.ChangeGameStateToPlay();
                }
            break;
        }
    }
    private void InitGame()
    {
        // Changes game mode randomly
        GameModeController.Instance.ChangeGameModeStateRandomly();
        GameController.Instance.ChangeGameStateToPlay();
        // Set player position
        //m_PlayerBlackboard.RestartPlayerPosition();
        // Set time Scale to 1
        Time.timeScale = 1;
        // Start round
        RoundController.Instance.StartRound();
    }
    public GunBulletResponse GetGunBulletInactive()
    {
        for(int i = 0; i< m_GunBullets.Length; i++)
        {
            if(!m_GunBullets[i].m_Active)
                return m_GunBullets[i];
        }
        return null;
    }
    public GunBulletResponse GetHeliGunBulletInactive()
    {
        for(int i = 0; i< m_HeliGunBullets.Length; i++)
        {
            if(!m_HeliGunBullets[i].m_Active)
                return m_HeliGunBullets[i];
        }
        return null;
    }
    public GranadeDecal GetGranadeDecals()
    {
        for(int i = 0; i< m_GranadeDecals.Length; i++)
        {
            if(!m_GranadeDecals[i].m_Active)
                return m_GranadeDecals[i];
        }
        return null;
    }
    public GranadeBulletResponse GetGranadeBulletInactive()
    {
        for(int i = 0; i< m_GranadeBullets.Length; i++)
        {
            if(!m_GranadeBullets[i].m_Active)
                return m_GranadeBullets[i];
        }
        return null;
    }
    public MeleeEnemy GetTeethEnemyInactive()
    {
        for( int i = 0; i < m_TeethEnemies.Length; i++)
        {
            if(!m_TeethEnemies[i].m_Active)
                return m_TeethEnemies[i];
        }
        return null;
    }
    public MeleeEnemy GetRobotEnemyInactive()
    {
        for( int i = 0; i < m_RobotEnemies.Length; i++)
        {
            if(!m_RobotEnemies[i].m_Active)
                return m_RobotEnemies[i];
        }
        return null;
    }
    public DistanceEnemy GetHelicotperEnemyInactive()
    {
        for( int i = 0; i < m_HelicotperEnemies.Length; i++)
        {
            if(!m_HelicotperEnemies[i].m_Active)
                return m_HelicotperEnemies[i];
        }
        return null;
    }
    public Decal GetDecalNerfInactive()
    {
        for(int i = 0; i< m_NerfDecals.Length; i++)
        {
            if(!m_NerfDecals[i].m_Active)
                return m_NerfDecals[i];
        }
        return null;
    }
    public ProjectileNerfCollisionParticleSystem GetNerfProjectileCollisionParticle()
    {
        for(int i = 0; i< m_NerfProjectileParticleSystem.Length; i++)
        {
            if(!m_NerfProjectileParticleSystem[i].m_Active)
                return m_NerfProjectileParticleSystem[i];
        }
        return null;
    }
    // Load main menu scene
    public void ReturnToMainMenuScene()
    {
        // Set time Scale to 1
        Time.timeScale = 1;
        SceneManager.LoadScene("Start", LoadSceneMode.Single);
    }
    public void PlayAgain()
    {
        FMOD.Studio.Bus playerBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
        playerBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene("ToyToy",LoadSceneMode.Single);
    }
    public IEnumerator SetJumpToTrue()
    {
        yield return new WaitForSeconds(0.5f);
        m_PlayerBlackboard.m_CanJump = true;
    }
    public void ChangeGameState(TGameState l_newGameState)
    {
        m_ElapsedTime = 0;
        switch(l_newGameState)
        {
            case TGameState.Play:
                StartCoroutine(SetJumpToTrue());
                if(m_CurrentGameState == TGameState.Upgrade)
                {
                    SoundController.Instance.PlayAudioSource();
                    // Changes game mode randomly
                    GameModeController.Instance.ChangeGameModeStateRandomly();
                    m_PlayerBlackboard.AddHealth(m_PlayerBlackboard.m_MaxHealth);
                    // Set player position
                    m_PlayerBlackboard.RestartPlayerPosition();
                    // Start round
                    RoundController.Instance.StartRound();
                }
                Time.timeScale = 1;
                break;
            case TGameState.Upgrade:
                RoundController.Instance.DestroyEnemiesAlive();
                // Set time Scale to 0
                Time.timeScale = 0;
                break;
            case TGameState.Pause:
                m_PlayerBlackboard.m_CanJump = false;
                // Set time Scale to 0
                Time.timeScale = 0;
                break;
            case TGameState.GameOver:
                RoundController.Instance.DestroyEnemiesAlive();
                // Set time Scale to 1
                Time.timeScale = 1;
                break;
        }
        // Set current state to new state
        m_CurrentGameState = l_newGameState;
        // Restart game elements
        RestartGameElements();
        // Active game elements depending on game state
        ActiveGameStateElements();
        switch(m_CurrentGameState)
        {
            case TGameState.Upgrade:
               // Select upgraade button
                HUDController.Instance.SelectUpgradeButton();
                //Upgrade HUD
                HUDController.Instance.UpdateUpgradeHUDCost();
                HUDController.Instance.DesactiveObjectiveImagesFalse();
                UpdateBuyButtons();
                break;
            case TGameState.Pause:
                HUDController.Instance.SelectPauseButton();
                 Debug.Log(HUDController.Instance.m_EventSystem.currentSelectedGameObject.name);
                break;
            case TGameState.GameOver:
                HUDController.Instance.SelectGameOverButton();
                break;
        }
    }
    // Change game state to pause
    public void ChangeGameStateToPause(){ChangeGameState(TGameState.Pause);}
    // Change game state to game over
    public IEnumerator ChangeGameStateToGameOver()
    {
        // Set dead animation
        m_PlayerBlackboard.SetDeadAnimation();
        // Fade out
        HUDController.Instance.SetFadeInAnimation();
        yield return new WaitForSeconds(HUDController.Instance.m_FadeInAnimationClip.length);
        ChangeGameState(TGameState.GameOver);
    }
    // Change game state to upgrade
    public void ChangeGameStateToUpgrade(){ChangeGameState(TGameState.Upgrade);}
    // Change game state to play
    public void ChangeGameStateToPlay(){ChangeGameState(TGameState.Play);}
    // Change the difficulty when overpass the int m_RoundToIncreaseDifficulty
    public void SetCurrentDifficulty()
    { 
        if(m_CurrentRound > m_RoundToIncreaseDifficultyToNormal) m_CurrentDifficulty = TDifficulty.Normal;
        else if(m_CurrentRound > m_RoundToIncreaseDiffcultyToHard ) m_CurrentDifficulty = TDifficulty.Hard; 
        GameModeController.Instance.SetCurrentTimeRound(m_CurrentDifficulty);
    }
    // Upgrade level nerf
    public void UpgradeNerf()
    {
        if (CanUpgradeNerf())
        {
            SoundController.Instance.PlayOneShootAudio("event:/UI/ClickBuy",m_PlayerBlackboard.m_PlayerCamera.transform);
            // Substract coins
            m_PlayerBlackboard.SubstractCoins(m_PlayerBlackboard.GetNerfUpgradeCost());
            // Upgrade nerf
            m_PlayerBlackboard.UpgradeNerf();
            // Update HUD
            HUDController.Instance.UpdateUpgradeHUDCost();
            HUDController.Instance.UpdateCurrentCoins();
            HUDController.Instance.UpdateCurrentAmmo();
            // Enable button
            UpdateBuyButtons();
        }
    }
    // Upgrade level croco
    public void UpgradeCroco()
    {
        if (CanUpgradeCroco())
        {
            SoundController.Instance.PlayOneShootAudio("event:/UI/ClickBuy",m_PlayerBlackboard.m_PlayerCamera.transform);
            //Substract coins
            m_PlayerBlackboard.SubstractCoins(m_PlayerBlackboard.GetCrocoUpgradeCost());
            //Upgrade croco
            m_PlayerBlackboard.UpgradeCroco();
            // Update HUD
            HUDController.Instance.UpdateUpgradeHUDCost();
            HUDController.Instance.UpdateCurrentCoins();
            HUDController.Instance.UpdateCurrentAmmo();
            // Enable button
            UpdateBuyButtons();
        }
    }
    // Upgrade level punch
    public void UpgradePunch()
    {
        if (CanUpgradePunch())
        {
            SoundController.Instance.PlayOneShootAudio("event:/UI/ClickBuy",m_PlayerBlackboard.m_PlayerCamera.transform);
            // Substract coins
            m_PlayerBlackboard.SubstractCoins(m_PlayerBlackboard.GetPunchUpgradeCost());
            // Upgrade punch
            m_PlayerBlackboard.UpgradePunch();
            // Update HUD
            HUDController.Instance.UpdateUpgradeHUDCost();
            HUDController.Instance.UpdateCurrentCoins();
            HUDController.Instance.UpdateCurrentAmmo();
            // Enable button
            UpdateBuyButtons();
        }
    }
    // Desactive walls and allows player pass throw ammmo table
    public void BuyAmmoTable()
    {
        if(CanBuyAmmoTable()) 
        {
            m_AmmoTablePS.SetActive(true);
            SoundController.Instance.PlayOneShootAudio("event:/UI/ClickBuy",m_PlayerBlackboard.m_PlayerCamera.transform);
            // Substract coins
            m_PlayerBlackboard.SubstractCoins(m_CostToBuyAmmoTable);
            // Active table
            SetActiveAmmoTable(false);
            // Update HUD
            HUDController.Instance.UpdateUpgradeHUDCost();
            HUDController.Instance.UpdateCurrentCoins();
            // Enable button
            HUDController.Instance.EnableMiniTableAmmoButton(false);
        }
    }
    // Desactive walls an allow palyer acces to health table
    public void BuyHealthTable()
    {
        if(CanBuyHealthTable())
        {
            m_HealthTablePS.SetActive(true);
            SoundController.Instance.PlayOneShootAudio("event:/UI/ClickBuy",m_PlayerBlackboard.m_PlayerCamera.transform);
            // Substract coins
            m_PlayerBlackboard.SubstractCoins(m_CostToBuyHealthTable);
            // Active table
            SetActiveHealthTable(false);
            // Update HUD
            HUDController.Instance.UpdateUpgradeHUDCost();
            HUDController.Instance.UpdateCurrentCoins();
            // Enable button
            HUDController.Instance.EnableMiniTableHealthButton(false);
        } 
    }
    // Desactive walls and allows player pass throw buff table
    public void BuyBuffTable()
    {
        if(CanBuyBuffTable()) 
        {
            m_BuffTablePS.SetActive(true);
            SoundController.Instance.PlayOneShootAudio("event:/UI/ClickBuy",m_PlayerBlackboard.m_PlayerCamera.transform);
            //Substract coins
            m_PlayerBlackboard.SubstractCoins(m_CostToBuyBuffTable);
            // Active table
            SetActiveBuffTalbe(false);
            // Update HUD
            HUDController.Instance.UpdateUpgradeHUDCost();
            HUDController.Instance.UpdateCurrentCoins();
            // Enable button
            HUDController.Instance.EnableMiniTableBuffButton(false);
        }
    }
    // Return true if player can buy ammo table otherwise false
    private bool CanBuyAmmoTable() { return m_PlayerBlackboard.m_CurrentCoins > m_CostToBuyAmmoTable; }
    // Return true if palyer can buy health table otherwise false
    private bool CanBuyHealthTable(){ return m_PlayerBlackboard.m_CurrentCoins > m_CostToBuyHealthTable; }
    // Return true if player can buy buff table otherwise false
    private bool CanBuyBuffTable() { return m_PlayerBlackboard.m_CurrentCoins > m_CostToBuyBuffTable; }
    public void SetActiveAmmoTable(bool active){ m_AmmoTableGameObject.SetActive(active); }
    public void SetActiveHealthTable(bool active){ m_HealthTableGameObject.SetActive(active); }
    public void SetActiveBuffTalbe(bool active){ m_BuffTableGameObject.SetActive(active); }

    // Return true if player can buy upgrade otherwise false
    private bool CanUpgradeNerf(){return m_PlayerBlackboard.m_CurrentCoins >= m_PlayerBlackboard.m_NerfSO.m_WeaponLevel[m_PlayerBlackboard.m_NerfCurrentLevel].m_UpgradeCost;}
    private bool CanUpgradeCroco(){return m_PlayerBlackboard.m_CurrentCoins >= m_PlayerBlackboard.m_CrocoSO.m_WeaponLevel[m_PlayerBlackboard.m_CrocoCurrentLevel].m_UpgradeCost;}
    private bool CanUpgradePunch(){return m_PlayerBlackboard.m_CurrentCoins >= m_PlayerBlackboard.m_PunchSO.m_WeaponLevel[m_PlayerBlackboard.m_PunchCurrentLevel].m_UpgradeCost;}
    private void UpdateBuyButtons()
    {
        if(m_AmmoTableGameObject.activeSelf)
            HUDController.Instance.EnableMiniTableHealthButton(CanBuyHealthTable());
        if(m_HealthTableGameObject.activeSelf)
            HUDController.Instance.EnableMiniTableAmmoButton(CanBuyAmmoTable());
        if(m_BuffTableGameObject.activeSelf)
            HUDController.Instance.EnableMiniTableBuffButton(CanBuyBuffTable());
        HUDController.Instance.EnableCrocoButton(CanUpgradeCroco());
        HUDController.Instance.EnableNerfButton(CanUpgradeNerf());
        HUDController.Instance.EnablePunchButton(CanUpgradePunch());
        HUDController.Instance.SelectUpgradeButton();
    }
    // Active all restartable game elements
    private void RestartGameElements()
    {
        foreach(var element in m_RestartGameElements)
        {
            element.RestartGameElement();
        }
    }
    // Active all elements in certain game state
    private void ActiveGameStateElements()
    {
        foreach(var element in m_ActiveDuringGameStateElements)
        {
            element.ActiveGameElement();
        }
    }
}
