/* 
Oscar Forra Carbonell
*/
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        // If player colldies set player gameOver
        if(other.tag == "Player")
        {
            // Play Sound
            SoundController.Instance.StopAudioSource();
            SoundController.Instance.PlayOneShootAudio("event:/GamePlay/GameOver", GameController.Instance.m_PlayerBlackboard.m_PlayerCamera.transform);
            StartCoroutine(GameController.Instance.ChangeGameStateToGameOver());
        }
    }
}
