using Godot;
using System;

public partial class followPlayerC : Node
{
    [Export]
	private float speed = 1;
    private AnimatedSprite2D sprite;
    private CharacterBody2D enemy;
    public override void _Ready()
	
    {
        enemy = (CharacterBody2D)GetParent();
        sprite = (AnimatedSprite2D)enemy.GetNode("AnimatedSprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        if(GameManager.isGameOver) return;
        
        Vector2 playerPosition = GameManager.playerPosition;
        Vector2 difference = playerPosition - enemy.Position;
        Vector2 inputVector = difference.Normalized();

        enemy.Velocity = inputVector * speed * 100.0f;
        enemy.MoveAndSlide();

        if (inputVector.X > 0)
        {
            sprite.FlipH = false;
        }
        else if (inputVector.X < 0)
        {
            sprite.FlipH = true;
        }
    }
}
