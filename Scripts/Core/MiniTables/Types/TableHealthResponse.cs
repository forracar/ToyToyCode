/* 
 * Oscar Forra Carbonell
*/
using UnityEngine;

public class TableHealthResponse : MonoBehaviour,ITableResponse
{
   
    public int m_HealthToAdd = 5;                                   // Health that will be added when player collides
    public float m_TimeBetweenAdd = 2;                              // Time between each heal
    private float m_ElapsedTime;                                    // Elapsed Time since game start
    private void Update() 
    {
        // Cumpute elapsed time
        m_ElapsedTime += Time.deltaTime;
    }
    // This function is called in OnTriggerEnter in TableController
    public void OnEnter()
    {
        HUDController.Instance.SetHealthTableHUD(true);
        // Add health to player
        GameController.Instance.m_PlayerBlackboard.AddHealth(m_HealthToAdd);
        GameController.Instance.m_PlayerBlackboard.m_AddingHp = true;
        // Set elapsed time to 0
        m_ElapsedTime = 0;
    }
    // This function is called in OnTriggerStay in TableController
    public void OnStay()
    {
        // If elapsed time is greater than time between each heal
        if(m_ElapsedTime > m_TimeBetweenAdd)
        {
            // Heal player and set elapsed time to 0
            GameController.Instance.m_PlayerBlackboard.AddHealth(m_HealthToAdd);
            m_ElapsedTime = 0;
        }  
    }
    // Function called when player exits of minitable
    public void OnExit()
    {
        HUDController.Instance.SetHealthTableHUD(false);
        // Set to false that player is in minitable
        GameController.Instance.m_PlayerBlackboard.m_AddingHp = false;
    }
}
