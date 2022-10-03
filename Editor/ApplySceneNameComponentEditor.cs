using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kogane.Internal
{
    [InitializeOnLoad]
    internal static class ApplySceneNameComponentEditor
    {
        static ApplySceneNameComponentEditor()
        {
            EditorSceneManager.sceneOpened         -= OnSceneOpened;
            EditorSceneManager.sceneOpened         += OnSceneOpened;
            EditorSceneManager.sceneSaving         -= OnSceneSaving;
            EditorSceneManager.sceneSaving         += OnSceneSaving;
            ObjectFactory.componentWasAdded        -= OnComponentWasAdded;
            ObjectFactory.componentWasAdded        += OnComponentWasAdded;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnSceneOpened( Scene scene, OpenSceneMode mode )
        {
            if ( EditorApplication.isPlaying ) return;
            Rename();
        }

        private static void OnSceneSaving( Scene scene, string path )
        {
            if ( EditorApplication.isPlaying ) return;
            Rename();
        }

        private static void OnComponentWasAdded( Component component )
        {
            if ( EditorApplication.isPlaying ) return;
            Rename();
        }

        private static void OnPlayModeStateChanged( PlayModeStateChange change )
        {
            if ( change != PlayModeStateChange.ExitingEditMode ) return;
            Rename();
        }

        private static void Rename()
        {
            if ( EditorApplication.isPlaying ) return;

            var activeScene = SceneManager.GetActiveScene();
            var sceneName   = activeScene.name;

            if ( string.IsNullOrEmpty( sceneName ) ) return;

            var gameObjects = activeScene
                    .GetRootGameObjects()
                    .SelectMany( x => x.GetComponentsInChildren<ApplySceneNameComponent>( true ) )
                    .Where( x => x != null )
                    .Where( x => x.name != sceneName )
                    .Select( x => x.gameObject )
                    .ToArray()
                ;

            if ( gameObjects.Length <= 0 ) return;

            Undo.RecordObjects( gameObjects, "Rename" );

            foreach ( var gameObject in gameObjects )
            {
                gameObject.name = sceneName;
            }
        }
    }
}