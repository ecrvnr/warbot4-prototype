using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WarBotEngine.Items;

namespace WarBotEngine.WarBots {
    public class GatherController : MonoBehaviour {
        /*********************************** EDITOR ATTRIBUTES ************************************
		 * This section holds public attributes, which are visible and editable within the editor.*
		 * For attributes accessible publicly but not visible in the editor, use properties.      *
		 ******************************************************************************************/

        /*********************************** HIDDEN ATTRIBUTES **************************************
		 * This section holds private/protected attributes, which are NOT visible within the editor.*
		 * Use this section for attributes that aren't meant to be accessible from other classes.   *
		 ********************************************************************************************/

        /*********************************************** PROPERTIES **************************************************
		 * This section holds properties that we want to make publicly accessible without showing them in the editor.*
		 *************************************************************************************************************/

        /// <summary>
        /// Gets the detected items.
        /// </summary>
        /// <value>The detected items.</value>
        public List<GameObject> DetectedItems { get { return this.GetComponent<DetectionController>().DetectedItems; } }

        /***************************************** DELEGATES AND EVENTS ***************************************************
		 * This section holds all delegates and events, that we will use to notify subscribers of state changes.          * 
		 * For example, whenever the perception range is changed, notify the perception sphere to make it change its size.*
		 ******************************************************************************************************************/

        /******************************************* UNITY FUNCTIONS ********************************************
		 * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
		 * In principle, every function in this section will run once per frame, except Start().                *
		 ********************************************************************************************************/

        /// <summary>
        /// Start this instance.
        /// </summary>
        void Start() {
            
        }

        /// <summary>
        /// Update this instance.
        /// </summary>
        void Update() {
            
        }

        /// <summary>
        /// Raises the trigger enter event.
        /// Used to pick up resources.
        /// </summary>
        /// <param name="collision">Collision.</param>
        void OnTriggerEnter(Collider collision) {
            if (collision.gameObject.CompareTag("WarItem")) {
                InventoryController inventory = this.GetComponent<InventoryController>();
                if (!inventory.Full) {
                    //inventory.Add(collision.gameObject.GetComponent<WarResource>().value);
                    inventory.Add(InventoryController.TakeCount);
                    this.DetectedItems.Remove(collision.gameObject);
                    collision.gameObject.GetComponent<WarResource>().PickedUp();
                }
            }
        }

        /******************************************** PRIMITIVES **********************************************
		 * This section holds all our primitives, which are the functions that the FSM will be allowed to use.* 
		 * In principle, any unit's main loop should only call primitives.                                    *
		 ******************************************************************************************************/

        /******************************************** OTHER FUNCTIONS ***************************************************
		 * This section holds all other functions that might be called by the primitives to retrieve values for example.* 
		 * In principle, every function in this section should only be called from within primitives.                   *
		 ****************************************************************************************************************/

    }
}