using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.BouncyCastle.Asn1.Crmf;

public class Server : NetworkBehaviour
{
    public DatabaseController databaseController;
    private string connetionString = "Server=localhost;Database=inkTide;User=root;Password=Zxy196829;";
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        databaseController = new DatabaseController();
        databaseController.ConnectToDatabase(connetionString);
    }

    private void Start()
    {
        NetworkServer.RegisterHandler<LoginRequestMessage>(OnLoginRequest);
        NetworkServer.RegisterHandler<RegisterRequestMessage>(OnRegisterRequest);
        NetworkServer.RegisterHandler<UserInfoRequestMessage>(OnUserInfoRequest);
        NetworkServer.RegisterHandler<ModifyUserInfoRequestMessage>(OnModifyUserInfoRequest);
    }

    private void OnLoginRequest(NetworkConnectionToClient conn, LoginRequestMessage msg)
    {
        LoginResponseMessage reply = new LoginResponseMessage
        {
            loginResult = databaseController.isValidAccount(msg.account, msg.password)
        };
        
        conn.Send(reply);
        conn.Disconnect();
    }

    private void OnRegisterRequest(NetworkConnectionToClient conn, RegisterRequestMessage msg)
    {
        RegisterResponseMessage reply = new RegisterResponseMessage
        {
            registerResult = databaseController.RegisterUser(msg.account, msg.password)
        };
        
        conn.Send(reply);
        conn.Disconnect();
    }

    private void OnUserInfoRequest(NetworkConnectionToClient conn, UserInfoRequestMessage msg)
    {
        UserInfoResponseMessage reply = databaseController.GetUserInfo(msg.account);
        
        conn.Send(reply);
        conn.Disconnect();
    }

    private void OnModifyUserInfoRequest(NetworkConnectionToClient conn, ModifyUserInfoRequestMessage msg)
    {
        ModifyUserInfoResponseMessage reply = new ModifyUserInfoResponseMessage
        {
            modifyResult = databaseController.ModifyUserInfo(msg.account, msg.settingType, msg.settingValue)
        };
        
        conn.Send(reply);
        conn.Disconnect();
    }
}
