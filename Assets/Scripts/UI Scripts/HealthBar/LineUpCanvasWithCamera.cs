using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarBotEngine.UI {

    /// <summary>
    /// Hallow UI elements in the scene to be lined with the camera.
    /// </summary>
    public class LineUpCanvasWithCamera : MonoBehaviour {

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
         ********************************************************************************************/
        /// <summary>
        /// The main camera.
        /// </summary>
        [HideInInspector] public Camera mainCamera;

        /******************************************* UNITY FUNCTIONS ********************************************
         * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
         * In principle, every function in this section will run once per frame, except Start().                *
         ********************************************************************************************************/
         //Update this instance.
        void Update() {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.back,
                             mainCamera.transform.rotation * Vector3.down);
        }
    }

}


