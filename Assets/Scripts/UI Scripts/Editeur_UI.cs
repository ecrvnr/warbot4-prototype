using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using WarBotEngine.Editeur;
using UnityEngine.UI;

namespace WarBotEngine.UI {
    /// <summary>
    /// This class handle the GUI for the editor.
    /// </summary>
    public class Editeur_UI : MonoBehaviour {

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
         ********************************************************************************************/

        /// <summary>
        /// Toggle the menu.
        /// </summary>
        private bool toggle_Menu;

		/// <summary>
		/// The toggle text.
		/// </summary>
		private bool toggle_Text;

        /// <summary>
        /// Hold the UI GameOjects
        /// </summary>
        private GameObject  Panel_Blur,
                            Panel_Menu,
                            Panel_Creation_equipe,
							Panel_Text;

        /// <summary>
        /// Validation text when the user create a new team.
        /// </summary>
        private Text Validation;

        /******************************************* UNITY FUNCTIONS ********************************************
         * This section holds all functions strictly related to Unity, such as updates, collision detection etc.* 
         * In principle, every function in this section will run once per frame, except Start().                *
        **********************************************************************************************************/

        // Use this for initialization
        void Start() {
            toggle_Menu = false; 
			toggle_Text = false;
            Panel_Blur = GameObject.Find("Panel_Blur");
            Panel_Menu = GameObject.Find("Panel_MenuEsc");
            Panel_Creation_equipe = GameObject.Find("Panel_Create_NewTeam");

			Panel_Text = GameObject.Find ("Panel_Text");
        }

        // Update is called once per frame
        void Update() {
        }

        /******************************************** GUI FUNCTIONS *********************************************
         * This section holds all functions for the user interface.                                             *
         ********************************************************************************************************/

        /// <summary>
        /// Toggle the menu on button click.
        /// </summary>
        public void Toggle_Menu() {
            MainLayout.Actual.DisplayUI = false;
            MainLayout.Actual.UI_Text.SetActive(true);
        }


        /// <summary>
        /// Get the new team name from the text input field.
        /// </summary>
        public void GetNewTeamName() {
            string text = Panel_Creation_equipe.GetComponentInChildren<InputField>().text;

            if (text != "") {
                //Replace non-ASCII caracters by empty ones.
                string teamName = Encoding.ASCII.GetString(Encoding.Convert(Encoding.UTF8,
                                                                            Encoding.GetEncoding(
                                                                                                 Encoding.ASCII.EncodingName,
                                                                                                 new EncoderReplacementFallback(string.Empty),
                                                                                                 new DecoderExceptionFallback()
                                                                                                ),
                                                                            Encoding.UTF8.GetBytes(text)
                                                                           )
                                                          );
                CreateTeam.CreateNewTeam(teamName);
                Editeur.MainLayout.Actual.UI_Text.SetActive(true);
                Editeur.MainLayout.Actual.DisplayUI = false;
            }
            
        }

		/// <summary>
		/// Assign the text value from the Panel_Text.
		/// </summary>
		public void AssignTextValue()
		{
			string text = Panel_Text.GetComponentInChildren<InputField>().text;

			if (text != "") {
				//Replace non-ASCII caracters by empty ones.
				string value = Encoding.ASCII.GetString(Encoding.Convert(Encoding.UTF8,
					Encoding.GetEncoding(
						Encoding.ASCII.EncodingName,
						new EncoderReplacementFallback(string.Empty),
						new DecoderExceptionFallback()
					),
					Encoding.UTF8.GetBytes(text)
				)
				);
				if (Primitive.primitiveToAssign != null) 
				{
					Primitive.primitiveToAssign.AddText = value;
					Primitive.primitiveToAssign = null;
                    MainLayout.Actual.Editor.Save();
				}
			}
			MainLayout.Actual.UI_Canvas.SetActive (true);
			MainLayout.Actual.DisplayUI = false;
		}

    }

}
