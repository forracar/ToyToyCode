/* 
Oscar Forra Carbonell
*/
using UnityEngine;

public class AmmoInteractable : MonoBehaviour
{
    public int m_AmmoToAdd;                                             // Ammo that will be added to player when collides
    void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Player" && GameController.Instance.m_PlayerBlackboard.CanAddAmmo())
        {
            SoundController.Instance.PlayOneShootAudio("event:/GamePlay/PickUpAmmo",this.transform);
            // Add ammo and destroy game object
            GameController.Instance.m_PlayerBlackboard.AddAmmo(m_AmmoToAdd);
            Destroy(gameObject);
        }
    }
}
