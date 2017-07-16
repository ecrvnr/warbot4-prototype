using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarBotEngine.UI {

    /// <summary>
    /// Manage the music.
    /// </summary>
    public class MusicManager : MonoBehaviour {

        void Awake() {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Music");
            if (objs.Length > 1)
                Destroy(this.gameObject);

            DontDestroyOnLoad(this.gameObject);

        }


        public void ToggleMusic(bool toggle) {
            if (toggle) {
                GameObject.Find("Music").GetComponent<AudioSource>().Play();
            } else {
                GameObject.Find("Music").GetComponent<AudioSource>().Stop();
            }
        }

    }


}