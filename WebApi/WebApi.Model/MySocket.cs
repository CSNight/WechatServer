using Fleck;
using System;
using System.Threading.Tasks;

namespace WebApi.Model
{
	public class MySocket : IWebSocketConnection
	{
		Action IWebSocketConnection.OnOpen
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		Action IWebSocketConnection.OnClose
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		Action<string> IWebSocketConnection.OnMessage
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		Action<byte[]> IWebSocketConnection.OnBinary
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		Action<byte[]> IWebSocketConnection.OnPing
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		Action<byte[]> IWebSocketConnection.OnPong
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		Action<Exception> IWebSocketConnection.OnError
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		IWebSocketConnectionInfo IWebSocketConnection.ConnectionInfo
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool IWebSocketConnection.IsAvailable
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		void IWebSocketConnection.Close()
		{
			throw new NotImplementedException();
		}

		Task IWebSocketConnection.Send(string message)
		{
			throw new NotImplementedException();
		}

		Task IWebSocketConnection.Send(byte[] message)
		{
			throw new NotImplementedException();
		}

		Task IWebSocketConnection.SendPing(byte[] message)
		{
			throw new NotImplementedException();
		}

		Task IWebSocketConnection.SendPong(byte[] message)
		{
			throw new NotImplementedException();
		}
	}
}
