using Godot;
using System;

public partial class BonfireInt : Node2D
{
	private bool inRange = false;
	
	// Visual components - adjust node paths as needed for your setup
	private AnimatedSprite2D exclamationSprite;
	private Area2D interactArea;
	
	public override void _Ready()
	{
		// Get child nodes - adjust these paths to match your scene structure
		interactArea = GetNode<Area2D>("InteractArea");
		exclamationSprite = GetNode<AnimatedSprite2D>("InteractArea/Exclamation");
		
		// Connect area signals
		interactArea.BodyEntered += OnPlayerEnter;
		interactArea.BodyExited += OnPlayerExit;
		
		// Hide exclamation initially
		exclamationSprite.Visible = false;
	}
	
	private void OnPlayerEnter(Node body)
	{
		if (body is Player)
		{
			inRange = true;
			exclamationSprite.Visible = true;
		}
	}
	
	private void OnPlayerExit(Node body)
	{
		if (body is Player)
		{
			inRange = false;
			exclamationSprite.Visible = false;
		}
	}
	
	public override void _Process(double delta)
	{
		if (inRange && Input.IsActionJustPressed("interact"))
		{
			InteractWithBonfire();
		}
	}
	
	private void InteractWithBonfire()
	{
		// Save the game
		if (GameManager.Instance.SaveGame())
		{
			ShowMessage("Game saved at bonfire.");
		}
		else
		{
			ShowMessage("Failed to save game.");
		}
	}
	
	private void ShowMessage(string message)
	{
		// Simple message display - you can enhance this with a proper UI
		GD.Print(message);
		
		// Optional: Create a temporary label to show the message on screen
		var label = new Label();
		label.Text = message;
		label.Position = new Vector2(400, 300); // Adjust position as needed
		label.AddThemeStyleboxOverride("normal", new StyleBoxFlat());
		
		GetTree().CurrentScene.AddChild(label);
		
		// Remove the message after a few seconds
		var timer = GetTree().CreateTimer(3.0);
		timer.Timeout += () => {
			if (IsInstanceValid(label))
				label.QueueFree();
		};
	}
}
