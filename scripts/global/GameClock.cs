using Godot;
using System;

public partial class GameClock : Node
{
	public const int HOURS_PER_DAY = 24;
	public const int STANDARD_HOURS_PER_NIGHT = 9;
	public const int MINUTES_PER_HOUR = 60;

	[Signal] public delegate void ClockAdvancedEventHandler(int night, int minutesRemaining);
	[Signal] public delegate void ClockDaybreakEventHandler(int night, bool sheltered);

	public int Night { get; private set; } = 1;
	public int MinutesRemaining { get; private set; }
	public int DayBreak { get; private set; } = 6 * MINUTES_PER_HOUR;

	public GameClock(int hoursRemaining)
	{
		MinutesRemaining = hoursRemaining * MINUTES_PER_HOUR;
		GD.Print($"gameclock constructor, set to {MinutesRemaining} minutes");
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.PrintErr("\nGame clock ready!\n");
		SetMinutes(STANDARD_HOURS_PER_NIGHT * MINUTES_PER_HOUR);
	}

	public void SetMinutes(int minutesRemaining)
	{
		MinutesRemaining = minutesRemaining;
	}

	public void SetNight(int night)
	{
		Night = night;
	}

	public bool Advance(int minutes)
	{
		int timeLeft = MinutesRemaining - minutes;
		bool sunThreat = false;
		if (timeLeft <= 0)
		{
			Night++;
			sunThreat = true;
			MinutesRemaining = STANDARD_HOURS_PER_NIGHT * MINUTES_PER_HOUR;
		}
		else
		{
			MinutesRemaining = timeLeft;
		}
		EmitSignal(SignalName.ClockAdvanced, Night, MinutesRemaining);
		if (sunThreat) { EmitSignal(SignalName.ClockDaybreak, Night); }
		return sunThreat;
	}

	public static Tuple<int, int> MinutesToHours(int minutes)
	{
		int hours = Math.DivRem(minutes, MINUTES_PER_HOUR, out int minutesLeft);
		return new(hours, minutesLeft);
	}

	public static string GetClockTimeStamp(int night, int minutes)
	{
		Tuple<int, int> timeStamp = MinutesToHours(minutes);
		return $"Night {night}, {timeStamp.Item1:D2}:{timeStamp.Item2:D2}";
	}

	public static string GetTimeEstimate(int minutes)
	{
		if (minutes == 1) { return "1 minute"; }
		else if (minutes < MINUTES_PER_HOUR) { return $"{minutes} minutes"; }
		else
		{
			int hours = (int)Math.Floor((decimal)minutes / MINUTES_PER_HOUR);
			return hours != 1 ? $"{hours} hours" : "1 hour";
		}
	}

	public static string GetClockCountdown(int minutes)
	{
		if (minutes < 20)
		{
			return "Sunrise imminent";
		}
		else if (minutes < MINUTES_PER_HOUR)
		{
			return "Less than an hour before sunrise";
		}
		else if (minutes < MINUTES_PER_HOUR * (STANDARD_HOURS_PER_NIGHT - 2))
		{
			return $"{GetTimeEstimate(minutes)} till sunrise";
		}
		else
		{
			return "The night is young";
		}
	}

	public string GetClockOutput()
	{
		GD.Print("GetClockOutput() called!!!");
		int maxMinutes = HOURS_PER_DAY * MINUTES_PER_HOUR;
		if (MinutesRemaining > maxMinutes)
		{
			throw new ArgumentOutOfRangeException(
				$"Clock has {MinutesRemaining} minutes; should never exceed {maxMinutes} minutes!"
			);
		}
		int minsPastMidnight = DayBreak - MinutesRemaining;
		int displayMinutes = minsPastMidnight;
		if (minsPastMidnight < 0)
		{
			displayMinutes = maxMinutes - Math.Abs(minsPastMidnight);
		}

		return $"{GetClockTimeStamp(Night, displayMinutes)}. " +
			$"{GetClockCountdown(MinutesRemaining)}.";
	}

	public override string ToString()
	{
		GD.Print("Clock ToSTring called");
		return $"{GetClockOutput()}";
	}
}
