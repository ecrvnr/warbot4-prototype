using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Editeur.Interpreter;

namespace WarBotEngine.Editeur
{

    /// <summary>
	/// Primitive to place in the editor
    /// </summary>
    public class Primitive : Widget
    {


        /*********************************
         ****** ATTRIBUTS STATIQUES ******
         *********************************/


        /// <summary>
		/// Width of primitive
        /// </summary>
        public static readonly int DIM_WIDTH = 170;

        /// <summary>
		/// Title height of a primitive
        /// </summary>
        public static readonly int DIM_TITLE_HEIGHT = 25;

        /// <summary>
		/// Name of start primitive
        /// </summary>
        public static readonly string NAME_PRIMITIVE_BEGIN = "START";

		/// <summary>
		/// Maximal lenght-name of a primitive to avoid going out of bound
		/// </summary>
		public static readonly int MAX_NAME_LENGTH = 17;

		/// <summary>
		/// The margin for the add text icn.
		/// </summary>
		private static readonly int MARGIN_ICN = 5;

		/// <summary>
		/// The icn size.
		/// </summary>
		private static readonly int ICN_SIZE = 10;

        /// <summary>
		/// Background color
        /// </summary>
        private static readonly Color COLOR_1 = new Color((float)0xcc / 255, (float)0xcc / 255, (float)0xff / 255);
        
		/// <summary>
		/// Border color
        /// </summary>
        private static readonly Color COLOR_2 = new Color((float)0x00 / 255, (float)0x00 / 255, (float)0x00 / 255);


		public static Primitive primitiveToAssign = null;

        /***********************
         ****** ATTRIBUTS ******
         ***********************/

            
        /// <summary>
		/// Primitive title
        /// </summary>
        protected Label title;

		/// <summary>
		/// The text if the instruction needs any (Message for example).
		/// </summary>
		protected Label text;

        /// <summary>
		/// minimal height of a primitive
        /// </summary>
        protected int minimum_height = 0;

        /// <summary>
		/// Condition primitives
        /// </summary>
		protected PrimitiveContainer inner = null;

        /// <summary>
        /// Action primitives
        /// </summary>
		protected PrimitiveContainer outer = null;

		/// <summary>
		/// Else action Primitives
		/// </summary>
		protected PrimitiveContainer elseOuter = null;

        /// <summary>
        /// Next Primitive
        /// </summary>
        protected Primitive next = null;

		/// <summary>
		/// The instruction.
		/// </summary>
		protected Instruction instruction = null;


        /************************************
         ****** EVENEMENTS SPECIFIQUES ******
         ************************************/


        /// <summary>
		/// Called while primitive extend
        /// </summary>
        public event Widget.EventDelegate ExtendHeight;


        /************************
         ****** ACCESSEURS ******
         ************************/


        /// <summary>
		/// Next primitive
        /// </summary>
        public Primitive Next
        {
            get
            {
                return this.next;
            }
            set
            {
                if (this.next != null)
                {
                    this.RemoveChild(this.next);
                    if (value != null)
                        value.Last = this.next;
                }
                this.next = value;
                if (this.next != null)
                {
                    this.AddChild(value);
                    value.LocalPosition = new Vector2(0, this.LocalArea.height);
                    this.ExtendPrimitive(this.Next.TotalHeight);
                }
                else
                    this.ExtendPrimitive(0);
            }
        }

        /// <summary>
		/// Last primitive of the column
        /// </summary>
        public virtual Primitive Last
        {
            get
            {
                if (this.next != null)
                    return this.next.Last;
                else
                    return this;
            }
            set
            {
                if (this.next != null)
                    this.next.Last = value;
                else
                    this.Next = value;
            }
        }

        /// <summary>
		/// Total height of the tree
        /// </summary>
        public int TotalHeight
        {
            get
            {
                if (this.Next == null)
                    return (int)this.area.height;
                else
                    return (int)this.area.height + this.Next.TotalHeight;
            }
        }

