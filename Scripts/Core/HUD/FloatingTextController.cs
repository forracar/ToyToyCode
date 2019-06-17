/* 
Oscar Forra Carbonell
*/
using UnityEngine;

public class FloatingTextController : MonoBehaviour
{
    private static FloatingText m_PopUpTextPrefab;                                                  // Pop up prefab

    private static Canvas m_Canvas;                                                                 // Canvas in scene
    public static void Initialize()
    {
        // Get Canvas in scene
        m_Canvas = GameObject.Find("HUDCanvas").GetComponent<Canvas>();
        // Get pop up prefab
        if(!m_PopUpTextPrefab)
            m_PopUpTextPrefab = GameController.Instance.m_FloatingTextPrefab;
    }
    // Instantaites floating text
    public static void CreateFloatingText(string text, Transform location)
    {
        // Insantaite prefab
        FloatingText m_Instance = Instantiate(m_PopUpTextPrefab);
        // Get position in canvas
        Vector2 l_ScreenPosition = GameController.Instance.m_PlayerBlackboard.m_PlayerCamera.WorldToScreenPoint(location.position);
        // Set parent
        m_Instance.transform.SetParent(m_Canvas.transform);
        // Set position
        m_Instance.transform.position = l_ScreenPosition;
        // Set text
        m_Instance.SetText(text,location);
    }
}
