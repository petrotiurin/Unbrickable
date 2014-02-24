using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockControl : MonoBehaviour {
	
	private GameObject[] blocks;
	
	private GameObject shadow;
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
		Board b = GetComponent<Board>();
		maxPinsX = b.nx;
		//maxPinsY = b.ny;
		maxPinsZ = b.nz;
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
	// Update is called once per frame.
	void Update () {
		GameObject block = GameObject.Find("ActiveBlock");
		Vector3 translation = Vector3.zero;
		Vector3 rotation = Vector3.zero;
		
		timer -= Time.deltaTime;
		if(timer<=0){
			timer=1;
			block.transform.Translate(0,-1,0);
		}		
		//ROTATE right
		if (Input.GetKeyDown("v")){		
			//board has been moved (2,0,2) 2in x, 2in z-->> --^
			rotation = new Vector3(0,90,0);
		}		
		//ROTATE left
		if (Input.GetKeyDown("c")){
			rotation = new Vector3(0,90,0);
		}
		//MOVE forward
		if (Input.GetKeyDown("up")){
			if ((block.transform.position.z + boundZ) + 1 <= maxPinsZ){
				translation = new Vector3(0,0,1);
			}
		}
		//MOVE back
		if (Input.GetKeyDown("down")){
			if ((block.transform.position.z - boundZ) - 1 >= 0){
				translation = new Vector3(0,0,-1);
			}
		}
		//MOVE right
  		if (Input.GetKeyDown("right")){
			if ((block.transform.position.x + boundX) + 1 <= maxPinsX){
				translation = new Vector3(1,0,0);
			}
  		}
  		//MOVE left
  		if (Input.GetKeyDown("left")){
			if ((block.transform.position.x - boundX) - 1 >= 0){
				translation = new Vector3(-1,0,0);
			}
		}
		
		shadow.transform.Rotate(rotation,Space.Self);
		shadow.transform.Translate(translation);
		if (sh.isCollided()){
			sh.reset(block.transform.position, block.transform.rotation);
		} else {
			//block.transform.position = new Vector3(block.transform.position.x,block.transform.position.y,newPosition);
			block.transform.Rotate(rotation,Space.Self);
			block.transform.Translate(translation);
			posX += translation.x;
			posZ += translation.z;
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
		Object blockPrefab = Resources.LoadAssetAtPath("Assets/block.prefab", typeof(GameObject));
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
		shapeObj.transform.position = new Vector3(globalX, startHeight, globalZ);
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
	
		shadow = Instantiate(shapeObj, shapeObj.transform.position, shapeObj.transform.rotation) as GameObject;
		shadow.name = "ActiveShadow";
		foreach (Transform child in shadow.transform){
			child.gameObject.renderer.enabled = false;
			child.gameObject.collider.isTrigger = true;
		}
		Destroy(shadow.GetComponent<BlockCollision>());
		sh = shadow.AddComponent<ShadowCollision>();
		addToScene(shadow);
		addToScene(shapeObj);
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
		
		BlockCollision b = shapeObj.AddComponent<BlockCollision>();
		b.setShapeSize(shapeLength);
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
