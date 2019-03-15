﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerWCF.Context;
using ServerWCF.Encryption;
using ServerWCF.Model;

namespace ServerWCF.Initializers
{
    class UserDBInitializer : DropCreateDatabaseIfModelChanges<UserContext>
    {
        protected override void Seed(UserContext context)
        {
            List<User> defUsers = new List<User>();

            defUsers.Add(new User() {FirstName = "John", LastName = "Snow", Login = "johnsnow1", Email = "johnsnow1@gmail.com", Password = AESEncryptor.encryptPassword("123123aa"), Status = DateTime.Now.ToString()});
            defUsers.Add(new User() {FirstName = "Bill", LastName = "Gates", Login = "billgates1", Email = "billgates1@gmail.com", Password = AESEncryptor.encryptPassword("123123aa"), Status = DateTime.Now.ToString()});
            defUsers.Add(new User() {FirstName = "Harry", LastName = "Potter", Login = "harrypotter1", Email = "harrypotter1@gmail.com", Password = AESEncryptor.encryptPassword("123123aa"), Status = DateTime.Now.ToString()});
            defUsers.Add(new User() {FirstName = "Duke", LastName = "Nukem", Login = "dukenukem1", Email = "dukenukem1@gmail.com", Password = AESEncryptor.encryptPassword("123123aa"), Status = DateTime.Now.ToString()});
            defUsers.Add(new User() {FirstName = "Will", LastName = "Bedone", Login = "willbedone1", Email = "willbedone1@gmail.com", Password = AESEncryptor.encryptPassword("123123aa"), Status = DateTime.Now.ToString()});

            context.Users.AddRange(defUsers);

            base.Seed(context);
        }
    }
}