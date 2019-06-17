/* 
Oscar Forra Carbonell
*/
using UnityEngine;

public class CollectableZone : MonoBehaviour
{
    public TObjectCollected m_objectCollectedZone = TObjectCollected.None;                                          // Collectable type zone
    private void OnTriggerEnter(Collider other)
    {
        // If player 
        if (other.tag == "Player")
        {
            // If player have the same type of gameObject
            if (GameController.Instance.m_PlayerBlackboard.m_CurrentColelctableGameObjectType == m_objectCollectedZone)
            {
                SoundController.Instance.PlayOneShootAudio("event:/UI/drop",this.transform);
                switch (m_objectCollectedZone)
                {
                    case TObjectCollected.Lego:
                        HUDController.Instance.SetLegoHeadSprite(false);
                        break;
                    case TObjectCollected.Crown:
                        HUDController.Instance.SetCrownGoalSprite(false);
                        break;
                    case TObjectCollected.AidKit:
                        HUDController.Instance.SetAidKitSprite(false);
                        break;
                }
                // Change current collectable object to none
                GameController.Instance.m_PlayerBlackboard.m_CurrentColelctableGameObjectType = TObjectCollected.None;
                if (GameModeController.Instance)
                {
                    // Set currrent capturalbe object left -1
                    GameModeController.Instance.m_CapturableObjectsLeft--;
                    // Update HUD
                    HUDController.Instance.UpdateCurrentCollectedObjects(((GameModeController.Instance.m_CapturableObjectsLeft)).ToString());
                }
                    
            }
        }
    }
}
