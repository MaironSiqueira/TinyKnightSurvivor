using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class main : Node2D
{

	[Export]
	public CanvasLayer gameUI;
	[Export]
	public PackedScene gameOverUiTemplate;
	public GamerOverUI gamerOverUI;
	public GameManager gameManager;

	public override void _Ready()
	{

		gameOverUiTemplate = (PackedScene)ResourceLoader.Load("res://src/gameUI/GamerOverUI.tscn");
		GD.Print("main _Ready called");
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
			GD.Print("GameManager encontrado na main!");
			gameManager.GameOver += TriggerGameOver;
			GD.Print("Encontrou o signal na main");
		}
		else
		{
			GD.PrintErr("GameManager não encontrado!");
		}

	}
	public void TriggerGameOver()
	{
		GD.Print("TriggerGameOver chamado");
		if (gameUI != null)
		{
			GD.Print("Liberando gameUI");
			gameUI.QueueFree();
			gameUI = null;
		}

		if (gameUI == null)
		{

			gamerOverUI = (GamerOverUI)gameOverUiTemplate.Instantiate();
			GD.Print("Game Over UI instanciada corretamente");

			GD.Print("Game Over UI adicionada à cena");
			AddChild(gamerOverUI);

			GD.Print($"gameOverUiTemplate: {gameOverUiTemplate}");
			GD.Print($"gamerOverScene: {gamerOverUI}");
		}
		gameManager.GameOver -= TriggerGameOver;
	}
}
