using UnityEngine;

public class Decal : MonoBehaviour
{
    public Animation m_Animation;
    public AnimationClip m_AnimationFadeInClip;
    public AnimationClip m_AnimationFadeOutClip;
    public int m_TimeToDesactive = 5;
    public bool m_Active;
    private Vector3 m_StartPosition;
    private Quaternion m_StartRotation;
    private float m_ElapsedTime;
    // Start is called before the first frame update
    void Start()
    {
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Active)
        {
            m_ElapsedTime += Time.deltaTime;
            if (m_ElapsedTime > m_TimeToDesactive)
                Desactive();
        }
        
    }
    public void Initialize(Vector3 _position,Vector3 _normal)
    {
        m_Active= true;
        transform.position = _position;
        transform.rotation = Quaternion.LookRotation(_normal);
        m_Animation.clip = m_AnimationFadeOutClip;
        m_Animation.Play();
    }
    void Desactive()
    {
        m_Animation.clip = m_AnimationFadeInClip;
        m_Animation.Play();
        m_ElapsedTime = 0;
        m_Active = false;
        transform.position = m_StartPosition;
        
    }
}
