using UnityEngine;
using System.Collections;

public class setUpWebcam : MonoBehaviour {
	
	bool setup = false;
	public WebCamPhoto webcam1;
	public WebCamPhoto webcam2;
	public WebCamPhoto webcam3;
	public WebCamPhoto webcam4;

	void Start () {

		
	}

	
	public void takeSnap()
	{
		print ("1");
		if(setup)
		{
			print ("actually taking a photo");
			//      cam_texture.Pause();
			/*      webcam1.pause();
                        webcam2.pause();
                        webcam3.pause();
                        webcam4.pause();*/
			webcam1.BlitImage();
			webcam2.BlitImage();
			webcam3.BlitImage();
			webcam4.BlitImage();
			/*webcam1.play();
                        webcam2.play();
                        webcam3.play();
                        webcam4.play();*/
			
		}
		
	}
	
	public void setUpCams(){

		print ("setting new webcam");
		webcam1 = new WebCamPhoto(2, 30);
		webcam2 = new WebCamPhoto(1, 70);
		webcam3 = new WebCamPhoto(4, 30);
		webcam4 = new WebCamPhoto(3, 70);
		
		
		/*webcam1.go();
		webcam2.go();
		webcam3.go();
		webcam4.go(); */


		this.setup = true;
		//webcam2.go();
		
		
	}
	
	
	
}