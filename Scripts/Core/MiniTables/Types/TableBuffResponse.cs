/* 
 * Oscar Forra Carbonell
*/
using UnityEngine;

public class TableBuffResponse : MonoBehaviour, ITableResponse
{
    
    public float m_TimeBetweenAdd = 2;                          // Time between add buff
    private float m_ElapsedTime;                                // Elapsed time since start game
    private void Update() {
        // Compute elapsed Time
        m_ElapsedTime += Time.deltaTime;
    }
    public void OnEnter()
    {
        HUDController.Instance.SetBuffTableHUD(true);
        // Add buff to player controller
        GameController.Instance.m_PlayerBlackboard.AddBuff();
        // Set elapsed time to 0
        m_ElapsedTime = 0;
        HUDController.Instance.m_BuffTableSprite.fillAmount = 1;
    }
    public void OnStay()
    {
        // If elapsedTime is higher than time between add and Buff isn't active
        if(m_ElapsedTime > m_TimeBetweenAdd && !GameController.Instance.m_PlayerBlackboard.BuffActive()){
            // Add buff to player controller
            GameController.Instance.m_PlayerBlackboard.AddBuff();
            // Set elapsed time to 0
            m_ElapsedTime = 0;
        }
            
    }

    public void OnExit()
    {
        
    }
}
