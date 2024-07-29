using Godot;
using System;

public partial class damageDigit : Node2D
{
	 private int value = 0;

    public override void _Ready()
    {
       
        Label label = GetNode<Label>("%Label");
        label.Text = value.ToString();
    }
	
}
