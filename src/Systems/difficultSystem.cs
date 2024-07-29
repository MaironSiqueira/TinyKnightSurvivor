using Godot;
using System;

public partial class difficultSystem : Node
{
	[Export]
	public mobSpawner mobSpawner;

	[Export]
	public float initialSpawnRate = 60.0f;
	[Export]
	public float waveDuration = 20.0f;
	[Export]
	public float spawnRatePerMinute = 30.0f;
	[Export]
	public float breakIntensity = 0.5f;
	public float time = 0.0f;


	public override void _Process(double delta)
	{
		if(GameManager.isGameOver) return;
		
		time += (float)delta;

		var spawnRate = initialSpawnRate + spawnRatePerMinute * (time / 60.0f);

		var sinWave = Math.Sin(time * Math.Tau / waveDuration);
		float waveFactor = (float)Mathf.Lerp(1, breakIntensity, Mathf.InverseLerp(-1, 1, sinWave));
		

		spawnRate *= waveFactor;

		mobSpawner.mobsPerMinute = spawnRate;
	}
}