		/// <summary>
		/// Gets or sets the optional text of the instruction if needed (message).
		/// </summary>
		/// <value>The optional text.</value>
		public string AddText
		{
			get
			{
				if (this.instruction != null && this.instruction.needTextInput ())
					return this.instruction.AddText;
				else
					return null;
			}

			set
			{
				if (this.instruction != null && this.instruction.needTextInput ()) 
				{
					//Debug.Log ("message to set : " + value);
					if (this.text != null)
						this.RemoveChild (this.text);
					this.instruction.AddText = value;
					this.text = new Label (new Rect (5, DIM_TITLE_HEIGHT + MARGIN_ICN, this.area.width - 10, (value.Length / 20 + 1) * 20), value);
					this.text.Color = COLOR_2;
					this.AddChild (this.text);
					this.minimum_height = DIM_TITLE_HEIGHT*2 + MARGIN_ICN + (int)this.text.GlobalArea.height;
					this.OnPrimitiveContainerExtend(null, null);
				}
			}
		}

        /// <summary>
		/// Total width of the tree
        /// </summary>
        public int TotalWidth
        {
            get
            {
                float res = this.area.width;
				if(this.inner != null)
					res = Mathf.Max(res, this.inner.LocalArea.x + this.inner.First.TotalWidth);
				if(this.outer != null)
					res = Mathf.Max(res, this.outer.LocalArea.x + this.outer.First.TotalWidth);
				if(this.elseOuter != null)
					res = Mathf.Max(res, this.elseOuter.LocalArea.x + this.elseOuter.First.TotalWidth);
                if (this.Next != null)
                    res = Mathf.Max(res, this.Next.TotalWidth);
                return (int)res;
            }
        }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public Label Title{get{ return title;} set{this.title = value;}}

		/// <summary>
		/// Gets or sets the instruction.
		/// </summary>
		/// <value>The instruction.</value>
		public Instruction Instruction{ 
			get{
				return this.instruction;
			} 
			set{ this.instruction = value;}
		}

		/// <summary>
		/// Gets or sets the inner.
		/// </summary>
		/// <value>The inner.</value>
		public PrimitiveContainer Inner{get{ return this.inner;} set{ this.inner = value;}}
        
		/// <summary>
		/// Gets or sets the outer.
		/// </summary>
		/// <value>The outer.</value>
		public PrimitiveContainer Outer{get{ return this.outer;} set{ this.outer = value;}}

		/// <summary>
		/// Gets or sets the else outer.
		/// </summary>
		/// <value>The else outer.</value>
		public PrimitiveContainer ElseOuter{get{ return this.elseOuter;} set{ this.elseOuter = value;}}

		/// <summary>
		/// Retrieve the first PrimitiveContainer that contains this primitive
		/// to verify if it accept conditions or actions
		/// </summary>
		/// <returns>The first PrimitiveContainer.</returns>
		public PrimitiveContainer FirstParentContainer()
		{
			Widget pc = this.Parent;
			while (pc != null && !(pc is PrimitiveContainer))
			{
				pc = pc.Parent;
			}

			return (PrimitiveContainer)pc;
		}
		/********************************************
         ****** METHODES SPECIFIQUES AU WIDGET ******
         ********************************************/


        /// <summary>
		/// Empty constructor
        /// </summary>
		protected Primitive(){}

		/// <summary>
		/// Constructor of an Anchor primitive
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="name">Name.</param>
		public Primitive(Vector2 position, string name) 
		{
			this.instruction = null;
			this.LocalArea = new Rect (position.x, position.y, DIM_WIDTH, DIM_TITLE_HEIGHT);

			this.title = new Label (new Rect (0, 0, this.area.width, DIM_TITLE_HEIGHT), name);
			this.title.Color = Color.red;
			this.AddChild (this.title);
		}

