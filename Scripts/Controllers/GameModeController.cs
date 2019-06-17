/* 
 * Oscar Forra Carbonell
*/
using UnityEngine;
using Utils;
public class GameModeController : Singleton<GameModeController>
{
    public enum TGameMode {Capture,Destroy,Protect,None}                                // Enum used to know whitch game mode is
    public TGameMode m_CurrentGameMode = TGameMode.None;                                // Current game mode
    public float m_TimePerRoundEasy = 120;                                              // Time that round will have
    public float m_TimePerRoundNormal = 240;                                            // Time per round in normal mode
    public float m_TimePerRoundHard = 360;                                              // Time per round in hard mode
    public int m_DestroyableObjectsPerRound = 3;                                        // Number of destroyalbe objets that will appear in a round
    public int m_CapturableObjectsPerRound = 3;                                         // Number of capturable objects that will appear in a round
    public int m_ProtectableObjectsPerRound = 2;                                        // Number of protectable objects that will appear in a round

    private GameObject[] m_DestroyableGameObjects;                                      // Array that contains all destroyable gameObjects
    public GameObject[] m_ProtectableGameObjects;                                      // Array that contains all Protectable gameObjects
    public GameObject[] m_CapturableGameObjectsType1;                                  // Array that contains all Capturable gameObjects of type 1
    public GameObject[] m_CapturableGameObjectsType2;                                  // Array that contains all Capturable gameObjects of type 2
    public GameObject[] m_CapturableGameObjectsType3;                                  // Array that contains all Capturable gameObjects of type 3

