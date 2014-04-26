using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;




public class BlockControl : MonoBehaviour {
	
	private GameObject[] blocks;
	//setUpWebcam cam;
	private GameObject shadow, highlight;
	private ShadowCollision sh;
	RotateCamera cameraScript;
	private int globalX, globalZ;
	private int timeGap = 0;
	private int maxPinsX,maxPinsZ;
	string legoCode;
	
	private float boundX,boundZ,boundingBox;
	
	private int pass;

	private bool firstBlock = true;

	private bool waitActive = false;

	//sample shape, just fo shows
	private int[,,] shape1 = new int[,,] {{{1,1,1},{1,1,0},{1,0,0}},
									   	  {{0,0,0},{0,0,0},{0,0,0}},
									      {{0,0,0},{0,0,0},{0,0,0}}};
	
	private int[,,] shape2 = new int[,,] {{{1,1,1},{1,1,1},{0,0,0}},
									   	  {{1,1,1},{1,1,1},{0,0,0}},
									      {{1,1,1},{1,1,1},{0,0,0}}};
	
	//[,,]full outer number, middle inner, inside the smallest
	private int[,,] shape3 = new int[,,] {{{1,1,1},{0,0,0},{0,0,0}},
									   	  {{1,1,1},{0,0,0},{0,0,0}},
									      {{1,1,1},{0,0,0},{0,0,0}}};

	//private int[,,] shapeTemp = new int[20,20,20];

	private int[,,] shape4;
	[DllImport ("make2")]
	private static extern IntPtr lego();

	[DllImport ("make2")]
	private static extern int main();

	
	/* size of a single "pin", i.e. a cube 
	 * that makes a building block of a shape. */
	public float pinSize = 1.0f;
	
	Board gameBoard;
	
	//starting height where shapes are created
	public float startHeight = 50.0f;
	
	// Variable to assign pins unique names.
	private int pin = 0;
	
	private float timer;
	
	private int shapeMove;
	
	private GameObject FragmentCube;

	//[DllImport ("make2")]
	//private static extern IntPtr lego();

	//for moving the shapes need to know the centre of shape
	private float posX,posZ;	
	private Vector3 centreRotation = new Vector3 (2,1,2);
	private int currentShapeLength;
	
	//rotate the shape array
	int[,,] rotateShape(int[,,] shape, bool clockwise){
		
		int [,,] newShape = new int[,,]{{{0,0,0},{0,0,0},{0,0,0}},
									   	{{0,0,0},{0,0,0},{0,0,0}},
									    {{0,0,0},{0,0,0},{0,0,0}}};
		
		//loop through shape array
		for(int i=0; i< shape.GetLength(2); i++){
			for(int j=0; j< shape.GetLength(1); j++){
				for(int k = 0;k<shape.GetLength(0);k++){
					//rotate 90 degrees around centre of bounding box
					if (clockwise)
						newShape[j, shape.GetLength(2)-1-i, k] = shape[i,j,k];
					else
						newShape[shape.GetLength(1)-1-j, i, k] = 1;
				}
			}
		}
		return newShape;
	}




	// Pre-Initialization.
	void Awake(){
		Debug.Log("initialise cam");
		//cam = new setUpWebcam();
		//cam.setUpCams();
		globalX = 0;
		globalZ = 0;

		gameBoard = GetComponent<Board>();
		getBoardValues();
	}
	
	// Initialization.
	void Start () {

		pass = 0;	
		timer = 1;
		shapeMove=0;
		cameraScript = GameObject.Find("Main Camera").GetComponent<RotateCamera>();
		//getShapeArray();

	}
	
	// Set maximum amount of pins that can fit in each direction.
	public void getBoardValues(){
		maxPinsX = gameBoard.nx;
		maxPinsZ = gameBoard.nz;
	}


	// For testing purposes
	public int[,,] getShapeArray(){
		int[,,] shapeTemp = new int[20,20,20];
		string data = "13.1.11.1.13.1.10.1.12.1.10.1.11.1.10.1.10.1.10.1.10.2.10.1.10.3.10.1.11.2.10.1.";
		string[] dA = data.Split('.');
		for (int i = 0; i < dA.Length - 1; i+=4){
			int x = Int32.Parse(dA[i]);
			int y = Int32.Parse(dA[i+1]);
			int z = Int32.Parse(dA[i+2]);
			shapeTemp[x,y,z] = Int32.Parse(dA[i+3]);
		}
		return transformShape(shapeTemp);
	}

