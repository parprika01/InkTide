using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEditor;
using UnityEngine;

public class AuthClient
{
    [Header("event")]
    [SerializeField] private BoolEventChannel loginEvent;
    [SerializeField] private BoolEventChannel registerEvent;
    
    private string account;
    
    public AuthClient()
    {
        NetworkClient.RegisterHandler<LoginResponseMessage>(OnLoginResponse);
        NetworkClient.RegisterHandler<RegisterResponseMessage>(OnRegisterResponse);
    }
    
    public void Login(string account, string password)
    {
        LoginRequestMessage msg = new LoginRequestMessage
        {
            account = account,
            password = password
        };
        this.account = account;
        NetworkClient.Send(msg);
    }

    public void Register(string account, string password)
    {
        RegisterRequestMessage msg = new RegisterRequestMessage
        {
            account = account,
            password = password
        };
        
        NetworkClient.Send(msg);
    }

    private void OnLoginResponse(LoginResponseMessage msg)
    {
        if (msg.loginResult)
        {
            PlayerInfo.Instance.SetAccount(account);
        }
        loginEvent.Raise(msg.loginResult);
    }

    private void OnRegisterResponse(RegisterResponseMessage msg)
    {
        registerEvent.Raise(msg.registerResult);
    }
}
