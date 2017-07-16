using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace WarBotEngine.UI {
    /// <summary>
    /// This class is used to handle the speed of the game.
    /// </summary>
    public class GameSpeed : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
        * This section holds public attributes, which are visible and editable within the editor.*
        * For attributes accessible publicly but not visible in the editor, use properties.      *
        ******************************************************************************************/
        /// <summary>
        /// The pause button.
        /// </summary>
        public Button pauseButton;

        /// <summary>
        /// The speed slider.
        /// </summary>
        public Slider sliderSpeed;

        /// <summary>
        /// This is the text that will display the game speed.
        /// </summary>
        public Text gameSpeedText;

         /*********************************** HIDDEN ATTRIBUTES **************************************
          * This section holds private/protected attributes, which are NOT visible within the editor.*
          * Use this section for attributes that aren't meant to be accessible from other classes.   *
          ********************************************************************************************/
        private bool paused;
        private float oldGameSpeed;

        /******************************************* UNITY FUNCTIONS ********************************************
        * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
        * In principle, every function in this section will run once per frame, except Start().                *
        ********************************************************************************************************/
        private void Start() {
            paused = false;
            Time.timeScale = 1.0f;
            oldGameSpeed = Time.timeScale;
            pauseButton.GetComponentInChildren<Text>().text = "||";
        }

        private void Update() {
            displayGameSpeed();
        }

        /******************************************** GUI FUNCTIONS ********************************************
        * This section holds all functions for the user interface.                                             *
        ********************************************************************************************************/
        /// <summary>
        /// Change the speed of the game. The value is changed with the slider on the UI.
        /// </summary>
        /// <param name="newSpeed">The new speed of the game</param>
        public void changeGameSpeed(float newSpeed) {
            Time.timeScale = newSpeed;
            oldGameSpeed = newSpeed;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
        }

        /// <summary>
        /// Return the actual speed of the game. 
        /// </summary>
        public void displayGameSpeed() {
            gameSpeedText.text = Math.Round(Time.timeScale , 2).ToString();
        }

        /// <summary>
        /// Change the value of paused to true/false when the Pause button is clicked.
        /// </summary>
        public void Pause() {

            paused = !paused;

            if (paused) {
                Time.timeScale = 0;
                sliderSpeed.enabled = false;
                pauseButton.GetComponentInChildren<Text>().text = "|>";
            } else {
                Time.timeScale = oldGameSpeed;
                sliderSpeed.enabled = true;
                pauseButton.GetComponentInChildren<Text>().text = "||";
            }
        }
    }
}