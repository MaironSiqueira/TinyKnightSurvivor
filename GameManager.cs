using Godot;
using System;

public partial class GameManager : Node
{
	[Signal]
	public delegate void GameOverEventHandler();
	public static Player player;
	public static Vector2 playerPosition;

	public float timeElapsed = 0.0f;
	public static string timeElapsedString;

	public static int monsterDefeatedCounter = 0;

	public static bool isGameOver = false;
	public override void _Process(double delta)
	{
		timeElapsed += (float)delta;
		int timeElapsedInSeconds = Mathf.FloorToInt(timeElapsed);
		int seconds = timeElapsedInSeconds % 60;
		int minutes = timeElapsedInSeconds / 60;
		timeElapsedString = string.Format("{0:00}:{1:00}", minutes, seconds);
	}
	public void EndGame()
	{
		if (isGameOver) return;
		isGameOver = true;
		EmitSignal(SignalName.GameOver);
	}

	public void Reset()
	{
		timeElapsed = 0.0f;
		timeElapsedString = "00:00";
		monsterDefeatedCounter = 0;

		playerPosition = Vector2.Zero;
		isGameOver = false;
	}
}