        /// <summary>
		/// Base constructor
        /// </summary>
        /// <param name="position">position of the primitive</param>
        /// <param name="name">nom of the primitive</param>
		public Primitive(Vector2 position, string name, Instruction instruction)
		{
			this.instruction = instruction;
			if (name == NAME_PRIMITIVE_BEGIN)
				this.LocalArea = new Rect (position.x, position.y, DIM_WIDTH, DIM_TITLE_HEIGHT);
			else if (instruction.GetType ().Equals (typeof(Task))) {
				Task a = (Task)instruction;
				this.LocalArea = new Rect (position.x, position.y, DIM_WIDTH, 200);

				this.inner = (new PrimitiveContainer (new Vector2 (this.area.width, 0), "CONDITION", this, true));
				this.AddChild (this.inner);
				this.inner.SetConditions (a.getConditions ());

				this.outer = (new PrimitiveContainer (new Vector2 (this.area.width, 0), "THEN", this, false));
				this.AddChild (this.outer);
				this.outer.SetActions (a.getActions ());


				this.inner.ExtendHeight += this.OnPrimitiveContainerExtend;
				this.outer.ExtendHeight += this.OnPrimitiveContainerExtend;

			} else if (instruction.GetType ().Equals (typeof(If))) {
				If a = (If)instruction;
				this.LocalArea = new Rect (position.x, position.y, DIM_WIDTH, 200);

				this.inner = (new PrimitiveContainer (new Vector2 (this.area.width, 0), "CONDITION", this, true));
				this.AddChild (this.inner);
				this.inner.SetConditions (a.getConditions ());

				this.outer = (new PrimitiveContainer (new Vector2 (this.area.width, 0), "THEN", this, false));
				this.AddChild (this.outer);
				this.outer.SetActions (a.getActions ());

				this.elseOuter = (new PrimitiveContainer (new Vector2 (this.area.width, 0), "ELSE", this, false));
				this.AddChild (this.elseOuter);
				this.elseOuter.SetActions (a.getElseActions ());


				this.inner.ExtendHeight += this.OnPrimitiveContainerExtend;
				this.outer.ExtendHeight += this.OnPrimitiveContainerExtend;
				this.elseOuter.ExtendHeight += this.OnPrimitiveContainerExtend;

			} else {
				this.LocalArea = new Rect (position.x, position.y, DIM_WIDTH, DIM_TITLE_HEIGHT * 2);
				this.minimum_height = DIM_TITLE_HEIGHT * 2;
			}

			// Handle long titles
			if (name.Length > MAX_NAME_LENGTH) 
			{
				char[] nameChar = name.ToCharArray();
				nameChar [MAX_NAME_LENGTH-1] = '.';
				string newName = new string(nameChar, 0, MAX_NAME_LENGTH);
				name = newName;
			}

			this.title = new Label (new Rect (0, 0, this.area.width, DIM_TITLE_HEIGHT), name);
			this.title.Color = COLOR_2;
			this.AddChild (this.title);

			if (this.instruction != null && this.instruction.needTextInput ()) 
			{
				string textToAdd = this.instruction.AddText;
				this.text = new Label (new Rect (0, DIM_TITLE_HEIGHT + MARGIN_ICN, this.area.width, (textToAdd.Length / 18 + 1) * 20), textToAdd);
				this.text.Color = COLOR_2;
				this.AddChild (this.text);
				this.minimum_height = DIM_TITLE_HEIGHT*2 + MARGIN_ICN + (int)this.text.GlobalArea.height;
			}

			this.OnPrimitiveContainerExtend(null, null);
        }

