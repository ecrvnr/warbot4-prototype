using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.Items {

    public class WarResource : MonoBehaviour {

        /// <summary>
        /// Maximal ressources number on the map
        /// </summary>
        public static int MaxRessources;

        /*********************************** EDITOR ATTRIBUTES ************************************
		 * This section holds public attributes, which are visible and editable within the editor.*
		 * For attributes accessible publicly but not visible in the editor, use properties.      *
		 ******************************************************************************************/

        //public int value;

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
         ********************************************************************************************/

        /*********************************************** PROPERTIES **************************************************
		 * This section holds properties that we want to make publicly accessible without showing them in the editor.*
		 *************************************************************************************************************/

        /***************************************** DELEGATES AND EVENTS ***************************************************
		 * This section holds all delegates and events, that we will use to notify subscribers of state changes.          * 
		 * For example, whenever the perception range is changed, notify the perception sphere to make it change its size.*
		 ******************************************************************************************************************/

        public delegate void PickedUpEventHandler(GameObject sender);

        public event PickedUpEventHandler PickedUpEvent;

        /******************************************* UNITY FUNCTIONS ********************************************
         * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
         * In principle, every function in this section will run once per frame, except Start().                *
         ********************************************************************************************************/

        // Use this for initialization
        void Start() {
        }

        // Update is called once per frame
        void Update() {

        }

        /******************************************** OTHER FUNCTIONS ***************************************************
		 * This section holds all other functions that might be called by the primitives to retrieve values for example.* 
		 * In principle, every function in this section should only be called from within primitives.                   *
		 ****************************************************************************************************************/

        /// <summary>
        /// When picked up, notify all subscribers and destory this instance.
        /// </summary>
        public void PickedUp() {
            if (this.PickedUpEvent != null) {
                this.PickedUpEvent(this.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
}