	// Get the shape from the computer vision stuff and puts in to the shape array
	public int[,,] getShapeArray(string data){
		int[,,] shapeTemp = new int[20,20,20];
		string[] dA = data.Split('.');
		for (int i = 0; i < dA.Length - 1; i+=4){
			int x = Int32.Parse(dA[i]);
			int y = Int32.Parse(dA[i+1]);
			int z = Int32.Parse(dA[i+2]);
			shapeTemp[x,y,z] = Int32.Parse(dA[i+3]);
		}
		return transformShape(shapeTemp);
	}
	/*
	 * Gets rid of empty space around the shape box. Inverts the shape.
	 */
	private int[,,] transformShape(int[,,] shape){
		int[,,] shape4;
		int minY = shape.GetLength(0);
		int maxY = 0;
		int minX = shape.GetLength(0);
		int maxX = 0;
		int minZ = shape.GetLength(0);
		int maxZ = 0;
		for (int x=0; x < shape.GetLength(0); x++){
			for (int y=0; y < shape.GetLength(1); y++){
				for (int z=0; z < shape.GetLength(2); z++){
					if (shape[x,y,z] != 0){
						if (y < minY) minY=y;
						if (y > maxY) maxY=y;
						if (x < minX) minX=x;
						if (x > maxX) maxX=x;
						if (x < minZ) minZ=z;
						if (x > maxZ) maxZ=z;
					}
				}
			}
		}
		Debug.Log("Minmax: " + (maxY-minY+1));
		shape4 = new int[maxX-minX+1,maxY-minY+1,maxZ-minZ+1];
		for (int x=0; x < shape.GetLength(0); x++){
			for (int y=0; y < shape.GetLength(1); y++){
				for (int z=0; z < shape.GetLength(2); z++){
					if (shape[x,y,z] != 0){
						shape4[maxX-x,maxY-y,maxZ-z]=shape[x,y,z];
						//shape4[x,minY + y,z]=shape[x,y,z];	
					}
				}
			}
		}
		return shape4;
	}
	/*public void getShapeArray(){
		List<int[]> list = new List<int[]>();
		list.Add(new int[]{1,1,1,1});
		list.Add(new int[]{1,2,1,1});
		list.Add(new int[]{1,3,1,1});
		list.Add(new int[]{2,2,1,1});
		//Debug.Log("hi");
		foreach (int[] i in list){ // Loop through List with foreach
			//getting array [x,y,z,c];
			//shape4[i[0],i[1],i[2]] = i[3];
		}		
	}*/
	
	//check the pieces that the user uses against the allowed pieces
	public int checkPieces(int[,,] shape){

		Array_GameObj pieceArray;
		//access Array_GameObj through this
		pieceArray = GameObject.Find("Allowed pieces").GetComponent<Array_GameObj>();

		//number of suggested pieces for looping through the suggested pieces
		int numberOfPieces = pieceArray.noOfSuggestedPieces;

		int yellow=0 , green=0, blue=0, red=0;

		//count the amount of colours from the shape array
		for (int x=0; x < shape.GetLength(0); x++){
			for (int y=0; y < shape.GetLength(1); y++){
				for (int z=0; z < shape.GetLength(2); z++){
					switch(shape[x,y,z]){
					case 0:	break;//no block
					case 1: red++; break;//red
					case 2: green++; break;//green
					case 3: blue++; break;//blue
					case 4: yellow++; break;//yellow maybe CHECK
					default: throw new System.ArgumentException("Unrecognised colour number."+ shape[x,y,z]);
					}
				}
			}
		}

		//for the pieces that are suggested
		int suggestedRed=0,suggestedGreen=0,suggestedBlue=0, suggestedYellow=0;
		//count the amount of colours from the pieces suggested
		for(int i=0; i<numberOfPieces;i++){
			//case number from Array_GameObj so different to the cases above from the shape array
			switch(pieceArray.suggestedPieces[i]){
			case 0: suggestedYellow++; break;//yellow
			case 1: suggestedGreen++; break;//green
			case 2: suggestedBlue++; break;//blue
			case 3: suggestedRed++; break;//red
			}
		}
		//amountColour - amount of the certain colour the user has used
		//suggestedColour - amount of certain colour the computer has used
		if(!(red==suggestedRed*8 && blue==suggestedBlue*6 && green==suggestedGreen*4 && yellow==suggestedYellow*2)){

			print ("THE USER HAS NOT USED THE CORRECT SHAPES!!!");
		}


		return 0;
	}
	
