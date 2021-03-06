using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class Board : MonoBehaviour {
	
	private GameObject[] blocksLayer;
	private int[] blocksInLayer;
	
	// Dimensions of the board in "shapes".
	public int nx = 17;	// width
	public int ny = 15;	// height
	public int nz = 17;	// depth 

	public float transx,transz;
	
	private int flashPass;

	public int timeGap = 10; //default value. Change gap here!
    private int time = 1000;
    int timer = 0;
    float starttimer = 0.0f;
    bool countdown = false;
    bool pieceSuggestor = false;

    //scoring stuff
    public int score = 0;
    private bool viewLBoard = false;
    private bool scoreSubmitted = false;
    private bool receivedLboard = false;
    private bool rcvdPartScoreboard = false;
    ArrayList dispScores = new ArrayList();
    ArrayList dispPartScores = new ArrayList();
    private int lboardPosition = 0;
	
    private string uname = "Voldemort";

    //1-easy, 2-intermediate, 3-expert
    public int level = 1; 
    //stores number of times the player has constructed a piece
    public int rounds = 0; 
	private bool first = true;
    //Make shape fall when enter is pressed
    bool shapeFalling = false;
    int xTime = 0;

	//timer progress bar
    private float barDisplay = 1; //current progress
    private Vector2 pos = new Vector2(50,100);
    private Vector2 size = new Vector2(150,100);
    private Texture2D emptyTex;
    private Texture2D fullTex;
    //end timer progress bar

    //start display 2D images.
    private Texture2D[] lego;
    int[] legoSuggestions;

    //for wall, it is one unit of the walls.
    private UnityEngine.Object grid;

	private bool[,,] boardArray;
	
	//for rotating the board
	//ricky moved the board initialy so the bottom left == (0,0) so centre is this
	private Vector3 centreRotation = new Vector3 (2,1,2);

	//Audio stuff
	private AudioSource audio_source;
	private AudioSource wrong_piece_sound;
	
	// Four boundary walls
	private GameObject A,B,C,D;

	private BlockControl blockCtrl;
    private GameOver gOver;
    private Leaderboard lboard;

	private Camera topCam;

    //starting text
    private GUIText startGameText;
    private bool goText = true;

    //start time
    float startTime;
	
	public GameObject LBoard_BG;
	public GameObject textbox;

	// Initialization.
	void Awake () {
        Debug.Log("Initialisation");
        startTime = Time.realtimeSinceStartup;
        Debug.Log("Awake Time ---> " + startTime);

		GameObject.Find("Main Camera").AddComponent<AudioListener>();
		GameObject cam = GameObject.Find("Main Camera");
		if (cam.GetComponent<AudioListener>() == null) cam.AddComponent<AudioListener>();
		//boardArray keeps track of which positions on the board is occupied.
		boardArray = new bool[nx,ny,nz];
		Array.Clear(boardArray, 0, boardArray.Length);
		blockCtrl = GetComponent<BlockControl>();
		blockCtrl.enterPressed = false;
		//pinsPerShape = blockCtrl.getShapeSize();
		blocksLayer = new GameObject [ny];
		blocksInLayer = new int[ny];
		Array.Clear(blocksInLayer, 0, blocksInLayer.Length);
		
		//creating the bounding walls in the array
		for(int i=0; i< ny;i++){
			for(int j=0;j < nx;j++){
				boardArray[j,i,0] = true;
				boardArray[j,i,nz-1] = true;
				boardArray[0,i,j] = true;
				boardArray[nx-1,i,j] = true;
				//print (boardArray[j,i,0]);
			}
		}
		for (int i=0; i<ny; i++){
			blocksLayer[i] = new GameObject();
			String layerName = "Layer" + i;
			blocksLayer[i].name = layerName;
			addToScene(blocksLayer[i]);
		}
		
		//initialise textures for the GUI.
        initTextures();

        //Creates an array to store the 4 images of the lego pieces.
        //Number of pieces that are suggested and number of total pieces
        //  are currently hard-coded.
        lego = new Texture2D[4];
 
        lego[0] = Resources.Load("L2x1lit") as Texture2D;
        lego[1] = Resources.Load("L2x2lit") as Texture2D;
        lego[2] = Resources.Load("L3x2lit") as Texture2D;
        lego[3] = Resources.Load("L4x2lit") as Texture2D;

        legoSuggestions = new int[3];

		//shadow layer
		GameObject slayer = new GameObject();
		String lName = "ShadowLayer";
		slayer.name = lName;
		addToScene(slayer);
		
		flashPass = 0;
		
		grid = Resources.LoadAssetAtPath("Assets/Resources/grid_square.prefab", typeof(GameObject));
		DrawBoard();
		
		createTopCamera();
		//first = true;
		audio_source = GameObject.Find("Main Camera").GetComponent<AudioSource>();
		wrong_piece_sound = GameObject.Find("Main Camera").AddComponent<AudioSource>();
		wrong_piece_sound.clip = (AudioClip) Resources.LoadAssetAtPath("Assets/Music/error.wav", typeof(AudioClip));

		// Only uncomment if you want to *change* the music
		//		startMusic("Theme1");

		//start gameplay

        gOver = GetComponent<GameOver>();
        lboard = GetComponent<Leaderboard>();

		blockCtrl.assignTimeGap(timeGap);
	}

    void Start(){
        //pauseStarting(10);
        startTime = Time.realtimeSinceStartup;
        Debug.Log("Start time --> " + Time.realtimeSinceStartup);
        StartCoroutine(InitCountdown(4));
    }

    // Update is called once per frame.
    void Update (){
        if(countdown){
            if(Input.GetKeyDown("return")){
				blockCtrl.enterPressed = true;
                shapeFalling = true;
                Debug.Log ("ENTER");
            }
        }

        if (!viewLBoard && LBoard_BG.active)
			LBoard_BG.SetActive(false);
		else if(viewLBoard && !LBoard_BG.active)
			LBoard_BG.SetActive(true);	
    }

	private void createTopCamera(){
		UnityEngine.Object cameraPrefab = Resources.LoadAssetAtPath("Assets/TopCamera.prefab", typeof(Camera));
		topCam = GameObject.Instantiate(cameraPrefab) as Camera;
		topCam.transform.position = GameObject.Find("base").transform.position;
		float camHeight;
		if ((nx+nz)/2 > ny + 3) camHeight = (nx+nz)/2;
		else camHeight = ny + 3;
		topCam.transform.Translate(new Vector3(0,camHeight,0),Space.World);
		topCam.name = "Top Cam";
		//topCam.transform.Rotate()
	}

	// Used to change the track playing
	private void startMusic(String track){
		if (audio_source.isPlaying) audio_source.Stop();
		audio_source.clip = (AudioClip) Resources.LoadAssetAtPath("Assets/Music/" + track + ".wav", typeof(AudioClip));
		audio_source.Play();
	}

	public void setMaxVolume(){
		audio_source.volume = 1.0f;
	}

	public void playWarningSound(){
		if (wrong_piece_sound == null){
			throw new Exception("wrong_piece_sound is null!");
			return;
		}
		if (audio_source != null) audio_source.volume = 0.1f;
		wrong_piece_sound.Play();
		if (audio_source != null) Invoke( "setMaxVolume", wrong_piece_sound.clip.length );
	}
	
	// Create the base of the game board.
	private void DrawBoard(){

		GameObject legoBase = new GameObject();
		legoBase.name = "base";

		float halfLength = ((float)nx)/2;
		
		for (float x=0; x < nx-2; x++){
			for (float z=0; z < nz-2; z++){
				
				UnityEngine.Object blockPrefab = Resources.LoadAssetAtPath("Assets/block.prefab", typeof(GameObject));
				GameObject cube = GameObject.Instantiate(blockPrefab) as GameObject;
				
				cube.transform.position = new Vector3(1.5f + x - halfLength,
													  -0.45f,
					  								  1.5f + z - halfLength);
				
				Transform cubeTrans = cube.transform;
				
				cubeTrans.parent = legoBase.GetComponent<Transform>();
			}
		}
		
		
		
		legoBase.transform.position = new Vector3(6.0F, 0.0F, 6.0F);
		legoBase.AddComponent<CombineChildren>();
		
		foreach (Transform child in legoBase.transform){
			Destroy (child.gameObject);
		}
		
		// Make the base a child of the scene
		GameObject scene = GameObject.Find("Scene");
		Transform t = legoBase.transform;
		t.parent = scene.GetComponent<Transform>();
		if (nx%2 == 0) legoBase.transform.Translate(-0.5f,0.0f,-0.5f,Space.World);
		
		
		//Create the board's boundries dynamically
		A = new GameObject();
		A.transform.position = new Vector3(legoBase.transform.position.x, ny/2 , legoBase.transform.position.z + nz/2);
		A.name = "Wall A";
		
		B = new GameObject();
		B.transform.position = new Vector3(legoBase.transform.position.x + nx/2, ny/2, legoBase.transform.position.z);
		B.name = "Wall B";
		
		C = new GameObject();
		C.transform.position = new Vector3(legoBase.transform.position.x - nx/2, ny/2, legoBase.transform.position.z);
		C.name = "Wall C";
		
		D = new GameObject();
		D.transform.position = new Vector3(legoBase.transform.position.x, ny/2, legoBase.transform.position.z - nz/2);
		D.name = "Wall D";
		
		for(int i=0; i< ny - 3;i++){
			for(int j=1;j < nx-1;j++){
				
				float x;
				float xB;
				float xC;
				
				float z;
				float zA;
				float zD;
				
				if (nx%2==0){
					x = legoBase.transform.position.x - (nx)/2 + j + 0.5f;
					xB = legoBase.transform.position.x + (nx)/2-1;
					xC = legoBase.transform.position.x - (nx)/2+1;
					
					z = legoBase.transform.position.z - (nz)/2 + j+ 0.5f;
					zA = legoBase.transform.position.z + (nz)/2-1;
					zD = legoBase.transform.position.z - (nz)/2+1;
				}else{
					x = legoBase.transform.position.x - (nx)/2 + j;
					xB = legoBase.transform.position.x + (nx)/2 - 0.5f;
					xC = legoBase.transform.position.x - (nx)/2 + 0.5f;
					
					z = legoBase.transform.position.z - (nz)/2 + j;
					zA = legoBase.transform.position.z + (nz)/2 - 0.5f;
					zD = legoBase.transform.position.z - (nz)/2 + 0.5f;
				}
				
				GameObject A_square = GameObject.Instantiate(grid, new Vector3(x,i+0.5f,zA), Quaternion.Euler(new Vector3(-90, 0, 0))) as GameObject;
				A_square.transform.parent = A.GetComponent<Transform>();
				
				GameObject B_square = GameObject.Instantiate(grid, new Vector3(xB,i+0.5f,z),Quaternion.Euler(new Vector3(0, 0, 90))) as GameObject;
				B_square.transform.parent = B.GetComponent<Transform>();
				
				GameObject C_square = GameObject.Instantiate(grid, new Vector3(xC,i+0.5f,z),Quaternion.Euler(new Vector3(0, 0, -90))) as GameObject;
				C_square.transform.parent = C.GetComponent<Transform>();
				
				GameObject D_square = GameObject.Instantiate(grid, new Vector3(x,i+0.5f,zD),Quaternion.Euler(new Vector3(90, 0, 0))) as GameObject;
				D_square.transform.parent = D.GetComponent<Transform>();	
			}	
		}
		
		A.AddComponent<CombineChildren>();
		foreach(Transform child_A in A.transform){
			Destroy(child_A.gameObject);
		}
		
		B.AddComponent<CombineChildren>();
		foreach(Transform child_B in B.transform){
			Destroy(child_B.gameObject);
		}
		
		C.AddComponent<CombineChildren>();
		foreach(Transform child_C in C.transform){
			Destroy(child_C.gameObject);
		}
		
		D.AddComponent<CombineChildren>();
		foreach(Transform child_D in D.transform){
			Destroy(child_D.gameObject);
		}
	}

	// Add a pin object to its respective layer.
	public void FillPosition(int layer, GameObject pin) {
		addBlocks(layer, pin);
	}

	public void checkFullLayers(){
        int noOfLayer = 0;
		// Destroy the layers if they are full.
		for (int i = 0; i < blocksLayer.Length; i++){
			if(blocksInLayer[i] == (nx-2)*(nz-2)){
				clearLayer(i);
				i = -1; //will be back to 0 because of i++
                //TODO: Add counter here to check number of layers
                noOfLayer++;
			}
		}

        //TODO: Find out how and whn multiple layers are being cleared. - A
        scoring(1, noOfLayer);
	}
	
	/* We currently have a very basic scoring system in which
    ** the point increases by 10 every time a row is cleared.
    ** 10 is a rather arbitrary number but gives a bigger sense of
    ** achievement than getting 1 point after so much effort :P
    ** TODO: Expand this to add points @ rules. (eg. 2 adjacent red pieces)
    ** FLAGS - 0: Piece added, 1: x number of lines deleted.
    ** Overloaded function did not work -_-
    */
    void scoring(int flag, int addScore){
        if(flag == 0){
            score += time; //1000/timeGap * addScore;
            //score += addScore;
        }
        else
            score += addScore * 10;
    }


	// Clear the layer (i.e. reset the layer count to 0).
	public void clearLayer(int y) {
		//playLayerClearSound();

		foreach (Transform childTransform in blocksLayer[y].transform) {
		    Destroy(childTransform.gameObject);
		}

		//Handle scoring when layer is deleted.
		
		Destroy(blocksLayer[y]);
		blocksLayer[y] = null; //probably redundant
		
			
		//remove the layer from the board array
		//nx=17    x,y,z
		for(int i=1; i< nx-1;i++){
			for(int j=1;j < nz-1;j++){
				boardArray[i,y,j] = false;
			}
		}
		
		//from where the layer has been deleted, move all of the layers down one IN THE ARRAY
		for(int k = y;k<ny-1;k++){
			for(int i=1; i< nx-1;i++){
				for(int j=1;j < nz-1;j++){
					boardArray[i,k,j] =boardArray[i,k+1,j] ;
				}
			}
		}
		//clearing the top layer
		for(int i=1; i< nx-1;i++){
			for(int j=1;j < nz-1;j++){
				boardArray[i,ny-1,j] =false ;
			}
		}
		
		//float blockSize = blockCtrl.pinSize;
		
		//for moving the rest of the board down
		String layerName;
		for (int k = y + 1; k < ny; k++){
			if (blocksLayer[k] != null){
				blocksLayer[k-1] = blocksLayer[k];
				blocksInLayer[k-1] = blocksInLayer[k];
				// Translate down only if blocks present.
				if (blocksInLayer[k-1] > 0){
					// Easy to make smooth fall with "lerp".
					blocksLayer[k-1].transform.Translate(new Vector3(0, -1, 0)); //assuming size 1
				}
				layerName = "Layer" + (k - 1);
				blocksLayer[k-1].name = layerName;
			}
		}
		// Add an empty(removed) layer on top.
		blocksLayer[ny-1] = new GameObject();
		layerName = "Layer" + (ny-1);
		blocksLayer[ny-1].name = layerName;
		addToScene(blocksLayer[ny-1]);
		return;
	}
	
	// Add blocks to the layer.
	private void addBlocks(int layer, GameObject cube){
		if (layer >= ny){
			Application.LoadLevel("GameOver");
			return;
		}
		cube.name = "Block";
	    cube.transform.parent = blocksLayer[layer].transform;
		int x = (int)Math.Round(cube.transform.position.x + (nx - 17)/2) + 2;
		int z = (int)Math.Round(cube.transform.position.z + (nz - 17)/2) + 2;
		boardArray[x,layer,z] = true;
		blocksInLayer[layer]++;
		//Debug.Log(x+" "+layer+" "+z);
	}
	
	public bool checkPosition(int x, int y, int z){
		// if position is outside the array x and z coordiantes return collision
		// if the shape is above the array, but within boundaries let it move
		// else check for array collisions
		if (x < 0 || z < 0 || x >= nx || z >= nz) return true;
		else if (y >= ny) return false;
		else return boardArray[x,y,z];
	}
	

    //Called from BlockControl to check if the top layer has been filled.
    public bool isGameOver(){
    //    Debug.Log("checking if game over");
        for(int i = 1; i < nx-1; i++)
            for(int j = 1; j < nz-1; j++)
                if(boardArray[i,ny-3,j])
                    return true;
        return false;
    }
    

	public void printArray(){
		print("******A collision has happened,,,,,,,the board array looks like this ------->>>>>>");
		for(int k=0;k<ny;k++){
			for(int i=1; i< nx-2;i++){
				for(int j=1;j < nz-2;j++){
					if(boardArray[i,k,j]==true) print("x="+i+" y="+k+" z="+j);
						//Debug.Log(boardArray[i,k,j]);
				}
			}
		}
	}
	

	// Make an object a child of the Scene.
	private void addToScene(GameObject o){
		GameObject scene = GameObject.Find("Scene");
		Transform t = o.transform;
		t.parent = scene.GetComponent<Transform>();
	}

    /* Create a gap of x seconds to countdown 3-2-1-GO before the game
    **   actually starts.
    */
    IEnumerator InitCountdown(int x){
        yield return new WaitForSeconds(x);
        goText = false;
		blockCtrl.linkBlock(blockCtrl.xTime);
    }

	
	public void pauseGame(float start){
        if(!countdown){
            starttimer = start;
            //Debug.Log("Start Time = " + starttimer);
            countdown = true;
            pieceSuggestor = true;
        }
    }


    //set the timer to 0, stop the countdown and fetch next piece.
    public void unpauseGame(){
        // Debug.Log("Points? --------> " + timer);
        //prevent increase in score if game ends.
        if(!blockCtrl.gameOver)
            scoring(0, (int)timer);
        rounds++;
        timer = 0;
        countdown = false;
    }


	public void createMesh(){
		for (int i = 0; i < ny; i++){
			CombineChildren c = blocksLayer[i].GetComponent<CombineChildren>();
			if (c != null){
				Destroy(c);
			}
			blocksLayer[i].AddComponent<CombineChildren>();
		}
	}

    //initialise textures to colour the timer and
    //  display the suggested pieces as 2D textures.
    void initTextures(){
        //initialise the colours used.
        Color red_color = new Color(0.3f,0,0);
        Color white_color = new Color(1,1,1);
        Color black_color = new Color(0,0,0);
 
        //initialise colour for the progress bar.
        fullTex = new Texture2D(1,1);
        fullTex.SetPixel(0,0,red_color);
        fullTex.Apply();
 
        //colour for the ticker box. Currently black with white border.
        emptyTex = new Texture2D(150,50);
 
        //inside of the box
        for(int i = 1; i < emptyTex.width-1; i++){
            for(int j = 1; j < emptyTex.height-1; j++){
                emptyTex.SetPixel(i,j,black_color);
            }
        }
 
        //box's border
        for(int i = 0; i < emptyTex.height; i++){
            for(int j = 0; j < 5; j++)
                emptyTex.SetPixel(j,i,white_color);
            for(int j = emptyTex.width-6; j < emptyTex.width; j++)
                emptyTex.SetPixel(j,i,white_color);
        }
 
        for(int i = 0; i < emptyTex.width; i++){
            for(int j = 0; j < 5; j++)
                emptyTex.SetPixel(i,j,white_color);
            for(int j = emptyTex.height-6; j < emptyTex.height; j++)
                emptyTex.SetPixel(i,j,white_color);
        }
        emptyTex.Apply();
    }

    /* The GUI contains of 3 main objects - the score board, the timer and
    ** the piece suggestions.
    */
    void OnGUI () {
        //display start timer
        if((Time.realtimeSinceStartup - startTime) < 1){
            Texture count = Resources.Load("3") as Texture2D;
            GUI.DrawTexture(new Rect(Screen.width/2-215.5f, Screen.height/2-225, 431, 450), count);
        }
        else if((Time.realtimeSinceStartup - startTime) > 1 &&
            (Time.realtimeSinceStartup - startTime) < 2){
            Texture count = Resources.Load("2") as Texture2D;
            GUI.DrawTexture(new Rect(Screen.width/2-218.5f, Screen.height/2-225, 437, 450), count);
        }
        else if((Time.realtimeSinceStartup - startTime) > 2 &&
            (Time.realtimeSinceStartup - startTime) < 3){
            Texture count = Resources.Load("1") as Texture2D;
            GUI.DrawTexture(new Rect(Screen.width/2-115.5f, Screen.height/2-225, 231, 450), count);
        }
        else if((Time.realtimeSinceStartup - startTime) > 3 &&
            (Time.realtimeSinceStartup - startTime) < 4 && goText == true){
            Texture count = Resources.Load("go") as Texture2D;
            GUI.DrawTexture(new Rect(Screen.width/2-460, Screen.height/2-196.5f, 920, 393), count);
        }
		
        //GUIText
		Texture t = (Texture)Resources.LoadAssetAtPath("Assets/TopDownView.renderTexture", typeof(Texture));
		GUI.DrawTexture(new Rect ((Screen.width - 300),(Screen.height - 300),300,300),t);
		
		GUIStyle score_style = new GUIStyle();
		score_style.fontSize = 40;
		score_style.fontStyle = FontStyle.Bold;
		score_style.normal.textColor = Color.white;

        GUIStyle scoreText_style = new GUIStyle();
        scoreText_style.fontSize = 20;
        scoreText_style.fontStyle = FontStyle.Bold;
        scoreText_style.normal.textColor = Color.white;
		
		GUIStyle gameover_message = new GUIStyle();
		gameover_message.fontSize = 25;
		gameover_message.normal.textColor = Color.white;
		
		GUIStyle input_text = new GUIStyle();
		input_text.fontSize = 25;
		input_text.normal.textColor = Color.white;
		
		GUIStyle LBoard_title = new GUIStyle();
		LBoard_title.fontSize = 40;
		score_style.alignment = TextAnchor.MiddleCenter;
		LBoard_title.fontStyle = FontStyle.Bold;
		LBoard_title.normal.textColor = Color.white;
		
		GUIStyle LBoard_header = new GUIStyle();
		LBoard_header.fontSize = 30;
		LBoard_header.fontStyle = FontStyle.Bold;
		LBoard_header.normal.textColor = Color.white;
		
    	// The following line of code displays the current score.
        GUI.Box(new Rect (80,40,100,40), "Score: ", scoreText_style);
        GUI.Box(new Rect (35,40,150,100), "" + score, score_style);
        if(countdown)
            GUI.Label(new Rect(130,110,70,20), "+ " + time + " ", scoreText_style);
		
		

        // The following lines of code deals with the countdown timers.
        if(countdown){
            float timediff = Time.realtimeSinceStartup - starttimer;
            if(timediff < timeGap){
                //Debug.Log("timediff = " + timediff);
                timer = (int)timediff;
                //Debug.Log("Timer = " + timer);
                timer = timeGap - timer;
                if(!blockCtrl.gameOver){
                    barDisplay = 0.01f;
                    barDisplay = 1 - timediff * 1.0f/timeGap;//0.1f;
                }
            }
        }else{
            timer = 0;
        }

        //timer progress bar.
        //draw the background
		GUI.BeginGroup(new Rect((Screen.width - 200), 10, 150, 100));
			GUI.DrawTexture(new Rect(0,25,150,50), emptyTex);
			//draw the ticker
			GUI.BeginGroup(new Rect(5,31,140,39));
				GUI.DrawTexture(new Rect(0,0,140*barDisplay,40), fullTex);//, ScaleMode.ScaleToFit, true, 10.0F);
			GUI.EndGroup();

            //TODO: needs to be styled & re-positioned
            time = Convert.ToInt32(1000 * barDisplay);
            time /= 10;
            time *= 10;
		GUI.EndGroup();
        
        // The next bit of the code deals with piece suggestions and displaying them.
        if(pieceSuggestor){
           	//This is where piece suggestions will be made to display them
           	//	in the following boxes.

           	Array_GameObj showPieceScript;
			showPieceScript = GameObject.Find("Allowed pieces").GetComponent<Array_GameObj>();
			showPieceScript.SuggestLegoPiece();

			legoSuggestions = showPieceScript.suggestedPieces;

			pieceSuggestor = false;
        }
		
		GUIStyle suggest_style = new GUIStyle();

        GUI.Box(new Rect((Screen.width - 260), 50, 405, 270), lego[legoSuggestions[0]],suggest_style);
        GUI.Box(new Rect((Screen.width - 260), 200, 405, 270), lego[legoSuggestions[1]],suggest_style);
		first = false;

//        GUI.Box(new Rect((Screen.width - 260), 350, 405, 270), lego[legoSuggestions[2]],suggest_style);

        //Game over overlay graphics
        if(blockCtrl.gameOver){
            if(!viewLBoard){
			    textbox.SetActive(true);
			
                Texture gameOverTexture = Resources.Load("gameover") as Texture2D;
                GUI.DrawTexture(new Rect(Screen.width/2-305.2f, 100, 610.4f, 297.6f), gameOverTexture);			
    			
                GUI.Label(new Rect(Screen.width/2-200,450,200,25), "Final score: " + score, gameover_message);

                if(!scoreSubmitted){
                    GUI.Label(new Rect(Screen.width/2-200,500,300,25), "Enter your name:", gameover_message);
    				uname = GUI.TextField (new Rect (Screen.width/2-200,550,300,40), uname, input_text);
                    
                    if(GUI.Button(new Rect(Screen.width/2-150,Screen.height-300,300,25), "Submit Score")){
                        lboardPosition = lboard.AddScore(uname, score);
                        scoreSubmitted = true;
                        // Debug.Log("Score submitted = " + scoreSubmitted);
                        viewLBoard = true;
                    }
                }
            }
			/*
            //Display the player's score and the two higher & lower than his/hers.
            if(viewPartLBoard && !viewLBoard){
                if(!rcvdPartScoreboard){
                    dispPartScores = lboard.DisplayScores();
                    receivedLboard = true;
                }

                //when score is submitted, we want the small leaderboard to be displayed.

                GUI.Label(new Rect(Screen.width/2 - 200, 100, 400, 25), "Position");
                GUI.Label(new Rect(Screen.width/2 - 50, 100, 100, 25), "Name");
                GUI.Label(new Rect(Screen.width/2 + 200, 100, 400, 25), "Scores");
             
                Debug.Log("lboardPosition = " + lboardPosition + ";;;;" + dispPartScores.Count);

                int iStart = (lboardPosition-3)*2 > 0? (lboardPosition-3)*2 : 0;
                int iFinish = ((lboardPosition+2)*2) < dispPartScores.Count ? (lboardPosition+2)*2 : dispPartScores.Count;

                //Deals with special circumstances to show 5 entries,
                //  unless there is less than 5 entries present.
                //if the person's position is 1 or 2.
                if(iStart == 0)
                    iFinish = 10 < (dispPartScores.Count)? 10 : dispPartScores.Count;

                //if the player is in the bottom 2.
                if(iFinish == (dispPartScores.Count))
                    iStart = ((iFinish - 10) > 0) ? (iFinish - 10) : 0;


                int c = 0;
                for(int i = iStart; i < iFinish; i+=2){
                    GUI.Label(new Rect(Screen.width/2 - 200, 150 + c*35, 400, 25), "" + (i/2 + 1));
                    GUI.Label(new Rect(Screen.width/2 - 50, 150 + c*35, 400, 25), "" + dispPartScores[i]);
                    GUI.Label(new Rect(Screen.width/2 + 200, 150 + c*35, 400, 25), "" + dispPartScores[i+1]);
                    Debug.Log("data = " + dispPartScores[i] + "   score = " + dispPartScores[i+1]);
                    c++;
                }

                GUI.Box(new Rect(Screen.width/2 - 300, 40, 600, Screen.height-150), "LEADERBOARD OF SHAME AND FAME");

                if(GUI.Button(new Rect(Screen.width/2-200, Screen.height - 100, 150, 75), "View top 10 scores")){
                    viewLBoard = true;
                    viewPartLBoard = false;
                }
            }*/
		
            //Leaderboard graphics
            if(viewLBoard){
    			
    			textbox.SetActive(false);
    			
                if(!receivedLboard){
                    dispPartScores = lboard.DisplayScores();
                    receivedLboard = true; //!receivedLboard;
                }
    			
                GUI.Label(new Rect(Screen.width/2 - 250, 150, 400, 25), "Position", LBoard_header);
                GUI.Label(new Rect(Screen.width/2 - 75, 150, 100, 25), "Name", LBoard_header);
                GUI.Label(new Rect(Screen.width/2 + 150, 150, 400, 25), "Scores", LBoard_header);

				int iStart = (lboardPosition-3)*2 > 0? (lboardPosition-3)*2 : 0;
				int iFinish = ((lboardPosition+2)*2) < dispPartScores.Count ? (lboardPosition+2)*2 : dispPartScores.Count;
				
				//Deals with special circumstances to show 5 entries,
				//  unless there is less than 5 entries present.
				//if the person's position is 1 or 2.
				if(iStart == 0)
					iFinish = 10 < (dispPartScores.Count)? 10 : dispPartScores.Count;
				
				//if the player is in the bottom 2.
				if(iFinish == (dispPartScores.Count))
					iStart = ((iFinish - 10) > 0) ? (iFinish - 10) : 0;

				int c = 0;
				for(int i = iStart; i < iFinish; i+=2){
                    // Debug.Log("TEST -------- " + dispScores[i]);
                    GUI.Label(new Rect(Screen.width/2 - 200, 200 + c*50, 400, 25), ""+(i/2+1), gameover_message);
                    GUI.Label(new Rect(Screen.width/2 - 75, 200 + c*50, 400, 25), "" + dispPartScores[i], gameover_message);
                    GUI.Label(new Rect(Screen.width/2 + 150, 200 + c*50, 400, 25), "" + dispPartScores[i+1], gameover_message);
					c++;
                }

                GUI.Box(new Rect(Screen.width/2 - 250, 75, 600, Screen.height-200), "Leaderboard Position", LBoard_title);

                if(GUI.Button(new Rect(Screen.width/2-75,Screen.height - 500,150,75), "Continue")){
                    Time.timeScale = 1;
                    Application.LoadLevel("MainMenu");
                }
            }//end of leaderboard
        }//end of gameover display
    }//end of OnGUI
}//end of class

