using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInputController : MonoBehaviour
{
	private MovementController movement;
	private WeaponProjectile weapon;

	public string axisHorizontal = "Horizontal";
	public string axisVertical = "Vertical";
	public string axisShiftMove = "Shift";
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
		float axisHoriz = Input.GetAxisRaw(axisHorizontal);
		float axisVert = Input.GetAxisRaw(axisVertical);

		movement.Move(axisHoriz, axisVert);
		movement.shift = Input.GetAxisRaw(axisShiftMove) > 0;

		if (Input.GetButtonDown(buttonFire))
		{
			weapon.StartFire();
		}

		if (Input.GetButtonUp(buttonFire))
		{
			weapon.StopFire();
		}

		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		movement.LookTowards(mousePos);
	}
}