	/*public void getShapeArray(){
		List<int[]> list = new List<int[]>();
		list.Add(new int[]{1,1,1,1});
		list.Add(new int[]{1,2,1,1});
		list.Add(new int[]{1,3,1,1});
		list.Add(new int[]{2,2,1,1});
		//Debug.Log("hi");
		foreach (int[] i in list){ // Loop through List with foreach
			//getting array [x,y,z,c];
			//shape4[i[0],i[1],i[2]] = i[3];
		}		
	}*/

	private void getRotationCentre(int[,,] shape, float halfLength){

		float finalX, finalZ;
		finalX = shape.GetLength(0)-1;
		finalZ = shape.GetLength(2)-1;
		
		float centreX,centreZ;
		
		if(finalZ%2 == finalX%2){
			//both even or both odd
			centreX = finalX/2;
			centreZ = finalZ/2;
			print(finalX + "/2=" + centreX);
		}
		else{
			//one even side, one odd side - reduce shortest dimension by 1
			if (finalZ < finalX){
				centreX = finalX/2;
				centreZ = (finalZ - 1)/2;
			}
			else
			{
				centreX = (finalX - 1)/2;
				centreZ = finalZ/2;
			}
		}
		
		posX = centreX-halfLength;
		posZ = centreZ-halfLength;
	}
	
	private bool checkMoveAllowed(){
		//first, get all block coordinates
		List<int> xs = new List<int>();
		List<int> ys = new List<int>();
		List<int> zs = new List<int>();
		foreach (Transform child in shadow.transform){
			xs.Add((int)Math.Round(child.position.x + (gameBoard.nx - 17)/2 + 1));
			ys.Add((int)Math.Round(child.position.y - 0.38));
			zs.Add((int)Math.Round(child.position.z + (gameBoard.nz - 17)/2 + 1));
		};
		for (int i = 0; i < xs.Count; i++){
			if (ys[i] < 0) return false;
			//Debug.Log("Coord: "+(xs[i]+1)+":"+ys[i]+":" +(zs[i]+1));
			if (gameBoard.checkPosition(xs[i] + 1,ys[i],zs[i] + 1)){
				//Debug.Log("collision!"+xs[i]+":"+ys[i]+":" +zs[i]);
				return false;
			}
		}
		return true;
	}

	private bool checkArrayCollisions(){
		//first, get all block coordinates
		List<int> xs = new List<int>();
		List<int> ys = new List<int>();
		List<int> zs = new List<int>();
		foreach (Transform child in shadow.transform){
			xs.Add((int)Math.Round(child.position.x + (gameBoard.nx - 17)/2 + 1));
			ys.Add((int)Math.Round(child.position.y - 0.38));
			zs.Add((int)Math.Round(child.position.z + (gameBoard.nz - 17)/2 + 1));
		};
		for (int i = 0; i < xs.Count; i++){
			if (ys[i] < 0) return true;
			//Debug.Log("Coord: "+(xs[i]+1)+":"+ys[i]+":" +(zs[i]+1));
			if (gameBoard.checkPosition(xs[i] + 1,ys[i],zs[i] + 1)){
				Debug.Log("A collision has happened!"+xs[i]+1+":"+ys[i]+":" +zs[i]+1);
				gameBoard.printArray();
				return true;
			}
		}
		return false;
	}


	/* Creates a gap of x (currently 10 for initial testing purposes) seconds
	**	 before next shape is triggered.
	** The game is "paused" -- the shape doesn't descend but you can still
	**   rotate the board, if you need to see where you'd place the blocks.
	*/
	IEnumerator Wait(GameObject block){
		gameBoard.pauseGame(Time.realtimeSinceStartup);
        Debug.Log("Wait for " + timeGap + "s");
        waitActive = true;
        yield return new WaitForSeconds(timeGap);
        waitActive = false;
        Debug.Log("After waiting for " + timeGap + "s");

        if(block == null)
        	Debug.Log("Block is null :S !!");

        triggerNextShape(block);
		block = GameObject.Find("ActiveBlock");
		gameBoard.unpauseGame();
    }


	
	
