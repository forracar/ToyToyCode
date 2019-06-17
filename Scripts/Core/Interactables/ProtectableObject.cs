/* 
Oscar Forra Carbonell
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectableObject : MonoBehaviour, IDamageable
{
   
    
    public int m_EasyHealth;                            // HP when difficulty is easy
    public int m_MediumHealth;                          // HP when difficulty is medium
    public int m_HardHealth;                            // HP when difficulty is harrd
    private int m_CurrentHealth;                         // Current HP
    private int id;                                      // ID used to link it to HPBar in HUD
    public bool m_EnabledInGame = false;                // Bool used to know if target can search for it
    public void Initialize(int id)
    {
        //Play sound
        SoundController.Instance.PlayOneShootAudio("event:/GamePlay/AlarmClock",this.transform);
        m_EnabledInGame = true;
        // Set ID
        this.id = id;
        // Set current health
        switch(GameController.Instance.m_CurrentDifficulty)
        {
            case TDifficulty.Easy:
            m_CurrentHealth = m_EasyHealth;
            break;
            case TDifficulty.Normal:
            m_CurrentHealth = m_MediumHealth;
            break;
            case TDifficulty.Hard:
            m_CurrentHealth = m_HardHealth;
            break;
        }
        // Set HPBar
        HUDController.Instance.SetActiveObjectiveBar(id,true,true);
    }
    public void DealDamage(int damage)
    {
        // Update health
        m_CurrentHealth -= damage;
        // Update HUD Image
        switch(GameController.Instance.m_CurrentDifficulty)
        {
            case TDifficulty.Easy:
             HUDController.Instance.UpdateObjectiveBarHP(id,m_CurrentHealth,m_EasyHealth);
            break;
            case TDifficulty.Normal:
             HUDController.Instance.UpdateObjectiveBarHP(id,m_CurrentHealth,m_MediumHealth);
            break;
            case TDifficulty.Hard:
             HUDController.Instance.UpdateObjectiveBarHP(id,m_CurrentHealth,m_HardHealth);
            break;
        }
        if(m_CurrentHealth <= 0)
            Die();
    }
    public void Die()
    {
        // Set false HUD Image
        HUDController.Instance.SetActiveObjectiveBar(id,false,true);
        m_EnabledInGame = false;
        GameModeController.Instance.m_ProtectableObjectsLeft--;
        gameObject.SetActive(false);
    }
}
