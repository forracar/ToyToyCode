/* 
 * Oscar Forra Carbonell
*/
using UnityEngine;
public class ThirdPersonController : MonoBehaviour
{
    [Header("Rotation parameters")]
    public float m_GravityMultiplier;                                   // Used to add more force to gravity
    public float m_PitchRotationalSpeed;                                // Rotation speed in X axis
    public float m_YawRotationalSpeed;                                  // Rotation speed in Y axis
    public float m_PitchAimRotationalSpeed;                             // Rotation speed in X axis ( aim state )
    public float m_YawAimRotationalSpeed;                               // Rotation speed in Y axis ( aim state )
    [Header("Assist parameters")]
    public float m_YawRotationalAssistSpeed;                            // Rotation speed in Y that will be set it when player is aiming an enemy
    public float m_PitchRotationalAssistSpeed;                          // Rotation speed in X that will be set it when player is aiming an enemy
    public int m_AimAssistDistance;                                     // Distance that enemy have to be to allaw aim assist
    public float m_MinPitch;                                            // Mix X angle
    public float m_MaxPitch;                                            // Max X angle
    public LayerMask m_RaycastAssistLayerMask;                          // Layer mask for aim assist
    [Header("Invert joystick")]
    public bool m_InvertYaw;                                            // Boolean to invert joystick input
    public bool m_InvertPitch;                                          // Boolean to invert joystick input
    [Header("Controller components")]
    public Transform m_PitchControllerTransform;                        // Camera parent transform
    public Transform m_PlayerCameraTransform;                           // Camera transform
    public Transform m_CameraAiming;                                    // Camera aiming position
    public LayerMask m_RaycastLayerMask;                                // Layer mask for camera collision
    public float m_OffsetOnCollision;                                   // OffsetCollision for camera collision with envoirment
    private PlayerBlackboard m_PlayerBlackboard;                        // Reference to Player blackboard
    private float m_Pitch;                                              // Current pitch
    private float m_Yaw;                                                // Current yaw
    private bool m_OnGround;                                            // Boolean used to know if it's grounded
    private float m_VerticalSpeed;                                      // Current vertical speed
    private bool m_Aiming;                                              // Boolean used to know if camera is aiming
    private Vector3 m_PlayerCameraStartPosition;                        // Camera start position

