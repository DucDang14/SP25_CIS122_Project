using UnityEngine;

namespace FirstPersonArms {

    public class ShowCase : MonoBehaviour {

        public float angle = 10f;

        void Start() {

        }

        void Update() {
            transform.Rotate(new Vector3(0, angle, 0) * Time.deltaTime);
        }

    }

}