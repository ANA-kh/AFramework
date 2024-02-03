using System.Collections;
using UnityEngine;

namespace AFramework.ResModule.Utilities
{
    public class CoroutineRunner :MonoBehaviour
    {
        private static GameObject gameObject;
        private static CoroutineRunner _instance;

        public static void MStartCoroutine(IEnumerator routine)
        {
            if (gameObject == null)
            {
                gameObject = new GameObject("CoroutineRunner");
                _instance = gameObject.AddComponent<CoroutineRunner>();
                GameObject.DontDestroyOnLoad(gameObject);
            }
            _instance.StartCoroutine(routine);
        }
    }
}