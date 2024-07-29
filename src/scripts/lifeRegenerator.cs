using Godot;
using System;

public partial class lifeRegenerator : AnimatedSprite2D
{
	[Export]
	public int regeneratorAmount = 10;

	public Area2D meatArea;

	public Player player;


	public override void _Ready()
	{
		meatArea = GetNode<Area2D>("Area2D");
        AddToGroup("meat");
	}


	public void _onBodyEntered(Node2D body)
	{
		if(body.IsInGroup("player")){
			Player player = (Player)body;
			player.Heal(regeneratorAmount);

			// Emit the signal when meat is collected
            player.EmitSignal("MeatCollected", regeneratorAmount);

            GD.Print("Sinal emitido com amount: " + regeneratorAmount);
			
			QueueFree();
		}
	}
}
