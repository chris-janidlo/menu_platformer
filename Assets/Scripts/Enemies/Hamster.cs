﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(Animator))]
[RequireComponent(typeof(DestroyWhenChildrenInvisible))]
public class Hamster : BaseEnemy
{
	public float Gravity;
	public float AmbleSpeed, AmbleTime;
	public Vector2 AmblePauseTimeRange, FartTimeRange;
	public float DeathSpeed;

	public float SpawnAnimationTime;

	public HamsterGem Gem;
	public HamsterFart FartPrefab;

	Animator animator;

	Rigidbody2D rb;
	float currentSpeed;

	int walkingDirection = 1;

	bool started;

	protected override void Awake ()
	{
		base.Awake();
	}

	public void Start ()
	{
		MagicColor color = (MagicColor) Random.Range(0, 3);
		Gem.ColorPart.ChangeColor(color);
		Health.Color = color;

		transform.position = EnemySpawner.Instance.HamsterSpawnLocations.GetNext().position;

		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();

		StartCoroutine(startRoutine());

		Health.Death.AddListener(() => {
			Gem.Launch();

			currentSpeed = DeathSpeed;
			if (RandomExtra.Chance(.5f)) currentSpeed *= -1;

			StopAllCoroutines();

			GetComponent<Collider2D>().enabled = false;
			GetComponent<DestroyWhenChildrenInvisible>().ShouldDestroy = true;
		});
	}

	protected override void Update ()
	{
		base.Update();

		if (started)
		{
			var y = Health.Dead ? 0 : rb.velocity.y - Gravity * Time.deltaTime;
			rb.velocity = new Vector2(currentSpeed, y);
		}
	}

	IEnumerator startRoutine ()
	{
		float timer = 0, scale = 0;

		while (timer <= SpawnAnimationTime)
		{
			scale = timer / SpawnAnimationTime;
			transform.localScale = new Vector3(scale, scale, scale);
			timer += Time.deltaTime;
			yield return null;
		}

		transform.localScale = Vector3.one;

		started = true;

		StartCoroutine(ambleRoutine());
		StartCoroutine(fartRoutine());
	}

	IEnumerator ambleRoutine ()
	{
		while (true)
		{
			currentSpeed = 0;
			animator.SetBool("Walking", false);

			yield return new WaitForSeconds(RandomExtra.Range(AmblePauseTimeRange));

			float mult = walkingDirection;
			if (isFrozen) mult *= BaseMageBullet.IceSlowPercent;

			walkingDirection *= -1;

			currentSpeed = AmbleSpeed * mult;
			animator.SetBool("Walking", true);

			yield return new WaitForSeconds(AmbleTime * (isFrozen ? BaseMageBullet.IceSlowPercent : 1));
		}
	}

	IEnumerator fartRoutine ()
	{
		while (true)
		{
			yield return new WaitForSeconds(RandomExtra.Range(FartTimeRange));

			Instantiate(FartPrefab, transform.position, Quaternion.identity).Initialize(Gem.ColorPart.Color);
		}
	}
}
