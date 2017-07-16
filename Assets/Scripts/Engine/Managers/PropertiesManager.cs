using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

using UnityEngine;

namespace WarBotEngine.Managers
{

    /// <summary>
    /// Class permettant de lire le fichier des propriétés
    /// </summary>
    public class PropertiesManager
    {

        /// <summary>
        /// Actual properties
        /// </summary>
        protected static PropertiesManager actual = null;

        /// <summary>
        /// Actual properties
        /// </summary>
        public static PropertiesManager Actual {
            get
            {
                if (actual != null)
                    return actual;
                return actual = new PropertiesManager();
            }
        }

        /// <summary>
        /// Chemin d'accès au fichier des propriétés
        /// </summary>
        public static readonly string PROPERTIES_PATH = @"properties.yml";

        /// <summary>
        /// Class statique contenant toutes les fonctions de regex pour la lecture de fichier YAML
        /// </summary>
        public static class PropertyRegex
        {

            /// <summary>
            /// Regex constants pour le YAML
            /// </summary>

            public static readonly Regex rgxSpace = new Regex(@"^(\s)*");
            public static readonly Regex rgxUseful = new Regex(@"^\s*[\-a-zA-Z_$0-9]");
            public static readonly Regex rgxVariable = new Regex(@"^[a-zA-Z_$][\-a-zA-Z_$0-9]*");
            public static readonly Regex rgxUInt = new Regex(@"^[0-9]+");
            public static readonly Regex rgxFloat = new Regex(@"^[+-]?(([0-9]+(([eE][+-]?[0-9]+)|(\.([0-9]+([eE][+-]?[0-9]+)?)?))?)|(\.[0-9]+([eE][+-]?[0-9]+)?))");
            public static readonly Regex rgxNotValue = new Regex(@"^(\s)*$");

            /// <summary>
            /// Permet de lire les flottants au format US (avec des "." et non des ",")
            /// </summary>
            private static readonly CultureInfo usCulture = CultureInfo.CreateSpecificCulture("en-US");

            /// <summary>
            /// Détermine si la chaine de caractère contient un élément utile
            /// </summary>
            /// <param name="s">chaine à lire</param>
            /// <returns>Retourne true si c'est le cas et false sinon</returns>
            public static bool IsUseful(string s)
            {
                return rgxUseful.IsMatch(s);
            }

            /// <summary>
            /// Détermine si la chaine de caractère contient une variable
            /// </summary>
            /// <param name="s">chaine à lire</param>
            /// <returns>Retourne true si c'est le cas et false sinon</returns>
            public static bool IsVariable(string s)
            {
                return rgxVariable.IsMatch(s);
            }

            /// <summary>
            /// Détermine si la chaine de caractère contient un flottant
            /// </summary>
            /// <param name="s">chaine à lire</param>
            /// <returns>Retourne true si c'est le cas et false sinon</returns>
            public static bool IsFloat(string s)
            {
                return rgxFloat.IsMatch(s);
            }

            /// <summary>
            /// Détermine si la chaine de caractère contient une valeur
            /// </summary>
            /// <param name="s">chaine à lire</param>
            /// <returns>Retourne true si c'est le cas et false sinon</returns>
            public static bool IsValue(string s)
            {
                return !rgxNotValue.IsMatch(s);
            }

            /// <summary>
            /// Convertie une chaine de caractère en flottant
            /// </summary>
            /// <param name="s">chaine à lire</param>
            /// <returns>Retourne le flottant si réussi et NaN sinon</returns>
            public static float ConvertDouble(string s)
            {
                Thread.CurrentThread.CurrentCulture = usCulture;
                try
                {
                    return float.Parse(s);
                }
                catch (FormatException)
                {
                    return float.NaN;
                }
            }

            /// <summary>
            /// Indique le nombre d'espaces avant l'entrée à lire
            /// </summary>
            /// <param name="s">chaine à lire</param>
            /// <returns>Retourne le nombre d'espaces</returns>
            public static int GetSpaceLevel(string s)
            {
                return rgxSpace.Match(s).Length;
            }

