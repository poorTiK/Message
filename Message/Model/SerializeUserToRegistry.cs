using Message.Interfaces;
using Message.UserServiceReference;
using Microsoft.Win32;
using System;

namespace Message.Model
{
    public class SerializeUserToRegistry : ISerializeUser
    {
        public void SerializeUser(User user)
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey($"Software\\Message", true))
                {
                    key.SetValue("IsAutorize", true);
                    key.SetValue("Login", user.Login);
                    key.Close();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string GetCurrentUser()
        {
            try
            {
                string userLogin = null;
                using (var key = Registry.CurrentUser.CreateSubKey($"Software\\Message", true))
                {
                    var IsAutorize = Convert.ToBoolean(key.GetValue("IsAutorize", false));
                    if (IsAutorize)
                    {
                        userLogin = key.GetValue("Login", null) as string;
                    }
                    key.Close();
                    return userLogin;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void CleanCurrentUser()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey($"Software\\Message", true))
                {
                    key.DeleteValue("IsAutorize");
                    key.DeleteValue("Login");
                    key.Close();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}