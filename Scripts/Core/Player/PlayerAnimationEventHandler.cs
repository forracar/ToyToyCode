using UnityEngine;
/* 
 * Oscar Forra Carbonell
*/
public class PlayerAnimationEventHandler : MonoBehaviour
{
    public ParticleSystem m_StepPS;
    public void StepEvent()
    {
        m_StepPS.Play();
        SoundController.Instance.PlayOneShootAudio("event:/Character/playerRun", this.transform);
    }
}
