using System;
using System.Data.Entity.Migrations;
using System.Linq;
using ServerWCF.Context;
using ServerWCF.Contracts;

namespace ServerWCF.Services
{
    public class PhotoService : IPhotoService
    {
        public byte[] GetPhotoById(int id)
        {
            using (UserContext db = new UserContext())
            {
                return db.Users
                    .FirstOrDefault(x => x.Id == id)
                    ?.Avatar;
            }
        }

        public void SetPhotoById(int id, byte[] photoBytes)
        {
            using (UserContext db = new UserContext())
            {
                var user = db.Users
                    .FirstOrDefault(x => x.Id == id);

                user.Avatar = photoBytes;
                db.Users.AddOrUpdate(user);
                db.SaveChanges();
            }
        }
    }
}