        /// <summary>
		/// Try to place the primitive in parameter
        /// </summary>
        /// <param name="primitive">Primitive to place</param>
        /// <param name="cursor">Cursor position</param>
		public virtual bool PushPrimitive(Primitive primitive, Vector2 cursor)
        {
//			Debug.Log ("PushPrimitive");
            if (this.GlobalArea.Contains(cursor))
            {
				PrimitiveContainer pc = this.FirstParentContainer ();
				if (pc != null) 
				{
					if (pc is PrimitiveContainer)
					{
//						Debug.Log ("est dans un PrimitiveContainer");
						if (pc.Inner_Container && primitive.Instruction is Condition) {
//							Debug.Log ("PushPrimitive condition ok");
							this.Next = primitive;

							List<Condition> conditions = new List<Condition> ();
							Primitive p = pc.First.Next;
							while (p != null) {
								conditions.Add ((Condition)p.Instruction);
								p = p.Next;
							}

							((Control)((Primitive)pc.parent).Instruction).setConditions (conditions);
						
							return true;
						} else if (!pc.Inner_Container && (primitive.Instruction is Action || primitive.Instruction is If)) {	
//							Debug.Log ("PushPrimitive action ok");
							this.Next = primitive;

							List<Instruction> actions = new List<Instruction> ();
							Primitive p = pc.First.Next;
							while (p != null) {
								if (p is Action || p is If)
									actions.Add ((Action)p.Instruction);
								p = p.Next;
							}
							if (((Primitive)pc.parent).Instruction is Task) {
								((Task)((Primitive)pc.parent).Instruction).setActions (actions);
							} else if (((Primitive)pc.parent).Instruction is If) {
								if (pc.Equals (((Primitive)pc.parent).Outer))
									((If)((Primitive)pc.parent).Instruction).setActions (actions);
								else if (pc.Equals (((Primitive)pc.parent).ElseOuter))
									((If)((Primitive)pc.parent).Instruction).setElseActions (actions);
							}

							return true;
						} 
						else
						{
//							Debug.Log ("Primitive au mauvais endroit...");
							return false;
						}
					}
				}
//				Debug.Log ("Pas dans un PrimitiveContainer PushPrimitive ok");
				this.Next = primitive;
				return true;

            }
            else
            {
                foreach (Widget w in this.childs)
                {
					if (typeof(Primitive).Equals (w.GetType ())) {
//						Debug.Log ("PushPrimitive fils ?");
						if (((Primitive)w).PushPrimitive (primitive, cursor))
							return true;
					} else if (typeof(PrimitiveContainer).Equals (w.GetType ()))
					{
//						Debug.Log ("PushPrimitiveContainer fils ?");
						if (((PrimitiveContainer)w).PushPrimitive (primitive, cursor))
							return true;
					}
				}
            }
//			Debug.Log ("Primitive tombée dans le vide...");
			return false;
        }

        /// <summary>
        /// Called when primitive extend
        /// </summary>
        /// <param name="height"></param>
        public virtual void ExtendPrimitive(int height)
        {
			if (this.Parent != null && typeof(Primitive).Equals (this.Parent.GetType ())) 
				((Primitive)this.Parent).ExtendPrimitive ((int)this.area.height + height);
            if (this.ExtendHeight != null)
                this.ExtendHeight(this, (int)this.area.height + height);
			this.Reposition ();
        }

        /// <summary>
		/// Resize the primitive (called while childs extends)
        /// </summary>
        /// <param name="w">widget</param>
        /// <param name="args">arguments</param>
        public virtual void OnPrimitiveContainerExtend(Widget w, object args)
        {
            int y_pos = DIM_TITLE_HEIGHT;
            
			if (this.inner != null)
			{
				this.inner.LocalPosition = new Vector2 (this.area.width, y_pos);
				y_pos += (int)this.inner.LocalArea.height + this.inner.InnerHeight;
			}
            
			if (this.outer != null)
			{
				this.outer.LocalPosition = new Vector2 (this.area.width, y_pos);
				y_pos += (int)this.outer.LocalArea.height + this.outer.InnerHeight;
			}
            
			if (elseOuter != null)
			{
				this.elseOuter.LocalPosition = new Vector2 (this.area.width, y_pos);
				y_pos += (int)this.elseOuter.LocalArea.height + this.elseOuter.InnerHeight;
			}

            if (y_pos < this.minimum_height)
                this.LocalArea = new Rect(this.LocalArea.x, this.LocalArea.y, this.LocalArea.width, this.minimum_height);
            else
                this.LocalArea = new Rect(this.LocalArea.x, this.LocalArea.y, this.LocalArea.width, y_pos);

            if (this.Next != null)
                this.ExtendPrimitive(this.Next.TotalHeight);
            else
                this.ExtendPrimitive(0);
        }

		/// <summary>
		/// Used to put the childs at the right place when a primitive change size
		/// </summary>
		public void Reposition()
		{
			if (this.Next != null) 
			{
				this.Next.LocalPosition = new Vector2(0, this.LocalArea.height);
				this.Next.Reposition ();
			}
		}

