using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.Editeur {

    /// <summary>
    /// This class is used when we create a new team.
    /// </summary>
    public class CreateTeam : MonoBehaviour {

        /// <summary>
        /// Create a new team.
        /// </summary>
        /// <param name="teamName">The name of the new team.</param>
        public static void CreateNewTeam(string teamName) {
            XMLInterpreter xml = new XMLInterpreter();
            xml.generateEmptyFile(teamName, Constants.teamsDirectory);
            MainLayout.Actual.Team_selection.Reload();
            MainLayout.Actual.Team_selection.Team_selector.Selection = teamName;
            MainLayout.Actual.Editor.Set(teamName, WarBots.BotType.WarBase.ToString());
            MainLayout.Actual.Primitives_collection = new PrimitivesCollection(MainLayout.Actual.Team_selection.Unit_selector.Selection);
            MainLayout.Actual.DisplayUI = false;
            GameObject.Find("InputField_Team").GetComponent<UnityEngine.UI.InputField>().text = "";
        }
    }

}