using ServerWCF.Context;
using ServerWCF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ServerWCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Server : IServer
    {
        public bool AddNewUser(User value)
        {
            using (UserContext db = new UserContext())
            {
                try
                {
                    ApplicationSettingsContext applicationSettingsContext = new ApplicationSettingsContext();
                    User admin = new User("admin", "admin", "admin"); /*{ , Password = "admin", ShownName = "admin", Bio = "admin", Email = "admin", Phone = "99999999" };*/
                    ApplicationSettings applicationSettings = new ApplicationSettings(100, 1);
                    applicationSettings.UserFK = admin.LoginId;
                    db.Users.Add(value);
                    applicationSettingsContext.ApplicationSettings.Add(applicationSettings);


                    db.SaveChangesAsync();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                    throw;
                }
            }
        }

        //public CompositeType GetDataUsingDataContract(CompositeType composite)
        //{
        //    if (composite == null)
        //    {
        //        throw new ArgumentNullException("composite");
        //    }
        //    if (composite.BoolValue)
        //    {
        //        composite.StringValue += "Suffix";
        //    }
        //    return composite;
        //}
    }
}
