using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public struct LoginRequestMessage : NetworkMessage
{
    public string account;
    public string password;
}

public struct LoginResponseMessage : NetworkMessage
{
    public bool loginResult;
}

public struct RegisterRequestMessage : NetworkMessage
{
    public string account;
    public string password;
}

public struct RegisterResponseMessage : NetworkMessage
{
    public bool registerResult;
}