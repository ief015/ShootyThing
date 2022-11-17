using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInputController : MonoBehaviour
{
	private MovementController movement;
	private WeaponProjectile weapon;

	public string AxisHorizontal = "Horizontal";
	public string AxisVertical = "Vertical";
	public string ButtonFire = "Fire1";

	///////////////////////////////////////////////////////////////////////////
	void Awake()
	{
		movement = GetComponent<MovementController>();
		weapon = GetComponent<WeaponProjectile>();
	}

	///////////////////////////////////////////////////////////////////////////
	void Update()
	{
		movement.Move(Input.GetAxisRaw(AxisHorizontal),
			Input.GetAxisRaw(AxisVertical));

		if (Input.GetButtonDown(ButtonFire))
			weapon.StartFire();

		if (Input.GetButtonUp(ButtonFire))
			weapon.StopFire();

		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		movement.LookTowards(mousePos);
	}
}
