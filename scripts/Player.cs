using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class Player : CharacterBody2D
{
	[Export]
	public int swordDamage = 2;
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	[Export]
	public int health = 100;
	public ProgressBar healthProgressbar;

	public PackedScene deathPrefab;
	[Export]
	int maxHealth = 100;
	[Export]
	public int ritualDamage = 1;
	[Export]
	public float ritualInterval = 15.0f;

	public PackedScene ritualScene;
	private bool isAttacking = false;
	public float attackCd = 0.0f;
	public float hitBoxCd = 0.0f;
	public float ritualCd = 0.0f;
	public bool isRunning = false;

	private AnimationPlayer player;
	private Sprite2D character;

	private Area2D swordArea;
	private Area2D hitBoxArea;

	private enemy enemy;
	private ritual ritual;

	public GameManager gameManager;

	[Signal]
	public delegate void MeatCollectedEventHandler(int amount);
	public override void _Ready()
	{

		try
        {
            gameManager = GetNode<GameManager>("/root/GameManager");
        }
        catch (Exception e)
        {
            GD.PrintErr($"Erro ao encontrar GameManager: {e.Message}");
        }

        // Verifique se o GameManager foi encontrado
        if (gameManager != null)
        {
            GD.Print("GameManager encontrado no player!");
        }
        else
        {
            GD.PrintErr("GameManager não encontrado!");
        }

		player = GetNode<AnimationPlayer>("AnimationPlayer");
		character = GetNode<Sprite2D>("Sprite2D");
		swordArea = GetNode<Area2D>("swordArea");
		deathPrefab = GD.Load<PackedScene>("res://Misc/BigSkull.tscn");
		hitBoxArea = GetNode<Area2D>("HitBoxArea");
		ritualScene = (PackedScene)ResourceLoader.Load("res://Misc/ritual.tscn");
		healthProgressbar = GetNode<ProgressBar>("HealthProgressBar");
		
		GameManager.player = this;
	}
	public override void _PhysicsProcess(double delta)
	{

		Vector2 velocity = Velocity;

		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Y = direction.Y * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
		}

		bool wasRunning = isRunning;
		isRunning = !direction.IsZeroApprox();

		if (!isAttacking)
		{

			if (wasRunning != isRunning)
			{
				if (isRunning)
				{
					player.Play("run");
				}
				else
				{
					player.Play("idle");
				}
			}
		}

		if (!isAttacking)
		{
			rotateSprite();
		}

		if (Input.IsActionJustPressed("attack"))
		{
			Attack();
		}
		Velocity = velocity;
		MoveAndSlide();
	}


	public override void _Process(double delta)
	{

		GameManager.playerPosition = Position;

		if (isAttacking)
		{

			attackCd -= (float)delta;

			if (attackCd <= 0.0f)
			{
				isAttacking = false;
				isRunning = false;
				player.Play("idle");
			}
		}

		updateHitboxDetection(delta);
		updateRitual(delta);

		healthProgressbar.MaxValue = maxHealth;
		healthProgressbar.Value = health;
	}

	public void rotateSprite()
	{
		Vector2 velocity = Velocity;

		if (velocity.X > 0)
		{
			character.FlipH = false;
		}
		else if (velocity.X < 0)
		{
			character.FlipH = true;
		}

	}
	public void Attack()
	{
		if (isAttacking)
		{
			return;
		}
		else
		{

			player.Play("attackSide1");
			attackCd = 0.6f;
			isAttacking = true;
		}
	}

	public void dealDamageToEnemies()
	{
		var bodies = swordArea.GetOverlappingBodies();
		var enemies = GetTree().GetNodesInGroup("enemies");


		foreach (Node body in bodies)
		{
			if (body.IsInGroup("enemies"))
			{
				enemy enemy = (enemy)body;
				var directionToEnemy = enemy.Position - Position;
				Vector2 attackDirection;

				if (character.FlipH)
				{
					attackDirection = Vector2.Left;
				}
				else
				{
					attackDirection = Vector2.Right;
				}

				var dotProduct = directionToEnemy.Dot(attackDirection);


				GD.Print("Dot: ", dotProduct);

				if (dotProduct >= 3)
				{
					enemy.damage(swordDamage);
				}

			}
		}
	}

	public void updateHitboxDetection(double delta)
	{

		hitBoxCd -= (float)delta;

		if (hitBoxCd >= 0)
		{
			return;
		}

		hitBoxCd = 0.5f;

		var bodies = hitBoxArea.GetOverlappingBodies();

		foreach (Node body in bodies)
		{

			if (body.IsInGroup("enemies"))
			{
				enemy enemy = (enemy)body;
				var damageAmount = 1;
				damage(damageAmount);

			}
		}
	}
	public void damage(int amount)
	{
		if (health <= 0)
		{
			return;
		}
		else
		{

			health -= amount;
			GD.Print("Player recebeu dano de ", amount, ", a vida total é de: ", health, "/", maxHealth);

			character.Modulate = new Godot.Color(1, 0, 0);
			var tween = CreateTween();
			tween.SetEase(Tween.EaseType.In);
			tween.SetTrans(Tween.TransitionType.Quint);
			tween.TweenProperty(character, "modulate", new Godot.Color(1, 1, 1), 0.3f);

			if (health <= 0)
			{
				die();
			}
		}
	}
	public void die()
	{
		if (gameManager != null)
        {
            gameManager.EndGame();
			GD.Print("encontrou o engGame no player");
        }
        else
        {
            GD.PrintErr("GameManager não encontrado ao tentar finalizar o jogo!");
        }
		
		Node2D deathInstance = (Node2D)deathPrefab.Instantiate();
		GetParent().AddChild(deathInstance);

		deathInstance.Position = Position;

		GD.Print("Se fodeu!");
		QueueFree();
	}

	public int Heal(int amount)
	{
		health += amount;
		if (health >= maxHealth)
		{
			health = maxHealth;

		}
		GD.Print("Player recebeu cura de ", amount, ", a vida total é de: ", health, "/", maxHealth);
		return health;
	}

	public void updateRitual(double delta)
	{
		ritualCd -= (float)delta;

		if (ritualCd >= 0)
		{
			return;
		}

		ritualCd = ritualInterval;

		GD.Print(ritualCd);
		Node2D ritualInstance = (Node2D)ritualScene.Instantiate();
		AddChild(ritualInstance);
		GD.Print(ritualInstance);

		ritualDamage = ritual.damageAmountRT;

	}

}