		/// <summary>
		/// Update the instruction of a Task to verify if its conditions or actions have changed
		/// </summary>
		public void Maj()
		{
			if (this.instruction is Task) 
			{
				List<Condition> conditions = new List<Condition> ();
				List<Instruction> actions = new List<Instruction> ();

				Primitive p = this.inner.First.Next;
				while (p != null) 
				{
					conditions.Add ((Condition)p.instruction);
					p = p.Next;
				}

				p = this.outer.First.Next;
				while (p != null) 
				{
					if (p.instruction is Action || p.instruction is If) {
						actions.Add ((Instruction)p.instruction);
					}
					p = p.Next;
				}

				((Task)this.instruction).setConditions (conditions);
				((Task)this.instruction).setActions (actions);
			}
			else if (this.instruction is If) 
			{
				List<Condition> conditions = new List<Condition> ();
				List<Instruction> actions = new List<Instruction> ();
				List<Instruction> elseActions = new List<Instruction> ();

				Primitive p = this.inner.First.Next;
				while (p != null) 
				{
					conditions.Add ((Condition)p.instruction);
					p = p.Next;
				}

				p = this.outer.First.Next;
				while (p != null) 
				{
					if (p.instruction is Action || p.instruction is If) 
					{
						actions.Add ((Instruction)p.instruction);
					}
					p = p.Next;
				}

				p = this.elseOuter.First.Next;
				while (p != null) 
				{
					if (p.instruction is Action || p.instruction is If)
					{
						elseActions.Add ((Instruction)p.instruction);
					}
					p = p.Next;
				}

				((If)this.instruction).setConditions (conditions);
				((If)this.instruction).setActions (actions);
				((If)this.instruction).setElseActions (elseActions);
			}
		}

        /***********************************
         ****** METHODES D'EVENEMENTS ******
         ***********************************/


        public override void OnDrawWithGL()
        {
            GL.Begin(GL.QUADS);
            GL.Color(COLOR_1);
            Rect rect = this.GlobalArea;
            GL.Vertex3(rect.xMin, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMax, 0);
            GL.Vertex3(rect.xMin, rect.yMax, 0);
            GL.End();
            GL.Begin(GL.LINES);
            GL.Color(Color.black);
            GL.Vertex3(rect.xMin, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMax, 0);
            GL.Vertex3(rect.xMax, rect.yMax, 0);
            GL.Vertex3(rect.xMin, rect.yMax, 0);
            GL.Vertex3(rect.xMin, rect.yMax, 0);
            GL.Vertex3(rect.xMin, rect.yMin, 0);
            GL.End();

			if (this.instruction != null && this.instruction.needTextInput ())
			{
//				Debug.Log ("Dessin du icn add text");
				GL.Begin (GL.QUADS);
				GL.Color (Color.red);
				GL.Vertex3 (rect.xMax - MARGIN_ICN - ICN_SIZE, rect.yMin + MARGIN_ICN, 0);
				GL.Vertex3 (rect.xMax - MARGIN_ICN, rect.yMin + MARGIN_ICN, 0);
				GL.Vertex3 (rect.xMax - MARGIN_ICN, rect.yMin + ICN_SIZE + MARGIN_ICN, 0);
				GL.Vertex3 (rect.xMax - MARGIN_ICN - ICN_SIZE, rect.yMin + ICN_SIZE + MARGIN_ICN, 0);
				GL.End ();
			}
        }

        public override void OnMouseEvent(int button, bool pressed, int x, int y)
        {
            base.OnMouseEvent(button, pressed, x, y);
			Rect icnArea = new Rect (this.GlobalArea.xMax - MARGIN_ICN - ICN_SIZE, this.GlobalArea.yMin + MARGIN_ICN, ICN_SIZE, ICN_SIZE);
			if(pressed && this.instruction != null && icnArea.Contains(new Vector2(x, y)))
			{
				primitiveToAssign = this;
				MainLayout.Actual.DisplayUI = true;
				MainLayout.Actual.UI_Canvas.SetActive (false);
				if(this.instruction.AddText != "")
					GameObject.Find("AdditionalText_Input").GetComponent<UnityEngine.UI.InputField>().text = this.instruction.AddText;
				return;
			}

			if (pressed && this.instruction != null && !typeof(PrimitiveContainer).Equals(this.GetType()) && this.GlobalArea.Contains(new Vector2(x, y)))
            {
                this.LocalArea = this.GlobalArea;
				((Primitive)this.Parent).Next = null;
                MainLayout.Actual.Dropper.SelectPrimitive(this);
				//MainLayout.Actual.Editor.Save ();
            }
        }

    }

}