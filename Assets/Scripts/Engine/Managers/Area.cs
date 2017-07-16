using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.Managers {

    public class Area {

        /*********************************** EDITOR ATTRIBUTES ************************************
		 * This section holds public attributes, which are visible and editable within the editor.*
		 * For attributes accessible publicly but not visible in the editor, use properties.      *
		 ******************************************************************************************/

        public static readonly float MAXIMUM_RADIUS = 300f;

        /// <summary>
        /// Position du centre de la zone et ses dimensions
        /// </summary>
        public float width, height, xMin, xMax, zMin, zMax;

        /// <summary>
        /// The position of the center of the area.
        /// </summary>
        public Vector3 center;

        /*********************************** HIDDEN ATTRIBUTES **************************************
         * This section holds private/protected attributes, which are NOT visible within the editor.*
         * Use this section for attributes that aren't meant to be accessible from other classes.   *
         ********************************************************************************************/

        /******************************************** OTHER FUNCTIONS ***************************************************
         * This section holds all other functions that might be called by the primitives to retrieve values for example.* 
         * In principle, every function in this section should only be called from within primitives.                   *
         ****************************************************************************************************************/
        /// <summary>
        /// Default constructor. Will instantiate an area with the coordinates and size of the entire map.
        /// </summary>
        public Area() {
            Terrain map = GameObject.Find("Map").GetComponent<Terrain>() as Terrain;
            Vector3 terrainSize = map.terrainData.size;
            this.center = new Vector3(0.0f, 0.0f, 0.0f);
            this.width = terrainSize.x - 20.0f;
            this.height = terrainSize.z - 20.0f;
            this.xMin = this.center.x - this.width / 2.0f;
            this.xMax = this.center.x + this.width / 2.0f;
            this.zMin = this.center.z - this.height / 2.0f;
            this.zMax = this.center.z + this.height / 2.0f;
        }

        /// <summary>
        /// Constructs an area at the specified position and dimensions.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Area(Vector3 center , float width , float height) {
            this.center = center;
            this.width = width;
            this.height = height;
            this.xMin = this.center.x - this.width / 2.0f;
            this.xMax = this.center.x + this.width / 2.0f;
            this.zMin = this.center.z - this.height / 2.0f;
            this.zMax = this.center.z + this.height / 2.0f;
        }

        /// <summary>
        /// Returns a random point within the area.
        /// </summary>
        /// <returns></returns>
        public Vector3 RandomPoint() {
            Vector3[] results = new Vector3[10];
            float[] min_dist = new float[results.Length];
            GameObject[] units = GameObject.FindGameObjectsWithTag("WarBot");
            float distance;
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = new Vector3(Random.Range(this.xMin, this.xMax), 0.0f, Random.Range(this.zMin, this.zMax));
                min_dist[i] = MAXIMUM_RADIUS;
                foreach (GameObject unit in units)
                {
                    if ((distance = Vector3.Distance(results[i], unit.transform.position)) < MAXIMUM_RADIUS)
                    {
                        if (min_dist[i] > distance)
                            min_dist[i] = distance;
                    }
                }
            }

            return results[new List<float>(min_dist).IndexOf(Mathf.Max(min_dist))];
        }

        /// <summary>
        /// Détermine si la position se trouve dans la zone
        /// </summary>
        /// <param name="pos">position à vérifier</param>
        /// <returns></returns>
        public bool InsideArea(Vector3 pos) {
            if (this.xMin <= pos.x && this.xMax >= pos.x && this.zMin <= pos.z && this.zMax >= pos.z)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Syntactic sugar to check if a specified GameObject is in the area.
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public bool InsideArea(GameObject go) {
            return this.InsideArea(go.transform.position);
        }

        /// <summary>
        /// Détermine l'intersection entre les segments allant de p1 à p2 et de p3 à p4
        /// </summary>
        /// <param name="p1">premier point du premier segment</param>
        /// <param name="p2">second point du premier segment</param>
        /// <param name="p3">premier point du second segment</param>
        /// <param name="p4">second point du second segment</param>
        /// <returns>Retourne la position de l'intersection s'il y en a une et null sinon</returns>
        private static Vector2? GetIntersection(Vector2 p1 , Vector2 p2 , Vector2 p3 , Vector2 p4) {
            float Ix = p2.x - p1.x, Iy = p2.y - p1.y, Jx = p4.x - p3.x, Jy = p4.y - p3.y;
            float den = Ix * Jy - Iy * Jx;
            if (den == 0) return null;
            float Ax = p1.x, Ay = p1.y, Cx = p3.x, Cy = p3.y;
            float m = -(-Ix * Ay + Ix * Cy + Iy * Ax - Iy * Cx) / den;
            float k = -(Ax * Jy - Cx * Jy - Jx * Ay + Jx * Cy) / den;
            if (m < 0 || m > 1 || k < 0 || k > 1) return null;
            return new Vector2(Ax + k * Ix , Ay + k * Iy);
        }

        /// <summary>
        /// Met à jour la position de l'objet mais en le gardant dans la zone
        /// </summary>
        /// <param name="obj">objet à mettre à jour</param>
        /// <param name="new_pos">position souhaitée</param>
        /// <returns>Retourne true s'il y a eu une collision avec les bords et false sinon</returns>
        public bool UpdatePosition(GameObject obj , Vector3 new_pos) {
            if (InsideArea(new_pos)) {
                obj.transform.position = new_pos;
                return false;
            } else {
                Vector2 p3 = new Vector2(obj.transform.position.x , obj.transform.position.z), p4 = new Vector2(new_pos.x , new_pos.z);
                Vector2? intersec = GetIntersection(
                    new Vector2(this.xMin , this.zMin) ,
                    new Vector2(this.xMax , this.zMin) ,
                    p3 ,
                    p4);
                if (intersec == null) {
                    intersec = GetIntersection(
                    new Vector2(this.xMax , this.zMin) ,
                    new Vector2(this.xMax , this.zMax) ,
                    p3 ,
                    p4);
                    if (intersec == null) {
                        intersec = GetIntersection(
                        new Vector2(this.xMax , this.zMax) ,
                        new Vector2(this.xMin , this.zMax) ,
                        p3 ,
                        p4);
                        if (intersec == null) {
                            intersec = GetIntersection(
                            new Vector2(this.xMin , this.zMax) ,
                            new Vector2(this.xMin , this.zMin) ,
                            p3 ,
                            p4);
                            if (intersec == null) {
                                obj.transform.position = new_pos;
                                return false;
                            }
                        }
                    }
                }
                obj.transform.position = new Vector3(intersec.Value.x , new_pos.y , intersec.Value.y);
                return true;
            }
        }
    }
}