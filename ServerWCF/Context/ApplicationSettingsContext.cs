using ServerWCF.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWCF.Context
{
    public class ApplicationSettingsContext : DbContext
    {
        public ApplicationSettingsContext() : base("DbConnection") { }

        public DbSet<ApplicationSettings> ApplicationSettings { get; set; }
    }
}
