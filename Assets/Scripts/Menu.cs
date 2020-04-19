using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

	public GameObject ball;
	public GameObject player;
	public GameObject ai;
    public float MasterVolume = 1.0f;
	public int AiDifficulty = 3;
	public int PlayerSize = 40;
	public int AiSize = 40;
	public int BallSize = 8;
	public int NumberOfBalls = 1;
	public int InitialBallSpeed = 500;
	public int BallSpeedIncreasePerBounce = 10;

	Rect menuRect;
	Manager manager;
	
	void Awake() {
		manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>();
	}
	
	void Update() {
		if (!manager.Paused) {
			return;
		}
		
		player.transform.localScale = new Vector3(player.transform.localScale.x, PlayerSize, 1);
		player.transform.position = new Vector3(20, (Screen.height / 2) - (PlayerSize / 2), 0);
		
		ai.transform.localScale = new Vector3(ai.transform.localScale.x, AiSize, 1);
		ai.transform.position = new Vector3(Screen.width - 20 - ai.GetComponent<Collider>().bounds.size.x, (Screen.height / 2) - (AiSize / 2), 0);
	}
	
	void OnGUI() {
		if (!manager.Paused) {
			return;
		}

        // Position our menu dead center.
		menuRect = new Rect((Screen.width - menuRect.width) * 0.5f, (Screen.height - menuRect.height) * 0.5f, 200, 400);
		
        // Draw the menu.
		DrawMenu();
	}
	
	/**
	 * Initialize the game. 
	 * Set all values according to the selections in the menu.
	 */
	public void Initialize() {
		for (int i = 0; i < NumberOfBalls; i++) {
			GameObject newBall = Instantiate(ball, new Vector3(Screen.width / 2, Screen.height / 2), Quaternion.identity) as GameObject;
			newBall.GetComponent<Ball>().size = new Vector3(BallSize, BallSize, 1);
			newBall.GetComponent<Ball>().speed = InitialBallSpeed;
			newBall.GetComponent<Ball>().speedIncreasePerBounce = BallSpeedIncreasePerBounce;
		}
		GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
		ai.GetComponent<AI>().balls = balls;
		ai.GetComponent<AI>().difficulty = AiDifficulty;
        
        AudioListener.volume = MasterVolume;
	}

    /**
     * Draws the menu.
     */
    private void DrawMenu() {
        GUILayout.BeginArea(menuRect);
        GUILayout.BeginVertical();

		GUILayout.Label("Sound volume: " + MasterVolume);
		MasterVolume = GUILayout.HorizontalSlider(MasterVolume, 0f, 1f);

		GUILayout.Label("AI difficulty: " + AiDifficulty);
		AiDifficulty = (int) GUILayout.HorizontalSlider(AiDifficulty, 1, 5);

		GUILayout.Label("Number of balls: " + NumberOfBalls);
		NumberOfBalls = (int) GUILayout.HorizontalSlider(NumberOfBalls, 1, 10);

		GUILayout.Label("Initial ball speed: " + InitialBallSpeed);
		InitialBallSpeed = (int) GUILayout.HorizontalSlider(InitialBallSpeed, 1, 5000);

		GUILayout.Label("Speed increase per bounce: " + BallSpeedIncreasePerBounce);
		BallSpeedIncreasePerBounce = (int) GUILayout.HorizontalSlider(BallSpeedIncreasePerBounce, 1, 100);

		GUILayout.Label("Size of player paddle: " + PlayerSize);
		PlayerSize = (int) GUILayout.HorizontalSlider(PlayerSize, 40, 480);
		
		GUILayout.Label("Size of AI paddle: " + AiSize);
		AiSize = (int) GUILayout.HorizontalSlider(AiSize, 40, 480);
		
		GUILayout.Label("Size of ball(s): " + BallSize);
		BallSize = (int) GUILayout.HorizontalSlider(BallSize, 1, 32);

		if (GUILayout.Button(new GUIContent("PLAY"))) {
			manager.Paused = false;
			Initialize();
		}

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}