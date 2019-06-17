using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public Animator m_Animator;                         // Attached animator
    private Text m_DamageText;                          // Text component attached to animator
    // Start is called before the first frame update
    void Start()
    {
        // Get clipinfo 0
        AnimatorClipInfo[] l_ClipInfo = m_Animator.GetCurrentAnimatorClipInfo(0);
        // Destroy game object with delay of X
        Destroy(gameObject,l_ClipInfo[0].clip.length);
        // Get floating text text
        m_DamageText = m_Animator.GetComponent<Text>();
    }

    public void SetText(string text, Transform target)
    {
        // Set floating text text
        m_Animator.GetComponent<Text>().text = text;
    }
}
