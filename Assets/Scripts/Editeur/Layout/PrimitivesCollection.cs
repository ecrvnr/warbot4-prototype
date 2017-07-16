using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Editeur.Interpreter;

namespace WarBotEngine.Editeur
{

	/// <summary>
	/// List of primitive
	/// </summary>
	public class PrimitivesCollection : Container
	{


		/*********************************
	         ****** ATTRIBUTS STATIQUES ******
	         *********************************/


		/// <summary>
		/// Height of list title
		/// </summary>
		private static readonly int DIM_TITLE_HEIGHT = 50;

		/// <summary>
		/// Background color
		/// </summary>
		private static readonly Color BACKGROUND_COLOR = new Color ((float)0xb5 / 255, (float)0xcf / 255, (float)0xd8 / 255);
		//#b5cfd8

		/// <summary>
		/// List of primitive that this unit can access
		/// </summary>
		private List<Instruction>[] instructionsByUnits;


		/************************
	         ****** ACCESSEURS ******
	         ************************/


		/// <summary>
		/// List of category
		/// </summary>
		public Category[] Categories {
			get {
				List<Category> list = new List<Category> ();
				for (int i = 1; i < this.childs.Count; i++) {
					list.Add ((Category)this.childs [i]);
				}
				return list.ToArray ();
			}
		}

		/// <summary>
		/// The Instructions selected.
		/// </summary>
		/// <returns>Instruction.</returns>
		/// <param name="category">Number of the Category.</param>
		/// <param name="selection">which item in the above category.</param>
		public Instruction InstructionSelected(int category, int selection)
		{
            List<Instruction> instructions = instructionsByUnits[category];
            if (instructions.Count <= selection)
                return null;
            else
                return instructions[selection];
		}

		/********************************************
	         ****** METHODES SPECIFIQUES AU WIDGET ******
	         ********************************************/

		/// <summary>
		/// Initializes a new instance of the <see cref="WarBotEngine.Editeur.PrimitivesCollection"/> class.
		/// </summary>
		/// <param name="unitName">Unit name.</param>
		public PrimitivesCollection (string unitName) : base (new Rect (0, Screen.height * TeamSelection.DIM_HEIGHT, Screen.width * TeamSelection.DIM_WIDTH, Screen.height * (1 - TeamSelection.DIM_HEIGHT)))
		{
			this.AllowScrollbar = true;
			this.Background = BACKGROUND_COLOR;

			instructionsByUnits = new List<Instruction>[3];

			List<Instruction> Control = new List<Instruction> ();
			Control.Add (new Task());
			Control.Add (new If());

//			Debug.Log ("Lecture des Conditions");
			// Dynamically retrieve the condition list
			List<Instruction> Condition = new List<Instruction> ();
			foreach (string s in Unit.GetConditions (unitName)) 
			{
//				Debug.Log (s);
				Condition.Add (new Condition (s));
			}
				
//			Debug.Log ("Lecture des Actions");
			List<Instruction> Action = new List<Instruction> ();
			foreach (string s in Unit.GetActions (unitName)) 
			{
//				Debug.Log (s);
				Action.Add (new Action (s));
			}

			instructionsByUnits [0] = Control;
			instructionsByUnits [1] = Condition;
			instructionsByUnits [2] = Action;
			this.AddChild (new Label (new Rect (0, 0, this.area.width, DIM_TITLE_HEIGHT), "Primitives pour " + unitName));

	           
			// Adding control list
			Category category = new Category ("Control", new Vector2 (0, DIM_TITLE_HEIGHT), (int)this.area.width - Scrollbar.DIM_WIDTH);
			category.Resize += OnCategoryResize;
			string[] Elements = new string[instructionsByUnits [0].Count];
			int cpt = 0;
			foreach (Instruction i in instructionsByUnits [0]) {
				Elements [cpt] = i.Type();

				// Resizing name
				if (Elements [cpt].Length > Primitive.MAX_NAME_LENGTH) 
				{
					char[] nameChar = Elements [cpt].ToCharArray();
					nameChar [Primitive.MAX_NAME_LENGTH-1] = '.';
					string newName = new string(nameChar, 0, Primitive.MAX_NAME_LENGTH);
					Elements [cpt] = newName;
				}
				cpt++;
			}
			category.Elements = Elements;
			this.AddChild (category);

			// Adding the condition list
			category = new Category ("Condition", new Vector2 (0, DIM_TITLE_HEIGHT + Category.DIM_ELEMENT_HEIGHT), (int)this.area.width - Scrollbar.DIM_WIDTH);
			category.Resize += OnCategoryResize;
			Elements = new string[instructionsByUnits [1].Count];
			cpt = 0;
			foreach (Instruction i in instructionsByUnits [1]) {
				Elements [cpt] = i.Type();

				// Resizing name
				if (Elements [cpt].Length > Primitive.MAX_NAME_LENGTH) 
				{
					char[] nameChar = Elements [cpt].ToCharArray();
					nameChar [Primitive.MAX_NAME_LENGTH-1] = '.';
					string newName = new string(nameChar, 0, Primitive.MAX_NAME_LENGTH);
					Elements [cpt] = newName;
				}
				cpt++;
			}
			category.Elements = Elements;
			this.AddChild (category);

			// Adding action list
			category = new Category ("Action", new Vector2 (0, DIM_TITLE_HEIGHT + Category.DIM_ELEMENT_HEIGHT*2), (int)this.area.width - Scrollbar.DIM_WIDTH);
			category.Resize += OnCategoryResize;
			Elements = new string[instructionsByUnits [2].Count];
			cpt = 0;
			foreach (Instruction i in instructionsByUnits [2]) {
				Elements [cpt] = i.Type();

				// Resizing name
				if (Elements [cpt].Length > Primitive.MAX_NAME_LENGTH) 
				{
					char[] nameChar = Elements [cpt].ToCharArray();
					nameChar [Primitive.MAX_NAME_LENGTH-1] = '.';
					string newName = new string(nameChar, 0, Primitive.MAX_NAME_LENGTH);
					Elements [cpt] = newName;
				}
				cpt++;
			}
			category.Elements = Elements;
			this.AddChild (category);


		}

		/// <summary>
		/// Called during resizing of a category
		/// </summary>
		/// <param name="widget">Widget resizing</param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void OnCategoryResize (Widget widget, int  x, int y)
		{
			for (int i = 1; i < this.childs.Count; i++) {
				this.childs [i].LocalPosition = new Vector2 (this.childs [i].LocalPosition.x, this.childs [i - 1].LocalArea.yMax);
				this.positions [i] = this.positions [i - 1] + new Vector2 (0, this.childs [i - 1].LocalArea.height);
			}
			this.scrollbar.ScrollHeight = this.childs [this.childs.Count - 1].LocalArea.yMax;
		}

		/// <summary>
		/// Overriding the inherited fonction to avoid errors
		/// </summary>
		protected override void RefreshDiplaying ()
		{
			foreach (Widget widget in this.childs) {
				widget.Active = true;
			}
		}

	}

}