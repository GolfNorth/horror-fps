using System.Collections;
using UnityEngine;

namespace Game.Core.Coroutines
{
    /// <summary>
    /// MonoBehaviour that runs coroutines for non-MonoBehaviour classes.
    /// </summary>
    public sealed class CoroutineRunner : MonoBehaviour, ICoroutineRunner
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public Coroutine Run(IEnumerator coroutine)
        {
            return StartCoroutine(coroutine);
        }

        public void Stop(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        public void StopAll()
        {
            StopAllCoroutines();
        }
    }
}
