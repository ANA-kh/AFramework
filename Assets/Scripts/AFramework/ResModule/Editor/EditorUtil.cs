using UnityEditor.SceneManagement;

namespace AFramework.ResModule.Editor
{
    public class EditorUtil
    {
        public static bool HasDirtyScenes()
        {
            var sceneCount = EditorSceneManager.sceneCount;
            for (var i = 0; i < sceneCount; ++i)
            {
                var scene = EditorSceneManager.GetSceneAt(i);
                if (scene.isDirty)
                    return true;
            }

            return false;
        }
    }
}