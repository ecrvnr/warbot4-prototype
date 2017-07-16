using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.UI {
    /// <summary>
    /// This class handle the pause menu of the game.
    /// </summary>
    public class EscMenu : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
         * This section holds public attributes, which are visible and editable within the editor.*
         * For attributes accessible publicly but not visible in the editor, use properties.      *
         ******************************************************************************************/

        /// <summary>
        /// The pannel menu.
        /// </summary>
        public GameObject pannel_Menu;

        /// <summary>
        /// Le "Background" behind the menu.
        /// </summary>
        public GameObject pannel_blur;

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
         ********************************************************************************************/

        /// <summary>
        /// Toggle the menu.
        /// </summary>
        private bool toggleMenu;

        /// <summary>
        /// Toggle the pause when the player is on the menu.
        /// </summary>
        private bool paused;

        /// <summary>
        /// Old gamespeed. 
        /// </summary>
        private float oldGameSpeed;

        /******************************************* UNITY FUNCTIONS ********************************************
         * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
         * In principle, every function in this section will run once per frame, except Start().                *
         ********************************************************************************************************/
        void Start() {
            toggleMenu = false;
            pannel_Menu.SetActive(false);
            pannel_blur.SetActive(false);
            oldGameSpeed = Time.timeScale;
        }

        void Update() {
            if (Input.GetKeyUp(KeyCode.Escape)) {
                ToggleMenu();
            }
        }

        /******************************************** GUI FUNCTIONS ********************************************
        * This section holds all functions for the user interface.                                             *
        ********************************************************************************************************/
        /// <summary>
        /// Toggle the menu which contains options to come back to main menu or exit the game.
        /// </summary>
        public void ToggleMenu() {
            toggleMenu = !toggleMenu;
            pannel_Menu.SetActive(toggleMenu);
            pannel_blur.SetActive(toggleMenu);
            Pause();
        }

        /// <summary>
        /// Pause the game on menu.
        /// </summary>
        public void Pause() {
            paused = !paused;
            Time.timeScale = paused ? 0 : oldGameSpeed;
        }

    }

}