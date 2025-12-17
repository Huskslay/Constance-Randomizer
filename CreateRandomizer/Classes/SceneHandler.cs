using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;

namespace CreateRandomizer.Classes;

public static class SceneHandler
{
    public static readonly List<string> scenes = [];
    public static readonly List<string> flashbackScenes = [];

    public static void Init()
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scene = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            if (scene.Contains("_"))
            {
                if (scene.Contains("Flashback")) flashbackScenes.Add(scene);
                else scenes.Add(scene);
            }
        }
        Plugin.Logger.LogMessage($"Total scenes: {scenes.Count}");
    }
}
