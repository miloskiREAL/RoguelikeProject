using Godot;
using System;

public partial class BonfireInt : Node2D
{
	private bool inRange = false;
	
	// Visual components
	private AnimatedSprite2D exclamationSprite;
	private Area2D interactArea;
	
	public override void _Ready()
	{
		//Get child nodes
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
		// As seen before upon pressing E it performs the task
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
	}
	
	private void ShowMessage(string message)
	{
		GD.Print(message);
		
		//Creates a temporary label to show the message on screen for confirmation of saving
		var label = new Label();
		label.Text = message;
		label.Position = new Vector2(480, 440);
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
