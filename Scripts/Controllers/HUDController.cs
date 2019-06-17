/* 
 * Oscar Forra Carbonell
*/
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Utils;

public class HUDController : Singleton<HUDController>
{
    public EventSystem m_EventSystem;                                                       // Event System in the current scene
    public StandaloneInputModule m_StandaloneInputModule;                                   // StandaloneInputModule in current scene
    public GameObject m_FirstSelectionInUpgrade;                                            // GameObject that will be selected when upgrade menu appears
    public GameObject m_FirstSelectionInPause;                                              // GameObject  that will be selected when pause menu appears
    public GameObject m_FirstSelectionInGameOver;                                           // GameObject that will be selected when gameover menu appears
    public Image m_PlayerHealthImage;                                                       // Player Image that contains Health
    [Header("Gameplay HUD")]
    public Text m_PlayerHealthText;                                                         // Player health shown in hp bar (int)
    public Text m_RoundTimeText;                                                            // Text that contains the round time
    public Text m_CurrentRoundText;                                                         // Text that shows current round
    public Text m_CurrenttAmmoText;                                                         // Text that shows current ammo
    public Text m_CurrentCoinsText;                                                         // Text where shows the current coins that player have
    [Header("Weapons HUD")]
    public Sprite m_NerfSprite;                                                             // Sprite used when gun is selected
    public Sprite m_CrocoSprite;                                                            // Sprite used when gun is deselected
    public Sprite m_PunchSprite;                                                            // Punch sprite
    public Image m_MidWeaponImage;                                                          // Background Image where posit is seted in nerf
    public Image m_LeftWeaponImage;                                                         // Background Image where posit is seted in croco
    public Image m_RightWeaponImage;                                                        // Background Image where posit is seted in punch
    [Header("Goals HUD")]
    public GameObject m_CaptureGoalImage;                                                   // Image that shows capture  goal
    public GameObject m_DestroyGoalImage;                                                   // Image that shows destroy  goal
    public GameObject m_ProtectGoalImage;                                                   // Image that shows protect goal
    public GameObject m_CrownImage;                                                         // Crown Image
    public GameObject m_CrownImageParent;                                                   // Crown Image parent
    public GameObject m_LegoHeadImage;                                                      //..
    public GameObject m_LegoHeadParent;                                                     //..
    public GameObject m_AidKitImage;                                                        //..
    public GameObject m_AidKitImageParent;                                                  //..
    public GameObject m_ObjectiveDestroyImage;                                              //..
    public GameObject m_ObjectiveAlarmImage;                                                //..
    public Animation m_AnimationDestroyGoal;                                                // Animation attached to destroy goal text
    public Animation m_AnimationProtectGoal;                                                // Animation attached to protect goal text
    public Animation m_AnimationCaptureGoal;                                                // Animation attached to capture goal text
    public AnimationClip m_GoalAnimationClip;                                               // AnimationClip of text goal
    [Header("Fade")]
    public Animation m_Animation;                                                           // Panel animation
    public AnimationClip m_FadeInAnimationClip;                                             // Animation clip of fade in
    public AnimationClip m_FadeOutAnimationClip;                                            // Animation clip of fade out
    public Image[] m_ObjectiveBarsHP;                                                       // Images of objective bars
    public Text m_CurrentCollectedObjects;                                                  // Text that shows the collected objects left
    [Header("Upgrade HUD Components")]
    public Text m_NerfCostText;                                                             // Text that shows nerf cost
    public Text m_CrocoCostText;                                                            // Text that swhos croco cost
    public Text m_PunchCostText;                                                            // Text that shows punch cost
    public Text m_AmmoMiniTableText;                                                        // Ammo mini table tex
    public Text m_BuffMiniTableText;                                                        //...
    public Text m_HealthMiniTableText;                                                      //...
    public Button m_NerfUpgradeButton;                                                      // Upgrade button Nerf
    public Button m_CrocoUpgradeButton;                                                     // Upgrade button croco
    public Button m_PunchUpgradeButton;                                                     // Upgrade button punch
    public Button m_AmmoBuyButton;                                                          // Mini table ammo
    public Button m_HealthBuyButton;                                                        // Mini table health
    public Button m_BuffBuyButton;                                                          // Mini table buff
    public Text m_CoinsText;                                                                // Current coins text
    public GameObject m_AdviceBloquedPathText;                                              // Advice bloqued text
    public GameObject m_Victory;                                                            // Victory gameObject (Image acitaavated when player wins)
    public Animation m_VictoryAnim;                                                         // Victory gameObject animation
    public AnimationClip m_VictoryAnimClip;                                                 // Victory animation clip
    public GameObject m_NerfPuntero;                                                        // Nerf puntero  
    public GameObject m_CrocoPuntero;                                                       // Croco puntero
    public GameObject m_PunchPuntero;                                                       // Punch puntero
    public GameObject m_AmmoTableHUD;                                                       // Ammo table that shows when is in mini table
    public GameObject m_BuffTableHUD;                                                       //...
    public GameObject m_HealthTableHUD;                                                     //...
    public Image m_BuffTableSprite;
    private new void Awake() 
    {
         base.Awake();
    }
    void Start() 
    {
        // Play fade out anim
        SetFadeOutAnimation();
    }
    public void SetActiveAdivceText(bool active)
    {
        m_AdviceBloquedPathText.SetActive(active);
    }
    // Plays victory animation and play sound
    public void PlayVictoryAnimationAndChangeStateToUpgrade()
    {
        // Play sound
        SoundController.Instance.StopAudioSource();
        SoundController.Instance.PlayOneShootAudio("event:/GamePlay/Win",GameController.Instance.m_PlayerBlackboard.m_PlayerCamera.transform);
        // Desactive HUD
        DesactiveObjectiveImagesFalse();
        RoundController.Instance.DestroyEnemiesAlive();
        //Active victory animation
        m_Victory.SetActive(true);
        StartCoroutine(DesactiveVictoryText());
    }
    // Desactive victory text
    IEnumerator DesactiveVictoryText()
    {
        yield return new WaitForSeconds(m_VictoryAnimClip.length);
        m_Victory.SetActive(false);
        GameController.Instance.ChangeGameStateToUpgrade();
    }
    // Desactive objective images
    public void DesactiveObjectiveImagesFalse()
    {
        m_ObjectiveAlarmImage.SetActive(false);
        m_ObjectiveDestroyImage.SetActive(false);
    }
    // Desactive collected goals (Crown,lego....)
    public void DesactiveCollectGoals()
    {
        HUDController.Instance.SetLegoHeadSprite(false);
        HUDController.Instance.SetCrownGoalSprite(false);
        HUDController.Instance.SetAidKitSprite(false);
    }
    // Set crown goal active or not
    public void SetCrownGoalSprite(bool active)
    {
        m_CrownImageParent.SetActive(active);
        m_CrownImage.SetActive(active);
    }
    // Set aid kit goal active or not
    public void SetAidKitSprite(bool active)
    {
        m_AidKitImageParent.SetActive(active);
        m_AidKitImage.SetActive(active);
    }
    // Set legohead goal active or not
    public void SetLegoHeadSprite(bool active)
    {
        m_LegoHeadParent.SetActive(active);
        m_LegoHeadImage.SetActive(active);
    }
    // Select the upgrade button
    public void SelectUpgradeButton()
    {
        m_EventSystem.SetSelectedGameObject(m_FirstSelectionInUpgrade);
    }
    // Active sprite in hud when player is in ammo table
    public void SetAmmoTableHUD(bool active)
    {
        m_AmmoTableHUD.SetActive(active);
    }
    // Active sprite in hud when player is in buff table
    public void SetBuffTableHUD(bool active)
    {
        m_BuffTableHUD.SetActive(active);
    }
    // Active sprite in hud when player is in health table
    public void SetHealthTableHUD(bool active)
    {
        m_HealthTableHUD.SetActive(active);
    }
    // Select the first selection in pause panel
    public void SelectPauseButton()
    {
        m_EventSystem.SetSelectedGameObject(m_FirstSelectionInPause);
    }
    // Select the first gameOver selection in panel
    public void SelectGameOverButton()
    {
        m_EventSystem.SetSelectedGameObject(m_FirstSelectionInGameOver);
    }
    // Update player health in HUD
    public void UpdatePlayerHealthFillAmount()
    {
        if(GameController.Instance)
            m_PlayerHealthImage.fillAmount = (float)GameController.Instance.m_PlayerBlackboard.m_CurrentHealth / GameController.Instance.m_PlayerBlackboard.m_MaxHealth;
        m_PlayerHealthText.text = GameController.Instance.m_PlayerBlackboard.m_CurrentHealth.ToString();
    }
    // Update player round time in HUD
    public void UpdateRoundCurrentTime()
    {
        if(GameModeController.Instance)
            m_RoundTimeText.text =  ((int)(GameModeController.Instance.m_CurrentTimeRound - GameModeController.Instance.m_ElapsedTime)).ToString();
    }
    public void UpdateCurrentRound()
    {
        if(GameController.Instance)
            m_CurrentRoundText.text = GameController.Instance.m_CurrentRound.ToString();
    }
    // Update player coins
    public void UpdateCurrentCoins()
    {
        if(GameController.Instance)
            m_CurrentCoinsText.text = GameController.Instance.m_PlayerBlackboard.m_CurrentCoins.ToString();
    }
    // Update ammo
    public void UpdateCurrentAmmo()
    {
        switch(GameController.Instance.m_PlayerBlackboard.m_CurrentWeapon)
        {
            case (int)TWeaponTypes.Nerf:
                m_CurrenttAmmoText.text = GameController.Instance.m_PlayerBlackboard.m_NerfCurrentAmmo.ToString();
                break;
            case (int)TWeaponTypes.Croco:
                m_CurrenttAmmoText.text = GameController.Instance.m_PlayerBlackboard.m_CrocoCurrentAmmo.ToString();
                break;
            case (int)TWeaponTypes.Punch:
                m_CurrenttAmmoText.text = "Infinite";
                break;
        }// End switch
    }
    // Function called on player blackboard ChangeWeapon
    public void UpdateSelectedWeapon(TWeaponTypes weapon,bool last)
    {
        // Set the posti selected sprite to new weapon
        switch(weapon)
        {
            case TWeaponTypes.Nerf:
                m_NerfPuntero.SetActive(true);
                m_PunchPuntero.SetActive(false);
                m_CrocoPuntero.SetActive(false);
                m_MidWeaponImage.sprite = m_NerfSprite;
                if (!last)
                {
                    m_LeftWeaponImage.sprite = m_PunchSprite;
                    m_RightWeaponImage.sprite = m_CrocoSprite;
                }
                else
                {
                    m_RightWeaponImage.sprite = m_PunchSprite;
                    m_LeftWeaponImage.sprite = m_CrocoSprite;
                }
                break;
            case TWeaponTypes.Croco:
                m_CrocoPuntero.SetActive(true);
                m_PunchPuntero.SetActive(false);
                m_NerfPuntero.SetActive(false);
                m_MidWeaponImage.sprite = m_CrocoSprite;
                if (!last)
                {
                    m_LeftWeaponImage.sprite = m_NerfSprite;
                    m_RightWeaponImage.sprite = m_PunchSprite;
                }
                else
                {
                    m_RightWeaponImage.sprite = m_PunchSprite;
                    m_LeftWeaponImage.sprite = m_NerfSprite;
                }
                break;
            case TWeaponTypes.Punch:
                m_PunchPuntero.SetActive(true);
                m_NerfPuntero.SetActive(false);
                m_CrocoPuntero.SetActive(false);
                m_MidWeaponImage.sprite = m_PunchSprite;
                if (!last)
                {
                    m_LeftWeaponImage.sprite = m_CrocoSprite;
                    m_RightWeaponImage.sprite = m_NerfSprite;
                }
                else
                {
                    m_RightWeaponImage.sprite = m_NerfSprite;
                    m_LeftWeaponImage.sprite = m_CrocoSprite;
                }
                break;
        }// End switch
        SetWeaponNativeSize();
    }
    // Set native size of weapons HUD
    void SetWeaponNativeSize()
    {
        m_RightWeaponImage.SetNativeSize();
        m_LeftWeaponImage.SetNativeSize();
        m_MidWeaponImage.SetNativeSize();
    }
    // Update gameMode
    public void UpdateGameModeObjective(GameModeController.TGameMode newGameMode)
    {
        // Depend on last game mode desative goal image
        switch(GameModeController.Instance.m_CurrentGameMode)
        {
            case GameModeController.TGameMode.Capture:
                m_CaptureGoalImage.SetActive(false);
                break;
            case GameModeController.TGameMode.Destroy:
                m_DestroyGoalImage.SetActive(false);
                break;
            case GameModeController.TGameMode.Protect:
                m_ProtectGoalImage.SetActive(false);
                break;
        }// End switch
        // Depend on new game mode active goal image
        switch(newGameMode)
        {
            case GameModeController.TGameMode.Capture:
                m_CaptureGoalImage.SetActive(true);
                break;
            case GameModeController.TGameMode.Destroy:
                m_DestroyGoalImage.SetActive(true);
                break;
            case GameModeController.TGameMode.Protect:
                m_ProtectGoalImage.SetActive(true);
                break;
        }// End switch
    }
    // Play text goal animation depending on gameMode
    public void PlayAnimationGoal(GameModeController.TGameMode gameMode)
    {
        switch (gameMode)
        {
            case GameModeController.TGameMode.Capture:
                m_ObjectiveAlarmImage.SetActive(false);
                m_ObjectiveDestroyImage.SetActive(false);
                m_AnimationCaptureGoal.clip = m_GoalAnimationClip;
                m_AnimationCaptureGoal.Play();
                break;
            case GameModeController.TGameMode.Protect:
                m_ObjectiveAlarmImage.SetActive(true);
                m_ObjectiveDestroyImage.SetActive(false);
                m_AnimationProtectGoal.clip = m_GoalAnimationClip;
                m_AnimationProtectGoal.Play();
                break;
            case GameModeController.TGameMode.Destroy:
                m_ObjectiveDestroyImage.SetActive(true);
                m_ObjectiveAlarmImage.SetActive(false);
                m_AnimationDestroyGoal.clip = m_GoalAnimationClip;
                m_AnimationDestroyGoal.Play();
                break;
        }
    }
    // Set active or disctive the text that indicates the collected objects left
    public void SetActiveObjectiveText(bool active)
    {
        m_CurrentCollectedObjects.gameObject.SetActive(active);
        if(active)
            m_CurrentCollectedObjects.text = "3 Left";
    }
    // Update the current collected objects left
    public void UpdateCurrentCollectedObjects(string text)
    {
        m_CurrentCollectedObjects.text = text + " Left";
    }
    // Set active or disactive one HPBar of objectives
    public void SetActiveObjectiveBar(int id, bool active, bool protectable)
    {
        // If disactve the bar set fill amount to 1
        GameObject l_child = m_ObjectiveBarsHP[id].gameObject.transform.GetChild(0).gameObject;
        if(protectable)
            l_child.GetComponent<Image>().color = Color.blue;
        else
            l_child.GetComponent<Image>().color = Color.red;
    }
    // Set active or disactive all HPBar of objectives
    public void SetActiveAllObjectiveBar(bool active, int activeBars)
    {
        foreach(var a in m_ObjectiveBarsHP)
        {
            a.gameObject.transform.GetChild(0).GetComponent<Image>().fillAmount = 1;
        }
        for(int i = 0; i< activeBars; i++)
        {
            m_ObjectiveBarsHP[i].gameObject.SetActive(active);
        }
    }
    // Update the current HP of objective
    public void UpdateObjectiveBarHP(int id, int currentHP,int MaxHP)
    {
        GameObject l_child = m_ObjectiveBarsHP[id].gameObject.transform.GetChild(0).gameObject;
        l_child.GetComponent<Image>().fillAmount = (float)currentHP/MaxHP;
    }
    // Plays fadeout animation
    public void SetFadeOutAnimation()
    {
        m_Animation.clip = m_FadeOutAnimationClip;
        m_Animation.Play();
    }
    // Plays fadein animation
    public void SetFadeInAnimation()
    {
        m_Animation.clip = m_FadeInAnimationClip;
        m_Animation.Play();
    }
    // Update
    public void UpdateUpgradeHUDCost()
    {
        SetNerfCost();
        SetCrocoCost();
        SetPunchCost();
        SetAmmoMiniTableCost();
        SetBuffMiniTableCost();
        SetHealthMiniTableCost();
        UpgradeCoinsInUpgrade();
        SelectUpgradeButton();
    }
    // Update coins when player is in upgrade state
    public void UpgradeCoinsInUpgrade() { m_CoinsText.text = GameController.Instance.m_PlayerBlackboard.m_CurrentCoins.ToString(); }
    // Update nerf cost
    public void SetNerfCost(){ m_NerfCostText.text =  GameController.Instance.m_PlayerBlackboard.GetNerfUpgradeCost().ToString(); UpgradeCoinsInUpgrade();}
    // Update croco cost
    public void SetCrocoCost(){ m_CrocoCostText.text = GameController.Instance.m_PlayerBlackboard.GetCrocoUpgradeCost().ToString();UpgradeCoinsInUpgrade();}
    // Update punch cost
    public void SetPunchCost(){ m_PunchCostText.text = GameController.Instance.m_PlayerBlackboard.GetPunchUpgradeCost().ToString();UpgradeCoinsInUpgrade();}
    // Update ammo table cost
    public void SetAmmoMiniTableCost(){ m_AmmoMiniTableText.text = GameController.Instance.m_CostToBuyAmmoTable.ToString();UpgradeCoinsInUpgrade();}
    // Update buff table cost
    public void SetBuffMiniTableCost(){ m_BuffMiniTableText.text = GameController.Instance.m_CostToBuyBuffTable.ToString();UpgradeCoinsInUpgrade();}
    // Update health table cost
    public void SetHealthMiniTableCost(){ m_HealthMiniTableText.text = GameController.Instance.m_CostToBuyHealthTable.ToString();UpgradeCoinsInUpgrade();}
    public void EnableNerfButton(bool active)
    {
        m_NerfUpgradeButton.interactable = active;
    }
    public void EnablePunchButton(bool active)
    {
        m_PunchUpgradeButton.interactable = active;
    }
    public void EnableCrocoButton(bool active)
    {
        m_CrocoUpgradeButton.interactable = active;
    }
    public void EnableMiniTableAmmoButton(bool active)
    {
        m_AmmoBuyButton.interactable = active;
    }
    public void EnableMiniTableBuffButton(bool active)
    {
        m_BuffBuyButton.interactable = active;
    }
    public void EnableMiniTableHealthButton(bool active)
    {
        m_HealthBuyButton.interactable = active;
    }
}
