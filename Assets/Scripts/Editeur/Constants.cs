using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.Editeur
{
    public class Constants
    {
        // Path to the directory containing all xml files
        public static readonly string teamsDirectory = "./teams/";

        // files extension
        public static readonly string xmlExtension = ".wbt";

        // nodes names
        public static readonly string nodeContainer = "behavior";
        public static readonly string nodeTeam = "teamName";
        public static readonly string nodeUnit = "unit";

        // attributes names
        public static readonly string attributeName = "name";

    }
}
