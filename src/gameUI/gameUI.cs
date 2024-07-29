using Godot;
using System;

public partial class gameUI : CanvasLayer
{
	
	 private int collectedMeats = 0;
	public Label timerLabel;
	public Label goldLabel;
	public Label meatLabel;

	
	public override void _Ready()
	{
		 timerLabel = GetNode<Label>("%TimerLabel");
		 meatLabel = GetNode<Label>("%MeatLabel");
		 meatLabel.Text = collectedMeats.ToString();
		GameManager.player.MeatCollected += OnMeatCollected;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timerLabel.Text = GameManager.timeElapsedString;
	}

	private void OnMeatCollected(int amount)
    {
		GD.Print("Sinal OnMeatCollected recebido com amount: " + amount);
        collectedMeats += 1;
        meatLabel.Text = collectedMeats.ToString();
        
    }
}
