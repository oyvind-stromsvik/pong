using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public Vector2 size = new Vector3(8, 40);
	
	Manager manager;
	
	void Awake() {
		manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>();
	}

	void Start() {
		gameObject.GetComponent<MeshFilter>().mesh = manager.CreateQuad();
		transform.localScale = new Vector3(size.x, size.y, 1);
		transform.position = new Vector3(20, Screen.height / 2, 0);
	}

	void Update() {
		// If the game is paused we do nothing.
		if (manager.Paused) {
			return;
		}

		// Set our position according to the mouse pointer.
        transform.position = new Vector3(transform.position.x, Input.mousePosition.y, 0);

		// Make sure we can't move off the screen.
		if (transform.position.y > (Screen.height - GetComponent<Collider>().bounds.size.y)) {
			transform.position = new Vector3(transform.position.x, (Screen.height - GetComponent<Collider>().bounds.size.y), 0);
		}
		else if (transform.position.y < 0) {
			transform.position = new Vector3(transform.position.x, 0, 0);
		}
	}
}