    void Start()
    {
        // If player blackboard is null get it
        if (m_PlayerBlackboard == null)
            m_PlayerBlackboard = GetComponent<PlayerBlackboard>();
        else
            Debug.LogError("PlayerBlackboard isn't attached to GameObject");
        // Set camera start position
        m_PlayerCameraStartPosition = m_PlayerCameraTransform.localPosition;
    }
    void LateUpdate()
    {
        if (GameController.Instance.m_CurrentGameState == TGameState.Play)
        {
            // Create Vectors for player movement
            Vector3 l_Movement, l_Forward, l_Right;
            // Create floats for user inputs
            float l_StickLeftHorizontal, l_StickLeftVertical, l_StickRightHorizontal, l_StickRightVertical, l_TriggerLeft;
            // Get controller inputs
            GetJoystickInputsAndTrigger(out l_StickLeftHorizontal, out l_StickLeftVertical, out l_StickRightHorizontal, out l_StickRightVertical, out l_TriggerLeft);
            // Camera position
            CameraAimController(l_TriggerLeft);
            // Body rotation
            BodyOrientation(l_StickRightHorizontal, l_StickRightVertical, out l_Movement, out l_Forward, out l_Right);
            // Player movement and gravity force
            PlayerMovement(l_StickLeftHorizontal, l_StickLeftVertical, l_Right, l_Forward, l_Movement);
        }
    }
    private void GetJoystickInputsAndTrigger(out float l_StickLeftHorizontal,out float l_StickLeftVertical,out float l_StickRightHorizontal, out float l_StickRightVertical, out float l_TriggerLeft)
    {
        l_StickLeftHorizontal = Input.GetAxis("StickLeftHorizontal_WIN");
        l_StickLeftVertical = Input.GetAxis("StickLeftVertical_WIN");
        l_StickRightHorizontal = Input.GetAxis("StickRightHorizontal_WIN");
        l_StickRightVertical = Input.GetAxis("StickRightVertical_WIN");
        l_TriggerLeft = Input.GetAxis("TriggerLeft_WIN");
        // Get the scalar product of Vector3.up of player and the ptich controller forward
        float l_HorizontalDotProduct = Vector3.Dot(m_PlayerBlackboard.m_PlayerTransform.up, m_PitchControllerTransform.forward);
        m_PlayerBlackboard.m_PlayerAnimator.SetFloat("HorizontalAxis_StickRight", l_HorizontalDotProduct);
        // Stick left animator inputs
        m_PlayerBlackboard.m_PlayerAnimator.SetFloat("HorizontalAxis_StickLeft", l_StickLeftHorizontal);
        m_PlayerBlackboard.m_PlayerAnimator.SetFloat("VerticalAxis_StickLeft", l_StickLeftVertical);
        if (l_StickLeftHorizontal == 0 && l_StickLeftVertical == 0)
            m_PlayerBlackboard.m_PlayerAnimator.SetBool("Moving", false);
        else
            m_PlayerBlackboard.m_PlayerAnimator.SetBool("Moving", true);
        
    }
    private void PlayerMovement(float l_StickLeftHorizontal, float l_StickLeftVertical, Vector3 l_Right, Vector3 l_Forward, Vector3 l_Movement)
    {

        // Set right and forward vectors
        l_Right *= l_StickLeftHorizontal;
        l_Forward *= l_StickLeftVertical;
        // Set movement vector
        l_Movement = l_Forward + l_Right;
        l_Movement.Normalize();
        // Change speed depending on direction and camera state
        float l_Speed = m_Aiming ? l_Speed = m_PlayerBlackboard.m_AimMovementSpeed : l_StickLeftVertical < 0 ? l_Speed = m_PlayerBlackboard.m_BackWardsMovementSpeed : l_Speed = m_PlayerBlackboard.m_ForwardMovementSpeed;
        l_Movement = l_Movement * Time.deltaTime * l_Speed;
        // Move
        CollisionFlags l_CollisionFlags = m_PlayerBlackboard.m_CharacterController.Move(l_Movement);
        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime * m_GravityMultiplier;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;
        // Add gravity
        l_CollisionFlags = m_PlayerBlackboard.m_CharacterController.Move(l_Movement);
        // If player collision with feet onground true
        if ((l_CollisionFlags & CollisionFlags.Below) != 0)
        {
            m_VerticalSpeed = 0.0f;
            m_OnGround = true;
            m_PlayerBlackboard.m_PlayerAnimator.SetBool("Jump", false);
            m_PlayerBlackboard.m_PlayerAnimator.SetBool("Falling", false);
        }
        // Else isn't on ground
        else
        {
            m_OnGround = false;
        }
        // If collision with head is true
        if ((l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f)
        {
            m_PlayerBlackboard.m_PlayerAnimator.SetBool("Falling", true);
            m_VerticalSpeed = 0.0f;
        }
        // If is going down falling true
        if(m_VerticalSpeed<0.0f)
        {
            m_PlayerBlackboard.m_PlayerAnimator.SetBool("Falling", true);
        }
        if (m_OnGround && Input.GetAxis("ButtonA_WIN") > 0 && m_PlayerBlackboard.m_CanJump)
        {
            //Jump
            m_VerticalSpeed = m_PlayerBlackboard.m_JumpSpeed;
            m_PlayerBlackboard.m_PlayerAnimator.SetBool("Jump", true);
            // Play sound
            SoundController.Instance.PlayOneShootAudio("event:/Character/Jump", m_PlayerBlackboard.m_PlayerTransform);
        }
    }
    private void BodyOrientation(float l_StickRightHorizontal, float l_StickRightVertical,out Vector3 l_Movement,out Vector3 l_Forward,out Vector3 l_Right)
    {
        l_Movement = Vector3.zero;
        // Invert rotation if is needed
        InvertRotation(m_InvertYaw,ref l_StickRightHorizontal);
        InvertRotation(m_InvertPitch,ref l_StickRightVertical);
        // Change rotational speed depending on camera status
        float l_PitchRotationSpeed = m_Aiming ? l_PitchRotationSpeed = m_PitchAimRotationalSpeed : m_PitchRotationalSpeed;
        float l_YawRotationSpeed = m_Aiming ? l_YawRotationSpeed = m_YawAimRotationalSpeed : m_YawRotationalSpeed;
        // Aim assist
        if(m_Aiming)
        {
            RaycastHit l_RaycastHit;
            Ray l_Ray = new Ray(m_PitchControllerTransform.position, m_PitchControllerTransform.forward);
            Debug.DrawRay(m_PitchControllerTransform.position, m_PitchControllerTransform.forward, Color.green);
            if (Physics.Raycast(l_Ray, out l_RaycastHit, m_AimAssistDistance, m_RaycastAssistLayerMask.value))
            {
                l_PitchRotationSpeed = m_PitchRotationalAssistSpeed;
                l_YawRotationSpeed = m_YawRotationalAssistSpeed;
            }
        }
        // Compute yaw and pitch
        m_Pitch += l_StickRightVertical * l_PitchRotationSpeed * Time.deltaTime;
        m_Yaw += l_StickRightHorizontal * l_YawRotationSpeed * Time.deltaTime;
        m_Pitch = Mathf.Clamp(m_Pitch,m_MinPitch,m_MaxPitch);
        //Rotate
        m_PlayerBlackboard.m_PlayerTransform.rotation = Quaternion.Euler(0.0f,m_Yaw,0.0f);
        m_PitchControllerTransform.localRotation = Quaternion.Euler(m_Pitch,0.0f,0.0f);
        float l_YawInRadians = m_Yaw * Mathf.Deg2Rad;
        float l_Yaw90InRadians = (m_Yaw + 90.0f) * Mathf.Deg2Rad;
        l_Forward = new Vector3(Mathf.Sin(l_YawInRadians),0.0f,Mathf.Cos(l_YawInRadians));
        l_Right = new Vector3(Mathf.Sin(l_Yaw90InRadians),0.0f,Mathf.Cos(l_Yaw90InRadians));
    }
    private void CameraAimController(float l_TriggerLeft)
    {
        Vector3 l_Direction = (m_PlayerCameraTransform.position - m_PlayerBlackboard.m_PlayerTransform.position).normalized;
        float l_Distance = (m_PlayerBlackboard.m_PlayerTransform.position - m_PlayerCameraTransform.position).magnitude;
        // Change camera position and set m_Aiming to true if player aiming
        if(l_TriggerLeft > 0) {
            m_Aiming = true;
            m_PlayerCameraTransform.localPosition = Vector3.MoveTowards(m_PlayerCameraTransform.transform.localPosition,m_CameraAiming.localPosition,0.1f);
        }
        else {
            m_Aiming = false;
            m_PlayerCameraTransform.localPosition = Vector3.MoveTowards(m_PlayerCameraTransform.transform.localPosition,m_PlayerCameraStartPosition,0.1f);
        }
        //RaycastHit l_RaycastHit;
        //Ray l_Ray = new Ray(transform.position + l_PlayerOffset, -l_Direction);
        //Debug.DrawRay(transform.position + l_PlayerOffset,l_Direction,Color.green);
        //if (Physics.Raycast(l_Ray, out l_RaycastHit, l_Distance, m_RaycastLayerMask.value)){
        //    Debug.Log("Collision");
        ///    m_PlayerCameraTransform.localPosition = l_RaycastHit.point;
        //}
    }
    private void InvertRotation(bool invert, ref float input)
    {
        if(invert)
            input = -input;
        return;  
    }
}
