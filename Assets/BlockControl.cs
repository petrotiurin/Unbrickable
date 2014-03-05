using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BlockControl : MonoBehaviour {
	
	private GameObject[] blocks;
	
	private GameObject shadow, highlight;
	private ShadowCollision sh;
	
	private int globalX, globalZ;
	
	private int maxPinsX,maxPinsZ;
	
	private float boundX,boundZ,boundingBox;
	
	private int pass;
	
	//sample shape, just fo shows
	private int[,,] shape1 = new int[,,] {{{0,1,0},{0,0,0},{0,0,0}},
									   	  {{1,1,1},{0,0,0},{0,0,0}},
									      {{0,1,0},{0,0,0},{0,0,0}}};
	
	private int[,,] shape2 = new int[,,] {{{0,1,0},{0,0,0},{0,0,0}},
									   	  {{0,1,0},{0,2,0},{0,0,0}},
									      {{0,1,0},{0,2,0},{0,3,0}}};
	
	private int[,,] shape3 = new int[,,] {{{0,0,0},{0,0,0},{0,0,1}},
									   	  {{0,0,0},{0,0,1},{0,0,1}},
									      {{0,1,1},{0,0,1},{0,0,0}}};
	
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

	//for moving the shapes need to know the centre of shape
	private float posX=2,posZ=2;	
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
	}
	
	// Set maximum amount of pins that can fit in each direction.
	public void getBoardValues(){
		maxPinsX = gameBoard.nx;
		maxPinsZ = gameBoard.nz;
	}
	
	public void PivotTo(GameObject o, Vector3 position){
	    Vector3 offset = o.transform.position - position;
	 
	    foreach (Transform child in o.transform)
	        child.transform.position += offset;
	 
	    o.transform.position = position;
	}
	
	private void getRotationCentre(int[,,] shape){
		//go throught each part of the array and search for the 1's, keep count and go throught all of th arrays in the z direction
		//same for the x direction
		//count the distance between the starting 1 and the last 1
		//if a 1 is on the edge and nowhere else in the line, this is potentially the furthest edge
		
		GameObject active = GameObject.Find("ActiveBlock");
		
		double length, width;
		int[] lengthSize = new int[shape.GetLength(0)];
		int[] widthSize = new int[shape.GetLength(1)];
		//to work out the length of the shape.
		//for the y direction
		for(int i=0; i<shape.GetLength(1);i++){
			//for the z direction
			for(int j = 0; j<shape.GetLength(2);j++){
				//for the x direction
				for(int k = 0;k<shape.GetLength(0);k++){
					if(shape[j,i,k] != 0){
						lengthSize[k] = 1;
						widthSize[j] = 1;
					}
				}
			}
		}
		//to calulate the length of the longest block of 1's
		int finalLength,startLength = 0,endLength = 0,found1 = 0;
		for(int i=0;i<shape.GetLength(2);i++){
			//get the start position in the array of the shape
			if(lengthSize[i] != 1&&found1 == 0){
				found1 = 1;
				startLength = i;
			}
			if(lengthSize[i] == 1){
				endLength = i;
			}			
		}
		//to calculate the width of the longest blocks of 1's
		int finalWidth,startWidth = 0,endWidth = 0,foundWidth1 = 0;
		for(int i=0;i<shape.GetLength(1);i++){
			//get the start position in the array of the shape
			if(widthSize[i] == 1&&foundWidth1 == 0){
				foundWidth1 = 1;
				startWidth = i;
			}
			if(widthSize[i] == 1){
				endWidth = i;
			}			
		}
		//calculate the final length from the start 1 to the last 1
		finalLength = (endLength - startLength) + 1;
		//gets the centre of the length
		length = active.transform.position.z + (startLength*pinSize) + ((finalLength * pinSize)/2);
		//calculate the final width from the start 1 to the last 1
		finalWidth = (endWidth - startWidth) + 1;
		//gets the centre of the width
		width = active.transform.position.x + (startWidth*pinSize) + ((finalWidth * pinSize)/2);
		
		/*final length and width = the actual longest lengths
		  length and width = the centre of gravityin relation to the origin corner*/
		
		//if length and width is the same size then rotate around the center of gravity
		//else rotate around the nearest corner
		if(finalLength == finalWidth){
			posX = active.transform.position.x;
			posZ = active.transform.position.z;
		}else{
			posX = (float)(int)active.transform.position.x;
			posZ = (float)(int)active.transform.position.z;
		}
		//calculate the bounding box
		boundX = finalWidth/2;
		boundZ = finalLength/2;
		
		
		//calculate the size of the bounding box
		if(finalWidth > finalLength)boundingBox=finalWidth*finalWidth;
		else boundingBox=finalLength*finalLength;
		
		//get centre and set the pivot
		centreRotation = new Vector3 (posX,active.transform.position.y,posZ);
		PivotTo(active,centreRotation);
	}
	private bool checkMoveAllowed(){
		//first, get all block coordinates
		List<int> xs = new List<int>();
		List<int> ys = new List<int>();
		List<int> zs = new List<int>();
		foreach (Transform child in shadow.transform){
			xs.Add((int)Math.Round(child.position.x) + 1);
			ys.Add((int)Math.Round(child.position.y - 0.38));
			zs.Add((int)Math.Round(child.position.z) + 1);
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
			xs.Add((int)Math.Round(child.position.x) + 1);
			ys.Add((int)Math.Round(child.position.y - 0.38));
			zs.Add((int)Math.Round(child.position.z) + 1);
		};
		for (int i = 0; i < xs.Count; i++){
			Debug.Log("Coord: "+(xs[i]+1)+":"+ys[i]+":" +(zs[i]+1));
			if (gameBoard.checkPosition(xs[i] + 1,ys[i],zs[i] + 1)){
				Debug.Log("collision!"+xs[i]+":"+ys[i]+":" +zs[i]);
				return true;
			}
		}
		return false;
	}
	private void triggerNextShape(GameObject block){
		for (int i = 0; i < Math.Pow(currentShapeLength, 3); i++){
			Transform childTransform = block.transform.FindChild("Current pin" + i.ToString());
			if (childTransform != null){
				int layer = (int)Math.Round(childTransform.position.y - 0.38);
				Debug.Log("Layer: " + layer);
				gameBoard.FillPosition(layer, childTransform.gameObject); 
			}
		}
		//show next suggested piece
		Array_GameObj showPieceScript;
		showPieceScript = GameObject.Find("Allowed pieces").GetComponent<Array_GameObj>();
		showPieceScript.SuggestLegoPiece();
		block.rigidbody.isKinematic = true;
		shadow.rigidbody.isKinematic = true;
		//create new shape and destroy the old empty container
		Destroy(block);
		Destroy(shadow);
		createShape();
	}
	// Update is called once per frame.
	void Update () {
		GameObject block = GameObject.Find("ActiveBlock");
		Vector3 translation = Vector3.zero;
		Vector3 rotation = Vector3.zero;
		int hasMoved = 0;
		
		timer -= Time.deltaTime;
		if(timer<=0){
			timer=1;
			shadow.transform.Translate(0,-1,0);
			if (checkMoveAllowed()){
				block.transform.Translate(0,-1,0);
			} else {
				triggerNextShape(block);
				block = GameObject.Find("ActiveBlock");
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
		//MOVE forward
		if (Input.GetKey("up")){
			//check every 4 frames
			if(shapeMove%4 == 0){
				
					translation = new Vector3(0,0,1);
					hasMoved = 1;
				
			}
		}
		//MOVE back
		if (Input.GetKey("down")){
			//check every 4 frames
			if(shapeMove%4 == 0){
				
					translation = new Vector3(0,0,-1);
					hasMoved = 1;
				
			}
		}
		//MOVE right
  		if (Input.GetKey("right")){
			//check every 4 frames
			if(shapeMove%4 == 0){
					translation = new Vector3(1,0,0);
					hasMoved = 1;
				
			}
  		}
  		//MOVE left
  		if (Input.GetKey("left")){
			//check every 4 frames
			if(shapeMove%4 == 0){
					translation = new Vector3(-1,0,0);
					hasMoved = 1;
				
			}
		}
		Vector3 backupPos = shadow.transform.position;
		Quaternion backupRot = shadow.transform.rotation;
		shadow.transform.Rotate(rotation,Space.Self);
		shadow.transform.Translate(translation, Space.World);
		if (checkArrayCollisions()){
			shadow.transform.position = backupPos;
			shadow.transform.rotation = backupRot;
		}else{
			block.transform.Rotate(rotation,Space.Self);
			block.transform.Translate(translation, Space.World);
			posX += translation.x;
			posZ += translation.z;
		}
		
		if(hasMoved==1){
			//highlightLanding(block);
			hasMoved = 0;
		}
		shapeMove++;
  	}
	
	/*
	private void highlightLanding(GameObject block){
		int landingFound=0;
		highlight = Instantiate(block,block.transform.position,block.transform.rotation) as GameObject;
		highlight.name = "landing";
		foreach (Transform child in highlight.transform){
			child.gameObject.renderer.enabled = false;
			child.gameObject.collider.isTrigger = true;
		}
		
		highlight.addComponent<BlockCollision>();
		addToScene(highlight);
		
		while(!landingFound){
			
			//check for collision
			if (collision HashSet been detected)
				landingFound = 1;
			else
				highlight.transform.Translate(0,-1,0);
		}	

	}
	*/
	
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
	public void createShape(int[,,] shape, int colour){
		if (shape.GetLength(0) != shape.GetLength(1)){
			throw new System.Exception("Shape x and y dimensions must match");
		}
		
		/* Skip one shape at the end of the first layer.
		 * For demonstration purposes. */
		/*
		 * 

		// Wrap-around.
		if (globalX >= maxPinsX){
			globalX = 0;
			globalZ = (globalZ + 3) % maxPinsZ;
		}*/
		
		pin = 0;
		
		globalX = 7;
		globalZ = 7;			
		
		GameObject shapeObj = new GameObject();
		shapeObj.transform.localPosition = new Vector3(globalX, startHeight, globalZ);
		float halfLength = shape.GetLength(0)/2;
		
		for (int x=0; x < shape.GetLength(0); x++){
			for (int y=0; y < shape.GetLength(0); y++){
				for (int z=0; z < shape.GetLength(0); z++){
					if (shape[x,y,z] != 0){
						GameObject currentCube = createPointCube(x-halfLength,y-halfLength,z-halfLength);
						
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
						addToShape(shapeObj, currentCube);
						
					}
				}
			}
		}
		
		addComponents(shapeObj, shape.GetLength(0));
		currentShapeLength = shape.GetLength(0);
		shadow = Instantiate(shapeObj, shapeObj.transform.position, shapeObj.transform.rotation) as GameObject;
		shadow.name = "ActiveShadow";
		foreach (Transform child in shadow.transform){
			child.gameObject.renderer.enabled = false;
			child.gameObject.collider.isTrigger = true;
		}
		//addToScene(shadow);
		addToScene(shapeObj);
		//Debug.Log(shapeObj.transform.position);
		//Debug.Log(shapeObj.transform.localPosition);
		//shapeObj.transform.Translate(0,20,0);
		GameObject shadowLayer = GameObject.Find("ShadowLayer");
		Transform t = shadow.transform;
		t.parent = shadowLayer.GetComponent<Transform>();
		//change the centre rotation vector for the shape centre
		getRotationCentre(shape);
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
	
	// For demonstration purposes.
	public void createShape(){
		// Add here shape creation code.
		
		// Cycle through these three shapes for now...
		if(pass%3==0)
			createShape(shape1, pass);
		else if(pass%3==1)
			createShape(shape2, pass);
		else
			createShape(shape3, pass);

		pass++;
	}
	
	//TODO: remove this shit
	/*
	 * public int getShapeSize(){
		return shape.GetLength(0);
	}
	 */
	
	//Makes given cube a child of the current shape
	private void addToShape(GameObject shape, GameObject cube){
		Transform t = cube.transform;
		t.parent = shape.GetComponent<Transform>();
	}
	
}
