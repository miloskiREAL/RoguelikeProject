using Godot;
using System.Collections.Generic;

public partial class ActivityIndicator : Label
{
	private Queue<string> messageQueue = new();
	private Timer messageTimer;
	private bool isProcessing = false;
	
	// Delay between displaying messages
	[Export] private float messageDelay = 0.8f;  
	// Maximum number of messages to keep in queue
	[Export] private int maxMessages = 8;       
	
	public override void _Ready()
	{
		// Set up the message timer
		messageTimer = new Timer();
		messageTimer.WaitTime = messageDelay;
		messageTimer.Timeout += ProcessNextMessage;
		messageTimer.OneShot = true;
		AddChild(messageTimer);
		
		// Initial battle message
		Text = "Battle begins!";
	}
	
	// Adds a new message to the queue, removing oldest if at max capacity
	public void AddMessage(string message)
	{
		// Remove oldest message if queue is full
		if (messageQueue.Count >= maxMessages)
		{
			messageQueue.Dequeue();
		}
		
		messageQueue.Enqueue(message);
		
		// Start processing if not already doing so
		if (!isProcessing)
		{
			StartProcessing();
		}
	}
	
	// Begins processing messages in the queue
	private void StartProcessing()
	{
		isProcessing = true;
		// Short initial delay for immediate feedback
		messageTimer.WaitTime = 0.1f; 
		messageTimer.Start();
	}
	
	// Displays the next message in queue and handles timer for subsequent messages
	private void ProcessNextMessage()
	{
		// Stop if no messages left
		if (messageQueue.Count == 0)
		{
			isProcessing = false;
			return;
		}
		
		// Display next message
		Text = messageQueue.Dequeue();
		
		// Continue with next message if available
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
	
	// Logs message both to console and display queue
	public void Log(string message)
	{
		GD.Print(message);  
		AddMessage(message);  
	}
	
	// Returns whether there are still messages being processed or in queue
	public bool IsProcessing()
	{
		return isProcessing || messageQueue.Count > 0;
	}
}
