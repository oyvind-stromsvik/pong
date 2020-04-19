using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour {

	public Material mat;
	[HideInInspector]
	public bool Paused = true;

    string[] numbers;
    int playerScore = 0;
	int aiScore = 0;
	GameObject playerScoreHolder;
	GameObject aiScoreHolder;
	GameObject activeBlock;
	GameObject player;
	GameObject ball;
	GameObject net;

    void Start() {
		playerScoreHolder = new GameObject("PlayerScoreDisplay");
		aiScoreHolder = new GameObject("AIScoreDisplay");
		
		CreateNumbers();
        DisplayScore("Player");
        DisplayScore("AI");
		CreateNet();
		net = GameObject.Find("Net");

        // No idea why I need this, but my builds completely ignore the
        // resolution I set in the player settings.
        Screen.SetResolution(960, 480, false);
	}

	void Update() {
		if (Paused && net.activeSelf) {
			net.SetActive(false);
		}
		else if (!Paused && !net.activeSelf) {
			net.SetActive(true);
        }
	}

	/**
	 * Increase the score.
	 */
	public void IncreaseScore(string type) {
		if (type == "Player") {
			playerScore += 1;
		}
		else {
			aiScore += 1;
		}
        DisplayScore(type);
    }
    
    /**
     * Creates a gameobject with a quad which is part of our score numbers.
     */
    void CreateNumberPart(Vector2 position, Transform parent) {
        activeBlock = new GameObject("NumberPart");
		activeBlock.transform.parent = parent;
        activeBlock.AddComponent<MeshFilter>();
		activeBlock.GetComponent<MeshFilter>().mesh = CreateQuad();
        activeBlock.AddComponent<MeshRenderer>();

        activeBlock.transform.localScale = new Vector3(8, 8, 1);
        activeBlock.transform.position = new Vector3(position.x, position.y, 0);
		
		activeBlock.GetComponent<Renderer>().material = mat;
    }

	/**
	 * Creates a mesh with one quad.
	 */
    public Mesh CreateQuad() {
        // Create a new mesh.
        Mesh mesh = new Mesh();

        // Add a quad.
        Vector3[] vertices = new Vector3[] {
            new Vector3(1, 1, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 0, 0),
        };

        // Create some default uvs.
        Vector2[] uv = new Vector2[] {
            new Vector2(1, 1), // Top right
            new Vector2(1, 0), // Top left
            new Vector2(0, 1), // Bottom right
            new Vector2(0, 0)  // Bottom left
        };

        // Do some crap with triangles.
        int[] triangles = new int[] {
            0, 1, 2,
            2, 1, 3,
        };

        // Add everything to the mesh and return it.
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }

	/**
	 * Display the scores as a series of quad meshes. Why not?
	 */
    void DisplayScore(string type) {
		string text = "";
		if (type == "Player") {
			foreach (Transform child in playerScoreHolder.transform) {
				Destroy(child.gameObject);
			}
			text = playerScore.ToString();
		}
		else {
			foreach (Transform child in aiScoreHolder.transform) {
				Destroy(child.gameObject);
			}
			text = aiScore.ToString();
		}

        for (int i = 0; i < text.Length; i++) {
            int charNum = System.Convert.ToInt32(text[i]);
			string number = numbers[charNum];
	        for (int y = 0; y < 5; y++) {
		        for (int x = 0; x < 3; x++) {
					char placeholder = number[x + y * 3];
		            if (placeholder == '1') {
						Vector2 position = Vector2.zero;
						if (type == "Player") {
							position = new Vector2(100 + (x * 8) + (i * 32), 432 - (y * 8));
							CreateNumberPart(position, playerScoreHolder.transform);
						}
						else {
							position = new Vector2(Screen.width - 100 + 16 + (x * 8) + (i * 32) - (text.Length * 32), 432 - (y * 8));
							CreateNumberPart(position, aiScoreHolder.transform);
						}
					}
		        }
	        }
        }
    }
	
	/**
	 * Create the net. Just a bunch of lines evenly spaced across
	 * the middle of the screen.
	 */
	void CreateNet() {
		GameObject net = new GameObject("Net");
		int numberOfNetMasks = 16;
		int spacing = Screen.height / numberOfNetMasks;
		float maskWidth = 2;
		float maskHeight = spacing * 0.6f;
		
		for (int y = 0; y < numberOfNetMasks; y++) {
	        GameObject netMask = new GameObject("NetMask");
			netMask.transform.parent = net.transform;
	        netMask.AddComponent<MeshFilter>();
			netMask.GetComponent<MeshFilter>().mesh = CreateQuad();
            netMask.AddComponent<MeshRenderer>();

	        netMask.transform.localScale = new Vector3(maskWidth, maskHeight, 1);
			netMask.transform.position = new Vector3((Screen.width / 2) - (maskWidth / 2), spacing * y + ((spacing - maskHeight) / 2), 0);
			
			netMask.GetComponent<Renderer>().material = mat;
		}
	}

	/**
	 * The numbers 0-9 as points on a 3x5 grid. 
	 * The key is the ASCII code for the number.
	 */
	void CreateNumbers() {
        numbers = new string[128];
		
	    // 0
        numbers[48] = "111" +
	                  "101" +
	                  "101" +
	                  "101" +
	                  "111";
	    // 1
        numbers[49] = "001" +
			          "001" +
			          "001" +
			          "001" +
			          "001";
	    // 2
        numbers[50] = "111" +
			          "001" +
			          "111" +
			          "100" +
			          "111";
	    // 3
        numbers[51] = "111" +
			          "001" +
			          "111" +
			          "001" +
			          "111";
	    // 4
        numbers[52] = "101" +
			          "101" +
			          "111" +
			          "001" +
			          "001";
	    // 5
        numbers[53] = "111" +
			          "100" +
			          "111" +
			          "001" +
			          "111";
	    // 6
        numbers[54] = "111" +
			          "100" +
			          "111" +
			          "101" +
			          "111";
	    // 7
        numbers[55] = "111" +
			          "001" +
			          "001" +
			          "001" +
			          "001";
	    // 8
        numbers[56] = "111" +
			          "101" +
			          "111" +
			          "101" +
			          "111";
	    // 9
        numbers[57] = "111" +
			          "101" +
			          "111" +
			          "001" +
			          "001";
	}
}
