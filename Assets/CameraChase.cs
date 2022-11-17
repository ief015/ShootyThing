using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChase : MonoBehaviour
{
	/// <summary>
	/// Set override camera. If none, it will default to the Main Camera.
	/// </summary>
	public GameObject OverrideCamera;

	private Camera currentCamera = null;

	public enum ChaseMode { Tight, Smooth }
	public ChaseMode Mode = ChaseMode.Tight;

	/// <summary>
	/// Camera speed when using smooth chase mode.
	/// </summary>
	public float SmoothingSpeed = 5f;

	///////////////////////////////////////////////////////////////////////////
	void Start()
	{

	}

	///////////////////////////////////////////////////////////////////////////
	void Update()
	{
		if (!CheckCam())
			return;

		if (Mode == ChaseMode.Tight)
		{
			currentCamera.transform.position = transform.position + new Vector3(0, 0, -10);
		}
		else
		{
			Vector3 campos = currentCamera.transform.position;
			campos.z = 0;

			Vector3 objpos = transform.position;
			campos.z = 0;

			Vector3 newpos = Vector3.Lerp(campos, objpos, SmoothingSpeed * Time.deltaTime);
			currentCamera.transform.position = newpos + new Vector3(0, 0, -10);
		}
	}

	///////////////////////////////////////////////////////////////////////////
	bool CheckCam()
	{
		if (OverrideCamera)
			currentCamera = OverrideCamera.GetComponent<Camera>();
		else
			currentCamera = Camera.main;
		return currentCamera;
	}
}
