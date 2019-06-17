/* 
 * Oscar Forra Carbonell
*/
using UnityEngine;

public class TableAmmoResponse :MonoBehaviour,ITableResponse
{
    public int m_AmmoToAdd = 5;                                     // Ammo that will be added
    public float m_TimeBetweenAdd = 2;                              // Time between ammo added
    private float m_ElapsedTime;                                    // Elapsed time since start
    private void Update() {
        // Compute elapsed Time
        m_ElapsedTime += Time.deltaTime;
    }
    public void OnEnter()
    {
        HUDController.Instance.SetAmmoTableHUD(true);
        // Add ammo to current weapon
        GameController.Instance.m_PlayerBlackboard.AddAmmo(m_AmmoToAdd);
        GameController.Instance.m_PlayerBlackboard.m_AddingAmmo = true;
        // Set elapsed time to 0
        m_ElapsedTime = 0;
    }
    public void OnStay()
    {
        // If elapsed time is higher than time between add
        if(m_ElapsedTime > m_TimeBetweenAdd){
            // Add ammo to current weapon
            GameController.Instance.m_PlayerBlackboard.AddAmmo(m_AmmoToAdd);
            // Set elapsed time to 0
            m_ElapsedTime = 0;
        }
            
    }
    // Function called when player exits of minitable
    public void OnExit()
    {
        HUDController.Instance.SetAmmoTableHUD(false);
        // Set to false that player is in minitable
        GameController.Instance.m_PlayerBlackboard.m_AddingAmmo = false;
    }
}