            /// <summary>
            /// Détermine le nom de variable contenu dans la chaine
            /// </summary>
            /// <param name="s">chaine à lire</param>
            /// <returns>Retourne le nom si réussi et "" sinon</returns>
            public static string GetVariable(string s)
            {
                Match match = rgxVariable.Match(s);
                if (match.Success)
                    return match.Value;
                else
                    return "";
            }

            /// <summary>
            /// Détermine la valeur contenue dans la chaine (flottant ou chaine de caractères)
            /// </summary>
            /// <param name="s">chaine à lire</param>
            /// <returns>Retourne la valeur si réussi et null sinon</returns>
            public static object GetValue(string s)
            {
                if (IsValue(s))
                {
                    s = s.Substring(1);
                    s = s.Substring(GetSpaceLevel(s));
                    if (IsFloat(s))
                        return ConvertDouble(s);
                    else
                        return s;
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// Détermine la valeur de l'entier contenu dans la chaine
            /// </summary>
            /// <param name="s">chaine à lire</param>
            /// <returns>Retourne le nombre si réussi et 0 sinon</returns>
            public static int GetUint(string s)
            {
                Match match = rgxUInt.Match(s);
                if (match.Success)
                    return int.Parse(match.Value);
                else
                    return 0;
            }

        }

        /// <summary>
        /// Class permettant de lire et de stocker un élément
        /// </summary>
        public class PropertyElement
        {

            /// <summary>
            /// Nom de l'entrée
            /// </summary>
            private string name;
            /// <summary>
            /// Valeur de l'entrée
            /// </summary>
            private object value;

            /// <summary>
            /// Indique si l'entrée a été correctement lue
            /// </summary>
            private bool valid;

            /// <summary>
            /// Nom de l'entrée
            /// </summary>
            public string Name { get { return name; } }
            /// <summary>
            /// Type de la valeur de l'entrée
            /// </summary>
            public string Type { get { return value.GetType().ToString(); } }
            /// <summary>
            /// Valeur de l'entrée
            /// </summary>
            public object Value { get { return value; } }

            /// <summary>
            /// Indique si l'entrée a été correctement lue (sert uniquement pour le constructeur de "PropertiesManager")
            /// </summary>
            public bool Valid { get { return valid; } }

            /// <summary>
            /// Indique si la valeur est un nombre (float)
            /// </summary>
            public bool IsNumber { get { return typeof(float).Equals(value.GetType()); } }
            /// <summary>
            /// Indique si la valeur est une chaine de caractère (string)
            /// </summary>
            public bool IsString { get { return typeof(string).Equals(value.GetType()); } }
            /// <summary>
            /// Indique si la valeur est une liste (List<PropertyElement>)
            /// </summary>
            public bool IsList { get { return typeof(List<PropertyElement>).Equals(value.GetType()); } }

