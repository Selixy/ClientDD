using Unity.Scene;
using System.Threading.Tasks;

namespace RPG_System.API
{
    public static partial class PrivateAPI
    {
        public static string SceneLoader(string sceneName)
        {
            return Loader(sceneName).GetAwaiter().GetResult();
        }

        private static async Task<string> Loader(string sceneName)
        {
            var loader = new SceneToggleLoader(sceneName);
            var result = await loader.ToggleSceneAsync();
            return result;
        }
    }
}
