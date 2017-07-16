using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.Editeur
{

    /// <summary>
	/// Printable text
    /// </summary>
	public class Label : Widget
    {


        /***********************
         ****** ATTRIBUTS ******
         ***********************/


        /// <summary>
		/// Text label
        /// </summary>
        protected string text;

        /// <summary>
		/// text height
        /// </summary>
        protected int text_size = 14;

        /// <summary>
		/// text style
        /// </summary>
        protected FontStyle text_style = FontStyle.Normal;

        /// <summary>
		/// Text alignment
        /// </summary>
        protected TextAnchor text_align = TextAnchor.MiddleCenter;

        /// <summary>
		/// Text color
        /// </summary>
        protected Color text_color = Color.black;

        /// <summary>
		/// Background color
        /// </summary>
        protected Color background_color = Color.clear;


		/// <summary>
		/// The color of the border.
		/// </summary>
		protected Color border_color = Color.clear;

        /// <summary>
        /// Margin size
        /// </summary>
        protected int margin = 0;


        /************************
         ****** ACCESSEURS ******
         ************************/

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		/// <value>The text.</value>
        public string Text { get { return this.text; } set { this.text = value; } }

		/// <summary>
		/// Gets or sets the size of the text.
		/// </summary>
		/// <value>The size of the text.</value>
        public int TextSize { get { return this.text_size; } set { this.text_size = value; } }

		/// <summary>
		/// Gets or sets the text style.
		/// </summary>
		/// <value>The text style.</value>
        public FontStyle TextStyle { get { return this.text_style; } set { this.text_style = value; } }

		/// <summary>
		/// Gets or sets the text align.
		/// </summary>
		/// <value>The text align.</value>
        public TextAnchor TextAlign { get { return this.text_align; } set { this.text_align = value; } }

		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		/// <value>The color.</value>
        public Color Color { get { return this.text_color; } set { this.text_color = value; } }

		/// <summary>
		/// Gets or sets the background.
		/// </summary>
		/// <value>The background.</value>
        public Color Background { get { return this.background_color; } set { background_color = value; } }

		/// <summary>
		/// Gets or sets the border.
		/// </summary>
		/// <value>The border.</value>
		public Color Border { get { return this.border_color; } set { border_color = value; } }

		/// <summary>
		/// Gets or sets the margin.
		/// </summary>
		/// <value>The margin.</value>
        public int Margin { get { return this.margin; } set { this.margin = value; } }


        /********************************************
         ****** METHODES SPECIFIQUES AU WIDGET ******
         ********************************************/

		/// <summary>
		/// Initializes a new instance of the <see cref="WarBotEngine.Editeur.Label"/> class.
		/// </summary>
		/// <param name="r">The zone of the label.</param>
		/// <param name="s">Text.</param>
		public Label(Rect r, string s)
		{
			this.area = r;			
			this.text = s;
		}


        /***********************************
         ****** METHODES D'EVENEMENTS ******
         ***********************************/


        public override void OnDrawWithGL()
        {
            if (!this.active) return;
            if (this.background_color != Color.clear)
            {
                GL.Begin(GL.QUADS);
                GL.Color(this.background_color);
                Rect rect = this.GlobalArea;
                GL.Vertex3(rect.xMin, rect.yMin, 0);
                GL.Vertex3(rect.xMax, rect.yMin, 0);
                GL.Vertex3(rect.xMax, rect.yMax, 0);
                GL.Vertex3(rect.xMin, rect.yMax, 0);
                GL.End();
			}
			if (this.border_color != Color.clear) 
			{
				GL.Begin (GL.LINE_STRIP);
				GL.Color (this.border_color);
				Rect rect = this.GlobalArea;
				GL.Vertex3(rect.xMin, rect.yMin, 0);
				GL.Vertex3(rect.xMax, rect.yMin, 0);
				GL.Vertex3(rect.xMax, rect.yMax, 0);
				GL.Vertex3(rect.xMin, rect.yMax, 0);
				GL.Vertex3(rect.xMin, rect.yMin, 0);
				GL.End();
			}
        }

        public override void OnDrawWithoutGL()
        {
            if (!this.active) return;
            Rect zone = this.GlobalArea;
            zone.x += this.margin;
            zone.width -= 2 * this.margin;
			GUI.color = this.text_color;

            GUIStyle style = GUI.skin.GetStyle("label");
            style.alignment = this.text_align;
            style.fontSize = this.text_size;
            style.fontStyle = this.text_style;
            GUI.Label(zone, text);
		}

	}

}