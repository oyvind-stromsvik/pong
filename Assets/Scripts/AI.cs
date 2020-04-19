using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour {
	
	public Vector2 size = new Vector3(8, 40);
	public int difficulty = 3;

	[HideInInspector]
	public GameObject[] balls;

	float idealPosition;
	float currentPosition;
	GameObject closestBall;
	float distanceToClosestBall;
	Manager manager;

	void Awake() {
		manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>();
	}

	void Start() {
		gameObject.GetComponent<MeshFilter>().mesh = manager.CreateQuad();
		transform.localScale = new Vector3(size.x, size.y, 1);
		transform.position = new Vector3(Screen.width - 20 - GetComponent<Collider>().bounds.size.x, Screen.height / 2, 0);
	}
	
	void Update() {
		// If the game is paused we do nothing.
		if (manager.Paused) {
			return;
		}

		// If there are no balls we do nothing.
		if (balls.Length < 1) {
			return;
		}

		// Find the closest ball.
		FindClosestBall();

		// This position is right in front of the ball.
		idealPosition = closestBall.transform.position.y - GetComponent<Collider>().bounds.extents.y + closestBall.GetComponent<Collider>().bounds.extents.y;

		// Change how we track the ball depending on the difficulty.
		switch (difficulty) {
			// On the highest difficulty the AI is always in the ideal position so
			// it should be impossible to beat him if playing with just one ball.
			case 5:
				currentPosition = idealPosition;
				break;

			case 4:
				currentPosition = Mathf.Lerp(transform.position.y, idealPosition, Time.deltaTime * 20f);
				break;

			case 3:
				currentPosition = Mathf.Lerp(transform.position.y, idealPosition, Time.deltaTime * 15f);
				break;

			case 2:
				currentPosition = Mathf.Lerp(transform.position.y, idealPosition, Time.deltaTime * 105f);
				break;

			default:
				currentPosition = Mathf.Lerp(transform.position.y, idealPosition, Time.deltaTime * 5f);
				break;
		}

		// Set our position.
		transform.position = new Vector3(transform.position.x, currentPosition, 0);

		// Make sure the AI never moves off the screen.
		if (transform.position.y > (Screen.height - GetComponent<Collider>().bounds.size.y)) {
			transform.position = new Vector3(transform.position.x, (Screen.height - GetComponent<Collider>().bounds.size.y), 0);
		}
		else if (transform.position.y < 0) {
			transform.position = new Vector3(transform.position.x, 0, 0);
		}
	}
	
	/**
	 * Find the ball that is closest. 
	 */
    void FindClosestBall() {
        distanceToClosestBall = Mathf.Infinity;
        foreach (GameObject ball in balls) {
			Vector3 diff = transform.position - ball.transform.position;
            float curDistance = diff.sqrMagnitude;
			if (curDistance < distanceToClosestBall) {
                closestBall = ball;
				distanceToClosestBall = curDistance;
            }
        }
    }
}