	private void triggerNextShape(GameObject block){
		for (int i = 0; i < Math.Pow(currentShapeLength, 3); i++){
			Transform childTransform = block.transform.FindChild("Current pin" + i.ToString());
			if (childTransform != null){
				int layer = (int)Math.Round(childTransform.position.y - 0.38);
				gameBoard.FillPosition(layer, childTransform.gameObject); 
			}
		}
		gameBoard.checkFullLayers();
		block.rigidbody.isKinematic = true;
		shadow.rigidbody.isKinematic = true;
		//create new shape and destroy the old empty container
		Destroy(block);
		Destroy(shadow);
		createShape();

	}
	
	private void printShadow(GameObject shadow){
		//first, get all block coordinates
		//List<int> xs = new List<int>();
		//List<int> ys = new List<int>();
		//List<int> zs = new List<int>();
		return;
		foreach (Transform child in shadow.transform){
			Debug.Log("" + ((int)Math.Round(child.position.x) + 1) + " : " +
			((int)Math.Round(child.position.y - 0.38)) + " : " +
			((int)Math.Round(child.position.z) + 1));
		};	
	}
	// Update is called once per frame.
	void Update () {
		GameObject block = GameObject.Find("ActiveBlock");
		Vector3 translation = Vector3.zero;
		Vector3 rotation = Vector3.zero;
		int hasMoved = 0;
		int newblock = 0;

        if (block == null || shadow == null) return;
		//This keeps track of how many seconds we wait between two pieces.
		int x = 10;
		
		if (firstBlock){
			Destroy(highlight);
			highlightLanding();
			firstBlock = false;
		}
		timer -= Time.deltaTime;
		if(timer<=0){
			timer=1;
			shadow.transform.Translate(0,-1,0);
			if (checkMoveAllowed()){
				block.transform.Translate(0,-1,0);
			} else {
				//Pauses game for 10 secs before the next piece is triggered
				/* We first check if it's currently waiting or not, as
				** Update() is called several times in the 10s period.
				** We don't just wait for a value to be returned after the 10s
				** as we still need to move the board.
				*/
				if(!waitActive)
					StartCoroutine(Wait(block));
				/*triggerNextShape(block);
				block = GameObject.Find("ActiveBlock");*/
				newblock = 1;
			}
		}	
		
		//ROTATE right
		if (Input.GetKeyDown("v")){		
			rotation = new Vector3(0,90,0);
			hasMoved = 1;
		}		
		//ROTATE left
		if (Input.GetKeyDown("c")){
			rotation = new Vector3(0,-90,0);
			hasMoved = 1;
		}
		
		//piece falls down further with space bar
		if(Input.GetKey("space")){
			//check every 4 frames
			if(shapeMove%4 == 0){
				translation = new Vector3(0,-1,0);
				hasMoved = 1;
			}
		}
		
		//MOVE forward
		if (Input.GetKey("up")){
			//check every 4 frames
			if(shapeMove%4 == 0){
				if (cameraScript.rotationDir == 0){
					translation = new Vector3(0,0,1);
				}
				if (cameraScript.rotationDir == 1){
					translation = new Vector3(-1,0,0);
				}
				if (cameraScript.rotationDir == 2){
					translation = new Vector3(0,0,-1);
				}
				if (cameraScript.rotationDir == 3){
					translation = new Vector3(1,0,0);
				}
				
				hasMoved = 1;
			}
		}
		//MOVE back
		if (Input.GetKey("down")){
			//check every 4 frames
			if(shapeMove%4 == 0){
				if (cameraScript.rotationDir == 0){
					translation = new Vector3(0,0,-1);
				}
				if (cameraScript.rotationDir == 1){
					translation = new Vector3(1,0,0);
				}
				if (cameraScript.rotationDir == 2){
					translation = new Vector3(0,0,1);
				}
				if (cameraScript.rotationDir == 3){
					translation = new Vector3(-1,0,0);
				}
				
				hasMoved = 1;
			}
		}
		//MOVE right
  		if (Input.GetKey("right")){
			//check every 4 frames
			if(shapeMove%4 == 0){
				if (cameraScript.rotationDir == 0){
					translation = new Vector3(1,0,0);
				}
				if (cameraScript.rotationDir == 1){
					translation = new Vector3(0,0,1);
				}
				if (cameraScript.rotationDir == 2){
					translation = new Vector3(-1,0,0);
				}
				if (cameraScript.rotationDir == 3){
					translation = new Vector3(0,0,-1);
				}
				hasMoved = 1;
			}
  		}
  		//MOVE left
  		if (Input.GetKey("left")){
			//check every 4 frames
			if(shapeMove%4 == 0){
				if (cameraScript.rotationDir == 0){
					translation = new Vector3(-1,0,0);
				}
				if (cameraScript.rotationDir == 1){
					translation = new Vector3(0,0,-1);
				}
				if (cameraScript.rotationDir == 2){
					translation = new Vector3(1,0,0);
				}
				if (cameraScript.rotationDir == 3){
					translation = new Vector3(0,0,1);
				}
				hasMoved = 1;
			}
		}
		if (newblock != 1){
			Vector3 backupPos = shadow.transform.position;
			Quaternion backupRot = shadow.transform.rotation;
			shadow.transform.Rotate(rotation,Space.Self);
			shadow.transform.Translate(translation, Space.World);
			if (checkArrayCollisions()){
				Debug.Log("Array collision");
				Debug.Log("shadow");
				printShadow(shadow);
				Debug.Log("block");
				printShadow(block);
				shadow.transform.position = backupPos;
				shadow.transform.rotation = backupRot;
			}else{
				block.transform.Rotate(rotation,Space.Self);
				block.transform.Translate(translation, Space.World);
				posX += translation.x;
				posZ += translation.z;
			}
		}

		if(hasMoved==1 || newblock==1){
			Destroy(highlight);
			highlightLanding();
			hasMoved = 0;
			newblock = 0;
		}
		shapeMove++;
  	}
	
