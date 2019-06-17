/* 
 * Oscar Forra Carbonell
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class RoundController : Singleton<RoundController>
{
    public enum TRoundState{Spawning, Waiting, None};                                           // Enum used to know wround state
    public TRoundState m_CurrentRoundState;                                                     // Current round state
    [Header("Scriptable Object of waves")]
    public List<RoundCreator> m_RoundsDestroyEasy;                                              // Rounds type list of destroy mode easy
    public List<RoundCreator> m_RoundsCaptureEasy;                                              // Rounds type list of capture mode easy 
    public List<RoundCreator> m_RoundsProtectEasy;                                              // Rounds type list of protect mode easy
    public List<RoundCreator> m_RoundDestroyNormal;                                             // Round type list of destroy mode normaal
    public List<RoundCreator> m_RoundCaptureNormal;                                             // Round type list of capture mode normal
    public List<RoundCreator> m_RoundProtectNormal;                                             // Round type list of protect mode normal
    public List<RoundCreator> m_RoundsDestroyHard;                                              // Rounds type list of destroy mode hard
    public List<RoundCreator> m_RoundsCaptureHard;                                              // Rounds type list of capture mode hard
    public List<RoundCreator> m_RoundsProtectHard;                                              // Rounds type list of protect mode hard
    public float m_DelayBetweenWaves;                                                           // Delay between waves
    public float m_DelayRespawnProtectMode;                                                     // Delay in protect mode
    [Header("Spawns")]
    public Transform[] m_MedievalSpawnPositions1;                                               // Medieval spawn table 1 positions
    public float m_MedievalDelay1;                                                              // Medieval delay of table 1
    public Transform[] m_MedievalSpawnPositions2;                                               // Medieval spawn table 2 positions
    public float m_MedievalDelay2;                                                              // Medieval delay of table 2
    public Transform[] m_StudentSpanwPositions;                                                 // Stundent spawn positions
    public float m_StudentDelay;                                                                // Stundent delay between spawns
    public Transform[] m_MilitarySpawnPositions;                                                // Military spawn positions
    public float m_MilitaryDelay;                                                               // Military delay
    public Transform[] m_BarbieSpawnPositions;                                                  // Barbie spawn positions
    public float m_BarbieDelay;                                                                 // Barbie delay between spawns
    List<MeleeEnemy> m_MeleeEnemiesAliveList = new List<MeleeEnemy>();                          // List of melee enemies that are in scene
    List<DistanceEnemy> M_DistanceEnemiesAliveList = new List<DistanceEnemy>();                 // List of ranged enemies
    // Start is called before the first frame update
    public void StartRound()
    {
        //Update Round
        GameController.Instance.m_CurrentRound++;
        //Change difficulty if needed
        GameController.Instance.SetCurrentDifficulty();
        //Update current round HUD
        HUDController.Instance.UpdateCurrentRound();
        // Update Round state
        m_CurrentRoundState = TRoundState.Spawning;
        // Start round depending on difficulty
        switch (GameController.Instance.m_CurrentDifficulty)
        {
            case TDifficulty.Easy:
            StartCoroutine(StartEasyWaves());
                break;
            case TDifficulty.Normal:
            StartCoroutine(StartNormalWaves());
                break;
            case TDifficulty.Hard:
            StartCoroutine(StartHardWaves());
                break;
        }
        
    }
    private IEnumerator StartEasyWaves()
    {
        // Get random num to select
        int l_RandomRound;
        // Int used to know the number of waves that will spawn
        int l_NumWaves;
        // Wait a few seconds if GameMode is protect the clocks
        if(GameModeController.Instance.m_CurrentGameMode == GameModeController.TGameMode.Protect)
            yield return new WaitForSeconds(m_DelayRespawnProtectMode);
        switch (GameModeController.Instance.m_CurrentGameMode)
        {
            case GameModeController.TGameMode.Capture:
                // Get random round
                l_RandomRound = Random.Range(0, m_RoundsCaptureEasy.Count - 1);
                l_NumWaves = m_RoundsCaptureEasy[l_RandomRound].m_Waves.Count;
                for (int i = 0; i < l_NumWaves; i++)
                {
                    StartWave(i, m_RoundsCaptureEasy[l_RandomRound].m_Waves[i].m_Enemies);
                    yield return new WaitForSeconds(m_DelayBetweenWaves);
                }
                break;
            case GameModeController.TGameMode.Protect:
                // Get random round
                l_RandomRound = Random.Range(0, m_RoundsProtectEasy.Count - 1);
                l_NumWaves = m_RoundsProtectEasy[l_RandomRound].m_Waves.Count;
                for (int i = 0; i < l_NumWaves; i++)
                {
                    StartWave(i, m_RoundsProtectEasy[l_RandomRound].m_Waves[i].m_Enemies);
                    yield return new WaitForSeconds(m_DelayBetweenWaves);
                }
                break;
            case GameModeController.TGameMode.Destroy:
                // Get random round
                l_RandomRound = Random.Range(0, m_RoundsDestroyEasy.Count - 1);
                l_NumWaves = m_RoundsDestroyEasy[l_RandomRound].m_Waves.Count;
                for (int i = 0; i < l_NumWaves; i++)
                {
                    StartWave(i, m_RoundsDestroyEasy[l_RandomRound].m_Waves[i].m_Enemies);
                    yield return new WaitForSeconds(m_DelayBetweenWaves);
                }
                break;
        }
    }
    private IEnumerator StartNormalWaves()
    {
        // Get random num to select
        int l_RandomRound;
        // Int used to know the number of waves that will spawn
        int l_NumWaves;
        if(GameModeController.Instance.m_CurrentGameMode == GameModeController.TGameMode.Protect)
            yield return new WaitForSeconds(m_DelayRespawnProtectMode);
        switch (GameModeController.Instance.m_CurrentGameMode)
        {
            case GameModeController.TGameMode.Capture:
                l_RandomRound = Random.Range(0, m_RoundCaptureNormal.Count - 1);
                l_NumWaves = m_RoundCaptureNormal[l_RandomRound].m_Waves.Count;
                for (int i = 0; i < l_NumWaves; i++)
                {
                    StartWave(i, m_RoundCaptureNormal[l_RandomRound].m_Waves[i].m_Enemies);
                    yield return new WaitForSeconds(m_DelayBetweenWaves);
                }
                break;
            case GameModeController.TGameMode.Protect:
                l_RandomRound = Random.Range(0, m_RoundProtectNormal.Count - 1);
                l_NumWaves = m_RoundProtectNormal[l_RandomRound].m_Waves.Count;
                for (int i = 0; i < l_NumWaves; i++)
                {
                    StartWave(i, m_RoundProtectNormal[l_RandomRound].m_Waves[i].m_Enemies);
                    yield return new WaitForSeconds(m_DelayBetweenWaves);
                }
                break;
            case GameModeController.TGameMode.Destroy:
                l_RandomRound = Random.Range(0, m_RoundDestroyNormal.Count - 1);
                l_NumWaves = m_RoundDestroyNormal[l_RandomRound].m_Waves.Count;
                for (int i = 0; i < l_NumWaves; i++)
                {
                    StartWave(i, m_RoundDestroyNormal[l_RandomRound].m_Waves[i].m_Enemies);
                    yield return new WaitForSeconds(m_DelayBetweenWaves);
                }
                break;
        }
    }
    private IEnumerator StartHardWaves()
    {
        // Get random num to select
        int l_RandomRound;
        // Int used to know the number of waves that will spawn
        int l_NumWaves;
        if(GameModeController.Instance.m_CurrentGameMode == GameModeController.TGameMode.Protect)
            yield return new WaitForSeconds(m_DelayRespawnProtectMode);
        switch (GameModeController.Instance.m_CurrentGameMode)
        {
            case GameModeController.TGameMode.Capture:
                l_RandomRound = Random.Range(0, m_RoundsCaptureHard.Count - 1);
                l_NumWaves = m_RoundsCaptureHard[l_RandomRound].m_Waves.Count;
                for (int i = 0; i < l_NumWaves; i++)
                {
                    StartWave(i, m_RoundsCaptureHard[l_RandomRound].m_Waves[i].m_Enemies);
                    yield return new WaitForSeconds(m_DelayBetweenWaves);
                }
                break;
            case GameModeController.TGameMode.Protect:
                l_RandomRound = Random.Range(0, m_RoundsProtectHard.Count - 1);
                l_NumWaves = m_RoundsProtectHard[l_RandomRound].m_Waves.Count;
                for (int i = 0; i < l_NumWaves; i++)
                {
                    StartWave(i, m_RoundsProtectHard[l_RandomRound].m_Waves[i].m_Enemies);
                    yield return new WaitForSeconds(m_DelayBetweenWaves);
                }
                break;
            case GameModeController.TGameMode.Destroy:
                l_RandomRound = Random.Range(0, m_RoundsDestroyHard.Count - 1);
                l_NumWaves = m_RoundsDestroyHard[l_RandomRound].m_Waves.Count;
                for (int i = 0; i < l_NumWaves; i++)
                {
                    StartWave(i, m_RoundsDestroyHard[l_RandomRound].m_Waves[i].m_Enemies);
                    yield return new WaitForSeconds(m_DelayBetweenWaves);
                }
                break;
        }
    }
    // Spawn waves of each spawwn position
    private void StartWave(int currentWave, List<EnemyConfig> enemies)
    {
        for (int spawn = 0; spawn < m_MilitarySpawnPositions.Length; spawn++)
        {
            StartCoroutine(SpawnWave(enemies, TZones.Student, spawn, m_MilitaryDelay));
        }
        for (int spawn = 0; spawn < m_MedievalSpawnPositions1.Length; spawn++)
        {
            StartCoroutine(SpawnWave(enemies, TZones.Twin1, spawn, m_MedievalDelay1));
        }
        for (int spawn = 0; spawn < m_MedievalSpawnPositions2.Length; spawn++)
        {
            StartCoroutine(SpawnWave(enemies, TZones.Twin2, spawn, m_MedievalDelay2));
        }
        for (int spawn = 0; spawn < m_BarbieSpawnPositions.Length; spawn++)
        {
            StartCoroutine(SpawnWave(enemies, TZones.Barbie, spawn, m_BarbieDelay));
        }
        for (int spawn = 0; spawn < m_StudentSpanwPositions.Length; spawn++)
        {
            StartCoroutine(SpawnWave(enemies, TZones.Student, spawn, m_StudentDelay));
        }
    }
    // Instantiate waves
    private IEnumerator SpawnWave(List<EnemyConfig> enemies,TZones zone, int spawnID,float delay)
    {
        foreach(var enemy in enemies)
        {
            // If zone and spawn ID is the same as parameters Instantiate enemy
            if(zone == enemy.m_Zone && spawnID == (int)enemy.m_PositionID)
            {
                InstantiateEnemy(enemy.m_Enemy, enemy.m_Zone, (int)enemy.m_PositionID);
                yield return new WaitForSeconds(delay);
            }
        }
    }
    private void InstantiateEnemy(TEnemyTypes enemyType, TZones zone,int spawnID)
    {
        // Create variables
        MeleeEnemy l_CurrentMeleeEnemy = null;
        DistanceEnemy l_CurrentDistanceEnemy = null;
        // Depending on enemy type add in list and get enemy inactive
        switch (enemyType)
        {
            case TEnemyTypes.Teeth:
                l_CurrentMeleeEnemy = GameController.Instance.GetTeethEnemyInactive();
                m_MeleeEnemiesAliveList.Add(l_CurrentMeleeEnemy);
                break;
            case TEnemyTypes.Helicopter:
                l_CurrentDistanceEnemy = GameController.Instance.GetHelicotperEnemyInactive();
                 M_DistanceEnemiesAliveList.Add(l_CurrentDistanceEnemy);
                break;
            case TEnemyTypes.Robot:
                l_CurrentMeleeEnemy = GameController.Instance.GetRobotEnemyInactive();
                 m_MeleeEnemiesAliveList.Add(l_CurrentMeleeEnemy);
                break;
        }
        // If enemy is melee
        if(l_CurrentMeleeEnemy != null)
        {
            // ChangePosition depending on zone
            switch (zone)
            {
                case TZones.Military:
                    l_CurrentMeleeEnemy.OnInit(m_MilitarySpawnPositions[spawnID].transform.position);
                    l_CurrentMeleeEnemy.m_NavMeshAgent.enabled = true;
                    l_CurrentMeleeEnemy.m_NavMeshAgent.Warp(m_MilitarySpawnPositions[spawnID].transform.position);
                    break;
                case TZones.Twin1:
                    l_CurrentMeleeEnemy.OnInit(m_MedievalSpawnPositions1[spawnID].transform.position);
                    l_CurrentMeleeEnemy.m_NavMeshAgent.enabled = true;
                    l_CurrentMeleeEnemy.m_NavMeshAgent.Warp(m_MedievalSpawnPositions1[spawnID].transform.position);
                    break;
                case TZones.Twin2:
                    l_CurrentMeleeEnemy.OnInit(m_MedievalSpawnPositions2[spawnID].transform.position);
                    l_CurrentMeleeEnemy.m_NavMeshAgent.enabled = true;
                    l_CurrentMeleeEnemy.m_NavMeshAgent.Warp(m_MedievalSpawnPositions2[spawnID].transform.position);
                    break;
                case TZones.Student:
                    l_CurrentMeleeEnemy.OnInit(m_StudentSpanwPositions[spawnID].transform.position);;
                    l_CurrentMeleeEnemy.m_NavMeshAgent.enabled = true;
                    l_CurrentMeleeEnemy.m_NavMeshAgent.Warp(m_StudentSpanwPositions[spawnID].transform.position);
                    break;
                case TZones.Barbie:
                    l_CurrentMeleeEnemy.OnInit(m_BarbieSpawnPositions[spawnID].transform.position);
                    l_CurrentMeleeEnemy.m_NavMeshAgent.enabled = true;
                    l_CurrentMeleeEnemy.m_NavMeshAgent.Warp(m_BarbieSpawnPositions[spawnID].transform.position);
                    break;
            }
        }
        // If is distance enemy
        else
        {
            // ChangePosition depending on zone
            switch (zone)
            {
                case TZones.Military:
                    l_CurrentDistanceEnemy.OnInit(m_MilitarySpawnPositions[spawnID].transform.position);
                    l_CurrentDistanceEnemy.m_NavMeshAgent.enabled = true;
                    l_CurrentDistanceEnemy.m_NavMeshAgent.Warp(m_MilitarySpawnPositions[spawnID].transform.position);
                    break;
                case TZones.Twin1:
                    l_CurrentDistanceEnemy.OnInit(m_MedievalSpawnPositions1[spawnID].transform.position);
                    l_CurrentDistanceEnemy.m_NavMeshAgent.enabled = true;
                    l_CurrentDistanceEnemy.m_NavMeshAgent.Warp(m_MedievalSpawnPositions1[spawnID].transform.position);
                    break;
                case TZones.Twin2:
                    l_CurrentDistanceEnemy.OnInit(m_MedievalSpawnPositions2[spawnID].transform.position);
                    l_CurrentDistanceEnemy.m_NavMeshAgent.enabled = true;
                    l_CurrentDistanceEnemy.m_NavMeshAgent.Warp(m_MedievalSpawnPositions2[spawnID].transform.position);
                    break;
                case TZones.Student:
                    l_CurrentDistanceEnemy.OnInit(m_StudentSpanwPositions[spawnID].transform.position);
                    l_CurrentDistanceEnemy.m_NavMeshAgent.enabled = true;
                    l_CurrentDistanceEnemy.m_NavMeshAgent.Warp(m_StudentSpanwPositions[spawnID].transform.position);
                    break;
                case TZones.Barbie:
                    l_CurrentDistanceEnemy.OnInit(m_BarbieSpawnPositions[spawnID].transform.position);
                    l_CurrentDistanceEnemy.m_NavMeshAgent.enabled = true;
                    l_CurrentDistanceEnemy.m_NavMeshAgent.Warp(m_BarbieSpawnPositions[spawnID].transform.position);
                    break;
            }
            
        }
    }
    // Desactive all enemies
    public void DestroyEnemiesAlive()
    {
        foreach (var enemy in m_MeleeEnemiesAliveList)
        {
            enemy.DeasctiveEnemy();
        }
        foreach(var enemy in M_DistanceEnemiesAliveList)
        {
            enemy.DeasctiveEnemy();
        }
    }
}
