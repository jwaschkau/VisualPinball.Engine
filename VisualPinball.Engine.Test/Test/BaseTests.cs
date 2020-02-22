﻿using NLog;
using NLog.Targets;
using VisualPinball.Engine.Game;
using VisualPinball.Engine.Math;
using VisualPinball.Engine.VPT.Ball;
using VisualPinball.Engine.VPT.Table;
using Xunit.Abstractions;

namespace VisualPinball.Engine.Test.Test
{
	public abstract class BaseTests
	{
		protected readonly Logger Logger;

		protected BaseTests(ITestOutputHelper output)
		{
			var config = new NLog.Config.LoggingConfiguration();
			var logConsole = new TestTarget(output);
			config.AddRule(LogLevel.Trace, LogLevel.Fatal, logConsole);
			LogManager.Configuration = config;
			Logger = LogManager.GetCurrentClassLogger();
		}

		protected static Ball CreateBall(Player player, float x, float y, float z, float vx = 0, float vy = 0, float vz = 0)
		{
			return player.CreateBall(new TestBallCreator(x, y, z, vx, vy, vz));
		}
	}

	[Target("Test")]
	public class TestTarget : TargetWithLayout
	{
		private readonly ITestOutputHelper _output;

		public TestTarget(ITestOutputHelper output)
		{
			_output = output;
		}

		protected override void Write(LogEventInfo logEvent)
		{
			var msg = Layout.Render(logEvent);
			_output.WriteLine(msg);
		}
	}

	public class TestBallCreator : IBallCreationPosition
	{
		private readonly Vertex3D _pos;
		private readonly Vertex3D _vel;

		public TestBallCreator(float x, float y, float z, float vx, float vy, float vz)
		{
			_pos = new Vertex3D(x, y, z);
			_vel = new Vertex3D(vx, vy, vz);
		}

		public Vertex3D GetBallCreationPosition(Table table) => _pos;

		public Vertex3D GetBallCreationVelocity(Table table) => _vel;

		public void OnBallCreated(PlayerPhysics physics, Ball ball)
		{
			// do nothing
		}
	}
}
