using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.Particles {

    public class SpotlightFade : MonoBehaviour {

        public float fade_speed; //The speed at which the light fades. A value of 0.5f means the intensity is halved each frame.

        // Use this for initialization
        void Start() {
            this.transform.parent = null;
        }

        // Update is called once per frame
        void Update() {
            if (this.GetComponent<Light>().intensity > 0.001f) {
                this.GetComponent<Light>().intensity -= fade_speed * this.GetComponent<Light>().intensity;
            } else {
                Destroy(this.gameObject);
            }
        }
    }
}