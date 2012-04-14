/*
 * SlimNet - Networking Middleware For Games
 * Copyright (C) 2011-2012 Fredrik Holmström
 * 
 * This notice may not be removed or altered.
 * 
 * This software is provided 'as-is', without any expressed or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software. 
 * 
 * Attribution
 * The origin of this software must not be misrepresented; you must not
 * claim that you wrote the original software. For any works using this 
 * software, reasonable acknowledgment is required.
 * 
 * Noncommercial
 * You may not use this software for commercial purposes.
 * 
 * Distribution
 * You are not allowed to distribute or make publicly available the software 
 * itself or its source code in original or modified form.
 */

/*
 * This file is auto-generated, do not edit.
 */

using System.Reflection;
using System.Collections.Generic;

namespace SlimNet 
{
	public class RPCAction
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCAction(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		
		public void Invoke()
		{
			InvokeOn(RPC.Context.Client.Player);
		}
		
		public void InvokeOn(Player player)
		{
			InvokeOn(new[] { player });
		}
		
		public void InvokeOn(IEnumerable<Player> players)
		{
			RPC.Invoke(players, null, Name);
		}

		 
	}

	public class RPCFunc<R>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCFunc(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		 

		public RPCResult<R> InvokeOnServer()
		{
			return InvokeOn(RPC.Context.Client.Player);
		}

		public RPCResult<R> InvokeOn(Player player)
		{
			RPCResult<R> result = RPC.CreateResult<R>();
			
			RPC.Invoke(new[] { player }, result, Name);

			return result;
		}

		 
	}

	public class RPCAction<T0>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCAction(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		
		public void Invoke(T0 a0)
		{
			InvokeOn(RPC.Context.Client.Player, a0);
		}
		
		public void InvokeOn(Player player, T0 a0)
		{
			InvokeOn(new[] { player }, a0);
		}
		
		public void InvokeOn(IEnumerable<Player> players, T0 a0)
		{
			RPC.Invoke(players, null, Name, a0);
		}

		 
	}

	public class RPCFunc<T0, R>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCFunc(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		 

		public RPCResult<R> InvokeOnServer(T0 a0)
		{
			return InvokeOn(RPC.Context.Client.Player, a0);
		}

		public RPCResult<R> InvokeOn(Player player, T0 a0)
		{
			RPCResult<R> result = RPC.CreateResult<R>();
			
			RPC.Invoke(new[] { player }, result, Name, a0);

			return result;
		}

		 
	}

	public class RPCAction<T0, T1>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCAction(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		
		public void Invoke(T0 a0, T1 a1)
		{
			InvokeOn(RPC.Context.Client.Player, a0, a1);
		}
		
		public void InvokeOn(Player player, T0 a0, T1 a1)
		{
			InvokeOn(new[] { player }, a0, a1);
		}
		
		public void InvokeOn(IEnumerable<Player> players, T0 a0, T1 a1)
		{
			RPC.Invoke(players, null, Name, a0, a1);
		}

		 
	}

	public class RPCFunc<T0, T1, R>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCFunc(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		 

		public RPCResult<R> InvokeOnServer(T0 a0, T1 a1)
		{
			return InvokeOn(RPC.Context.Client.Player, a0, a1);
		}

		public RPCResult<R> InvokeOn(Player player, T0 a0, T1 a1)
		{
			RPCResult<R> result = RPC.CreateResult<R>();
			
			RPC.Invoke(new[] { player }, result, Name, a0, a1);

			return result;
		}

		 
	}

	public class RPCAction<T0, T1, T2>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCAction(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		
		public void Invoke(T0 a0, T1 a1, T2 a2)
		{
			InvokeOn(RPC.Context.Client.Player, a0, a1, a2);
		}
		
		public void InvokeOn(Player player, T0 a0, T1 a1, T2 a2)
		{
			InvokeOn(new[] { player }, a0, a1, a2);
		}
		
		public void InvokeOn(IEnumerable<Player> players, T0 a0, T1 a1, T2 a2)
		{
			RPC.Invoke(players, null, Name, a0, a1, a2);
		}

		 
	}

	public class RPCFunc<T0, T1, T2, R>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCFunc(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		 

		public RPCResult<R> InvokeOnServer(T0 a0, T1 a1, T2 a2)
		{
			return InvokeOn(RPC.Context.Client.Player, a0, a1, a2);
		}

		public RPCResult<R> InvokeOn(Player player, T0 a0, T1 a1, T2 a2)
		{
			RPCResult<R> result = RPC.CreateResult<R>();
			
			RPC.Invoke(new[] { player }, result, Name, a0, a1, a2);

			return result;
		}

		 
	}

