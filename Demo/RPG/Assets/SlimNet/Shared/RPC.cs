using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class RPC
{
    public static SlimNet.RPCAction<string, string> Login;
    public static SlimNet.RPCFunc<string, string, string> CreateAccount;
}