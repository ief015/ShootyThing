using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class Character : MonoBehaviour
{
	public float InitialHealth = -1f;
	[Min(0f)] public float MaxHealth = 20f;
	public string Faction = "";
	public string CharacterClass = "";

	public float currentHealth { get; private set; }

	private MovementController movement;

	public delegate void OnDamagedEvent(float damage, Character inflictor = null);
	public event OnDamagedEvent OnDamaged;

	public delegate void OnKilledEvent(Character inflictor = null);
	public event OnKilledEvent OnKilled;

	///////////////////////////////////////////////////////////////////////////
	public Vector2 position
	{
		get { return movement.position; }
	}
	
	///////////////////////////////////////////////////////////////////////////
	public override string ToString()
	{
		var faction = Faction == "" ? "[No Faction]" : Faction;
		var charClass = CharacterClass == "" ? "[No Class]" : CharacterClass;

        return base.ToString() + ":" + faction + "." + charClass;
	}

	///////////////////////////////////////////////////////////////////////////
	void Awake()
	{
		currentHealth = InitialHealth >= 0 ? InitialHealth : MaxHealth;
		movement = GetComponent<MovementController>();
	}

	///////////////////////////////////////////////////////////////////////////
	void OnDrawGizmos()
	{
		if (MaxHealth <= 0f)
			return;

		Vector3 startHPBar = new Vector3(-0.5f, 1f, 0f) + transform.position;
		Vector3 endHPBar   = new Vector3(0.5f, 1f, 0f) + transform.position;

		Gizmos.color = Color.red;
		Gizmos.DrawLine(startHPBar, endHPBar);

		float hp = currentHealth / MaxHealth;
		Gizmos.color = Color.green;
		Gizmos.DrawLine(startHPBar, Vector3.Lerp(startHPBar, endHPBar, hp));
	}

	///////////////////////////////////////////////////////////////////////////
	public void TakeDamage(float dmg, Character inflictor = null)
	{
		currentHealth = Mathf.Max(0f, currentHealth - dmg);

		OnDamaged?.Invoke(dmg, inflictor);

		if (currentHealth <= 0f)
			Kill(inflictor);
	}

	///////////////////////////////////////////////////////////////////////////
	public void Kill(Character inflictor = null)
	{
		OnKilled?.Invoke(inflictor);
		Destroy(gameObject);
	}
}
