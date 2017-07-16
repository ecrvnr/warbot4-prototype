using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Assets.Scripts.Editeur.Interpreter;

namespace WarBotEngine.Editeur
{

    /// <summary>
    /// Widget allowing DragAndDrop
    /// </summary>
    public class DragAndDrop : Widget
    {


        /*********************************
         ****** ATTRIBUTS STATIQUES ******
         *********************************/


        /// <summary>
		/// Shift of the primitive compared to the cursor at the creation
        /// </summary>
        private static readonly Vector2 PRIMITIVE_CURSOR_DEC = new Vector2(Primitive.DIM_WIDTH / 2, Primitive.DIM_TITLE_HEIGHT / 2);

		/// <summary>
		/// Background color for the description pop up
		/// </summary>
		private static readonly Color BACKGROUND_COLOR = new Color((float)0xfc / 255, (float)0xfc / 255, (float)0x49 / 255); //#fcfc49


        /***********************
         ****** ATTRIBUTS ******
         ***********************/


        /// <summary>
		/// Primitive list
        /// </summary>
        private PrimitivesCollection primitives;

        /// <summary>
		/// Right side editor
        /// </summary>
        private BehaviorEditor editor;

        /// <summary>
		/// Actual position of the cursor
        /// </summary>
        private Vector2 cursor = new Vector2();

        /// <summary>
		/// Position of the primitive when a click occur
        /// </summary>
        private Vector2 saved_position;

		/// <summary>
		/// Position of the cursor when a click occur
        /// </summary>
        private Vector2 saved_cursor;

		/// <summary>
		/// The created primitive waiting to be placed
		/// </summary>
		private Primitive primitiveCrée = null;

		/// <summary>
		/// Primitive description to display when hovering it
		/// </summary>
		private Label description = null;


		/**********************
		 ***** ACCESSEURS *****
		 **********************/

		/// <summary>
		/// Gets or sets the primitives collection.
		/// </summary>
		/// <value>The primitives collection.</value>
		public PrimitivesCollection Primitives_collection {
			get{ return this.primitives; } 
			set {
				this.primitives = value; 
				foreach (Category ca in primitives.Categories) {
					ca.SelectItem += OnSelectItem;
					ca.HoverItem += OnHoverItem;
				} 
			}
		}

		/// <summary>
		/// Gets or sets the editor.
		/// </summary>
		/// <value>The editor.</value>
		public BehaviorEditor Editor {	get{return this.editor;} set{this.editor = value;} }

        /********************************************
         ****** METHODES SPECIFIQUES AU WIDGET ******
         ********************************************/

		/// <summary>
		/// Initializes a new instance of the <see cref="WarBotEngine.Editeur.DragAndDrop"/> class.
		/// </summary>
		/// <param name="primitives">Primitives.</param>
		/// <param name="editor">Editor.</param>
		public DragAndDrop(PrimitivesCollection primitives, BehaviorEditor editor)
        {
            this.LocalArea = new Rect(0, 0, Screen.width, Screen.height);
            this.primitives = primitives;
            this.editor = editor;
            foreach (Category ca in primitives.Categories)
            {
                ca.SelectItem += OnSelectItem;
				ca.HoverItem += OnHoverItem;
				ca.HoverOutItem += OnHoverOutItem;
            }
        }

        /// <summary>
		/// Called when a primitive in the list is selected
        /// </summary>
        /// <param name="widget">widget</param>
        /// <param name="args">name of the primitive</param>
		private void OnSelectItem(Widget widget, object args)
        {
			// Identifying a category
			Category[] cats = this.primitives.Categories;
			string label = (string)args;
			Instruction instruction;

			int categorySelected = -1;
			int categorySelection = -1;
			foreach (Category c in cats) {
				categorySelected++;
				if (c.Selection == label) {
					categorySelection = c.EmplacementSelection; 
					break;
				}
			}

			if (categorySelected != -1) 
				instruction = this.primitives.InstructionSelected (categorySelected, categorySelection).Clone();
			else
				instruction = null;

//			Debug.Log ("Primitive sélectionnée : "+instruction.Type());

			Primitive p = new Primitive (cursor - PRIMITIVE_CURSOR_DEC, label, instruction);
            this.SelectPrimitive(p);
        }

        /// <summary>
        /// Select a created primitive
        /// </summary>
		/// <param name="p">The selected primitive</param>
        public void SelectPrimitive(Primitive p)
        {
            this.childs.Clear();
            this.AddChild(p);
            this.primitiveCrée = p;
            this.saved_position = this.primitiveCrée.GlobalPosition;
            this.saved_cursor = cursor;
        }

		/// <summary>
		/// Called while hovering an item in the list
		/// </summary>
		/// <param name="widget">Widget.</param>
		/// <param name="args">Arguments.</param>
		private void OnHoverItem(Widget widget, object args)
		{
			Event e = Event.current;

            if (this.description != null)
            {
                this.RemoveChild(this.description);
                this.description = null;
            }
			if (!e.isMouse) 
			{
				// Identify the category
				Category[] cats = this.primitives.Categories;
				string label = (string)args;
				Instruction instruction;

				int categorySelected = -1;
				int categorySelection = -1;
				foreach (Category c in cats) {
					categorySelected++;
					if (c.Selection == label) {
						categorySelection = c.EmplacementSelection; 
						break;
					}
				}

				if (categorySelected != -1)
					instruction = this.primitives.InstructionSelected (categorySelected, categorySelection);
				else {
//					Debug.Log ("Aucune instruction à décrire");
					return;
				}

                // Retrieving the text to print
                string desc = instruction.Description();
				this.description = new Label (new Rect (e.mousePosition, new Vector2 (150, (desc.Length/18 +1)* 20)), desc);
				this.description.Background = BACKGROUND_COLOR;
				this.description.Border = Color.black;
				this.AddChild (this.description);
				this.saved_position = this.description.GlobalPosition;
				this.saved_cursor = e.mousePosition;
			}
		}

		/// <summary>
		/// Called when we stop hovering an item
		/// </summary>
		private void OnHoverOutItem(Widget widget, object args)
		{
			Event e = Event.current;

			if(!e.isMouse)
			{
				this.RemoveChild (this.description);
				this.description = null;
			}
		}

        /***********************************
         ****** METHODES D'EVENEMENTS ******
         ***********************************/


        public override void OnMouseEvent(int button, bool pressed, int x, int y)
        {
			if (!pressed && this.childs.Count > 0 && primitiveCrée != null)
            {
//				Debug.Log ("Bouton relaché pour push primitive");
				this.editor.First.PushPrimitive(this.primitiveCrée, this.cursor);
				this.editor.Save ();
                this.RemoveAllChilds();
				this.primitiveCrée = null;
            }
        }

        public override void OnMotionEvent(int x, int y)
        {
            this.cursor = new Vector2(x, y);
			if (this.childs.Count > 0 && this.primitiveCrée != null) {
//				Debug.Log ("Deplacement primitive");
				this.primitiveCrée.GlobalPosition = this.saved_position + (this.cursor - this.saved_cursor);
			} 
			else if (this.childs.Count > 0 && this.description != null) 
			{
//				Debug.Log ("Deplacement description");
				this.description.GlobalPosition = this.saved_position + (this.cursor - this.saved_cursor);
			}
        }
    }

}
