using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WarBotEngine.Managers;


namespace WarBotEngine.WarBots {
    public class WalkController : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
		 * This section holds public attributes, which are visible and editable within the editor.*
		 * For attributes accessible publicly but not visible in the editor, use properties.      *
		 ******************************************************************************************/

        /// <summary>
        /// The unit's fixed move speed.
        /// </summary>
        public float move_speed = 1f;

        /*********************************** HIDDEN ATTRIBUTES **************************************
		 * This section holds private/protected attributes, which are NOT visible within the editor.*
		 * Use this section for attributes that aren't meant to be accessible from other classes.   *
		 ********************************************************************************************/

        protected Rigidbody rb;

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

        /// <summary>
        /// Start this instance.
        /// </summary>
        void Start() {
            //this.Init();
        }

        /// <summary>
        /// Update this instance.
        /// </summary>
        void Update() {
            
        }

        /// <summary>
        /// Raises the collision enter event.
        /// In the case of a generic WarUnit we will only deal with collision with walls or terrain, in which case we simply turn around.
        /// </summary>
        /// <param name="collision">Collision.</param>
        void OnCollisionEnter(Collision collision) {

        }

        /// <summary>
        /// Raises the collision stay event.
        /// </summary>
        /// <param name="collision">Collision.</param>
        void OnCollisionStay(Collision collision) {
            if (collision.gameObject.CompareTag("WarBot")) {
                //if we are in a prolonged collision with another unit, slightly drift to the side (to avoid getting stuck in case of a head-on collision)
                Area map = GameObject.Find("Game Manager").GetComponent<GameManager>().game_area;
                this.transform.Rotate(map.center);
                print("Bumped into another unit!");
            }
        }

        /******************************************** PRIMITIVES **********************************************
		 * This section holds all our primitives, which are the functions that the FSM will be allowed to use.* 
		 * In principle, any unit's main loop should only call primitives.                                    *
		 ******************************************************************************************************/

        /// <summary>
        /// Walk the specified distance.
        /// </summary>
        /// <param name="distance">Distance.</param>
        public void Walk() {
            if (GameManager.instance.game_area.UpdatePosition(this.gameObject, this.transform.position + this.transform.forward * move_speed * Time.deltaTime))
                this.Turn(Random.Range(-45f, 45f));
        }

        public void Turn(float angle)
        {
            Quaternion rotation = Quaternion.LookRotation(new Vector3(0.0f, 0.0f, 0.0f) - this.transform.position);
            this.transform.rotation = rotation * Quaternion.Euler(0.0f, angle, 0.0f);
        }

        public void Turn(Vector3 position)
        {
            Vector3 lookPos = position - this.transform.position;
            lookPos.y = 0.0f;
            this.transform.rotation = Quaternion.LookRotation(lookPos);
        }

        public void Turn(GameObject entity)
        {
            this.Turn(entity.transform.position);
        }

        /******************************************** OTHER FUNCTIONS ***************************************************
		 * This section holds all other functions that might be called by the primitives to retrieve values for example.* 
		 * In principle, every function in this section should only be called from within primitives.                   *
		 ****************************************************************************************************************/

        /// <summary>
        /// Init this instance.
        /// </summary>
        public void Init() {
            this.rb = GetComponent<Rigidbody>();
        }

    }
}