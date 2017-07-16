using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WarBotEngine.WarBots {

    /// <summary>
    /// Unit types
    /// </summary>
    [Flags]
    public enum BotType {
        WarBase = 0,
        WarExplorer = 1,
        WarHeavy = 2,
        WarEngineer = 3,
        WarTurret = 4
    }

    /// <summary>
    /// Extension methods for the BotType enum
    /// </summary>
    public static class BotTypes {

        /******************************************** OTHER FUNCTIONS ***************************************************
		 * This section holds all other functions that might be called by the primitives to retrieve values for example.* 
		 * In principle, every function in this section should only be called from within primitives.                   *
		 ****************************************************************************************************************/

        /// <summary>
        /// Retourne un GameObject à partir de son type d'unité
        /// </summary>
        /// <param name="type">type d'unité</param>
        /// <returns>GameObject de l'unité</returns>
        public static GameObject PrefabByType(this BotType type) {
            switch (type) {
                case BotType.WarBase:
                    return Resources.Load("Prefabs/Units/WarBase") as GameObject;
                case BotType.WarExplorer:
                    return Resources.Load("Prefabs/Units/WarExplorer") as GameObject;
                case BotType.WarHeavy:
                    return Resources.Load("Prefabs/Units/WarHeavy") as GameObject;
                case BotType.WarTurret:
                    return Resources.Load("Prefabs/Units/WarTurret") as GameObject;
                case BotType.WarEngineer:
                    return Resources.Load("Prefabs/Units/WarEngineer") as GameObject;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns a random type of bot to spawn.
        /// </summary>
        /// <returns></returns>
        public static BotType RandomType() {
            return (BotType)Random.Range(1 , 4);
        }

        /// <summary>
        /// Returns the cost of the specified unit type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static float UnitCost(this BotType type) {
            switch (type) {
                case BotType.WarEngineer:
                    return (float)Managers.PropertiesManager.Actual.GetElement("Units.WarEngineer.Cost").Value;
                case BotType.WarExplorer:
                    return (float)Managers.PropertiesManager.Actual.GetElement("Units.WarExplorer.Cost").Value;
                case BotType.WarHeavy:
                    return (float)Managers.PropertiesManager.Actual.GetElement("Units.WarHeavy.Cost").Value;
                case BotType.WarTurret:
                    return (float)Managers.PropertiesManager.Actual.GetElement("Units.WarTurret.Cost").Value;
                default:
                    return 0.0f;
            }
        }

        /// <summary>
        /// Return a string of the current BotType.
        /// </summary>
        public static String WarType(this BotType type) {
            switch (type) {
                case BotType.WarBase: return "WarBase";
                case BotType.WarExplorer: return "WarExplorer";
                case BotType.WarHeavy: return "WarHeavy";
                case BotType.WarEngineer: return "WarEngineer";
                case BotType.WarTurret: return "WarTurret";
                default: return "Error";
            }

        }
    }
}