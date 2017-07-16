using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.Editeur
{

    /// <summary>
	/// Selector in shape of a category 
    /// </summary>
    public class Category : Widget
    {


        /*********************************
         ****** ATTRIBUTS STATIQUES ******
         *********************************/


        /// <summary>
		/// element height
        /// </summary>
        public static readonly int DIM_ELEMENT_HEIGHT = 22;

        /// <summary>
		/// Margin of the category title
        /// </summary>
        private static readonly int DIM_MARGIN = 20;

        /// <summary>
		/// element margin
        /// </summary>
        private static readonly int DIM_ELEMENT_MARGIN = 30;

        /// <summary>
		/// Height of a text
        /// </summary>
        private static readonly int DIM_TEXT_SIZE = 14;

        /// <summary>
		/// Triangle dimension
        /// </summary>
        private static readonly float DIM_TRIANGLE_SIZE = 0.5f;

        /// <summary>
		/// Category title style
        /// </summary>
        private static readonly FontStyle LABEL_FONTSTYLE = FontStyle.Bold;


        /***********************
         ****** ATTRIBUTS ******
         ***********************/


        /// <summary>
		/// Category title
        /// </summary>
        protected Label label;

        /// <summary>
		/// Actual selection
        /// </summary>
        protected int selection = -1;

        /// <summary>
		/// Element container
        /// </summary>
        protected Container container;


        /************************************
         ****** EVENEMENTS SPECIFIQUES ******
         ************************************/


        /// <summary>
		/// Called when an item is selected
        /// </summary>
        public event Widget.EventDelegate SelectItem = null;

		/// <summary>
		/// Called when an item is hovered
		/// </summary>
		public event Widget.EventDelegate HoverItem = null;

		/// <summary>
		/// Called when an item is not hovered anymore
		/// </summary>
		public event Widget.EventDelegate HoverOutItem = null;

        /// <summary>
		/// Called when deploying or tucking the selector
        /// </summary>
        public event Widget.EventDelegate DeployOrTuck = null;


        /************************
         ****** ACCESSEURS ******
         ************************/

		/// <summary>
		/// Gets the emplacement selection.
		/// </summary>
		/// <value>The emplacement selection.</value>
		public int EmplacementSelection{get{ return this.selection; }}

        /// <summary>
        /// Selector's list of element
        /// </summary>
        public string[] Elements
        {
            get
            {
                int size = this.container.Childs.Length;
                string[] res = new string[size];
                for (int i = 0; i < size; i++)
                {
                    res[i] = ((Label)this.container.Childs[i]).Text;
                }
                return res;
            }
            set
            {
                this.container.RemoveAllChilds();
                for (int i = 0; i < value.Length; i++)
                {
                    Label label = new Label(new Rect(0, i * DIM_ELEMENT_HEIGHT, this.area.width - Scrollbar.DIM_WIDTH, DIM_ELEMENT_HEIGHT), value[i]);
                    label.TextAlign = TextAnchor.MiddleLeft;
                    label.Margin = DIM_ELEMENT_MARGIN;
                    label.Clic += OnSelect;
					label.CursorEnter += OnHover;
					label.CursorExit += OnHoverOut;
                    this.container.AddChild(label);
                }
                if (value.Length == 0) this.container.LocalArea = new Rect(0, this.area.height, this.area.width, DIM_ELEMENT_HEIGHT);
                else this.container.LocalArea = new Rect(0, this.area.height, this.area.width, value.Length * DIM_ELEMENT_HEIGHT + DIM_TEXT_SIZE);

                if (value.Length == 0) this.selection = -1;
                else this.selection = 0;
            }
        }

        /// <summary>
		/// Actual selection
        /// </summary>
        public string Selection
        {
            get
            {
                if (this.selection == -1)
                    return "";
                else
                    return this.Elements[this.selection];
            }
            set
            {
                int size = this.childs[0].Childs.Length;
                for (int i = 0; i < size; i++)
                {
                    if (((Label)this.container.Childs[i]).Text == value)
                    {
                        this.selection = i;
                        return;
                    }
                }
                this.selection = -1;
            }
        }


        /********************************************
         ****** METHODES SPECIFIQUES AU WIDGET ******
         ********************************************/

		/// <summary>
		/// Initializes a new instance of the <see cref="WarBotEngine.Editeur.Category"/> class.
		/// </summary>
		/// <param name="name">Title of the category.</param>
		/// <param name="pos">Position or the category.</param>
		/// <param name="width">Width of the category.</param>
		public Category(string name, Vector2 pos, int width)
        {
            this.area = new Rect(pos.x, pos.y, width, DIM_ELEMENT_HEIGHT);
            this.container = new Container(new Rect(0, this.area.height, this.area.width, 0));
            this.AddChild(this.container);
            this.label = new Label(new Rect(0, 0, this.area.width, this.area.height), name);
            this.AddChild(this.label);

            this.container.Parent = this;
            this.container.Active = false;

            this.label.Parent = this;
            this.label.Margin = DIM_MARGIN;
            this.label.TextAlign = TextAnchor.MiddleLeft;
            this.label.TextSize = DIM_TEXT_SIZE;
            this.label.TextStyle = LABEL_FONTSTYLE;
            this.label.Clic += OnLabelClic;
        }

        /// <summary>
		/// Called when the principal label is clicked
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="button">mouse button</param>
        /// <param name="x">coordinates x</param>
        /// <param name="y">coordinates y</param>
        protected void OnLabelClic(Widget widget, int button, int x, int y)
        {
            this.Deploy(!this.container.Active);
        }

        /// <summary>
		/// Called when an element of the selector is clicked
        /// </summary>
        /// <param name="widget">widget</param>
        /// <param name="button">mouse button</param>
        /// <param name="x">coordinates x</param>
        /// <param name="y">coordinates y</param>
        protected void OnSelect(Widget widget, int button, int x, int y)
        {
            this.Selection = ((Label)widget).Text;
			if (this.SelectItem != null) {
				string args = this.Selection;
				SelectItem (this, args);
			}
        }

		/// <summary>
		/// Called when an element is hovered
		/// </summary>
		/// <param name="widget">Widget.</param>
		protected void OnHover(Widget widget)
		{
			this.Selection = ((Label)widget).Text;
//			Debug.Log ("OnHover "+this.Selection);
			if (this.HoverItem != null) 
			{
				string args = this.Selection;
				HoverItem (widget, args);
			}
		}

		/// <summary>
		/// Called when an element is not hovered anymore
		/// </summary>
		/// <param name="widget">Widget.</param>
		protected void OnHoverOut(Widget widget)
		{
			if (this.HoverOutItem != null) 
			{
				HoverOutItem (widget, null);
			}
		}

        /// <summary>
		/// Deploy the selector
        /// </summary>
        public void Deploy()
        {
            if (this.container.Active == true) return;
            this.container.Active = true;
            if (this.DeployOrTuck != null)
                this.DeployOrTuck(this, true);
            this.LocalArea = new Rect(this.area.x, this.area.y, this.area.width, this.area.height + this.container.LocalArea.height);
        }

        /// <summary>
		/// Deploy or tuck the selector
        /// </summary>
        /// <param name="state">Selector state</param>
        public void Deploy(bool state)
        {
            if (this.container.Active == state) return;
            if (state)
                this.Deploy();
            else
                this.Tuck();
        }

        /// <summary>
        /// Tuck the selector
        /// </summary>
        public void Tuck()
        {
            if (this.container.Active == false) return;
            this.container.Active = false;
            if (this.DeployOrTuck != null)
                this.DeployOrTuck(this, false);
            this.LocalArea = new Rect(this.area.x, this.area.y, this.area.width, this.label.LocalArea.height);
        }

        public override void OnDrawWithGL()
        {
            base.OnDrawWithGL();

            Rect rect = this.label.GlobalArea;
            GL.Begin(GL.TRIANGLES);
            GL.Color(Color.black);
            if (this.container.Active)
            {
                //Flèche "bas"
                rect.width = DIM_TRIANGLE_SIZE * this.label.LocalArea.height;
                rect.height = 0.7f * rect.width;
                rect.x += (this.label.LocalArea.height - rect.width) / 2;
                rect.y += (this.label.LocalArea.height - rect.height) / 2;
                GL.Vertex3(rect.xMin, rect.yMin, 0);
                GL.Vertex3((rect.xMin + rect.xMax) / 2, rect.yMax, 0);
                GL.Vertex3(rect.xMax, rect.yMin, 0);
            }
            else
            {
                //Flèche "droite"
                rect.height = DIM_TRIANGLE_SIZE * this.label.LocalArea.height;
                rect.width = 0.7f * rect.height;
                rect.x += (this.label.LocalArea.height - rect.width) / 2;
                rect.y += (this.label.LocalArea.height - rect.height) / 2;
                GL.Vertex3(rect.xMin, rect.yMin, 0);
                GL.Vertex3(rect.xMax, (rect.yMin + rect.yMax) / 2, 0);
                GL.Vertex3(rect.xMin, rect.yMax, 0);
            }
            GL.End();
        }

    }

}
