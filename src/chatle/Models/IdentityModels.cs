using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Framework.OptionsModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Chat.CMACGM.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public bool IsConnected { get; set; }
        public string SignalRConnectionId { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Message> Messages { get; set; }

        public bool SetConnectionStatus(string userId, string connectionId, bool isConnected)
        {
            var user = this.Users.FirstOrDefault(x => x.UserName == userId);
            
            if (user != null)
            {                
                if (isConnected)
                {
                    var ret = !user.IsConnected;
                    SetConnectionStatus(true, connectionId, user);
                    return ret;
                }
                else if (user.SignalRConnectionId == connectionId)
                {                    
                    SetConnectionStatus(false, null, user);
                    return true;
                }                                
            }

            return false;
        }

        private void SetConnectionStatus(bool status, string connectionId, ApplicationUser user)
        {
            Trace.TraceInformation("[ApplicationDbContext] SetConnectionStatus {0} {1} {2}", user.UserName, connectionId, status);
            user.IsConnected = status;
            user.SignalRConnectionId = connectionId;
            this.SaveChanges();
        }
    }

    public class IdentityDbContextOptions : DbContextOptions
    {
        public bool CreateDatabase { get; set; }
    }
}