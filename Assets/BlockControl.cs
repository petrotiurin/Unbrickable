using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockControl : MonoBehaviour {
	
	private GameObject[] blocks;
	
	private int globalX, globalZ, pass;
	
	private int maxShapesX,maxShapesY,maxShapesZ;
	
	//sample shape, just fo shows
	private int[,,] shape = new int[,,] {{{1,1,1},{0,0,0},{0,0,0}},
									   	 {{1,1,1},{0,1,0},{0,0,0}},
									     {{1,1,1},{0,0,0},{0,0,0}}};
	
	/* size of a single "pin", i.e. a cube 
	 * that makes a building block of a shape. */
	public float pinSize = 0.33f;
	
	//starting height where shapes are created
	public float startHeight = 6.0f;
	
	// Variable to assign pins unique names.
	private int pin = 0;
	
	private GameObject FragmentCube;
	
	// Pre-Initialization.
	void Awake(){
		globalX = 0;
		globalZ = 0;
		pass = 0;
		getBoardValues();
	}
	
	// Initialization.
	void Start () {
	}
	
	// Set maximum amount of shapes that can fit in each direction.
	public void getBoardValues(){
		Board b = GetComponent<Board>();
		maxShapesX = b.nx;
		maxShapesY = b.ny;
		maxShapesZ = b.nz;
	}
	
	// Update is called once per frame.
	void Update () {
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
		cube.transform.position = new Vector3(globalX + x * pinSize,
											  startHeight + y * pinSize,
			  								  globalZ + z * pinSize);
		cube.transform.localScale = new Vector3(pinSize, pinSize, pinSize);
		pin++;
		return cube;
	}
	
	//creates a shape out of array, consisting of 0s and 1s
	public void createShape(int[,,] shape){
		if (shape.GetLength(0) != shape.GetLength(1)){
			throw new System.Exception("Shape x and y dimensions must match");
		}
		
		/* Skip one shape at the end of the first layer.
		 * For demonstration purposes. */
		if (pass == 0 && (globalX + globalZ*5) == 24){
			globalX++;
			pass=1;
		}
		// Wrap-around.
		if (globalX == maxShapesX){
			globalX = 0;
			globalZ = (globalZ + 1) % maxShapesZ;
		}
		
		pin = 0;
		
		GameObject shapeObj = new GameObject();
		shapeObj.transform.position = new Vector3(globalX, startHeight, globalZ);
		float halfLength = shape.GetLength(0)/2;
		
		for (int x=0; x < shape.GetLength(0); x++){
			for (int y=0; y < shape.GetLength(0); y++){
				for (int z=0; z < shape.GetLength(0); z++){
					if (shape[x,y,z] == 1){
						GameObject currentCube = createPointCube(x-halfLength,y-halfLength,z-halfLength);
						addToShape(shapeObj, currentCube);
					}
				}
			}
		}
		
		addComponents(shapeObj, shape.GetLength(0));
		addToScene(shapeObj);
		globalX++;
	}
	
	// Adds necessary components and initialisation to a shape.
	private void addComponents(GameObject shapeObj, int shapeLength){
		Rigidbody cubeRigidBody = shapeObj.AddComponent<Rigidbody>();
		cubeRigidBody.mass = 1;
		cubeRigidBody.useGravity = true;
		
		BlockCollision b = shapeObj.AddComponent<BlockCollision>();
		b.setPinSize(pinSize);
		b.setShapeSize(shapeLength);
		shapeObj.name = "ActiveBlock";
	}
	
	// For demonstration purposes.
	public void createShape(){
		// Add here shape creation code.
		createShape(shape);
	}
	
	//TODO: remove this shit
	public int getShapeSize(){
		return shape.GetLength(0);
	}
	
	//Makes given cube a child of the current shape
	private void addToShape(GameObject shape, GameObject cube){
		Transform t = cube.transform;
		t.parent = shape.GetComponent<Transform>();
	}
	
}
