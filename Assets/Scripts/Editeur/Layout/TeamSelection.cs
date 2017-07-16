using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WarBotEngine.WarBots;

namespace WarBotEngine.Editeur
{

    /// <summary>
	/// Selector menu for team and units
    /// </summary>
    public class TeamSelection : Container
    {


        /*********************************
         ****** ATTRIBUTS STATIQUES ******
         *********************************/

        /// <summary>
		/// menu width
        /// </summary
        public static readonly float DIM_WIDTH = 0.25f;
        /// <summary>
		/// menu height
        /// </summary>
        public static readonly float DIM_HEIGHT = 0.25f;

        /// <summary>
		/// selector width
        /// </summary>
        public static readonly float DIM_SELECTOR_WIDTH = 0.75f;
        
		/// <summary>
		/// selector height
        /// </summary>
        public static readonly int DIM_SELECTOR_HEIGHT = 30;

        /// <summary>
		/// height of the 1st drop list
        /// </summary>
        public static readonly float DIM_SELECTOR_DROP_HEIGHT_1 = 1f;
        
		/// <summary>
		/// height of the 2nd drop list
        /// </summary>
        public static readonly float DIM_SELECTOR_DROP_HEIGHT_2 = 1f;

        /// <summary>
		/// Background color
        /// </summary>
        private static readonly Color BACKGROUND_COLOR = new Color((float)0x73 / 255, (float)0x93 / 255, (float)0xa7 / 255); //#7393a7


        /***********************
         ****** ATTRIBUTS ******
         ***********************/


        /// <summary>
        /// Team selector
        /// </summary>
        private Selector team_selector;

        /// <summary>
        /// Unit selector
        /// </summary>
        private Selector unit_selector;

		/*********************
		 ***** ACCESSEURS*****
		 *********************/

		/// <summary>
		/// Gets or sets the team selector.
		/// </summary>
		/// <value>The team selector.</value>
		public Selector Team_selector{
			get
			{
				return this.team_selector;
			}
			set
			{ }
		}

		/// <summary>
		/// Gets or sets the unit selector.
		/// </summary>
		/// <value>The unit selector.</value>
		public Selector Unit_selector{
			get
			{
				return this.unit_selector;
			}
			set
			{ }
		}

        /********************************************
         ****** METHODES SPECIFIQUES AU WIDGET ******
         ********************************************/


        /// <summary>
        /// base menu constructor
        /// </summary>
		public TeamSelection() : base(new Rect(0, 0, Screen.width * DIM_WIDTH, Screen.height * DIM_HEIGHT))
        {
            this.Background = BACKGROUND_COLOR;
            team_selector = new Selector(new Rect(this.area.width * (1 - DIM_SELECTOR_WIDTH) / 2, (this.area.height - 2 * DIM_SELECTOR_HEIGHT) / 3, DIM_SELECTOR_WIDTH * this.area.width, DIM_SELECTOR_HEIGHT), (int)(DIM_SELECTOR_DROP_HEIGHT_1 * this.area.height), this);
            unit_selector = new Selector(new Rect(this.area.width * (1 - DIM_SELECTOR_WIDTH) / 2, (this.area.height - 2 * DIM_SELECTOR_HEIGHT) * 2 / 3 + DIM_SELECTOR_HEIGHT, DIM_SELECTOR_WIDTH * this.area.width, DIM_SELECTOR_HEIGHT), (int)(DIM_SELECTOR_DROP_HEIGHT_2 * this.area.height), this);

			this.AddChild(unit_selector);
            this.AddChild(team_selector);
            team_selector.SelectItem += OnSelectItem;
            team_selector.DeployOrTuck += OnDeployOrTuck;
            unit_selector.SelectItem += OnSelectItem;

            XMLInterpreter interpreter = new XMLInterpreter();

            // A SUPPRIMER
            team_selector.Elements = interpreter.allTeamsInXmlFiles(Constants.teamsDirectory).ToArray();
            
			List<string> units = new List<string> ();
			foreach (BotType t in BotType.GetValues(typeof(BotType))) 
			{
				units.Add (BotTypes.WarType (t));
			}

			unit_selector.Elements = units.ToArray();
        }

		/// <summary>
		/// Reload this instance.
		/// </summary>
        public void Reload()
        {
            XMLInterpreter interpreter = new XMLInterpreter();
            
            team_selector.Elements = interpreter.allTeamsInXmlFiles(Constants.teamsDirectory).ToArray();

            List<string> units = new List<string>();
            foreach (BotType t in BotType.GetValues(typeof(BotType)))
            {
                units.Add(BotTypes.WarType(t));
            }

            unit_selector.Elements = units.ToArray();
            if (team_selector.Elements.Length > 0)
                GameObject.Find("Button_Return").GetComponent<UnityEngine.UI.Button>().interactable = true;
        }

        /// <summary>
		/// Called while and element is selected
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="args"></param>
        void OnSelectItem(Widget widget, object args)
        {
            if (widget.ID == this.team_selector.ID)
            {
                //Sélection de l'équipe
				MainLayout.Actual.Editor = new BehaviorEditor(this.team_selector.Selection, this.Unit_selector.Selection);
            }
            else
            {
                //Sélection de l'unité
				MainLayout.Actual.Primitives_collection = new PrimitivesCollection (this.Unit_selector.Selection);
				MainLayout.Actual.Editor.Set(this.team_selector.Selection, this.Unit_selector.Selection);
            }
        }

        /// <summary>
		/// Called when selector is deployed
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="args"></param>
        void OnDeployOrTuck(Widget widget, object args)
        {
            bool state = (bool)args;
            if (widget.ID == this.team_selector.ID)
            {
                //Sélection de l'équipe
                this.unit_selector.Active = !state;
            }
        }

    }

}
