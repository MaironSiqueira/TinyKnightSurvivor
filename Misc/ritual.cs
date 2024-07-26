using Godot;
using System;

public partial class ritual : Node2D
{
	[Export]
	public int damageAmountRT = 1;

	public Area2D ritualArea;

	public enemy enemy;
	public override void _Ready()
	{
		ritualArea = GetNode<Area2D>("Area2D");
	}

	public void Damage(){
		var bodies = ritualArea.GetOverlappingBodies();

		foreach (Node body in bodies)
		{
			if (body.IsInGroup("enemies"))
			{
				enemy enemy = (enemy)body;
				enemy.damage(damageAmountRT);

			}
		}
	}


}