            /// <summary>
            /// Constructeur de la class PropertyElement
            /// </summary>
            /// <param name="syntax">une structure FIFO contenant les lignes du fichier à lire</param>
            public PropertyElement(ref Queue<string> syntax)
            {
                int space_level;

                // Initialisation
                valid = false;
                while (syntax.Count > 0 && !PropertyRegex.IsUseful(syntax.Peek()))
                    syntax.Dequeue();
                if (syntax.Count == 0) return;

                // Assignation du niveau d'espacement et du nom
                string peek = syntax.Dequeue();
                space_level = PropertyRegex.GetSpaceLevel(peek);
                peek = peek.Substring(space_level);
                if (!PropertyRegex.IsVariable(peek))
                {
                    // C'est un élément de liste
                    name = "";
                }
                else
                    name = PropertyRegex.GetVariable(peek);

                // Assignation si présente de la valeur sur la ligne
                peek = peek.Substring(name.Length);
                peek = peek.Substring(PropertyRegex.GetSpaceLevel(peek) + 1);
                if (PropertyRegex.IsValue(peek))
                {
                    value = PropertyRegex.GetValue(peek);
                    valid = true;
                    return;
                }

                // Vérification d'une valeur valide après
                while (syntax.Count > 0 && !PropertyRegex.IsUseful(syntax.Peek()))
                    syntax.Dequeue();
                if (syntax.Count == 0) return;


                // On ajoute les éléments
                List<PropertyElement> elems = new List<PropertyElement>();
                PropertyElement tmp;
                do
                {
                    peek = syntax.Peek();
                    if (PropertyRegex.GetSpaceLevel(peek) > space_level)
                    {
                        tmp = new PropertyElement(ref syntax);
                        if (!tmp.Valid) break;
                        elems.Add(tmp);
                        valid = true;
                    }
                    else
                    {
                        break;
                    }

                    while (syntax.Count > 0 && !PropertyRegex.IsUseful(syntax.Peek()))
                        syntax.Dequeue();

                } while (syntax.Count > 0);

                // On met les éléments dans la valeur
                if (valid)
                    value = elems;
            }

