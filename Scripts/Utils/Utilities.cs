/* 
 * Oscar Forra Carbonell
*/
using UnityEngine;

public static class Utilities
{
    public static GameObject FindNearestGameObjectOfTag(GameObject currentGameObject, GameObject[] arrayGameObjects)
    {
        // Create disntace and id variables
        float l_NearestDistance = 100;
        int l_Id = 0;

        // Look every gameobject and find nearest one
        for (int i = 0; i < arrayGameObjects.Length; i++)
        {
            // Compute distance with current GO
            float l_distance = (currentGameObject.transform.position - arrayGameObjects[i].transform.position).magnitude;
            // If distance  is less than before se current l_distance as nearest
            if (l_distance < l_NearestDistance && arrayGameObjects[i].GetComponent<ProtectableObject>().m_EnabledInGame) {
                l_NearestDistance = l_distance;
                l_Id = i;
            }
        }
        // Return gameObject
        return arrayGameObjects[l_Id];
    }
    public static float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
    public static void RandomizeArrayOfGameObjects(ref GameObject[]  array)
    {

        // For each spot in the array, pick
        // a random item to swap into that spot.
        for (int i = 0; i < array.Length - 1; i++)
        {
            int j = Random.Range(i, array.Length);
            GameObject temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}
