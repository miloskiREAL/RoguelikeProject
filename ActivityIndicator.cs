using Godot;
using System.Collections.Generic;

public partial class ActivityIndicator : Label
{
	private Queue<string> messageQueue = new();
	private Timer messageTimer;
	private bool isProcessing = false;
	
	[Export] private float messageDelay = 0.8f;  
	[Export] private int maxMessages = 8;       
	
	public override void _Ready()
	{
		messageTimer = new Timer();
		messageTimer.WaitTime = messageDelay;
		messageTimer.Timeout += ProcessNextMessage;
		messageTimer.OneShot = true;
		AddChild(messageTimer);
		
		Text = "Battle begins!";
	}
	
	public void AddMessage(string message)
	{
		if (messageQueue.Count >= maxMessages)
		{
			messageQueue.Dequeue();
		}
		
		messageQueue.Enqueue(message);
		
		if (!isProcessing)
		{
			StartProcessing();
		}
	}
	
	private void StartProcessing()
	{
		isProcessing = true;
		messageTimer.WaitTime = 0.1f; 
		messageTimer.Start();
	}
	
	private void ProcessNextMessage()
	{
		if (messageQueue.Count == 0)
		{
			isProcessing = false;
			return;
		}
		
		Text = messageQueue.Dequeue();
		
		if (messageQueue.Count > 0)
		{
			// Reset to normal delay for subsequent messages
			messageTimer.WaitTime = messageDelay;
			messageTimer.Start();
		}
		else
		{
			isProcessing = false;
		}
	}
	
	public void Log(string message)
	{
		GD.Print(message);  
		AddMessage(message);  
	}
	
	// Public method to check if the indicator is still processing messages
	public bool IsProcessing()
	{
		return isProcessing || messageQueue.Count > 0;
	}
}
