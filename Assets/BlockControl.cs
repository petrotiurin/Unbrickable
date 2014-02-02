using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockControl : MonoBehaviour {
	
	private GameObject[] blocks;
	
	private int i, j, pass;
	
	//sample shape, just fo shows
	private int[,,] shape = new int[,,] {{{1,1,1},{0,0,0},{0,0,0}},
									   	 {{1,1,1},{0,1,0},{0,0,0}},
									     {{1,1,1},{0,0,0},{0,0,0}}};
	
	/* size of a single "pin", i.e. a cube 
	 * that makes a building block of a shape. */
	public float pinSize = 0.33f;
	
	//variable to assign pins unique names
	private int pin = 0;
	
	private GameObject FragmentCube;

	// Use this for initialization
	void Start () {
		i = 0;
		j = 0;
		pass = 0;
		//CreateCube();
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	
	/* Creates a new cube at the top of our scene.
	 * Works only if the last object stopped moving. */
	public void CreateCube() {
		
		if (pass == 0 && (i + j*5) == 24){
			i++;
			pass=1;
		}
		
		//wrap-around
		if (i == 5){
			i = 0;
			j = (j+1)%5;
		}
		
		//GameObject cube = GameObject.Instantiate(Resources.LoadAssetAtPath("Assets/block.prefab", typeof(GameObject))) as GameObject;
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.name = "ActiveBlock";	
		
		//drop a brick on each space on the board in order
		cube.transform.position = new Vector3(i, 6, j); //TODO: randomize?
		
		Rigidbody cubeRigidBody = cube.AddComponent<Rigidbody>();
		cubeRigidBody.mass = 1;
		cubeRigidBody.useGravity = true;
		
		cube.AddComponent("BlockCollision");
		addToScene(cube);
		
		i++;
	}
	
	//Makes given object a child of the "Scene"
	private void addToScene(GameObject o){
		GameObject scene = GameObject.Find("Scene");
		Transform t = o.transform;
		t.parent = (Transform)scene.GetComponent("Transform");
	}
	
	//creates a cube that is a building block of a shape
	private GameObject createPointCube(float x, float y, float z, float globalx,float globaly){
		GameObject cube = GameObject.Instantiate(Resources.LoadAssetAtPath("Assets/block.prefab", typeof(GameObject))) as GameObject;
		string name = "Current pin" + pin.ToString();
		cube.name = name;
		cube.transform.position = new Vector3(globalx+x*pinSize, 6+y*pinSize, globaly+z*pinSize);
		cube.transform.localScale = new Vector3(pinSize,pinSize,pinSize);
		pin++;
		return cube;
	}
	
	//creates a shape out of array, consisting of 0s and 1s
	public void createShape(int[,,] shape){
		if (pass == 0 && (i + j*5) == 24){
			i++;
			pass=1;
		}
		
		//wrap-around
		if (i == 5){
			i = 0;
			j = (j+1)%5;
		}
		
		pin = 0;
		
		GameObject shapeObj = new GameObject();
		shapeObj.transform.position = new Vector3(i,6,j);
		float halfLength = shape.GetLength(0)/2;
		for (int x=0; x < shape.GetLength(0); x++)
			for (int y=0; y < shape.GetLength(0); y++)
				for (int z=0; z < shape.GetLength(0); z++){
					if (shape[x,y,z] == 1){
						GameObject currentCube = createPointCube(x-halfLength,y-halfLength,z-halfLength,i,j);
						addToShape(shapeObj, currentCube);
					}
				}
		Rigidbody cubeRigidBody = shapeObj.AddComponent<Rigidbody>();
		cubeRigidBody.mass = 1;
		cubeRigidBody.useGravity = true;
		
		shapeObj.AddComponent("BlockCollision");
		shapeObj.name = "ActiveBlock";
		addToScene(shapeObj);
		
		i++;
	}
	//TODO: get rid of this shit
	public void createShape(){
		createShape(shape);
	}
	
	public int getShapeSize(){
		return shape.GetLength(0);
	}
	
	//Makes given cube a child of the current shape
	private void addToShape(GameObject shape, GameObject cube){
		Transform t = cube.transform;
		t.parent = (Transform)shape.GetComponent("Transform");
	}
	
}
