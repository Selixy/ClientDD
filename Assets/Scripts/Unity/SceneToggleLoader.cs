using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement; // important

namespace Unity.Scene
{
    public class SceneToggleLoader
    {
        private readonly string sceneName;

        public SceneToggleLoader(string sceneName)
        {
            this.sceneName = sceneName;
        }

        public async Task<string> ToggleSceneAsync()
        {
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                string msg = $"[SceneToggleLoader] La scène \"{sceneName}\" n'existe pas dans les Build Settings.";
                Debug.LogWarning(msg);
                return msg;
            }

            // ✅ On précise le type complet
            UnityEngine.SceneManagement.Scene targetScene = SceneManager.GetSceneByName(sceneName);

            if (targetScene.IsValid() && targetScene.isLoaded)
            {
                string msg = $"[SceneToggleLoader] Déchargement de la scène \"{sceneName}\"...";
                Debug.Log(msg);

                var unloadOp = SceneManager.UnloadSceneAsync(sceneName);
                while (!unloadOp.isDone)
                    await Task.Delay(10);

                msg = $"[SceneToggleLoader] Scène \"{sceneName}\" déchargée.";
                Debug.Log(msg);
                return msg;
            }
            else
            {
                string msg = $"[SceneToggleLoader] Chargement de la scène \"{sceneName}\"...";
                Debug.Log(msg);

                var loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                while (!loadOp.isDone)
                    await Task.Delay(10);

                msg = $"[SceneToggleLoader] Scène \"{sceneName}\" chargée.";
                Debug.Log(msg);
                return msg;
            }
        }
    }
}
