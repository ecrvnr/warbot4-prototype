using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WarBotEngine.UI {

    /// <summary>
    /// Manage the scenes.
    /// </summary>
    public class SceneLoader : MonoBehaviour {

        /// <summary>
        /// Change the scene.
        /// </summary>
        /// <param name="sceneName">The name of the desired scene.</param>
        public void ChangeScene(string sceneName) {
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Exit the game.
        /// </summary>
        public void ExitGame() {
            Application.Quit();
        }

        /// <summary>
        /// This method is called when we return to the main menu or to the editor.
        /// </summary>
        public void DestroyInitializer() {
            Destroy(GameObject.Find("Initializer"));
        }

        /// <summary>
        /// End the music.
        /// </summary>
        public void DestroyMusic() {
            Destroy(GameObject.Find("Music"));
        }

    }
}
