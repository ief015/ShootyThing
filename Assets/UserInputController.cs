using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInputController : MonoBehaviour
{
	private MovementController movement;
	private WeaponProjectile weapon;

	public string axisHorizontal = "Horizontal";
	public string axisVertical = "Vertical";
	public string buttonFire = "Fire1";

	///////////////////////////////////////////////////////////////////////////
	void Awake()
	{
		movement = GetComponent<MovementController>();
		weapon = GetComponent<WeaponProjectile>();
	}

	///////////////////////////////////////////////////////////////////////////
	void Update()
	{
		movement.Move(Input.GetAxisRaw(axisHorizontal),
			Input.GetAxisRaw(axisVertical));

		if (Input.GetButtonDown(buttonFire))
			weapon.StartFire();

		if (Input.GetButtonUp(buttonFire))
			weapon.StopFire();

		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		movement.LookTowards(mousePos);
	}
}