	//creates and positions the highlighted landing for the shape
	private void highlightLanding(){
		int k = 0;
		bool flag = checkMoveAllowed();
		
		if (flag)
		{
			
			while(flag){
				shadow.transform.Translate(0,-1,0);
				k++;
				flag = checkMoveAllowed();
			}
			
			Vector3 highlightPos = new Vector3(shadow.transform.position.x, shadow.transform.position.y+1, shadow.transform.position.z);
			
			//copy shadow
			highlight = Instantiate(shadow, highlightPos, shadow.transform.rotation) as GameObject;
			highlight.name = "activeHighlight";
			
			foreach (Transform child in highlight.transform){
				//50% opacity on highlight pins
				
				child.renderer.material = new Material(Shader.Find("Transparent/Diffuse"));
		        child.renderer.material.color =  new Color(0.2F, 0.3F, 0.4F, 0.5F);;
				
				//child.renderer.material.color = Color.green;
				child.gameObject.renderer.enabled = true;
			}
			shadow.transform.Translate(0,k,0);
		}
	}
	
	
	// Makes given object a child of the Scene object.
	private void addToScene(GameObject o){
		GameObject scene = GameObject.Find("Scene");
		Transform t = o.transform;
		t.parent = scene.GetComponent<Transform>();
	}
	
	// Creates a cube that is a building block of a shape
	private GameObject createPointCube(float x, float y, float z){
		UnityEngine.Object blockPrefab = Resources.LoadAssetAtPath("Assets/block.prefab", typeof(GameObject));
		GameObject cube = GameObject.Instantiate(blockPrefab) as GameObject;
		cube.name = "Current pin" + pin.ToString();
		//set starting position
		cube.transform.position = new Vector3(globalX + x,
											  startHeight + y,
			  								  globalZ + z);
		//cube.transform.localScale = new Vector3(1, 1, 1); //prob redundant
		pin++;
		return cube;
	}
	
