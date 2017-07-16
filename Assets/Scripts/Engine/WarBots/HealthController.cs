using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WarBotEngine.Items;

namespace WarBotEngine.WarBots {
    /// <summary>
    /// Health.
    /// </summary>
    public class HealthController : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
	     * This section holds public attributes, which are visible and editable within the editor.*
	     * For attributes accessible publicly but not visible in the editor, use properties.      *
	     ******************************************************************************************/

        /// <summary>
        /// The unit's max health. Editable within the editor even during the scene is playing.
        /// </summary>
        public float max_health;

        /*********************************** HIDDEN ATTRIBUTES **************************************
	     * This section holds private/protected attributes, which are NOT visible within the editor.*
	     * Use this section for attributes that aren't meant to be accessible from other classes.   *
	     ********************************************************************************************/

        /// <summary>
        /// The unit's current health.
        /// </summary>
        protected float current_health;

        /*********************************************** PROPERTIES **************************************************
		 * This section holds properties that we want to make publicly accessible without showing them in the editor.*
		 *************************************************************************************************************/

        /// <summary>
        /// Gets the current health.
        /// </summary>
        /// <value>The current health.</value>
        public float CurrentHealth { get { return this.current_health; } }

        /// <summary>
        /// Gets the maximum health for this unit.
        /// </summary>
        /// <value>The maximum health.</value>
        public float MaxHealth { get { return this.max_health; } }

        /// <summary>
        /// Indicate if the unit is full of life
        /// </summary>
        public bool Full { get { return this.current_health == this.max_health; } }

        /***************************************** DELEGATES AND EVENTS ***************************************************
	     * This section holds all delegates and events, that we will use to notify subscribers of state changes.          * 
	     * For example, whenever the perception range is changed, notify the perception sphere to make it change its size.*
	     ******************************************************************************************************************/

        /// <summary>
        /// Delegate that will handle death events.
        /// </summary>
        public delegate void DeathEventHandler(GameObject sender);

        /// <summary>
        /// Occurs when the unit dies.
        /// </summary>
        public event DeathEventHandler DeathEvent;

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

        /******************************************** OTHER FUNCTIONS ***************************************************
		 * This section holds all other functions that might be called by the primitives to retrieve values for example.* 
		 * In principle, every function in this section should only be called from within primitives.                   *
		 ****************************************************************************************************************/

        /// <summary>
        /// Initializes this instance's attributes.
        /// This method is used to avoid charging the Start() method too much.
        /// </summary>
        public virtual void Init() {
            this.current_health = this.max_health;
        }

        /// <summary>
        /// Reduces the health.
        /// </summary>
        /// <param name="damage">Damage.</param>
        public void ReduceHealth(float damage) {
            this.current_health -= damage;
            if (this.current_health < 1.0f) {
                Die();
            }
        }

        /// <summary>
        /// Increase the health
        /// </summary>
        /// <param name="count">Health count</param>
        /// <returns>Return true if success and false otherwise</returns>
        public bool IncreaseHealth(float count)
        {
            if (this.Full) return false;
            this.current_health += count;
            if (this.current_health > this.max_health)
                this.current_health = this.max_health;
            return true;
        }

        /// <summary>
        /// Die this instance.
        /// </summary>
        public void Die() {
            foreach (GameObject enemy in GetComponent<DetectionController>().VisibleEnemies) {
                enemy.GetComponent<HealthController>().DeathEvent -= GetComponent<DetectionController>().OnEnemyLost;
            }

            foreach (GameObject item in GetComponent<DetectionController>().DetectedItems) {
                item.GetComponent<WarResource>().PickedUpEvent -= GetComponent<DetectionController>().OnItemLost;
            }

            if (this.DeathEvent != null) {
                DeathEvent(this.gameObject);
            }
            Destroy(this.gameObject);
        }
    }
}