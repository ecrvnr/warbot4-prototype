using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarBotEngine.Managers {
    public class GameSpeedControls : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
		 * This section holds public attributes, which are visible and editable within the editor.*
		 * For attributes accessible publicly but not visible in the editor, use properties.      *
		 ******************************************************************************************/

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
         ********************************************************************************************/

        /// <summary>
        /// The slider.
        /// </summary>
        private Slider slider;

        /// <summary>
        /// The button.
        /// </summary>
        private Button pause_button;

        /// <summary>
        /// The textual value of the current game speed.
        /// </summary>
        private Text game_speed_indicator;

        /// <summary>
        /// Boolean value telling us whether the game is paused or not.
        /// </summary>
        private bool game_paused;

        /*********************************************** PROPERTIES **************************************************
		 * This section holds properties that we want to make publicly accessible without showing them in the editor.*
		 *************************************************************************************************************/

        /***************************************** DELEGATES AND EVENTS ***************************************************
		 * This section holds all delegates and events, that we will use to notify subscribers of state changes.          * 
		 * For example, whenever the perception range is changed, notify the perception sphere to make it change its size.*
		 ******************************************************************************************************************/

        /******************************************* UNITY FUNCTIONS ********************************************
		 * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
		 * In principle, every function in this section will run once per frame, except Start().                *
		 ********************************************************************************************************/

        void Awake() {
            Time.timeScale = 1.0f;
        }

        /// <summary>
        /// Start this instance.
        /// </summary>
        void Start() {
            this.game_paused = false;
            this.game_speed_indicator = GameObject.Find("UI/Canvas/GameSpeed/Game Speed Label Area/Game Speed Indicator").GetComponent<Text>() as Text;
            this.pause_button = GameObject.Find("UI/Canvas/GameSpeed/Pause Button").GetComponent<Button>() as Button;
            this.slider = GameObject.Find("UI/Canvas/GameSpeed/Game Speed Slider").GetComponent<Slider>() as Slider;

            pause_button.onClick.AddListener(delegate {
                OnPauseButtonClicked();
            });

            slider.onValueChanged.AddListener(delegate {
                OnValueChanged(this.slider.value);
            });
        }

        /// <summary>
        /// Raises the value changed event.
        /// </summary>
        /// <param name="value">Value.</param>
        private void OnValueChanged(float value) {
            if (!(this.game_paused)) {
                Time.timeScale = value;
            }
            this.game_speed_indicator.text = System.Math.Round(value , 2).ToString();

        }

        /// <summary>
        /// Raises the pause button clicked event.
        /// </summary>
        private void OnPauseButtonClicked() {
            if (this.game_paused) {
                this.game_paused = false;
                Time.timeScale = this.slider.value;
                this.game_speed_indicator.text = System.Math.Round(this.slider.value , 2).ToString();
            } else {
                this.game_paused = true;
                Time.timeScale = 0.0f;
            }
        }

        /******************************************** OTHER FUNCTIONS ***************************************************
         * This section holds all other functions that might be called by the primitives to retrieve values for example.* 
         * In principle, every function in this section should only be called from within primitives.                   *
         ****************************************************************************************************************/
    }
}