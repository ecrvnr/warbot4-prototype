using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.Editeur
{

    /// <summary>
	/// Horizontal scrolling when hover with cursor or Shift + Scroll
    /// </summary>
    public class MotionScroll : Widget
    {

        /*********************************
         ****** ATTRIBUTS STATIQUES ******
         *********************************/


        /// <summary>
		/// Width of the handle
        /// </summary>
        private static readonly float DIM_SIZE = 0.1f;

        /// <summary>
		/// Color when scrolling is deactivated
        /// </summary>
        private static readonly Color COLOR_1 = new Color((float)0x27 / 255, (float)0x72 / 255, (float)0xdb / 255, 0.2f);
        /// <summary>
		/// Color when scrolling is activated
        /// </summary>
        private static readonly Color COLOR_2 = new Color((float)0x27 / 255, (float)0x72 / 255, (float)0xdb / 255, 0.5f);

        /// <summary>
		/// Scrolling speed with mousehover
        /// </summary>
        private static readonly float SCROLL_MOTION_SPEED = 200.0f;

        /// <summary>
		/// Scrolling speed with the scroll button
        /// </summary>
        private static readonly float SCROLL_MOUSE_SPEED = 10.0f;

        /// <summary>
		/// Max cursor speed for scrolling to enable
        /// </summary>
        private static readonly float MOTION_MAX_SPEED = 1f;


        /***********************
         ****** ATTRIBUTS ******
         ***********************/


        /// <summary>
		/// Container intern width
        /// </summary>
        private float scroll_width;

        /// <summary>
		/// Actual position
        /// </summary>
        private float actual_value = 0;
        
        /// <summary>
		/// Called event when change of actual value
        /// </summary>
        private event EventDelegate on_changevalue;

        //ATTRIBUTS D'EVENEMENTS

        /// <summary>
		/// Indicate if scrolling is in progress
        /// </summary>
        private bool is_scrolling = false;

        /// <summary>
		/// Scrolling direction
        /// </summary>
        private float scroll_direction = 0f;

        /// <summary>
		/// Last cursor position
        /// </summary>
        private Vector2 last_mouse_pos = new Vector2();


        /************************
         ****** ACCESSEURS ******
         ************************/


        /// <summary>
		/// Container total height
        /// </summary>
        public float ScrollWidth {
            get
            {
                return scroll_width;
            }
            set
            {
                scroll_width = value;
                if (actual_value > scroll_width - this.area.width)
                    CurrentValue = scroll_width - this.area.width;
                if (actual_value < 0)
                    CurrentValue = 0;
            }
        }

        /// <summary>
		/// Actual vertical position
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
                if (scroll_width - area.width < actual_value)
                    actual_value = scroll_width - area.width;
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
        /// Base constructor for the MotionScroll
        /// </summary>
        /// <param name="scroll_width">Total width of the container</param>
        /// <param name="on_changevalue">Event triggered during a scrolling change</param>
        /// <param name="parent">Parent widget</param>
        public MotionScroll(float scroll_width, EventDelegate on_changevalue, Widget parent)
        {
            this.scroll_width = scroll_width;
            this.on_changevalue = on_changevalue;
            this.parent = parent;
            this.area = new Rect(0, 0, this.parent.LocalArea.width, this.parent.LocalArea.height);
        }



        /***********************************
         ****** METHODES D'EVENEMENTS ******
         ***********************************/


        public override void OnUpdate()
        {
            base.OnUpdate();
            if (this.is_scrolling)
            {
                this.CurrentValue += this.scroll_direction * MotionScroll.SCROLL_MOTION_SPEED * Time.deltaTime;
            }
        }

        public override void OnDrawWithGL()
        {
            if (!this.active || this.area.width >= this.scroll_width) return;

            GL.Begin(GL.QUADS);

            if (this.is_scrolling)
                GL.Color(MotionScroll.COLOR_2);
            else
                GL.Color(MotionScroll.COLOR_1);
            Rect rect = this.GlobalArea;
            rect.width = this.parent.LocalArea.width * MotionScroll.DIM_SIZE;
            GL.Vertex3(rect.xMin, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMax, 0);
            GL.Vertex3(rect.xMin, rect.yMax, 0);

            rect.x = this.parent.LocalArea.xMax - rect.width;
            if (typeof(Container).Equals(this.parent.GetType()) && ((Container)this.parent).AllowScrollbar)
                rect.x -= Scrollbar.DIM_WIDTH;
            GL.Vertex3(rect.xMin, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMin, 0);
            GL.Vertex3(rect.xMax, rect.yMax, 0);
            GL.Vertex3(rect.xMin, rect.yMax, 0);

            GL.End();
        }

        public override void OnMotionEvent(int x, int y)
        {
            Vector2 mouse_pos = new Vector2(x, y);
            if (this.active)
            {
                Rect rect1 = this.GlobalArea;
                rect1.width = this.parent.LocalArea.width * MotionScroll.DIM_SIZE;
                Rect rect2 = new Rect(rect1);
                rect2.x = this.parent.LocalArea.xMax - rect2.width;
                if (typeof(Container).Equals(this.parent.GetType()) && ((Container)this.parent).AllowScrollbar)
                    rect2.x -= Scrollbar.DIM_WIDTH;

                if (rect1.Contains(mouse_pos))
                {
                    if ((this.last_mouse_pos - mouse_pos).sqrMagnitude * Time.deltaTime <= MotionScroll.MOTION_MAX_SPEED)
                    {
                        this.CurrentValue -= MotionScroll.SCROLL_MOTION_SPEED * Time.deltaTime;
                        this.is_scrolling = true;
                        this.scroll_direction = -1f;
                    }
                }
                else if (rect2.Contains(mouse_pos))
                {
                    if ((this.last_mouse_pos - mouse_pos).sqrMagnitude * Time.deltaTime <= MotionScroll.MOTION_MAX_SPEED)
                    {
                        this.CurrentValue += MotionScroll.SCROLL_MOTION_SPEED * Time.deltaTime;
                        this.is_scrolling = true;
                        this.scroll_direction = 1f;
                    }
                }
                else
                {
                    this.is_scrolling = false;
                }
            }

            this.last_mouse_pos = mouse_pos;
        }

        public override void OnScrollEvent(int delta)
        {
            if (!this.active) return;
            if (Input.GetKey(KeyCode.LeftShift) && this.parent.GlobalArea.Contains(this.last_mouse_pos))
            {
                this.CurrentValue += delta * MotionScroll.SCROLL_MOUSE_SPEED;
            }
        }


    }

}
