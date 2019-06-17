/* 
Oscar Forra Carbonell
*/
using UnityEngine;

public class CollectableObject : MonoBehaviour
{
    public TObjectCollected m_ObjectType = TObjectCollected.None;                                               // Object type
    public int m_TimeToAdd;
    private void OnTriggerEnter(Collider other)
    {
        // If player collides and can collect a object
        if (other.tag == "Player")
        {
            
            SoundController.Instance.PlayOneShootAudio("event:/UI/pickUp",this.transform);
            // Set current object type
            GameController.Instance.m_PlayerBlackboard.m_CurrentColelctableGameObjectType = m_ObjectType;
            GameModeController.Instance.m_CurrentTimeRound += m_TimeToAdd;
            // Depending on object type set different position
            switch(m_ObjectType)
            {
                case TObjectCollected.Lego:
                    HUDController.Instance.SetLegoHeadSprite(true);
                    break;
                case TObjectCollected.Crown:
                    HUDController.Instance.SetCrownGoalSprite(true);
                    break;
                case TObjectCollected.AidKit:
                    HUDController.Instance.SetAidKitSprite(true);
                    break;
            }
            if (GameModeController.Instance)
            {
                // Set currrent capturalbe object left -1
                GameModeController.Instance.m_CapturableObjectsLeft--;
                // Update HUD
                HUDController.Instance.UpdateCurrentCollectedObjects(((GameModeController.Instance.m_CapturableObjectsLeft)).ToString());
            }
            // Desactive gameObject
            this.gameObject.SetActive(false);
        }
    }
}