	public class RPCAction<T0, T1, T2, T3>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCAction(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		
		public void Invoke(T0 a0, T1 a1, T2 a2, T3 a3)
		{
			InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3);
		}
		
		public void InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3)
		{
			InvokeOn(new[] { player }, a0, a1, a2, a3);
		}
		
		public void InvokeOn(IEnumerable<Player> players, T0 a0, T1 a1, T2 a2, T3 a3)
		{
			RPC.Invoke(players, null, Name, a0, a1, a2, a3);
		}

		 
	}

	public class RPCFunc<T0, T1, T2, T3, R>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCFunc(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		 

		public RPCResult<R> InvokeOnServer(T0 a0, T1 a1, T2 a2, T3 a3)
		{
			return InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3);
		}

		public RPCResult<R> InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3)
		{
			RPCResult<R> result = RPC.CreateResult<R>();
			
			RPC.Invoke(new[] { player }, result, Name, a0, a1, a2, a3);

			return result;
		}

		 
	}

	public class RPCAction<T0, T1, T2, T3, T4>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCAction(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		
		public void Invoke(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4)
		{
			InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4);
		}
		
		public void InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4)
		{
			InvokeOn(new[] { player }, a0, a1, a2, a3, a4);
		}
		
		public void InvokeOn(IEnumerable<Player> players, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4)
		{
			RPC.Invoke(players, null, Name, a0, a1, a2, a3, a4);
		}

		 
	}

	public class RPCFunc<T0, T1, T2, T3, T4, R>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCFunc(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		 

		public RPCResult<R> InvokeOnServer(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4)
		{
			return InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4);
		}

		public RPCResult<R> InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4)
		{
			RPCResult<R> result = RPC.CreateResult<R>();
			
			RPC.Invoke(new[] { player }, result, Name, a0, a1, a2, a3, a4);

			return result;
		}

		 
	}

	public class RPCAction<T0, T1, T2, T3, T4, T5>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCAction(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		
		public void Invoke(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)
		{
			InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4, a5);
		}
		
		public void InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)
		{
			InvokeOn(new[] { player }, a0, a1, a2, a3, a4, a5);
		}
		
		public void InvokeOn(IEnumerable<Player> players, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)
		{
			RPC.Invoke(players, null, Name, a0, a1, a2, a3, a4, a5);
		}

		 
	}

	public class RPCFunc<T0, T1, T2, T3, T4, T5, R>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCFunc(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		 

		public RPCResult<R> InvokeOnServer(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)
		{
			return InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4, a5);
		}

		public RPCResult<R> InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5)
		{
			RPCResult<R> result = RPC.CreateResult<R>();
			
			RPC.Invoke(new[] { player }, result, Name, a0, a1, a2, a3, a4, a5);

			return result;
		}

		 
	}

	public class RPCAction<T0, T1, T2, T3, T4, T5, T6>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCAction(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		
		public void Invoke(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)
		{
			InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4, a5, a6);
		}
		
		public void InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)
		{
			InvokeOn(new[] { player }, a0, a1, a2, a3, a4, a5, a6);
		}
		
		public void InvokeOn(IEnumerable<Player> players, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)
		{
			RPC.Invoke(players, null, Name, a0, a1, a2, a3, a4, a5, a6);
		}

		 
	}

	public class RPCFunc<T0, T1, T2, T3, T4, T5, T6, R>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCFunc(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		 

		public RPCResult<R> InvokeOnServer(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)
		{
			return InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4, a5, a6);
		}

		public RPCResult<R> InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6)
		{
			RPCResult<R> result = RPC.CreateResult<R>();
			
			RPC.Invoke(new[] { player }, result, Name, a0, a1, a2, a3, a4, a5, a6);

			return result;
		}

		 
	}

	public class RPCAction<T0, T1, T2, T3, T4, T5, T6, T7>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCAction(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		
		public void Invoke(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7)
		{
			InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4, a5, a6, a7);
		}
		
		public void InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7)
		{
			InvokeOn(new[] { player }, a0, a1, a2, a3, a4, a5, a6, a7);
		}
		
		public void InvokeOn(IEnumerable<Player> players, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7)
		{
			RPC.Invoke(players, null, Name, a0, a1, a2, a3, a4, a5, a6, a7);
		}

		 
	}

	public class RPCFunc<T0, T1, T2, T3, T4, T5, T6, T7, R>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCFunc(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		 

		public RPCResult<R> InvokeOnServer(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7)
		{
			return InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4, a5, a6, a7);
		}

		public RPCResult<R> InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7)
		{
			RPCResult<R> result = RPC.CreateResult<R>();
			
			RPC.Invoke(new[] { player }, result, Name, a0, a1, a2, a3, a4, a5, a6, a7);

			return result;
		}

		 
	}

	public class RPCAction<T0, T1, T2, T3, T4, T5, T6, T7, T8>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCAction(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		
		public void Invoke(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8)
		{
			InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4, a5, a6, a7, a8);
		}
		
		public void InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8)
		{
			InvokeOn(new[] { player }, a0, a1, a2, a3, a4, a5, a6, a7, a8);
		}
		
		public void InvokeOn(IEnumerable<Player> players, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8)
		{
			RPC.Invoke(players, null, Name, a0, a1, a2, a3, a4, a5, a6, a7, a8);
		}

		 
	}

	public class RPCFunc<T0, T1, T2, T3, T4, T5, T6, T7, T8, R>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCFunc(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		 

		public RPCResult<R> InvokeOnServer(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8)
		{
			return InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4, a5, a6, a7, a8);
		}

		public RPCResult<R> InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8)
		{
			RPCResult<R> result = RPC.CreateResult<R>();
			
			RPC.Invoke(new[] { player }, result, Name, a0, a1, a2, a3, a4, a5, a6, a7, a8);

			return result;
		}

		 
	}

	public class RPCAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCAction(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		
		public void Invoke(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9)
		{
			InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9);
		}
		
		public void InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9)
		{
			InvokeOn(new[] { player }, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9);
		}
		
		public void InvokeOn(IEnumerable<Player> players, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9)
		{
			RPC.Invoke(players, null, Name, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9);
		}

		 
	}

	public class RPCFunc<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, R>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCFunc(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		 

		public RPCResult<R> InvokeOnServer(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9)
		{
			return InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9);
		}

		public RPCResult<R> InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9)
		{
			RPCResult<R> result = RPC.CreateResult<R>();
			
			RPC.Invoke(new[] { player }, result, Name, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9);

			return result;
		}

		 
	}

	public class RPCAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCAction(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		
		public void Invoke(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10)
		{
			InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
		}
		
		public void InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10)
		{
			InvokeOn(new[] { player }, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
		}
		
		public void InvokeOn(IEnumerable<Player> players, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10)
		{
			RPC.Invoke(players, null, Name, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
		}

		 
	}

	public class RPCFunc<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCFunc(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		 

		public RPCResult<R> InvokeOnServer(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10)
		{
			return InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);
		}

		public RPCResult<R> InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10)
		{
			RPCResult<R> result = RPC.CreateResult<R>();
			
			RPC.Invoke(new[] { player }, result, Name, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10);

			return result;
		}

		 
	}

	public class RPCAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCAction(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		
		public void Invoke(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10, T11 a11)
		{
			InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);
		}
		
		public void InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10, T11 a11)
		{
			InvokeOn(new[] { player }, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);
		}
		
		public void InvokeOn(IEnumerable<Player> players, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10, T11 a11)
		{
			RPC.Invoke(players, null, Name, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);
		}

		 
	}

	public class RPCFunc<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, R>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCFunc(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		 

		public RPCResult<R> InvokeOnServer(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10, T11 a11)
		{
			return InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);
		}

		public RPCResult<R> InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10, T11 a11)
		{
			RPCResult<R> result = RPC.CreateResult<R>();
			
			RPC.Invoke(new[] { player }, result, Name, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11);

			return result;
		}

		 
	}

	public class RPCAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCAction(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		
		public void Invoke(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10, T11 a11, T12 a12)
		{
			InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12);
		}
		
		public void InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10, T11 a11, T12 a12)
		{
			InvokeOn(new[] { player }, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12);
		}
		
		public void InvokeOn(IEnumerable<Player> players, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10, T11 a11, T12 a12)
		{
			RPC.Invoke(players, null, Name, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12);
		}

		 
	}

	public class RPCFunc<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, R>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCFunc(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		 

		public RPCResult<R> InvokeOnServer(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10, T11 a11, T12 a12)
		{
			return InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12);
		}

		public RPCResult<R> InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10, T11 a11, T12 a12)
		{
			RPCResult<R> result = RPC.CreateResult<R>();
			
			RPC.Invoke(new[] { player }, result, Name, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12);

			return result;
		}

		 
	}

	public class RPCAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCAction(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		
		public void Invoke(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10, T11 a11, T12 a12, T13 a13)
		{
			InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13);
		}
		
		public void InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10, T11 a11, T12 a12, T13 a13)
		{
			InvokeOn(new[] { player }, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13);
		}
		
		public void InvokeOn(IEnumerable<Player> players, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10, T11 a11, T12 a12, T13 a13)
		{
			RPC.Invoke(players, null, Name, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13);
		}

		 
	}

	public class RPCFunc<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, R>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCFunc(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		 

		public RPCResult<R> InvokeOnServer(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10, T11 a11, T12 a12, T13 a13)
		{
			return InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13);
		}

		public RPCResult<R> InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10, T11 a11, T12 a12, T13 a13)
		{
			RPCResult<R> result = RPC.CreateResult<R>();
			
			RPC.Invoke(new[] { player }, result, Name, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13);

			return result;
		}

		 
	}

	public class RPCAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCAction(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		
		public void Invoke(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10, T11 a11, T12 a12, T13 a13, T14 a14)
		{
			InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14);
		}
		
		public void InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10, T11 a11, T12 a12, T13 a13, T14 a14)
		{
			InvokeOn(new[] { player }, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14);
		}
		
		public void InvokeOn(IEnumerable<Player> players, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10, T11 a11, T12 a12, T13 a13, T14 a14)
		{
			RPC.Invoke(players, null, Name, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14);
		}

		 
	}

	public class RPCFunc<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, R>
	{
		public string Name { get; private set; }
		public RPCDispatcher RPC { get; private set; }

		public RPCFunc(RPCDispatcher rpc, string name)
		{
			Assert.NotNull(rpc, "rpc");
			Assert.NotNullOrEmpty(name, "name");

			RPC = rpc;
			Name = name;
		}
		 

		public RPCResult<R> InvokeOnServer(T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10, T11 a11, T12 a12, T13 a13, T14 a14)
		{
			return InvokeOn(RPC.Context.Client.Player, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14);
		}

		public RPCResult<R> InvokeOn(Player player, T0 a0, T1 a1, T2 a2, T3 a3, T4 a4, T5 a5, T6 a6, T7 a7, T8 a8, T9 a9, T10 a10, T11 a11, T12 a12, T13 a13, T14 a14)
		{
			RPCResult<R> result = RPC.CreateResult<R>();
			
			RPC.Invoke(new[] { player }, result, Name, a0, a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14);

			return result;
		}

		 
	}

	public partial class RPCDispatcher 
	{
		public List<FieldInfo> GetDispatcherFields() 
		{
			return Utils.TypeUtils2.GetFieldsWithTypes(
				BindingFlags.Public | BindingFlags.Static,
				typeof(RPCAction),
                typeof(RPCFunc<>),
                typeof(RPCAction<>),
                typeof(RPCFunc<,>),
                typeof(RPCAction<,>),
                typeof(RPCFunc<,,>),
                typeof(RPCAction<,,>),
                typeof(RPCFunc<,,,>),
                typeof(RPCAction<,,,>),
                typeof(RPCFunc<,,,,>),
                typeof(RPCAction<,,,,>),
                typeof(RPCFunc<,,,,,>),
                typeof(RPCAction<,,,,,>),
                typeof(RPCFunc<,,,,,,>),
                typeof(RPCAction<,,,,,,>),
                typeof(RPCFunc<,,,,,,,>),
                typeof(RPCAction<,,,,,,,>),
                typeof(RPCFunc<,,,,,,,,>),
                typeof(RPCAction<,,,,,,,,>),
                typeof(RPCFunc<,,,,,,,,,>),
                typeof(RPCAction<,,,,,,,,,>),
                typeof(RPCFunc<,,,,,,,,,,>),
                typeof(RPCAction<,,,,,,,,,,>),
                typeof(RPCFunc<,,,,,,,,,,,>),
                typeof(RPCAction<,,,,,,,,,,,>),
                typeof(RPCFunc<,,,,,,,,,,,,>),
                typeof(RPCAction<,,,,,,,,,,,,>),
                typeof(RPCFunc<,,,,,,,,,,,,,>),
                typeof(RPCAction<,,,,,,,,,,,,,>),
                typeof(RPCFunc<,,,,,,,,,,,,,,>),
                typeof(RPCAction<,,,,,,,,,,,,,,>),
                typeof(RPCFunc<,,,,,,,,,,,,,,,>)
			);
		}
	}
}