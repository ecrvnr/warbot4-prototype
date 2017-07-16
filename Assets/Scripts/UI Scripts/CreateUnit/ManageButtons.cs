using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarBotEngine.UI {
    /// <summary>
    /// This class is used to deactivate Unit Creation buttons in the unit creation pannel.
    /// It also disable the creation when the pannel is exited.
    /// </summary>
    public class ManageButtons : MonoBehaviour {

        /******************************************* UNITY FUNCTIONS ********************************************
         * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
         * In principle, every function in this section will run once per frame, except Start().                *
         ********************************************************************************************************/
        void OnEnable() {
            UnitCreation.toggle_creation = false; 
            UnitCreation.SetButtonActivable(false);
        }

        void OnDisable() {
            UnitCreation.toggle_creation = false;
        }

    }

}
