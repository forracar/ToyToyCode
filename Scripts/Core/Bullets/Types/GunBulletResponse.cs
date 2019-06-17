/* 
 * Oscar Forra Carbonell
*/
using UnityEngine;
public class GunBulletResponse : MonoBehaviour, IBulletResponse
{
    public enum TOwner {Player, Enemy};                                     // Enum used to know whitch owner is instantiating the bullet
    public TOwner m_Owner;                                                  // Current owner
    public bool m_Active;
    private int m_Damage;                                                   // Damage that bullet will do
    private float m_Force;                                                  // Speed that rb will move
    private Rigidbody m_RigidBody;                                          // Rigidbody attached to bullet
    private Vector3 m_Direction;                                            // Direction that bullet will move
    private Vector3 m_StartPosition;
    private Transform m_BulletTransform;
    public LayerMask m_LayerMask;
    public Vector3 m_DecalPosition;
    public Vector3 m_DecalNormal;
    private Vector3 m_TransportPosition;
    public void OnAwake()
    {
        m_BulletTransform = this.transform;
        m_Active = false;
        m_StartPosition = transform.position;
        m_RigidBody = GetComponent<Rigidbody>();
    }
    public void OnInit(int l_Damage, float l_Speed, Vector3 l_Direction,Vector3 l_Position)
    {
        // Set damage, speed and direction depending on player weapon. This function is called when the bullet is instantiated in PlayerWeaponController
        m_Damage = l_Damage;
        m_Force = l_Speed;
        m_Direction = l_Direction;
        m_BulletTransform.position = l_Position;
        m_TransportPosition = l_Position;
        m_Active = true;
        DecalPosition();
    }
    private void Update() {
        //if(m_Active)
            //DecalPosition();
    }
    public void DecalPosition()
    {
        RaycastHit l_RaycastHit;
        Ray l_Ray = new Ray(m_TransportPosition, m_Direction);
        //Debug.DrawRay(transform.position + l_PlayerOffset,l_Direction,Color.green);
        if (Physics.Raycast(l_Ray, out l_RaycastHit, Mathf.Infinity, m_LayerMask.value)){
            Debug.Log("Lestgo");
            m_DecalPosition = l_RaycastHit.point;
            m_DecalNormal = l_RaycastHit.normal;
        }
        
    }
    public void OnBulletDestroy()
    {
        SoundController.Instance.PlayOneShootAudio("event:/Nerf/Colision",m_BulletTransform);
        // Destroy bullet GameObject
        m_BulletTransform.position = m_StartPosition;
        m_Active = false;
        //Create Decal
        Decal l_Decal = GameController.Instance.GetDecalNerfInactive();
        l_Decal.Initialize(m_DecalPosition,m_DecalNormal);
    }
    // Called when trigger enters to GO
    public void OnDealDamage(GameObject collider,string tag)
    {
        if (m_Active)
        {
            // If collision tag is player
            if (tag == "Player" && m_Owner == TOwner.Enemy)
            {
                // Deal damage to player and destroy bullet
                GameController.Instance.m_PlayerBlackboard.DealDamage(m_Damage);
                OnBulletDestroy();
            }
            // Deal damagge to protectable object if is enemy
            else if (tag == "ProtectableObject" && m_Owner == TOwner.Enemy)
            {
                IDamageable l_Object = collider.GetComponent<IDamageable>();
                l_Object.DealDamage(m_Damage);
                OnBulletDestroy();
            }
            // Else if tag is enemy
            else if (tag == "Enemy" && m_Owner == TOwner.Player)
            {
                IEnemy l_EnemyDamaged = collider.GetComponent<IEnemy>();
                l_EnemyDamaged.DealDamage(m_Damage);
                OnBulletDestroy();
            }
            else if (tag == "BoxAmmo" && m_Owner == TOwner.Player)
            {
                BoxAmmo l_Box = collider.GetComponent<BoxAmmo>();
                l_Box.DropObject();
                OnBulletDestroy();
            }
            // Deal damage to destroyable object
            else if (tag == "DestroyableObject" && m_Owner == TOwner.Player)
            {
                IDamageable l_Object = collider.GetComponent<IDamageable>();
                l_Object.DealDamage(m_Damage);
                OnBulletDestroy();
            }
            else
            {
                OnBulletDestroy();
            }
        }
        
    }
    public void OnUpdate()
    {
        if(m_Active)
            // Move the rigidbody position
            m_RigidBody.MovePosition(m_BulletTransform.position + (m_Direction).normalized * m_Force * Time.deltaTime);
    }
}
