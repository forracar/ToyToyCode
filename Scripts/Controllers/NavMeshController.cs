/* 
Oscar Forra Carbonell 
*/
using UnityEngine.AI;
using Utils;

public class NavMeshController : Singleton<NavMeshController>
{
    public NavMeshSurface[] surfaces;
    // Start is called before the first frame update
    public void UpdateNavMesh()
    {
        for(int i = 0; i <surfaces.Length; i++)
        {
            surfaces[i].BuildNavMesh();
        }
    }
}

    
