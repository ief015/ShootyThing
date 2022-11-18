using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class Character : MonoBehaviour
{
	public float initialHealth = -1f;
	[Min(0f)] public float maxHealth = 20f;
	public string faction = "";
	public string characterClass = "";

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
		var faction = this.faction == "" ? "[No Faction]" : this.faction;
		var charClass = characterClass == "" ? "[No Class]" : characterClass;

		return base.ToString() + ":" + faction + "." + charClass;
	}

	///////////////////////////////////////////////////////////////////////////
	void Awake()
	{
		currentHealth = initialHealth >= 0 ? initialHealth : maxHealth;
		movement = GetComponent<MovementController>();
	}

	///////////////////////////////////////////////////////////////////////////
	void OnDrawGizmos()
	{
		if (maxHealth <= 0f)
			return;

		Vector3 startHPBar = new Vector3(-0.5f, 1f, 0f) + transform.position;
		Vector3 endHPBar   = new Vector3(0.5f, 1f, 0f) + transform.position;

		Gizmos.color = Color.red;
		Gizmos.DrawLine(startHPBar, endHPBar);

		float hp = currentHealth / maxHealth;
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
