using System;

namespace BFMixedConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			string message = BF.Message.Invoke();

			Console.WriteLine("Message is: " + message);

			string finalMessage = BF.FakeEncryption.Invoke(message);

			Console.WriteLine("Ciphertext is: " + finalMessage);
		}
	}
}
