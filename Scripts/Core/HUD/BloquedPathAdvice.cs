using UnityEngine;

public class BloquedPathAdvice : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player")
        {
            HUDController.Instance.SetActiveAdivceText(true);
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.tag == "Player")
        {
            HUDController.Instance.SetActiveAdivceText(false);
        }
    }
}