            /// <summary>
            /// Retourne l'élément identifié par la chaine
            /// </summary>
            /// <param name="ident">chaine identifiant</param>
            /// <returns>Retourne l'élément identifié si réussi et null sinon</returns>
            public PropertyElement GetElement(string ident)
            {
                if (ident == name) return this;
                if (ident.Length < name.Length) return null;

                string tmp = ident.Substring(0, name.Length);
                if (tmp != name) return null;

                ident = ident.Substring(name.Length);
                if (IsList)
                {
                    if (ident[0] == '.')
                    {
                        ident = ident.Substring(1);
                        string var_name = PropertyRegex.GetVariable(ident);
                        foreach (PropertyElement e in (List<PropertyElement>)value)
                        {
                            if (e.Name == var_name)
                                return e.GetElement(ident);
                        }
                        return null;
                    }
                    else if (ident[0] == '[')
                    {
                        ident = ident.Substring(1);
                        int idx = PropertyRegex.GetUint(ident);
                        ident = ident.Substring((idx / 10) + 2);
                        int it = 0;
                        foreach (PropertyElement e in (List<PropertyElement>)value)
                        {
                            if (e.Name == "" && (it++) == idx)
                                return e.GetElement(ident);
                        }
                        return null;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            /// Retourne les chaines identifiant tous les éléments
            /// </summary>
            /// <returns></returns>
            public string[] GetElements()
            {
                List<string> names = new List<string>();
                names.Add(name);
                if (IsList)
                {
                    int it = 0;
                    string[] tmp;
                    foreach (PropertyElement e in (List<PropertyElement>)value)
                    {
                        tmp = e.GetElements();
                        foreach (string s in tmp)
                        {
                            if (e.Name == "")
                                names.Add(name + "[" + it + "]" + s);
                            else
                                names.Add(name + "." + s);
                        }
                        if (e.Name == "")
                            it++;
                    }
                }
                return names.ToArray();
            }

        }

        /// <summary>
        /// Liste des éléments du fichier de propriétés
        /// </summary>
        private List<PropertyElement> E;

        /// <summary>
        /// Indique si les propriétés ont été assignées
        /// </summary>
        private bool properties_assigned = false;

        /// <summary>
        /// Indique si les propriétés ont été assignées
        /// </summary>
        public bool PropertiesAssigned { get { return properties_assigned; } }

        /// <summary>
        /// Constructeur de la class PropertiesManager
        /// </summary>
        public PropertiesManager()
        {
            PropertiesManager.actual = this;
            string fileContent;
            this.E = new List<PropertyElement>();

            if (!System.IO.File.Exists(PROPERTIES_PATH))
            {
                System.IO.StreamWriter fd = System.IO.File.CreateText(PROPERTIES_PATH);
                fd.Write(DEFAULT_PROPERTIES);
                fd.Close();
                fileContent = DEFAULT_PROPERTIES;
                Debug.Log("Création du fichier de propriétés...");
            }
            else
            {
                System.IO.StreamReader fd = System.IO.File.OpenText(PROPERTIES_PATH);
                fileContent = fd.ReadToEnd();
                fd.Close();
                Debug.Log("Lecture du fichier de propriétés...");
            }

            PropertyElement tmp;
            Queue<string> Q = new Queue<string>(fileContent.Split('\n'));
            while (Q.Count > 0)
            {
                tmp = new PropertyElement(ref Q);
                if (tmp.Valid)
                    this.E.Add(tmp);
            }

        }

        /// <summary>
        /// Détermine si l'élément identifié existe
        /// </summary>
        /// <param name="name">nom de l'élément à identifier</param>
        /// <returns>Retourne true s'il existe et false sinon</returns>
        public bool ElementExist(string name)
        {
            return GetElement(name) != null;
        }

        /// <summary>
        /// Permet de récupérer un élément par son nom
        /// </summary>
        /// <param name="name">nom de l'élément à identifier</param>
        /// <returns>Retourne l'élément s'il existe et null sinon</returns>
        public PropertyElement GetElement(string name)
        {
            string var_name = PropertyRegex.GetVariable(name);
            foreach (PropertyElement e in this.E)
            {
                if (var_name == e.Name)
                    return (PropertyElement)e.GetElement(name);
            }
            return null;
        }

        /// <summary>
        /// Permet de récupérer la liste des noms de tous les éléments du fichier de propriétés
        /// </summary>
        /// <returns>Retourne la liste des noms</returns>
        public string[] GetElementNames()
        {
            List<string> names = new List<string>();
            foreach (PropertyElement e in this.E)
            {
                names.AddRange(e.GetElements());
            }
            return names.ToArray();
        }

        /// <summary>
        /// Assigne les propriétés de chaque unités
        /// </summary>
        /// <param name="return_on_throw">Arrète la fonction si une valeur n'est pas valide sinon recrée le fichier et retente l'assignation</param>
        public void AssignProperties(bool return_on_throw = false)
        {
            Dictionary<string, PropertyElement> elems = new Dictionary<string, PropertyElement>();
            
            ////// UNITS //////

            //WarBase
            elems.Add("Units.WarBase.MaxHealth", this.GetElement("Units.WarBase.MaxHealth"));
            elems.Add("Units.WarBase.MaxInventory", this.GetElement("Units.WarBase.MaxInventory"));
            elems.Add("Units.WarBase.PerceptionRadius", this.GetElement("Units.WarBase.PerceptionRadius"));
            elems.Add("Units.WarBase.SpawnDelay", this.GetElement("Units.WarBase.SpawnDelay"));
            //WarTurret
            elems.Add("Units.WarTurret.MaxHealth", this.GetElement("Units.WarTurret.MaxHealth"));
            elems.Add("Units.WarTurret.MaxInventory", this.GetElement("Units.WarTurret.MaxInventory"));
            elems.Add("Units.WarTurret.PerceptionRadius", this.GetElement("Units.WarTurret.PerceptionRadius"));
            elems.Add("Units.WarTurret.ReloadTime", this.GetElement("Units.WarTurret.ReloadTime"));
            elems.Add("Units.WarTurret.Cost", this.GetElement("Units.WarTurret.Cost"));
            //WarExplorer
            elems.Add("Units.WarExplorer.MaxHealth", this.GetElement("Units.WarExplorer.MaxHealth"));
            elems.Add("Units.WarExplorer.MaxInventory", this.GetElement("Units.WarExplorer.MaxInventory"));
            elems.Add("Units.WarExplorer.PerceptionRadius", this.GetElement("Units.WarExplorer.PerceptionRadius"));
            elems.Add("Units.WarExplorer.Speed", this.GetElement("Units.WarExplorer.Speed"));
            elems.Add("Units.WarExplorer.Cost", this.GetElement("Units.WarExplorer.Cost"));
            //WarEngineer
            elems.Add("Units.WarEngineer.MaxHealth", this.GetElement("Units.WarEngineer.MaxHealth"));
            elems.Add("Units.WarEngineer.MaxInventory", this.GetElement("Units.WarEngineer.MaxInventory"));
            elems.Add("Units.WarEngineer.PerceptionRadius", this.GetElement("Units.WarEngineer.PerceptionRadius"));
            elems.Add("Units.WarEngineer.Speed", this.GetElement("Units.WarEngineer.Speed"));
            elems.Add("Units.WarEngineer.SpawnDelay", this.GetElement("Units.WarEngineer.SpawnDelay"));
            elems.Add("Units.WarEngineer.Cost", this.GetElement("Units.WarEngineer.Cost"));
            //WarHeavy
            elems.Add("Units.WarHeavy.MaxHealth", this.GetElement("Units.WarHeavy.MaxHealth"));
            elems.Add("Units.WarHeavy.MaxInventory", this.GetElement("Units.WarHeavy.MaxInventory"));
            elems.Add("Units.WarHeavy.PerceptionRadius", this.GetElement("Units.WarHeavy.PerceptionRadius"));
            elems.Add("Units.WarHeavy.Speed", this.GetElement("Units.WarHeavy.Speed"));
            elems.Add("Units.WarHeavy.ReloadTime", this.GetElement("Units.WarHeavy.ReloadTime"));
            elems.Add("Units.WarHeavy.Cost", this.GetElement("Units.WarHeavy.Cost"));

            ////// RESSOURCES //////

            elems.Add("Ressources.HealCost", this.GetElement("Ressources.HealCost"));
            elems.Add("Ressources.HealValue", this.GetElement("Ressources.HealValue"));
            elems.Add("Ressources.TakeCount", this.GetElement("Ressources.TakeCount"));
            elems.Add("Ressources.GiveCount", this.GetElement("Ressources.GiveCount"));
            elems.Add("Ressources.GiveDistance", this.GetElement("Ressources.GiveDistance"));
            elems.Add("Ressources.MaxRessources", this.GetElement("Ressources.MaxRessources"));

            foreach (PropertyElement e in elems.Values)
            {
                if (e == null || !e.IsNumber)
                    goto yml_error;
            }
            
            ////// UNITS //////

            //WarBase
            WarBots.WarBase.MaxHealth = (float)elems["Units.WarBase.MaxHealth"].Value;
            WarBots.WarBase.MaxInventory = (int)(float)elems["Units.WarBase.MaxInventory"].Value;
            WarBots.WarBase.PerceptionRadius = (float)elems["Units.WarBase.PerceptionRadius"].Value;
            WarBots.WarBase.SpawnDelay = (float)elems["Units.WarBase.SpawnDelay"].Value;
            //WarTurret
            WarBots.WarTurret.MaxHealth = (float)elems["Units.WarTurret.MaxHealth"].Value;
            WarBots.WarTurret.MaxInventory = (int)(float)elems["Units.WarTurret.MaxInventory"].Value;
            WarBots.WarTurret.PerceptionRadius = (float)elems["Units.WarTurret.PerceptionRadius"].Value;
            WarBots.WarTurret.ReloadTime = (float)elems["Units.WarTurret.ReloadTime"].Value;
            //WarExplorer
            WarBots.WarExplorer.MaxHealth = (float)elems["Units.WarExplorer.MaxHealth"].Value;
            WarBots.WarExplorer.MaxInventory = (int)(float)elems["Units.WarExplorer.MaxInventory"].Value;
            WarBots.WarExplorer.PerceptionRadius = (float)elems["Units.WarExplorer.PerceptionRadius"].Value;
            WarBots.WarExplorer.Speed = (float)elems["Units.WarExplorer.Speed"].Value;
            //WarEngineer
            WarBots.WarEngineer.MaxHealth = (float)elems["Units.WarEngineer.MaxHealth"].Value;
            WarBots.WarEngineer.MaxInventory = (int)(float)elems["Units.WarEngineer.MaxInventory"].Value;
            WarBots.WarEngineer.PerceptionRadius = (float)elems["Units.WarEngineer.PerceptionRadius"].Value;
            WarBots.WarEngineer.Speed = (float)elems["Units.WarEngineer.Speed"].Value;
            WarBots.WarEngineer.SpawnDelay = (float)elems["Units.WarEngineer.SpawnDelay"].Value;
            //WarHeavy
            WarBots.WarHeavy.MaxHealth = (float)elems["Units.WarHeavy.MaxHealth"].Value;
            WarBots.WarHeavy.MaxInventory = (int)(float)elems["Units.WarHeavy.MaxInventory"].Value;
            WarBots.WarHeavy.PerceptionRadius = (float)elems["Units.WarHeavy.PerceptionRadius"].Value;
            WarBots.WarHeavy.Speed = (float)elems["Units.WarHeavy.Speed"].Value;
            WarBots.WarHeavy.ReloadTime = (float)elems["Units.WarHeavy.ReloadTime"].Value;

            ////// RESSOURCES //////

            WarBots.InventoryController.HealCost = (int)(float)elems["Ressources.HealCost"].Value;
            WarBots.InventoryController.HealValue = (int)(float)elems["Ressources.HealValue"].Value;
            WarBots.InventoryController.TakeCount = (int)(float)elems["Ressources.TakeCount"].Value;
            WarBots.InventoryController.GiveCount = (int)(float)elems["Ressources.GiveCount"].Value;
            WarBots.InventoryController.GiveDistance = (float)elems["Ressources.GiveDistance"].Value;

            Items.WarResource.MaxRessources = (int)(float)elems["Ressources.MaxRessources"].Value;

            this.properties_assigned = true;
            return;
        yml_error:

            System.IO.StreamWriter fd = System.IO.File.CreateText(PROPERTIES_PATH);
            fd.Write(DEFAULT_PROPERTIES);
            fd.Close();
            Debug.Log("Recréation des propriétés...");
            new PropertiesManager().AssignProperties(true);
        }

        /// <summary>
        /// Fichier de propriétés par défaut
        /// </summary>
        private static readonly string DEFAULT_PROPERTIES = @"
#### FICHIER DE CONFIGURATION WARBOT ####

Units:
    WarBase:
        MaxHealth: 300
        MaxInventory: 200
        PerceptionRadius: 120
        SpawnDelay: 1.5
    WarTurret:
        MaxHealth: 30
        MaxInventory: 20
        PerceptionRadius: 70
        ReloadTime: 0.1
        Cost: 20
    WarExplorer:
        MaxHealth: 45
        MaxInventory: 100
        PerceptionRadius: 80
        Speed: 65
        Cost: 10
    WarEngineer:
        MaxHealth: 80
        MaxInventory: 100
        PerceptionRadius: 50
        Speed: 30
        SpawnDelay: 4.0
        Cost: 15
    WarHeavy:
        MaxHealth: 100
        MaxInventory: 50
        PerceptionRadius: 85
        Speed: 55
        ReloadTime: 1.0
        Cost: 30
Ressources:
    HealCost: 30
    HealValue: 20
    TakeCount: 20
    GiveCount: 20
    GiveDistance: 30
    MaxRessources: 20

# Exemple de code YAML

#var1: -2.5e3
#var2: chaine de caractere
#var3:
#    var31: 1
#    var32: 2
#    var33: 3
#var4:
#    - 1
#    - 2
#    - 3
#var5:
#    -
#        - 11
#        - 12
#    -
#        - 21
#        - 22
#    length: 2
";

    }

}
