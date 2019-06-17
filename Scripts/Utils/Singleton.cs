/* 
 * Oscar Forra Carbonell
*/

using UnityEngine;
namespace Utils 
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T instance;
        public static T Instance
        {
            get { return instance; }
        }
        public static bool Initialized
        {
            get { return instance != null; }
        }
        protected virtual void Awake()
        {
            if (instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = (T)this;
            }
        }
        protected virtual void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }

    }
}


