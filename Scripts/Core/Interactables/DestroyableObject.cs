/* 
Oscar Forra Carbonell
*/
using UnityEngine;

public class DestroyableObject : MonoBehaviour, IDamageable
{
    public int m_EasyHealth;
    public int m_MediumHealth;
    public int m_HardHealth;
    public int m_CurrentHealth;
    public int m_TimeAddedForDestroy;
    public ParticleSystem m_DestroyParticleSystemPrefab;
    public int id;
    public void Initialize(int id)
    {
        
        // Set id to know what hp bar is
        this.id = id;
        // Set hp depending ond difficulty
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
        // Activate bar
        HUDController.Instance.SetActiveObjectiveBar(id,true,false);
    }
    public void DealDamage(int damage)
    {
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
        Instantiate(m_DestroyParticleSystemPrefab,this.transform.position,Quaternion.identity).Play();
        //Play Audio
        SoundController.Instance.PlayOneShootAudio("event:/GamePlay/DevilBoxDestroy",this.transform);
        // Set false HUD Image
        HUDController.Instance.SetActiveObjectiveBar(id,false,false);
        if(GameModeController.Instance){
            GameModeController.Instance.m_DestroyableObjectsLeft--;
            GameModeController.Instance.m_CurrentTimeRound += m_TimeAddedForDestroy;
        }
        gameObject.SetActive(false);
    }

 
}
