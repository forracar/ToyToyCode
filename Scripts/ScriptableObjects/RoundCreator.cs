/* 
 * Oscar Forra Carbonell
 * Last Update: 04/05/2019
*/

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Round", menuName = "Rounds/New round", order = 1)]
public class RoundCreator : ScriptableObject
{
    [Tooltip("Number of waves a that round will have")]
    [Header("Waves that round will have")]
    public List<EnemyWave> m_Waves;
}

[System.Serializable]
public class EnemyWave
{
    [Tooltip("Number of enemies that the wave will have")]
    [Header("Enemies that a round will have")]
    public List<EnemyConfig> m_Enemies;
}

[System.Serializable]
public class EnemyConfig
{
    [Header("Enemy to instantiate")]
    public TEnemyTypes m_Enemy;
    [Header("Zone where will be spawned")]
    public TZones m_Zone;
    [Header("Position of the zone")]
    public TSpawnID m_PositionID;
}


