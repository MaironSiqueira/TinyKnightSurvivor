using Godot;
using System;
using System.Collections.Generic;

public partial class mobSpawner : Node2D
{
	public List<PackedScene> creaturesList = new List<PackedScene>();
	[Export]
	public float mobsPerMinute = 30.0f;

	private PathFollow2D pathFollow2D;
	private float cooldown = 0.0f;

	public override void _Ready()
	{
		pathFollow2D = GetNode<PathFollow2D>("Path2D/%PathFollow2D");

		creaturesList.Add((PackedScene)ResourceLoader.Load("res://enemies/goblin.tscn"));
		creaturesList.Add((PackedScene)ResourceLoader.Load("res://enemies/pawn.tscn"));
		creaturesList.Add((PackedScene)ResourceLoader.Load("res://enemies/sheep.tscn"));
	}

	public override void _Process(double delta)
	{
		if (GameManager.isGameOver) return;

		cooldown -= (float)delta;
		if (cooldown > 0.0f)
		{
			return;
		}

		float interval = 60.0f / mobsPerMinute;
		cooldown = interval;

		var point = GetPoint();
		var worldState = GetWorld2D().DirectSpaceState;
		var parameters = new PhysicsPointQueryParameters2D
		{
			Position = point,
			CollisionMask = 0b1001

		};

		Godot.Collections.Array result = (Godot.Collections.Array)worldState.IntersectPoint(parameters, 1);

		if (result.Count >= 1)
		{
			return;
		}


		int index = (int)GD.RandRange(0, creaturesList.Count - 1);
		PackedScene creatureScene = creaturesList[index];
		Node2D creature = (Node2D)creatureScene.Instantiate();
		creature.GlobalPosition = point;
		GetParent().AddChild(creature);

	}

	private Vector2 GetPoint()
	{
		pathFollow2D.ProgressRatio = (float)GD.Randf();
		return pathFollow2D.GlobalPosition;
	}
}
