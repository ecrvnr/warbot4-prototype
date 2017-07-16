using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WarBotEngine.UI {
    /// <summary>
    /// This class is used in the Editor. When we want to set the text of the Additional Text Input from the one in the button.
    /// </summary>
    public class AddTextFromButton : MonoBehaviour {

        public void SetText(string text) {
            GameObject.Find("AdditionalText_Input").GetComponent<InputField>().text += text;
        }

    }
}

