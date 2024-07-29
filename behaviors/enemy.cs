using Godot;
using System;
using System.Drawing;
using System.Linq.Expressions;
using System.Collections.Generic;

public partial class enemy : CharacterBody2D
{
	[ExportCategory ("Misc")]
	[Export]
	public float dropChance = 0.1f;
	[Export]
	public int health = 10;

	[ExportCategory ("Lists")]
	[Export]
	public Godot.Collections.Array<PackedScene> dropItems;
	[Export]
	public Godot.Collections.Array<float> dropChancess;

	
	//Scenes
	public PackedScene deathPrefab;
	public PackedScene damageDigitPrefab;
	public AnimatedSprite2D character;
	public Marker2D damageDigitMarker;

	public override void _Ready()
	{
		deathPrefab = GD.Load<PackedScene>("res://Misc/skull.tscn");
		character = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		damageDigitPrefab = GD.Load<PackedScene>("res://Misc/damageDigit.tscn");
		damageDigitMarker = GetNode<Marker2D>("DamageMarker");
	}

	public void damage(int amount)
	{
		health -= amount;
		GD.Print("Inimigo recebeu dano de ", amount, ", a vida total é de: ", health);

		character.Modulate = new Godot.Color(1, 0, 0);
		var tween = CreateTween();
		tween.SetEase(Tween.EaseType.In);
		tween.SetTrans(Tween.TransitionType.Quint);
		tween.TweenProperty(character, "modulate", new Godot.Color(1, 1, 1), 0.3f);


		var damageDigit = (Node2D)damageDigitPrefab.Instantiate();
		damageDigit.Set("value", amount);

		if (damageDigitMarker != null)
		{
			damageDigit.GlobalPosition = damageDigitMarker.GlobalPosition;
		}
		else
		{
			damageDigit.GlobalPosition = GlobalPosition;
		}

		GetParent().AddChild(damageDigit);
		if (health <= 0)
		{
			die();
		}
	}
	public void die()
	{
		Node2D deathInstance = (Node2D)deathPrefab.Instantiate();
		GetParent().AddChild(deathInstance);

		deathInstance.Position = Position;

		if (GD.Randf() <= dropChance)
		{
			DropItem();
		}

		GameManager.monsterDefeatedCounter += 1;

		QueueFree();
	}

	public void DropItem(){
		Node2D drop = (Node2D)GetRandomDropItem().Instantiate();
		GetParent().AddChild(drop);

		drop.Position = Position;
		GD.Print("instanciou o drop");
	}
	public PackedScene GetRandomDropItem()
    {
		GD.Print("entrou na lista");
        // Listas com 1 item
        if (dropItems.Count == 1)
        {
			
            return dropItems[0];
        }

        // Calcular chance máxima
        float maxChance = 0.0f;
        foreach (float dropChance in dropChancess)
        {
            maxChance += dropChance;
        }

        // Jogar dado
        float randomValue = (float)GD.Randf() * maxChance;

        // Girar a roleta
        float needle = 0.0f;
        for (int i = 0; i < dropItems.Count; i++)
        {
			GD.Print("entrou no for");
            PackedScene dropItem = dropItems[i];
            float dropChance = i < dropChancess.Count ? dropChancess[i] : 1.0f;
            if (randomValue < dropChance + needle)
            {
				GD.Print("entoru no if");
                return dropItem;
            }

            needle += dropChance;
			GD.PrintErr("deu erro para gerar");
        }
		GD.Print("passou do for");

        return dropItems[0]; // fallback
    }
}