    [HideInInspector] public int m_DestroyableObjectsLeft;                              // Current destroyable objets that still on scene
    [HideInInspector] public int m_CapturableObjectsLeft;                               // Current capturable objects that still on scene
    [HideInInspector] public int m_ProtectableObjectsLeft;                              // Current protectable objets that still on scene
    [HideInInspector] public float m_ElapsedTime;                                       // Elapsed time since game start
    [HideInInspector] public float m_CurrentTimeRound;                                  // Current time that round have
    new void Awake() 
    {
        base.Awake();
        // Find all interactables objects
        FindAllInteractableGameObjects();
    }
    void Update() 
    {
        // Compute elapsed time
        m_ElapsedTime += Time.deltaTime;
        // Update HUD
        HUDController.Instance.UpdateRoundCurrentTime();
        // Update depending on game mode
        switch(m_CurrentGameMode)
        {
            case TGameMode.Capture:
                UpdateCaptureState();
                break;
            case TGameMode.Destroy:
                UpdateDestroyState();
                break;
            case TGameMode.Protect:
                UpdateProtectState();
                break;
        }
    }
    // Change time depending on difficulty
    public void SetCurrentTimeRound(TDifficulty difficulty)
    {
        switch(difficulty)
        {
            case TDifficulty.Easy:
                m_CurrentTimeRound = m_TimePerRoundEasy;
                break;
            case TDifficulty.Normal:
                m_CurrentTimeRound = m_TimePerRoundNormal;
                break;
            case TDifficulty.Hard:
                m_CurrentTimeRound = m_TimePerRoundHard;
                break;
        }
    }
    private void ChangeGameModeState(TGameMode l_newGameMode)
    {
        // Desactivate any gameObject that still active
        switch(m_CurrentGameMode)
        {
            case TGameMode.Protect:
                // Desactive HUD bar
                HUDController.Instance.SetActiveAllObjectiveBar(false,m_ProtectableObjectsPerRound);
                ActiveAllInteractableGameObjects(false);
            break;
            case TGameMode.Destroy:
                HUDController.Instance.SetActiveAllObjectiveBar(false,m_DestroyableObjectsPerRound);
                ActiveAllInteractableGameObjects(false);
            break;
            case TGameMode.Capture:
                HUDController.Instance.SetActiveObjectiveText(false);
                HUDController.Instance.DesactiveCollectGoals();
                ActiveAllInteractableGameObjects(false);
            break;
            default:
            // Desactive all interactable objects
        ActiveAllInteractableGameObjects(false);
            break;
        } // End switch
        // Activate corresponding gameObjects
        switch(l_newGameMode)
        {
            case TGameMode.Protect:
                HUDController.Instance.SetActiveAllObjectiveBar(true,m_ProtectableObjectsPerRound);
                SetProtectableGameObjects();
            break;
            case TGameMode.Destroy:
                HUDController.Instance.SetActiveAllObjectiveBar(true,m_DestroyableObjectsPerRound);
                SetDestroyableGameObjects();
            break;
            case TGameMode.Capture:
                SetCapturableGameObjects();
            break;
        }
        //Update HUD
        HUDController.Instance.UpdateGameModeObjective(l_newGameMode);
        HUDController.Instance.PlayAnimationGoal(l_newGameMode);
        // Update current state
        m_CurrentGameMode = l_newGameMode;
        //Set elapsed time
        m_ElapsedTime = 0;
    }
    private void UpdateProtectState()
    {
        // If m_CurrentTimeRound time is over then player wins
        if(m_ElapsedTime > m_CurrentTimeRound)
        {
            HUDController.Instance.PlayVictoryAnimationAndChangeStateToUpgrade();
            ChangeGameModeState(TGameMode.None);
        }
        // otherwise any protectable object is destroyed gameover
        else if (m_ProtectableObjectsLeft == 0)
        {
            // Play Sound
        SoundController.Instance.StopAudioSource();
        SoundController.Instance.PlayOneShootAudio("event:/GamePlay/GameOver", GameController.Instance.m_PlayerBlackboard.m_PlayerCamera.transform);
            // Change game state to game Over
            StartCoroutine(GameController.Instance.ChangeGameStateToGameOver());
            ChangeGameModeState(TGameMode.None);
        }
    }
    private void UpdateCaptureState()
    {
        // If all capturable objects are reached
        if (m_CapturableObjectsLeft <= 0)
        {
            //Win and upgrade
            HUDController.Instance.PlayVictoryAnimationAndChangeStateToUpgrade();
            ChangeGameModeState(TGameMode.None);
        }
        // otherwise gameOver
        else if (m_ElapsedTime > m_CurrentTimeRound)
        {
            // Play Sound
        SoundController.Instance.StopAudioSource();
        SoundController.Instance.PlayOneShootAudio("event:/GamePlay/GameOver", GameController.Instance.m_PlayerBlackboard.m_PlayerCamera.transform);
            StartCoroutine(GameController.Instance.ChangeGameStateToGameOver());
            ChangeGameModeState(TGameMode.None);
        }
    }
    private void UpdateDestroyState()
    {

        // If all destroyable objects are destroyed WIN
        if (m_DestroyableObjectsLeft <= 0)
        {
            //Win and upgrade
            HUDController.Instance.PlayVictoryAnimationAndChangeStateToUpgrade();
            ChangeGameModeState(TGameMode.None);
        }
        // otherwise gameOver
        else if (m_ElapsedTime > m_CurrentTimeRound)
        {
            // Play Sound
        SoundController.Instance.StopAudioSource();
        SoundController.Instance.PlayOneShootAudio("event:/GamePlay/GameOver", GameController.Instance.m_PlayerBlackboard.m_PlayerCamera.transform);
            StartCoroutine(GameController.Instance.ChangeGameStateToGameOver());
            ChangeGameModeState(TGameMode.None);
        }
    }
    // Find all capturable gameobjects in scene
    private void FindCapturableGameObjects()
    {
        m_CapturableGameObjectsType1 = GameObject.FindGameObjectsWithTag("CapturableType1");
        m_CapturableGameObjectsType2 = GameObject.FindGameObjectsWithTag("CapturableType2");
        m_CapturableGameObjectsType3 = GameObject.FindGameObjectsWithTag("CapturableType3");
    }
    // Find all destroyable gameObjects in scene
    private void FindDestroyableGameObjects()
    {
        m_DestroyableGameObjects = GameObject.FindGameObjectsWithTag("DestroyableObject");
    }
    // Find all protectable gameObjects in scene
    private void FindProtectableGameObjects()
    {
        m_ProtectableGameObjects = GameObject.FindGameObjectsWithTag("ProtectableObject");
    }
    // Acitve capturable gameObjects
    private void ActiveCapturableGameObjects(bool active)
    {
        if(active)
        {
            Utilities.RandomizeArrayOfGameObjects(ref m_CapturableGameObjectsType1);
            Utilities.RandomizeArrayOfGameObjects(ref m_CapturableGameObjectsType2);
            Utilities.RandomizeArrayOfGameObjects(ref m_CapturableGameObjectsType3);
            m_CapturableGameObjectsType1[0].SetActive(active);
            m_CapturableGameObjectsType2[0].SetActive(active);
            m_CapturableGameObjectsType3[0].SetActive(active);
            HUDController.Instance.SetActiveObjectiveText(active);
        }else
        {
            HUDController.Instance.SetActiveObjectiveText(active);
            foreach(var capturable in m_CapturableGameObjectsType1){
                capturable.SetActive(active);
            }
            foreach(var capturable in m_CapturableGameObjectsType2){
                capturable.SetActive(active);
            }
            foreach(var capturable in m_CapturableGameObjectsType3){
                capturable.SetActive(active);
            }
        }
        
    }

