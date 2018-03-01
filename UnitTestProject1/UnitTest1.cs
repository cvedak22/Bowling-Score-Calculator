using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Bowling;
using System.Linq;

namespace UnitTestProject1
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestAll10()
		{
			var lines = File.ReadAllLines("all10.txt");
			var game = Game.FromLines(lines);
			Assert.AreEqual(game.Frames.Last().Score, 300);
		}

		[TestMethod]
		public void TestAllF()
		{
			var lines = File.ReadAllLines("allF.txt");
			var game = Game.FromLines(lines);
			Assert.AreEqual(game.Frames.Last().Score, 0);
		}

		[TestMethod]
		public void TestAll0()
		{
			var lines = File.ReadAllLines("all0.txt");
			var game = Game.FromLines(lines);
			Assert.AreEqual(game.Frames.Last().Score, 0);
		}


	}
}
