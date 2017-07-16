using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.Particles {

    public class ParticleDestroy : MonoBehaviour {

        // Use this for initialization
        void Start() {
            StartCoroutine(WaitAndDestroy());
        }

        // Update is called once per frame
        void Update() {
        }

        public IEnumerator WaitAndDestroy() {
            yield return new WaitForSeconds(1.2f);
            Destroy(this.gameObject);
        }
    }
}