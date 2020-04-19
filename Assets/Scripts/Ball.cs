using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {
	
	public LayerMask layerMask;
	public AudioClip paddleHit;
	public AudioClip wallHit;
	public AudioClip scorePoint;
	public Vector3 size = new Vector3(8, 8, 1);
    public float speed = 500f;
	public float speedIncreasePerBounce = 10f;
	[HideInInspector]
	public Vector3 velocity;
	
	Manager manager;
	Collider playerPaddle;
	Collider aiPaddle;
	Vector3 initialPosition;
	Vector3 oldPos;
	bool highSpeedCheckLeft = false;
	bool highSpeedCheckRight = false;
	bool hasChecked = false;

	void Awake() {
		manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>();
   		playerPaddle = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>();
		aiPaddle = GameObject.FindGameObjectWithTag("AI").GetComponent<Collider>();

	}

	void Start () {
        gameObject.GetComponent<MeshFilter>().mesh = manager.CreateQuad();
        transform.localScale = size;
		initialPosition = new Vector3((Screen.width / 2) - GetComponent<Collider>().bounds.extents.x, (Screen.height / 2) - GetComponent<Collider>().bounds.extents.y);
		StartCoroutine("Reset");
	}

	void Update() {
		// Move the ball according to the velocity.
		transform.position = transform.position + velocity * Time.deltaTime;

		// Check if the ball hit the bottom or top of the game area.
		WallCollisionCheck();

		// Check if the ball moved completely through a paddle in one frame.
		HighSpeedCollisionCheck();

		// Check if the ball overlap a paddle.
		PaddleCollisionCheck();

		// Check if the ball has exited the playing field in the x-axis.
		ScoreZoneCheck();

		// Save the position from the last frame. We use this for calculations.
		oldPos = transform.position;
	}
	
	/**
	 * Check to see if the ball has left the screen in the y-axis. If so
	 * we put it back in, invert the y-velocity and play a sound.
	 */
	void WallCollisionCheck() {
		if (transform.position.y > (Screen.height - GetComponent<Collider>().bounds.size.y)) {
			GetComponent<AudioSource>().PlayOneShot(wallHit);
			velocity = new Vector3(velocity.x, velocity.y * -1, 0);
			transform.position = new Vector3(transform.position.x, Screen.height - GetComponent<Collider>().bounds.size.y, 0);
		}
		if (transform.position.y < 0) {
			GetComponent<AudioSource>().PlayOneShot(wallHit);
			velocity = new Vector3(velocity.x, velocity.y * -1, 0);
			transform.position = new Vector3(transform.position.x, 0, 0);
		}
	}

	/**
	 * Checks if the ball passed through one of the paddles completely in one frame
	 * and then repositions the ball so that it will register a collision if the 
	 * paddle is in front of it. At least I think it does.
	 */
	void HighSpeedCollisionCheck() {
		// Reset the check flag when we are between the paddles again.
		if (GetComponent<Collider>().bounds.min.x > playerPaddle.bounds.max.x && GetComponent<Collider>().bounds.max.x < aiPaddle.bounds.min.x) {
			hasChecked = false;
		}

		// We passed the player paddle.
		if (GetComponent<Collider>().bounds.max.x < playerPaddle.bounds.min.x) {
			highSpeedCheckLeft = true;
		}

		// We passed the ai paddle.
		if (transform.position.x > aiPaddle.bounds.max.x) {
			highSpeedCheckRight = true;
		}

		// If we are still within the playing field do nothing.
		if (!highSpeedCheckLeft && !highSpeedCheckRight) {
			return;
		}

		// If we've already checked and are still behind the paddle.
		if (hasChecked) {
			return;
		}

		// How much we've moved in x and y since the last frame.
		float xTranslation = transform.position.x - oldPos.x;
		float yTranslation = transform.position.y - oldPos.y;

		// Because our actual positon is behind the paddle we fabricate 
		// a position which will compare a collision against. This fabricated
		// position will be on the line between our current positon and our old
		// position on the same x-coordinate as the paddle.
		Vector3 pos = Vector3.zero;
		if (highSpeedCheckLeft) {
			float xMove = playerPaddle.bounds.max.x - 1 - transform.position.x;
			float yMove = xMove * (yTranslation / xTranslation);
			pos = new Vector3(playerPaddle.bounds.max.x - 1, transform.position.y + yMove, 0);
		}
		else {
			float xMove = aiPaddle.bounds.min.x - GetComponent<Collider>().bounds.size.x + 1 - transform.position.x;
			float yMove = xMove * (yTranslation / xTranslation);
			pos = new Vector3(aiPaddle.bounds.min.x - GetComponent<Collider>().bounds.size.x + 1, transform.position.y + yMove, 0);
		}

		// The bounding box for our fabricated position.
		Bounds bounds = new Bounds(pos + new Vector3(GetComponent<Collider>().bounds.extents.x, GetComponent<Collider>().bounds.extents.y, 0), GetComponent<Collider>().bounds.size);

		// Check if this fabricated bounding box overlaps a paddle.
		// If it does we position the ball there so our actual collision
		// check will register a collision and reflect the ball.
		if (bounds.Intersects(playerPaddle.bounds)) {
			transform.position = pos;
		}
		if (bounds.Intersects(aiPaddle.bounds)) {
			transform.position = pos;
		}

		// Reset the checks, but set the has checked flag to true so we don't
		// do another high speed check until we've entered the playing field again.
		highSpeedCheckLeft = highSpeedCheckRight = false;
		hasChecked = true;
	}

	/**
	 * Checks if the ball is colliding with a paddle by comparing the bounds 
	 * of the ball and each of the paddles and seeing if they intersect.
	 */
	void PaddleCollisionCheck() {
		if (GetComponent<Collider>().bounds.Intersects(playerPaddle.bounds)) {
			ResolveCollision(playerPaddle);
		}
		if (GetComponent<Collider>().bounds.Intersects(aiPaddle.bounds)) {
			ResolveCollision(aiPaddle);
		}
	}
	
	/**
	 * Resolves a collision between the ball and the paddle it's intersecting with.
	 */
	void ResolveCollision(Collider paddle) {
		// Where did the ball hit, relative to the paddle's center?
		float hitPoint = GetComponent<Collider>().bounds.max.y - paddle.bounds.center.y;
		if (transform.position.y > paddle.transform.position.y) {
			hitPoint = GetComponent<Collider>().bounds.min.y - paddle.bounds.center.y; 
		}

		// Express the hit point as a percentage of half the paddle's height.
		float angleMultiplier = Mathf.Abs(hitPoint / paddle.bounds.extents.y); 

	    // Use the angle multiplier to determine the angle the ball should return at. 
		// Then use trig functions to determine new x/y velocities. 
	    float xVelocity = Mathf.Cos(60.0f * angleMultiplier * Mathf.Deg2Rad) * velocity.magnitude; 
	    float yVelocity = Mathf.Sin(60.0f * angleMultiplier * Mathf.Deg2Rad) * velocity.magnitude;

	    // If the ball hit the paddle below the center, the yVelocity should 
		// be flipped so that the ball is returned at a downward angle.
		if (hitPoint < 0) {
	        yVelocity *= -1;
	    }

	    // If the ball came in at an xVelocity of more than 0, we know the ball 
		// was travelling right when it hit the paddle. It should now start going left.        
	    if (velocity.x > 0) {
	        xVelocity *= -1;
	    }
		
		// Move the ball out so we don't collide anymore. We only care about the x-axis.
		float overlap = paddle.bounds.max.x - GetComponent<Collider>().bounds.min.x;
	    if (velocity.x > 0) {
			overlap = paddle.bounds.min.x - GetComponent<Collider>().bounds.max.x;
	    }
		transform.position = new Vector3(transform.position.x + overlap, transform.position.y, 0);
	     
	    // Set the ball's x and y velocities to the newly calculated values.
		velocity = new Vector3(xVelocity, yVelocity, 0);

		// Increase the velocity a little.
		if (velocity.magnitude <= 5000) {
			velocity = velocity + velocity.normalized * speedIncreasePerBounce;
		}

		GetComponent<AudioSource>().PlayOneShot(paddleHit);
	}
	
	/**
	 * Checks if the ball entered the score zone, ie. left the screen on the x-axis.
	 */
	void ScoreZoneCheck() {
		// The player scored.
		if (transform.position.x > Screen.width) {
			manager.IncreaseScore("Player");
			GetComponent<AudioSource>().PlayOneShot(scorePoint);
			StartCoroutine("Reset");
		}
		
		// The AI scored.
		if (transform.position.x < (0 - GetComponent<Collider>().bounds.size.x)) {
			manager.IncreaseScore("AI");
			GetComponent<AudioSource>().PlayOneShot(scorePoint);
			StartCoroutine("Reset");
		}
	}
	
	/**
	 * Resets the ball at the start of the game and after a point is scored.
	 */
	IEnumerator Reset() {
		transform.position = initialPosition;
		oldPos = transform.position;
		highSpeedCheckLeft = highSpeedCheckRight = false;
		hasChecked = true;
		velocity = Vector3.zero;
		
		yield return new WaitForSeconds(1);

		// Send the ball either left or right with an angle +/- 45 degrees.
		velocity = speed * new Vector3(1, Random.Range(-1f, 1f), 0).normalized;
		if (Random.value > 0.5f) {
			velocity.x *= -1;
		}
	}
}
