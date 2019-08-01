using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using crass;

[RequireComponent(typeof(Rigidbody2D))]
public class Hamster : BaseEnemy
{
	public override float MaxHealth => 50;

	public float Gravity;
	public float AmbleSpeed, AmbleTime;
	public Vector2 AmblePauseTimeRange;
	public Vector2 FartTimeRange;

	public float SpawnAnimationTime;

	public ColorMapApplier ColorPart;
	public HamsterFart FartPrefab;

	Rigidbody2D rb;
	float currentSpeed;

	int walkingDirection = 1;

	bool started;

	public void Initialize (MagicColor color)
	{
		ColorPart.ChangeColor(color);

		rb = GetComponent<Rigidbody2D>();

		StartCoroutine(startRoutine());
	}

	void Start ()
	{
		Initialize(MagicColor.Blue);
	}

	protected override void Update ()
	{
		base.Update();

		if (started)
		{
			rb.velocity = new Vector2
			(
				currentSpeed,
				rb.velocity.y - Gravity * Time.deltaTime
			);
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

			yield return new WaitForSeconds(RandomExtra.Range(AmblePauseTimeRange));

			float mult = walkingDirection;
			if (isFrozen) mult *= BaseMageBullet.IceSlowPercent;

			walkingDirection *= -1;

			currentSpeed = AmbleSpeed * mult;

			yield return new WaitForSeconds(AmbleTime * (isFrozen ? BaseMageBullet.IceSlowPercent : 1));
		}
	}

	IEnumerator fartRoutine ()
	{
		while (true)
		{
			yield return new WaitForSeconds(RandomExtra.Range(FartTimeRange));

			Instantiate(FartPrefab, transform.position, Quaternion.identity).Initialize(ColorPart.Color);
		}
	}
}
