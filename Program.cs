using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace WakeOnLan
{
	class Program
	{
		static void Main(string[] args)
		{
			// Verify the correct amount of arguments
			if (args.Length != 1)
			{
				Console.WriteLine("Usage: WakeOnLan <MAC>");
				return;
			}

			// Initialize an empty byte array
			byte[] byAddress = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

			// Store the MAC address for parsing
			string sAddress = args[0];

			// Check the format of the MAC address (1: FFFFFFFFFFFF, or 2: FF-FF-FF-FF-FF-FF )
			if (sAddress.Length == 12)
			{
				int nIndex = 0;
				int nPosition = 0;
				int nCharCount = 2;

				while (nPosition < sAddress.Length)
				{
					string s = sAddress.Substring(nPosition, nCharCount);
					byAddress[nIndex] = byte.Parse(s, System.Globalization.NumberStyles.HexNumber);
					nPosition += nCharCount;
					nIndex++;
				}

			}
			else
			{
				string[] s = sAddress.Split(new string[] { "-" }, StringSplitOptions.None);
				for (int i = 0; i < 6; i++)
					byAddress[i] = byte.Parse(s[i], System.Globalization.NumberStyles.HexNumber);
			}

			Console.WriteLine(byAddress.ToString());
			SendWakeUp(byAddress);
		}

		private static void SendWakeUp(byte[] byAddress)
		{
			// The WOL packet is broadcasted over UDP on port 40000
			UdpClient udpClient = new UdpClient();
			udpClient.Connect(IPAddress.Broadcast, 40000);

			// Initialize the WOL packet: 6-byte lead, then 6-byte MAC (repeated 16 times)
			byte[] byPacket = new byte[17 * 6];

			// Fill in the 6-byte lead
			for (int i = 0; i < 6; i++)
				byPacket[i] = 0xFF;

			// Fill in the 6-byte MAC address (16 times)
			for (int i = 1; i <= 16; i++)
				for (int j = 0; j < 6; j++)
					byPacket[i * 6 + j] = byAddress[j];

			// Submit the WOL packet
			udpClient.Send(byPacket, byPacket.Length);
		}
	}
}
