using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class Board : MonoBehaviour {
	
	private GameObject[] blocksLayer;
	
	// Dimensions of the board in "shapes".
	public int nx = 17;	// width
	public int ny = 15;	// height
	public int nz = 17;	// depth 

	public float transx,transz;
	
	private int flashPass;

	int timeGap = 3; //default value. Change gap here!
    int timer = 0;
    float starttimer = 0.0f;
    bool countdown = false;
    bool pieceSuggestor = false;

    //scoring stuff
    public int score = 0;
    private bool viewLBoard = false;
    private bool scoreSubmitted = false;
    private bool receivedLboard = false;
    ArrayList dispScores = new ArrayList();

    private string uname = "Voldemort";

    //1-easy, 2-intermediate, 3-expert
    public int level = 1; 
    //stores number of times the player has constructed a piece
    public int rounds = 0; 

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
    public Transform grid;

	private bool[,,] boardArray;
	
	//for rotating the board
	//ricky moved the board initialy so the bottom left == (0,0) so centre is this
	private Vector3 centreRotation = new Vector3 (2,1,2);

	//Audio stuff
	public AudioSource audio_source;
	public AudioSource layer_clear_sound;


	private BlockControl blockCtrl;
    private GameOver gOver;
    private Leaderboard lboard;

	private Camera topCam;

    //starting text
    private GUIText startGameText;
    private bool goText = true;

    //start time
    float startTime;

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
		//pinsPerShape = blockCtrl.getShapeSize();
		blocksLayer = new GameObject [ny];
		
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
 
        lego[0] = Resources.Load("L2x1") as Texture2D;
        lego[1] = Resources.Load("L2x2") as Texture2D;
        lego[2] = Resources.Load("L3x2") as Texture2D;
        lego[3] = Resources.Load("L4x2") as Texture2D;

        legoSuggestions = new int[3];

		//shadow layer
		GameObject slayer = new GameObject();
		String lName = "ShadowLayer";
		slayer.name = lName;
		addToScene(slayer);
		
		flashPass = 0;
		
		DrawBoard();
		
		GameObject block2 = (GameObject)Resources.LoadAssetAtPath("Assets/block2.prefab", typeof(GameObject));
		grid = block2.transform;
        //boundaries for initial orientation
        DrawBoundary(0);
		createTopCamera();

	//	audio_source = GameObject.Find("Main Camera").AddComponent<AudioSource>();
//		layer_clear_sound = GameObject.Find("Main Camera").AddComponent<AudioSource>();
//		layer_clear_sound.clip = (AudioClip) Resources.LoadAssetAtPath("Assets/Music/Triumph.wav", typeof(AudioClip));
//		startMusic("Theme1");

		//start gameplay.


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
                xTime = 88888888;
                shapeFalling = true;
                Debug.Log ("ENTER");
            }
        }

        /*if(blockCtrl.gameOver){
            Debug.Log("Score--------->" + score);
            gOver.endGame(score);
        }*/
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
	
	private void startMusic(String track){
		if (audio_source.isPlaying) audio_source.Stop();
		audio_source.clip = (AudioClip) Resources.LoadAssetAtPath("Assets/Music/" + track + ".wav", typeof(AudioClip));
		audio_source.Play();
	}

	private void setMaxVolume(){
		audio_source.volume = 1.0f;
	}

	public void playLayerClearSound(){
		//if (audio_source != null) audio_source.volume = 0.1f;
		//layer_clear_sound.Play();
		//if (audio_source != null) Invoke( "setMaxVolume", layer_clear_sound.clip.length );
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
		
		// Blocks fall from (0,0) into bottom corner.
		//cube.transform.localScale = new Vector3(nx-2, 0.2F, nz-2);
		//cube.transform.position = new Vector3(6.0F, -0.1F, 6.0F);
	
		//set the center to be the pivot
//		centreRotation = new Vector3 ((float)cube.transform.position.x,cube.transform.position.y,(float)cube.transform.position.z);
		
		// Make the base a child of the scene
		GameObject scene = GameObject.Find("Scene");
		Transform t = legoBase.transform;
		t.parent = scene.GetComponent<Transform>();
		if (nx%2 == 0) legoBase.transform.Translate(-0.5f,0.0f,-0.5f,Space.World);
		//cube.transform.Translate(0,-0.4f,0);
		//-1.5 -1.5
		/*Debug.Log(cube.renderer.bounds.min.x + " " + cube.renderer.bounds.min.z);
		transx = (float) (-1.5 - cube.renderer.bounds.min.x);
		transz = (float) (-1.5 - cube.renderer.bounds.min.z);*/
		//float transx = (float)Math.Abs(1.5 - Math.Abs(cube.renderer.bounds.min.x));
		//float transz = (float)Math.Abs(1.5 - Math.Abs(cube.renderer.bounds.min.z));
		//cube.transform.Translate(transx,0,transz);
		//cube.transform.localPosition = new Vector3(cube.transform.localPosition.x, -0.1f, cube.transform.localPosition.z);
	}
	


    //Create visible boundary walls.
    public void DrawBoundary(int rotDir){
        GameObject[] argo = GameObject.FindGameObjectsWithTag("WallPiece");
        foreach (GameObject go in argo) {
            Destroy(go);
        }

        for(int i=0; i< ny;i++){
            for(int j=-1;j < nx-3;j++){
                if(rotDir == 0){
                    Instantiate(grid, new Vector3(j,i,nz-3),Quaternion.identity);
                    Instantiate(grid, new Vector3(nx-3,i,j),Quaternion.identity);
                }
                else if(rotDir == 1){
                    Instantiate(grid, new Vector3(j,i,nz-3),Quaternion.identity);
                    Instantiate(grid, new Vector3(-2,i,j),Quaternion.identity);
                }
                else if(rotDir == 2){
                    Instantiate(grid, new Vector3(-2,i,j),Quaternion.identity);
                    Instantiate(grid, new Vector3(j,i,-2),Quaternion.identity);   
                }
                else{
                    Instantiate(grid, new Vector3(j,i,-2),Quaternion.identity); 
                    Instantiate(grid, new Vector3(nx-3,i,j),Quaternion.identity);

                }
            }
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
			if(blocksLayer[i].transform.childCount == (nx-2)*(nz-2)){
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
            score += 1000/timeGap * addScore;
            //score += addScore;
        }
        else
        score += addScore * 10;
    }

	// Clear the layer (i.e. reset the layer count to 0).
	public void clearLayer(int y) {
		playLayerClearSound();

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
				// Translate down only if blocks present.
				if (blocksLayer[k-1].transform.childCount > 0){
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
		cube.name = "Block";
	    cube.transform.parent = blocksLayer[layer].transform;
		int x = (int)Math.Round(cube.transform.position.x + (nx - 17)/2) + 2;
		int z = (int)Math.Round(cube.transform.position.z + (nz - 17)/2) + 2;
		boardArray[x,layer,z] = true;
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
        Debug.Log("checking if game over");
        for(int i = 1; i < nx-1; i++)
            for(int j = 1; j < nz-1; j++)
                if(boardArray[i,ny-1,j])
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
        StartCoroutine(Wait());
    }

	/* Creates a gap of x (currently 10 for initial testing purposes) seconds
	**	 before the first shape is triggered.
	** The game is "paused" -- the shape doesn't descend but you can still
	**   rotate the board, if you need to see where you'd place the blocks.
	*/
	IEnumerator Wait(){
        float time = Time.realtimeSinceStartup;
        if(time > 2)
        	pauseGame(Time.realtimeSinceStartup);
        else
            pauseGame(0f);

        xTime = 0;

        float startTimerr = Time.realtimeSinceStartup;
        while(xTime < (timeGap / 0.01) &&
            startTimerr > (Time.realtimeSinceStartup - timeGap)){
            yield return new WaitForSeconds(0.01f);
            Debug.Log("1");
            xTime++;
        }

        Debug.Log("2");

        Debug.Log("xTime = " + xTime);
        unpauseGame();
            
        //game over mechanism
        if(xTime == 88888889){ //xTime < (timeGap / 0.01)){
            //Debug.Log("ENTER PRESSED!");
            blockCtrl.createShape();
        }
        else{
            Debug.Log("Enter not pressed?");
            Time.timeScale = 0;
            blockCtrl.gameOver = true;
            //Application.LoadLevel("GameOver");
        }		
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
        Debug.Log("Points? --------> " + timer);
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
            Texture count = Resources.Load("3") as Texture2D;//(Texture)Resources.LoadAssetAtPath /////
            //GUI.Label(new Rect(Screen.width/2-50, Screen.height/2-25, 100, 50), "READY");
            GUI.DrawTexture(new Rect(Screen.width/2-100, Screen.height/2-100, 200, 200), count);
        }
        else if((Time.realtimeSinceStartup - startTime) > 1 &&
            (Time.realtimeSinceStartup - startTime) < 2){
            //GUI.Label(new Rect(Screen.width/2-50, Screen.height/2-25, 100, 50), "SET");
            Texture count = Resources.Load("2") as Texture2D;
            GUI.DrawTexture(new Rect(Screen.width/2-100, Screen.height/2-100, 200, 200), count);
        }
        else if((Time.realtimeSinceStartup - startTime) > 2 &&
            (Time.realtimeSinceStartup - startTime) < 3){// && goText == true){
            //GUI.Label(new Rect(Screen.width/2-50, Screen.height/2-25, 100, 50), "GO");
            Texture count = Resources.Load("1") as Texture2D;
            GUI.DrawTexture(new Rect(Screen.width/2-100, Screen.height/2-100, 200, 200), count);
        }
        else if((Time.realtimeSinceStartup - startTime) > 3 &&
            (Time.realtimeSinceStartup - startTime) < 4 && goText == true){
            Texture count = Resources.Load("go") as Texture2D;
            GUI.DrawTexture(new Rect(Screen.width/2-175, Screen.height/2-100, 350, 200), count);
        }
        //GUIText

		Texture t = (Texture)Resources.LoadAssetAtPath("Assets/TopDownView.renderTexture", typeof(Texture));
		GUI.DrawTexture(new Rect ((Screen.width - 300),(Screen.height - 300),300,300),t);

    	// The following line of code displays the current score.
        GUI.Box(new Rect (50,10,150,100), "Score " + score);

        // The following lines of code deals with the countdown timers.
        if(countdown){
            float timediff = Time.realtimeSinceStartup - starttimer;
            if(timediff < timeGap){
                barDisplay = 0.01f;
                //Debug.Log("timediff = " + timediff);
                timer = (int)timediff;
                //Debug.Log("Timer = " + timer);
                timer = timeGap - timer;
                barDisplay = 1 - timediff * 1.0f/timeGap;//0.1f;
            }
        }else{
            timer = 0;
        }

        //timer progress bar.
        //draw the background
		GUI.BeginGroup(new Rect((Screen.width - 200), 10, 150, 100));
			//GUI.Box(new Rect(0,25,150,50), timer + "s left.");
			GUI.DrawTexture(new Rect(0,25,150,50), emptyTex);
			//draw the ticker
			GUI.BeginGroup(new Rect(5,31,140,39));
				GUI.DrawTexture(new Rect(0,0,140*barDisplay,40), fullTex);//, ScaleMode.ScaleToFit, true, 10.0F);
			GUI.EndGroup();
		GUI.EndGroup();
        //GUI.Box(new Rect((Screen.width - 200), 10, 150, 100), timer + "s left.");
        
        // The next bit of the code deals with piece suggestions and displaying them.
        if(pieceSuggestor){
           	//This is where piece suggestions will be made to display them
           	//	in the following boxes.

           	Array_GameObj showPieceScript;
			showPieceScript = GameObject.Find("Allowed pieces").GetComponent<Array_GameObj>();
			showPieceScript.SuggestLegoPiece();

			legoSuggestions = showPieceScript.suggestedPieces;
			for(int i = 0; i < 3; i++)
				//Debug.Log("Lego pc suggestion (" + i + ") = " + legoSuggestions[i]);

			pieceSuggestor = false;
        }

        GUI.Box(new Rect((Screen.width/2 - 250), 10, 150, 100), lego[legoSuggestions[0]]);
        GUI.Box(new Rect((Screen.width/2 - 75), 10, 150, 100), lego[legoSuggestions[1]]);
        GUI.Box(new Rect((Screen.width/2 + 100), 10, 150, 100), lego[legoSuggestions[2]]);

        //Game over overlay graphics
        if(blockCtrl.gameOver && !viewLBoard){
            Texture gameOverTexture = Resources.Load("gameover") as Texture2D;
            GUI.DrawTexture(new Rect(Screen.width/2-250, 140, 450, 250), gameOverTexture);

            GUI.Label(new Rect(Screen.width/2-150,450,200,25), "Your score was " + score);

            if(!scoreSubmitted){
                GUI.Label(new Rect(Screen.width/2-150,470,300,25), "Type your name to add to the leaderboard.");
                uname = GUI.TextField (new Rect (Screen.width/2-150,510,300,30), uname);
                
                if(GUI.Button(new Rect(Screen.width/2-150,560,300,25), "Submit score to leaderboard.")){
                    lboard.AddScore(uname, score);
                    scoreSubmitted = true;
                    Debug.Log("Score submitted = " + scoreSubmitted);
                }
            }else{
                int position = 99999;
                GUI.Label(new Rect(Screen.width/2-150,450,200,25), uname + " has been added to leaderboard.");
                GUI.Label(new Rect(Screen.width/2-150,450,100,25), "You are position " + position + "!");
            }

            if(GUI.Button(new Rect(Screen.width/2-200, Screen.height - 100, 150, 75), "View Leaderboard"))
                viewLBoard = true;

            if(GUI.Button(new Rect(Screen.width/2+50, Screen.height - 100, 150,75), "Click to restart game")){
                Time.timeScale = 1;
                Application.LoadLevel("MainMenu");
            }
        }

        //Leaderboard graphics
        if(viewLBoard){
            if(!receivedLboard){
                dispScores = lboard.DisplayScores();
                receivedLboard = true;
            }

            GUI.Label(new Rect(Screen.width/2 - 200, 100, 400, 25), "Position");
            GUI.Label(new Rect(Screen.width/2 - 50, 100, 100, 25), "Name");
            GUI.Label(new Rect(Screen.width/2 + 200, 100, 400, 25), "Scores");

            
            for(int i = 0; i < dispScores.Count; i+=2){
                // Debug.Log("TEST -------- " + dispScores[i]);
                GUI.Label(new Rect(Screen.width/2 - 200, 100 + i*35, 400, 25), ""+(i+1));
                GUI.Label(new Rect(Screen.width/2 - 50, 150 + i*35, 400, 25), "" + dispScores[i]);
                GUI.Label(new Rect(Screen.width/2 + 200, 150 + i*35, 400, 25), "" + dispScores[i+1]);
            }


            GUI.Box(new Rect(Screen.width/2 - 300, 40, 600, Screen.height-150), "LEADERBOARD OF SHAME AND FAME");

            if(GUI.Button(new Rect(Screen.width/2-200, Screen.height - 100, 150, 75), "View Game Over"))
                viewLBoard = false;

            if(GUI.Button(new Rect(Screen.width/2+50,Screen.height - 100,150,75), "Click to restart game")){
                Time.timeScale = 1;
                Application.LoadLevel("MainMenu");
            }
        }
    }
}
