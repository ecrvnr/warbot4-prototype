using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Editeur.Interpreter;

namespace WarBotEngine.Editeur
{

    /// <summary>
	/// Condition(enter/inner) or Action(exit/outer) of a primitive
    /// </summary>
    public class PrimitiveContainer : Primitive
    {


        /*********************************
         ****** ATTRIBUTS STATIQUES ******
         *********************************/


        /// <summary>
		/// Minimal height between the last primitive and the container end
        /// </summary>
        protected static readonly int DIM_MINIMUM_SPACE = 20;


        /***********************
         ****** ATTRIBUTS ******
         ***********************/


        /// <summary>
		/// Indicate if the primitives are in enter (condition) or exit (action)
        /// </summary>
        protected bool inner_container;

        /// <summary>
		/// Intern container
        /// </summary>
        protected Container container;


        /************************
         ****** ACCESSEURS ******
         ************************/


        /// <summary>
		/// First element of the column
        /// </summary>
        public Primitive First { get { return (Primitive)this.container.Childs[0]; } }

		/// <summary>
		/// Last element of the column
		/// </summary>
		/// <value>The last.</value>
		public override Primitive Last { get{ return this.First.Last;} }

        /// <summary>
        /// Intern height
        /// </summary>
        public int InnerHeight { get { return (this.container.Active) ? this.First.TotalHeight + DIM_MINIMUM_SPACE : 0; } }

		/// <summary>
		/// Gets a value indicating whether this <see cref="WarBotEngine.Editeur.PrimitiveContainer"/> inner container.
		/// </summary>
		/// <value><c>true</c> if is inner container (condition); otherwise, <c>false (action)</c>.</value>
		public bool Inner_Container{ get{ return this.inner_container;}}

        /********************************************
         ****** METHODES SPECIFIQUES AU WIDGET ******
         ********************************************/

		/// <summary>
		/// Initializes a new instance of the <see cref="WarBotEngine.Editeur.PrimitiveContainer"/> class.
		/// </summary>
		/// <param name="pos">Position of the container.</param>
		/// <param name="name">Name.</param>
		/// <param name="parent">Parent.</param>
		/// <param name="inner">If set to <c>true</c> is inner (accept condition, otherwise action).</param>
        public PrimitiveContainer(Vector2 pos, string name, Primitive parent, bool inner)
        {
            this.inner_container = inner;
            this.area = new Rect(pos.x, pos.y, Primitive.DIM_WIDTH, 0);
            this.container = new Container(new Rect(0, 0, this.area.width, DIM_TITLE_HEIGHT + DIM_MINIMUM_SPACE));

            this.AddChild(this.container);
            Primitive begin = new Primitive(new Vector2(0, 0), name);
            this.container.AddChild(begin);
            begin.ExtendHeight += OnInnerExtend;

//            this.title = new Label(new Rect(0, 0, this.area.width, this.area.height), name);
//            this.title.Color = Color.red;
//            this.AddChild(this.title);
        }

		/// <summary>
		/// Try to place the primitive in parameter
		/// </summary>
		/// <param name="primitive">Primitive to place</param>
		/// <param name="cursor">Cursor position</param>
		/// <returns><c>true</c>, if primitive was pushed, <c>false</c> otherwise.</returns>
		public override bool PushPrimitive(Primitive primitive, Vector2 cursor)
		{
//			Debug.Log ("PushPrimitive PrimitiveContainer, inner : " + this.inner_container);
			return this.First.PushPrimitive (primitive, cursor);
		}


        /// <summary>
		/// Set the conditions
		/// </summary>
		///<param name="conditions">The list to set</param>
		public void SetConditions(List<Condition> conditions)
        {
			if (this.inner_container)
			{
				foreach (Condition c in conditions) 
				{
					Primitive p = new Primitive (new Vector2 (), c.Type (), c);
					this.Last.Next = p;
				}
			}
        }

		/// <summary>
		/// Sets the actions.
		/// </summary>
		/// <param name="actions">Actions.</param>
		public void SetActions(List<Instruction> actions)
		{
			if (!this.inner_container) {
				foreach (Instruction a in actions) {
					if (a is Action || a is If) 
					{
						Primitive p = new Primitive (new Vector2 (), a.Type (), a);
						this.Last.Next = p;
					}
				}
			}
		}
        /// <summary>
		/// Called when intern primitives extends
        /// </summary>
        /// <param name="w"></param>
        /// <param name="args"></param>
        protected void OnInnerExtend(Widget w, object args)
        {
            this.container.LocalArea = new Rect(0, this.area.height, this.area.width, this.First.TotalHeight + DIM_MINIMUM_SPACE);
            if (this.container.Active)
                this.ExtendPrimitive((int)this.container.LocalArea.height);
            else
                this.ExtendPrimitive(0);
        }

    }

}
