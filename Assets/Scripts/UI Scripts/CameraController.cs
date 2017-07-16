using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WarBotEngine.Managers;

namespace WarBotEngine.UI {

    /// <summary>
    /// This class is used to control the camera during gameplay.
    /// </summary>
    public class CameraController : MonoBehaviour {

        /*********************************** EDITOR ATTRIBUTES ************************************
         * This section holds public attributes, which are visible and editable within the editor.*
         * For attributes accessible publicly but not visible in the editor, use properties.      *
         ******************************************************************************************/
        /// <summary>
        /// The size of the zone where the mouse is close enought from the border of the screen.
        /// Entering this zone will triggered the mouvement of the camera.
        /// </summary>
        public float scrollZone;

        /// <summary>
        /// The zoom speed.
        /// </summary>
        public float scrollSpeed;

        /// <summary>
        /// The mouvement speed when in the scroll zone.
        /// </summary>
        public float panSpeed;

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
         ********************************************************************************************/

        /// <summary>
        /// The minimums and maximums positions where the camera cannot go further.
        /// </summary>
        private float   xMin,
                        xMax, 
                        yMin, 
                        yMax, 
                        zMin, 
                        zMax;

        /******************************************* UNITY FUNCTIONS ********************************************
         * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
         * In principle, every function in this section will run once per frame, except Start().                *
        **********************************************************************************************************/

        /// <summary>
        /// On starting this instance, set the boundaries for mainCamera movements according to the size of the map.
        /// </summary>
        private void Start() {
            Area map = GameObject.Find("Game Manager").GetComponent<GameManager>().game_area;
            this.transform.position = new Vector3(map.center.x , 435.0f, map.center.z - 275.0f);
            this.xMin = map.center.x - map.width / 2.0f + 30.0f;
            this.xMax = map.center.x + map.width / 2.0f - 30.0f;
            this.yMin = 70.0f;
            this.yMax = 435.0f;
            this.zMin = map.center.z - map.height / 2.0f - (this.transform.position.y / 2.0f) - 30.0f;
            this.zMax = map.center.z + map.height / 2.0f - (this.transform.position.y / 2.0f) - 30.0f;
        }

        // Update is called once per frame
        void Update() {
            Area map = GameObject.Find("Game Manager").GetComponent<GameManager>().game_area;
            this.zMin = map.center.z - map.height / 2.0f - (this.transform.position.y / 2.0f) - 30.0f;
            this.zMax = map.center.z + map.height / 2.0f - (this.transform.position.y / 2.0f) - 30.0f;
            Vector3 new_pos = this.transform.position;

            if (Input.mousePosition.x < scrollZone || Input.GetAxis("Horizontal") < 0.0f)
                new_pos -= this.transform.right * this.panSpeed; //LEFT
            if (Input.mousePosition.x > Screen.width - scrollZone || Input.GetAxis("Horizontal") > 0.0f)
                new_pos += this.transform.right * this.panSpeed; //RIGHT
            if (Input.mousePosition.y < scrollZone || Input.GetAxis("Vertical") < 0.0f)
                new_pos -= Vector3.forward * this.panSpeed; //DOWN
            if (Input.mousePosition.y > Screen.height - scrollZone || Input.GetAxis("Vertical") > 0.0f)
                new_pos += Vector3.forward * this.panSpeed; //UP
            new_pos += this.transform.forward * Input.GetAxis("Mouse ScrollWheel") * this.scrollSpeed;

            new_pos.x = Mathf.Clamp(new_pos.x, xMin, xMax);
            new_pos.y = Mathf.Clamp(new_pos.y, yMin, yMax);
            new_pos.z = Mathf.Clamp(new_pos.z, zMin, zMax);

            this.transform.position = new_pos;
        }
    }
}