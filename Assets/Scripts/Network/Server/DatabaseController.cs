using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.Data;

public class DatabaseController
{
    public MySqlConnection connection;
        public void ConnectToDatabase(string connectionString)
    {
        try
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            Debug.Log("Database connection opened");
        }
        catch (MySqlException ex)
        {
            Debug.Log(ex.Message);
        }
    }
    
    public void CloseConnection()
    {
        if (connection != null && connection.State == ConnectionState.Open)
        {
            connection.Close();
        }
    }

    public bool isValidAccount(string account, string password)
    {
        string query = "SELECT COUNT(*) FROM userinfo WHERE account = @account AND password = @password";
        try
        {
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                // 添加参数
                command.Parameters.AddWithValue("@account", account);
                command.Parameters.AddWithValue("@password", password); // 建议存储哈希值而非明文
                    
                int count = Convert.ToInt32(command.ExecuteScalar());
                    
                return count > 0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("登录验证出错: " + ex.Message);
            return false;
        }
    }

    public bool RegisterUser(string account, string password)
    {
        string query = "INSERT INTO userinfo VALUES (@account, @password)";
        try
        {
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@account", account);
                command.Parameters.AddWithValue("@password", password);
                
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected == 1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("注册过程出错： " + ex.Message);
            return false;
        }
    }

    public UserInfoResponseMessage GetUserInfo(string account)
    {
        string query = "SELECT * FROM userinfo WHERE account = @account";
        using (MySqlCommand command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@account", account);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    UserInfoResponseMessage user = new UserInfoResponseMessage
                    {
                        playerName = (string)reader["username"],
                        mainWeapon = (string)reader["weapon"],
                        eyebrow = (string)reader["eyebrow"],
                        hair = (string)reader["hair"],
                        top = (string)reader["top"],
                        bottom = (string)reader["bottom"],
                        shoes = (string)reader["shoes"],
                        head = (string)reader["head"],
                        victoryAction = (string)reader["victoryAction"],
                        color = (string)reader["color"],
                        colorIndex = (string)reader["colorIndex"]
                    };
                    return user;
                }
            }
        }
        return new UserInfoResponseMessage();
    }

    public bool ModifyUserInfo(string account, string settingType, string settingValue)
    {
        string query = "UPDATE userinfo SET @settingType = @settingValue WHERE account = @account";
        try
        {
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@account", account);
                command.Parameters.AddWithValue("@settingType", settingType);
                command.Parameters.AddWithValue("@settingValue", settingValue);
                
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected == 1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("注册过程出错： " + ex.Message);
            return false;
        }
    }
}