	//creates a shape out of array, consisting of 0s and 1s
	public void createShape(int[,,] shape){
	
		//while (shape == null);
		if (shape == null){
			throw new Exception("shape is null!");
		}
		/*if (shape.GetLength(0) != shape.GetLength(2)){
			throw new System.Exception("Shape x and z dimensions must match");
		}*/
		
		pin = 0;
		
		globalX = 7;
		globalZ = 7;	

		/*globalX = (int)((gameBoard.nx - 2)/2);
		globalZ = (int)((gameBoard.nz - 2)/2);*/
		Debug.Log(globalX + " " +globalZ);
		GameObject shapeObj = new GameObject();
		addComponents(shapeObj, shape.GetLength(0));		
		float halfLength = shape.GetLength(0)/2;
		getRotationCentre(shape, halfLength);
		
		shapeObj.transform.position = new Vector3(globalX + posX, startHeight, globalZ + posZ);
		
		for (int x=0; x < shape.GetLength(0); x++){
			for (int y=0; y < shape.GetLength(1); y++){
				for (int z=0; z < shape.GetLength(2); z++){
					if (shape[x,y,z] != 0){
						GameObject currentCube = createPointCube(x-halfLength,y-1.5f,z-halfLength);
						
						currentCube.GetComponent<MeshRenderer>();
						
						//Apply colour to each block
						switch (shape[x,y,z]){
						
							case 1: currentCube.renderer.material.color = Color.red;
									break;
								
							case 2: currentCube.renderer.material.color = Color.green;
									break;
								
							case 3: currentCube.renderer.material.color = Color.blue;
									break;
								
							default : break;
						}
						
						//make pins the same material as block
						GameObject child = currentCube.transform.Find("pin").gameObject;
						child.renderer.material = currentCube.renderer.material;
						
						addToShape(shapeObj, currentCube);
						
					}
				}
			}
		}
		
		currentShapeLength = shape.GetLength(0);
		
		//Shadow block
		shadow = Instantiate(shapeObj, shapeObj.transform.position, shapeObj.transform.rotation) as GameObject;
		shadow.name = "ActiveShadow";
		foreach (Transform child in shadow.transform){
			GameObject shadowPin = child.Find("pin").gameObject;
			Destroy (shadowPin);
			child.gameObject.renderer.enabled = false;
			child.gameObject.collider.isTrigger = true;
		}
		//addToScene(shadow);
		addToScene(shapeObj);
		//Debug.Log(shapeObj.transform.position);
		Debug.Log("Transform local " + shapeObj.transform.localPosition);
		float displacement = startHeight/10-shapeObj.transform.localPosition.y;
		shapeObj.transform.Translate(0, displacement,0);
		shadow.transform.Translate(0, displacement,0);
		//shapeObj.transform.Translate(0,20,0);
		GameObject shadowLayer = GameObject.Find("ShadowLayer");
		Transform t = shadow.transform;
		t.parent = shadowLayer.GetComponent<Transform>();
		
		addToScene(shapeObj);
		
		globalX=globalX+3;
	}
	
	// Adds necessary components and initialisation to a shape.
	private void addComponents(GameObject shapeObj, int shapeLength){
		Rigidbody cubeRigidBody = shapeObj.AddComponent<Rigidbody>();
		cubeRigidBody.mass = 1;
		cubeRigidBody.useGravity = false;
		cubeRigidBody.drag = 4;
		
		//BlockCollision b = shapeObj.AddComponent<BlockCollision>();
		//b.setShapeSize(shapeLength);
		shapeObj.name = "ActiveBlock";
	}
	
	public void assignTimeGap(int gap){
		timeGap = gap;
	}

	IEnumerator Wait2(int seconds){
		gameBoard.pauseGame(Time.realtimeSinceStartup);
		//legoCode = Marshal.PtrToStringAnsi(lego());
		
		Debug.Log("Wait for " + seconds + "s");
		waitActive = true;
		yield return new WaitForSeconds(seconds);
		waitActive = false;
		Debug.Log("After waiting for " + seconds + "s");
		
		
		gameBoard.unpauseGame();
	}
	
	
	
	private string Load(string fileName)
	{
		string line = "";
		// Handle any problems that might arise when reading the text
		try
		{
			
			// Create a new StreamReader, tell it which file to read and what encoding the file
			// was saved as
			StreamReader theReader = new StreamReader(fileName, Encoding.Default);
			

			using (theReader)
			{
				// While there's lines left in the text file, do this:
				line = theReader.ReadLine();
				print ("line = " + line);

				
				// Done reading, close the reader and return true to broadcast success    
				theReader.Close();
				return line;
			}
		}
		
		// If anything broke in the try block, we throw an exception with information
		// on what didn't work
		catch (Exception e)
		{
			Console.WriteLine("{0}\n", e.Message);
			return line;
		}
	}
	
	
	// For demonstration purposes.
	public void createShape(){
		// Add here shape creation code.
		//cam.takeSnap();
		
		// call c++ code
		//int hello = main ();
		
		//StartCoroutine(Wait2(3));
		//string legoCode = Load("/Users/guyhowcroft/Documents/gameImages/result.txt");
		//StartCoroutine(Wait2(1));
		//print (legoCode);
		//shape4 = getShapeArray(legoCode);    //If your using the webcams to get the shape
		
			shape4 = getShapeArray();   //If your using a hardcoded shape
		
		createShape(shape4);
	}

	
	//Makes given cube a child of the current shape
	private void addToShape(GameObject shape, GameObject cube){
		Transform t = cube.transform;
		t.parent = shape.GetComponent<Transform>();
	}
	
}
