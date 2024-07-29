using Godot;
using System;

public partial class GamerOverUI : CanvasLayer
{
	[Export]
	public float restartDelay = 5.0f;
	public Label timeLabel;
	public Label MonstersLabel;

	public GameManager gameManager;

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
		
		timeLabel = GetNode<Label>("%TimeLabel");
		MonstersLabel = GetNode<Label>("%MonstersDefeated");

		timeLabel.Text = GameManager.timeElapsedString;
		MonstersLabel.Text = GameManager.monsterDefeatedCounter.ToString();

		
	}

	public override void _Process(double delta)
	{
		restartDelay -= (float)delta;

		if (restartDelay <= 0.0f)
		{
			RestartGame();
		}
	}

	public void RestartGame()
	{
		gameManager.Reset();
		GetTree().ReloadCurrentScene();
	}

}
