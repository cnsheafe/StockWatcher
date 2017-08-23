using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using Twilio;
using Twilio.Types;
using Twilio.Rest.Notify.V1.Service;

using StockWatcher.Model;
using StockWatcher.Model.Schemas;


namespace StockWatcher.Model.Services 
{
    public class ManageUser {
        private StockDbContext context;
        public ManageUser(StockDbContext _StockDbContext) {
            context = _StockDbContext;
        }
        public bool AddUser(User user) {
            bool success = true;
                try
                {
                    context.Users.Add(user);
                    context.SaveChanges();
                }
                catch (DbUpdateException dbException)
                {
                    var exception = (Npgsql.PostgresException) dbException.InnerException;
                    string SqlCode = exception.SqlState;
                    Console.WriteLine(SqlCode);
                    success = string.CompareOrdinal(SqlCode, 1, "01P01", 1, length: 1) < 0;
                }
                finally
                {
                    if(success)
                    {
                        // Gets the matching User for binding
                        User selectUser = context.Users
                            .Where(u => u.Username == user.Username)
                            .Single();
                        BindUser(selectUser);
                    }
                }
            return success;
        }

        public bool RemoveUser(User user) {
            bool success = true;
                try
                {
                    User selectedUser = context.Users
                        .Single(u => u.Username == user.Username);
                    context.Remove(selectedUser);
                    context.SaveChanges();

                }
                catch (DbUpdateException dbException)
                {
                    var exception = (Npgsql.PostgresException) dbException.InnerException;
                    Console.WriteLine(exception.SqlState);
                    success = false;
                }

           return success;
        }

        /// <summary>
        /// Registers user phone number to a UUID on Twilio
        /// </summary>
        /// <param name="user">
        /// User credentials for binding
        /// </param>
        private BindingResource BindUser(User user) 
        {
            TwilioClient.Init(
                Environment.GetEnvironmentVariable("TwilioAcctSid"),
                Environment.GetEnvironmentVariable("TwilioAuthToken")
            );
            var binding = BindingResource.Create(
                Environment.GetEnvironmentVariable("TwilioServiceSid"),
                identity: user.Uuid,
                bindingType: BindingResource.BindingTypeEnum.Sms,
                address: user.Phone
            );
            return binding;
        }
    }
}