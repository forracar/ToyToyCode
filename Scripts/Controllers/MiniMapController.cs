/* 
Oscar Forra Carbonell 
*/

using UnityEngine;
using UnityEngine.UI;

public class MiniMapController : MonoBehaviour
{
    public RawImage m_RawImage;                                             // Raw Image that shows the renderTexture
    public RenderTexture[] m_RenderTextures;                                // All render textures in the scene
    public float m_TimeToChangeImage = 5f;                                  // Time betweeen renderTexture changes on rawImage
    public float m_ElapsedTime;                                             // Elapsed time since game start
    private void Start() {
        // Set random render texture to raw image
        SetRandomRenderTexture();
    }
    void Update() 
    {
        // Compute elapsed time
        m_ElapsedTime += Time.deltaTime;
        // If elapsed time is higher than time to change
        if(m_ElapsedTime > m_TimeToChangeImage)
        {
            // Set random render texture to raw image
            SetRandomRenderTexture();
        }
    }
    void SetRandomRenderTexture()
    {
        // Get random ID
        int l_RandomID = Random.Range(0,m_RenderTextures.Length);
        // Set random renderTexture
        m_RawImage.texture = m_RenderTextures[l_RandomID];
        // Set elapsed time to 0
        m_ElapsedTime = 0;
    }
}
