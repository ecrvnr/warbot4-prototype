using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.Editeur
{

    /// <summary>
    /// Scollbar verticale
    /// </summary>
    public class Scrollbar : Widget
    {

        /*********************************
         ****** ATTRIBUTS STATIQUES ******
         *********************************/


        /// <summary>
        /// Largeur d'une scrollbar
        /// </summary>
        public static readonly int DIM_WIDTH = 20;

        /// <summary>
        /// Largeur de trait
        /// </summary>
        private static readonly int DIM_MARGIN = 2;
        /// <summary>
        /// Dimension des triangles
        /// </summary>
        private static readonly float DIM_TRIANGLE_SIZE = 0.8f;

        /// <summary>
        /// Couleur 1
        /// </summary>
        private static readonly Color COLOR_1 = new Color((float)0x07 / 255, (float)0x0f / 255, (float)0x4e / 255);
        /// <summary>
        /// Couleur 2
        /// </summary>
        private static readonly Color COLOR_2 = new Color((float)0x2c / 255, (float)0x3e / 255, (float)0x50 / 255);
        /// <summary>
        /// Couleur 3
        /// </summary>
        private static readonly Color COLOR_3 = new Color((float)0xbd / 255, (float)0xc3 / 255, (float)0xc7 / 255);
        /// <summary>
        /// Couleur 4
        /// </summary>
        private static readonly Color COLOR_4 = new Color((float)0xec / 255, (float)0xf0 / 255, (float)0xf1 / 255);

        /// <summary>
        /// Vitesse de scrolling en utilisant le bouton de scroll de la souris
        /// </summary>
        private static readonly float SCROLL_SPEED = 10.0f;

        /// <summary>
        /// Vitesse de scrolling lors du clic sur les boutons "haut" et "bas"
        /// </summary>
        private static readonly float SCROLL_CLIC_SPEED = 200.0f;


        /***********************
         ****** ATTRIBUTS ******
         ***********************/


        /// <summary>
        /// Hauteur interne
        /// </summary>
        private float scroll_height;

        /// <summary>
        /// Valeur actuelle
        /// </summary>
        private float actual_value = 0;

        /// <summary>
        /// Evènement appelé lors du changement de la valeur actuelle
        /// </summary>
        private event EventDelegate on_changevalue;

        //ATTRIBUTS D'EVENEMENTS

        /// <summary>
        /// Indique si le curseur est au-dessus de la scrollbar
        /// </summary>
        private bool is_hover = false;

        /// <summary>
        /// Indique si la scrollbar a été cliquée
        /// </summary>
        private bool is_clicked = false;

        /// <summary>
        /// Position du curseur au moment du clic
        /// </summary>
        private Vector2 clic_position = new Vector2();

        /// <summary>
        /// Valeur de la scrollbar au moment du clic
        /// </summary>
        private float clic_value = 0;

        /// <summary>
        /// Position actuelle du curseur
        /// </summary>
        private Vector2 mouse_position = new Vector2();

        /// <summary>
        /// Indique si le scrolling est en cours
        /// </summary>
        private bool is_scrolling = false;

        /// <summary>
        /// Direction du scrolling
        /// </summary>
        private float scroll_direction = 0f;


        /************************
         ****** ACCESSEURS ******
         ************************/


        /// <summary>
        /// Hauteur totale du conteneur
        /// </summary>
        public float ScrollHeight {
            get
            {
                return scroll_height;
            }
            set
            {
                scroll_height = value;
                if (actual_value > scroll_height - this.area.height)
                    CurrentValue = scroll_height - this.area.height;
                if (actual_value < 0)
                    CurrentValue = 0;
            }
        }

        /// <summary>
        /// Position verticale actuelle
        /// </summary>
        public float CurrentValue
        {
            get
            {
                return actual_value;
            }
            set
            {
                float current = actual_value;
                actual_value = value;
                if (scroll_height - area.height < actual_value)
                    actual_value = scroll_height - area.height;
                if (actual_value < 0)
                    actual_value = 0;
                if (current != actual_value && on_changevalue != null)
                    on_changevalue(this, actual_value);
            }
        }


        /********************************************
         ****** METHODES SPECIFIQUES AU WIDGET ******
         ********************************************/


        /// <summary>
        /// Constructeur de base de la scrollbar
        /// </summary>
        /// <param name="area">coordonnées et dimensions</param>
        /// <param name="scroll_height">hauteur totale du conteneur</param>
        /// <param name="on_changevalue">évènement provoqué d'un changement de scrolling</param>
        /// <param name="parent">parent du widget</param>
        public Scrollbar(Rect area, float scroll_height, EventDelegate on_changevalue, Widget parent)
        {
            this.area = area;
            this.scroll_height = scroll_height;
            this.on_changevalue = on_changevalue;
            this.parent = parent;
        }


        /***********************************
         ****** METHODES D'EVENEMENTS ******
         ***********************************/


        public override void OnUpdate()
        {
            if (!this.Active) return;
            base.OnUpdate();
            if (this.is_scrolling)
            {
                this.CurrentValue += this.scroll_direction * Scrollbar.SCROLL_CLIC_SPEED * Time.deltaTime;
            }
        }

        public override void OnDrawWithGL()
        {
            //Si le widget n'est pas actif, on ne l'affiche pas
            if (!this.active || this.area.height >= this.scroll_height) return;

            Vector2 pos = this.GlobalPosition;
            Rect rect = new Rect(pos.x, pos.y, this.area.width, this.area.height);

            GL.Begin(GL.QUADS);

            //Contour
            if (this.is_hover || this.is_clicked)
                GL.Color(Scrollbar.COLOR_1);
            else
                GL.Color(Scrollbar.COLOR_2);
            GL.Vertex3(rect.xMin, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMax, 0);
            GL.Vertex3(rect.xMin, rect.yMax, 0);

            //Intérieur
            GL.Color(Scrollbar.COLOR_4);
            rect = new Rect(pos.x + Scrollbar.DIM_MARGIN, pos.y + Scrollbar.DIM_MARGIN, this.area.width - 2 * Scrollbar.DIM_MARGIN, this.area.height - 2 * Scrollbar.DIM_MARGIN);
            GL.Vertex3(rect.xMin, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMax, 0);
            GL.Vertex3(rect.xMin, rect.yMax, 0);

            //Bouton "haut"
            GL.Color(Scrollbar.COLOR_3);
            rect.height = rect.width;
            GL.Vertex3(rect.xMin, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMax, 0);
            GL.Vertex3(rect.xMin, rect.yMax, 0);

            //Bouton "bas"
            rect.y = pos.y + this.area.height - rect.height - Scrollbar.DIM_MARGIN;
            GL.Vertex3(rect.xMin, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMax, 0);
            GL.Vertex3(rect.xMin, rect.yMax, 0);

            //Scroll
            rect.height = (this.area.height - 2 * this.area.width) * Mathf.Max(0.2f, this.area.height / this.scroll_height);
            rect.y = (pos.y + this.area.width) + (this.area.height - 2 * this.area.width - rect.height) * (this.actual_value / (this.scroll_height - this.area.height));
            GL.Vertex3(rect.xMin, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMax, 0);
            GL.Vertex3(rect.xMin, rect.yMax, 0);

            GL.End();

            GL.Begin(GL.TRIANGLES);

            //Flèche "haut"
            rect.width = DIM_TRIANGLE_SIZE * rect.width;
            rect.height = 0.7f * rect.width;
            rect.x = pos.x + Scrollbar.DIM_MARGIN + ((this.area.width - 2 * Scrollbar.DIM_MARGIN) - rect.width) / 2;
            rect.y = pos.y + Scrollbar.DIM_MARGIN + ((this.area.width - 2 * Scrollbar.DIM_MARGIN) - rect.height) / 2;
            GL.Vertex3(rect.xMin, rect.yMax, 0);
            GL.Vertex3((rect.xMin + rect.xMax) / 2, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMax, 0);

            //Flèche "bas"
            rect.y = pos.y + this.area.height - this.area.width + Scrollbar.DIM_MARGIN + ((this.area.width - 2 * Scrollbar.DIM_MARGIN) - rect.height) / 2;
            GL.Vertex3(rect.xMin, rect.yMin, 0);
            GL.Vertex3((rect.xMin + rect.xMax) / 2, rect.yMax, 0);
            GL.Vertex3(rect.xMax, rect.yMin, 0);

            GL.End();
        }

        public override void OnMouseEvent(int button, bool pressed, int x, int y)
        {
            //Si le widget n'est pas actif, on ne gère pas cet évènement
            if (!this.active) return;

            if (button == 0)
            {
                if (pressed)
                {
                    Rect zone = this.GlobalArea;
                    if (zone.Contains(new Vector2(x, y)))
                    {
                        Vector2 pos = new Vector2(zone.x, zone.y);
                        Rect rect = new Rect(
                            pos.x + Scrollbar.DIM_MARGIN,
                            0,
                            this.area.width - 2 * Scrollbar.DIM_MARGIN,
                            (this.area.height - 2 * this.area.width) * Mathf.Max(0.2f, this.area.height / this.scroll_height));
                        rect.y = (pos.y + this.area.width) + (this.area.height - 2 * this.area.width - rect.height) * (this.actual_value / (this.scroll_height - this.area.height));
                        if (rect.Contains(new Vector2(x, y)))
                        {
                            this.is_clicked = true;
                            this.clic_position = new Vector2(x, y);
                            this.clic_value = this.actual_value;
                        }
                    }
                    zone.height = zone.width;
                    if (zone.Contains(new Vector2(x, y)))
                    {
                        this.CurrentValue -= Scrollbar.SCROLL_CLIC_SPEED * Time.deltaTime;
                        this.scroll_direction = -1f;
                        this.is_scrolling = true;
                    }
                    zone = this.GlobalArea;
                    zone.y = zone.yMax - zone.width;
                    zone.height = zone.width;
                    if (zone.Contains(new Vector2(x, y)))
                    {
                        this.CurrentValue += Scrollbar.SCROLL_CLIC_SPEED * Time.deltaTime;
                        this.scroll_direction = 1f;
                        this.is_scrolling = true;
                    }
                }
                else
                {
                    this.is_clicked = false;
                    this.is_scrolling = false;
                }
            }
        }

        public override void OnMotionEvent(int x, int y)
        {
            this.mouse_position = new Vector2(x, y);

            //Si le widget n'est pas actif, on ne gère pas cet évènement
            if (!this.active) return;

            this.is_hover = this.GlobalArea.Contains(new Vector2(x, y));
            if (is_clicked)
            {
                this.CurrentValue = this.clic_value + (y - this.clic_position.y) * this.scroll_height / (this.area.height - 2 * this.area.width);
            }
        }

        public override void OnScrollEvent(int delta)
        {
            if (this.parent != null && this.parent.GlobalArea.Contains(this.mouse_position) && !Input.GetKey(KeyCode.LeftShift))
            {
                this.CurrentValue += Scrollbar.SCROLL_SPEED * delta;
            }
        }

    }

}
