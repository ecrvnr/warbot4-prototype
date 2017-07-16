using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.WarBots {

  public class WarExplorer : WarBotController {

        public static float MaxHealth { get; set; }
        public static int MaxInventory { get; set; }
        public static float PerceptionRadius { get; set; }
        public static float Speed { get; set; }

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
        /// Gets the walk controller.
        /// </summary>
        /// <value>The walk controller.</value>
        public WalkController WalkController {
            get {
                if (this.GetComponent<WalkController>() != null) {
                    return this.GetComponent<WalkController>();
                } else {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets the gather controller.
        /// </summary>
        /// <value>The gather controller.</value>
        public GatherController GatherController {
            get {
                if (this.GetComponent<GatherController>() != null) {
                    return this.GetComponent<GatherController>();
                } else {
                    return null;
                }
            }
        }

        /***************************************** DELEGATES AND EVENTS ***************************************************
         * This section holds all delegates and events, that we will use to notify subscribers of state changes.          * 
         * For example, whenever the perception range is changed, notify the perception sphere to make it change its size.*
         ******************************************************************************************************************/


        /******************************************* UNITY FUNCTIONS ********************************************
		 * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
		 * In principle, every function in this section will run once per frame, except Start().                *
		 ********************************************************************************************************/

        protected override void Start()
        {
            base.Start();

            HealthController health = this.GetComponent<HealthController>();
            health.max_health = MaxHealth;
            health.Init();
            InventoryController inventory = this.GetComponent<InventoryController>();
            inventory.max_inventory = MaxInventory;
            inventory.Init();
            DetectionController detection = this.GetComponent<DetectionController>();
            detection.perception_radius = PerceptionRadius;
            detection.Init();
            WalkController walk = this.GetComponent<WalkController>();
            walk.move_speed = Speed;
            walk.Init();
        }

        protected override void Update() {
            base.Update();
        }

        /******************************************** PRIMITIVES **********************************************
		 * This section holds all our primitives, which are the functions that the FSM will be allowed to use.* 
		 * In principle, any unit's main loop should only call primitives.                                    *
		 ******************************************************************************************************/


        /******************************************** OTHER FUNCTIONS ***************************************************
		 * This section holds all other functions that might be called by the primitives to retrieve values for example.* 
		 * In principle, every function in this section should only be called from within primitives.                   *
		 ****************************************************************************************************************/

        protected override void Init() {
            base.Init();
            this.type = BotType.WarExplorer;
        }

        public override BotType GetUnitType()
        {
            return BotType.WarExplorer;
        }
    }
}