    // Active destroyable gameObjects
    private void ActiveDestroyableGameObjects(bool active)
    {
        if (active)
        {
            Utilities.RandomizeArrayOfGameObjects(ref m_DestroyableGameObjects);
            for (int i = 0; i < m_DestroyableObjectsPerRound; i++)
            {
                m_DestroyableGameObjects[i].SetActive(active);
                m_DestroyableGameObjects[i].GetComponent<DestroyableObject>().Initialize(i);
            }
        }
        else
        {
            for (int i = 0; i < m_DestroyableGameObjects.Length; i++)
            {
                m_DestroyableGameObjects[i].SetActive(active);
            }
        }
        
    }
    // Active Protectable gameObjects
    private void ActiveProtectableGameObjects(bool active)
    {
        if(active)
        {
            Utilities.RandomizeArrayOfGameObjects(ref m_ProtectableGameObjects);
            for (int i = 0; i < m_ProtectableObjectsPerRound; i++)
            {
                m_ProtectableGameObjects[i].SetActive(active);
                m_ProtectableGameObjects[i].GetComponent<ProtectableObject>().Initialize(i);
            }
        }
        else
        {
            for (int i = 0; i < m_ProtectableGameObjects.Length; i++)
            {
                m_ProtectableGameObjects[i].GetComponent<ProtectableObject>().m_EnabledInGame = false;
                m_ProtectableGameObjects[i].SetActive(active);
            }
        }
        
    }
    // Active or desactive all interactable objects
    private void ActiveAllInteractableGameObjects(bool active)
    {
        ActiveCapturableGameObjects(active);
        ActiveDestroyableGameObjects(active);
        ActiveProtectableGameObjects(active);
    }
    /// Find all interactaable objects
    private void FindAllInteractableGameObjects()
    {
        FindCapturableGameObjects();
        FindDestroyableGameObjects();
        FindProtectableGameObjects();
    }
    // Find and Active protectable objects
    private void SetProtectableGameObjects()
    {
        ActiveProtectableGameObjects(true);
        m_ProtectableObjectsLeft = m_ProtectableObjectsPerRound;
    }
    // Find and Active capturable objects
    private void SetCapturableGameObjects()
    {
        ActiveCapturableGameObjects(true);
        m_CapturableObjectsLeft = m_CapturableObjectsPerRound;
        
    }
    // Find and Active destroyable objects
    private void SetDestroyableGameObjects()
    {
        ActiveDestroyableGameObjects(true);
        m_DestroyableObjectsLeft = m_DestroyableObjectsPerRound;
    }
    // Change game mode to a random one
    public void ChangeGameModeStateRandomly()
    {
        int l_RandomNum = Random.Range(0,3);
        ChangeGameModeState((TGameMode)l_RandomNum);
    }
    // Change game mode to capture
    public void ChangeGameModeStateToCapture()
    {
        ChangeGameModeState(TGameMode.Capture);
    }
    // Change game mode to destroy
    public void ChangeGameModeStateToDestroy()
    {
        ChangeGameModeState(TGameMode.Destroy);
    }
    // Change game mode to protect
    public void ChangeGameModeStateToProtect()
    {
        ChangeGameModeState(TGameMode.Protect);
    }
}
