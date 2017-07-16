using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WarBotEngine.Managers;

namespace WarBotEngine.WarBots {

    public class SpawnController : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
		 * This section holds public attributes, which are visible and editable within the editor.*
		 * For attributes accessible publicly but not visible in the editor, use properties.      *
		 ******************************************************************************************/

        /// <summary>
        /// The minimum amount of time we wait between to unit spawns.
        /// </summary>
        public float spawn_delay;

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
         ********************************************************************************************/

        /// <summary>
        /// Keep tabs on spawning events.
        /// </summary>
        public float last_spawn = 0;

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

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            
        }

        /******************************************** PRIMITIVES **********************************************
		 * This section holds all our primitives, which are the functions that the FSM will be allowed to use.* 
		 * In principle, any unit's main loop should only call primitives.                                    *
		 ******************************************************************************************************/

        /// <summary>
        /// Place une unité dans la zone d'apparition en faisant attention au collisions
        /// </summary>
        /// <param name="type">type de l'unité</param>
        public bool SpawnUnit(BotType type)
        {
            if (!this.CanCreate(type))
                return false;
            GameObject unit = Instantiate(type.PrefabByType() , this.transform.position + this.transform.forward * 2.0f , this.transform.rotation * Quaternion.Euler(0.0f , Random.Range(-20.0f , 20.0f) , 0.0f) , this.GetComponent<WarBotController>().TeamManager.GetComponent<TeamManager>().transform);
            this.GetComponent<WarBotController>().TeamManager.GetComponent<TeamManager>().InitUnit(unit, type);
            this.GetComponent<InventoryController>().Remove((int)type.UnitCost());
            this.last_spawn = Time.time;
            return true;
        }

        public bool CanCreate(BotType type)
        {
            if ((this.GetComponent<InventoryController>().CurrentInventory < type.UnitCost()) || (this.last_spawn + this.spawn_delay >= Time.time))
                return false;
            BotType self = this.GetComponent<WarBotController>().Type;
            if (self == BotType.WarBase)
                return (
                    type == BotType.WarEngineer ||
                    type == BotType.WarExplorer ||
                    type == BotType.WarHeavy
                );
            if (self == BotType.WarEngineer)
                return (type == BotType.WarTurret);
            return false;
        }

        /******************************************** OTHER FUNCTIONS ***************************************************
		 * This section holds all other functions that might be called by the primitives to retrieve values for example.* 
		 * In principle, every function in this section should only be called from within primitives.                   *
		 ****************************************************************************************************************/

    }
}