using UnityEngine;
using System.Collections;
using System.IO;

public class WebCamPhoto : MonoBehaviour {
	WebCamTexture cam_texture;
	int x, y;
	void Start () {
		/*WebCamDevice[] cam_devices = WebCamTexture.devices;
		cam_texture = new WebCamTexture(cam_devices[x].name);
		if(cam_texture != null)
			cam_texture.Play();*/
	}
	
	public WebCamPhoto(int x, int y){
		this.x = x;
		this.y = y;
	}
	
	public void go(){
		//Debug.Log("hello lol");
		WebCamDevice[] cam_devices = WebCamTexture.devices;
		cam_texture = new WebCamTexture(cam_devices[x].name, 400, 400, 1);
		if(cam_texture != null)
			cam_texture.Play();
		//renderer.material.mainTexture = wct;
		//	wct.Play();
		//Debug.Log("hello lol2");
	}
	
	
	public void pause(){
		cam_texture.Pause();
	}
	
	
	public void play(){
		cam_texture.Play ();
	}
	
	
	public void BlitImage()
	{
	//	print ("width = " + cam_texture.width);
	//	print ("height = " + cam_texture.height);
		Texture2D destTexture = new Texture2D(cam_texture.width, cam_texture.height, TextureFormat.ARGB32, false);
		//print ("width = " + destTexture.width);
		//print ("height = " + destTexture.height);
		Color[] textureData = cam_texture.GetPixels();
		
		
		destTexture.SetPixels(textureData);
		destTexture.Apply();
		byte[] pngData = destTexture.EncodeToPNG();
		if(File.Exists("/Users/guyhowcroft/Documents/gameImages/" + x + ".png"))
		{
			print ("deleted");
			File.Delete("/Users/guyhowcroft/Documents/gameImages/" + x + ".png");
		}
		File.WriteAllBytes("/Users/guyhowcroft/Documents/gameImages//" + x + ".png",pngData);
		Debug.Log("/Users/guyhowcroft/Documents/gameImages/" + x + ".png");
		
	}
}