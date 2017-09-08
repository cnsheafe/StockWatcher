using System;
using System.Linq;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using Twilio;
using Twilio.Types;
using Twilio.Rest.Notify.V1.Service;

using StockWatcher.Model;
using StockWatcher.Model.Schemas;


namespace StockWatcher.Model.Services
{
    public class ManageUser
    {
        private StockDbContext context;
        private IDataProtector passwordProtector;
        private IDataProtector phoneProtector;
        public ManageUser(StockDbContext _StockDbContext, IDataProtectionProvider provider)
        {
            context = _StockDbContext;
            passwordProtector = provider.CreateProtector("password");
            phoneProtector = provider.CreateProtector("phone");
        }
        public bool AddUser(User user)
        {
            bool success = true;
            try
            {
                user.Password = passwordProtector.Protect(user.Password);
                context.Users.Add(user);
                context.SaveChanges();
            }
            catch (DbUpdateException dbException)
            {
                var exception = (Npgsql.PostgresException)dbException.InnerException;
                string SqlCode = exception.SqlState;
                Console.WriteLine(SqlCode);
                success = string.CompareOrdinal(SqlCode, 1, "01P01", 1, length: 1) < 0;
            }
            finally
            {
                if (success)
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

        public bool LoginUser(Login login)
        {
            bool success = false;
            try
            {
                var username = context.Users
                    .Single(u => (u.Username == login.Username) && (passwordProtector.Unprotect(u.Password) == login.Password))
                    .Username;
                success = true;
            }
            catch (DbUpdateException dbException)
            {
                var exception = (Npgsql.PostgresException)dbException.InnerException;
                string SqlCode = exception.SqlState;
                Console.WriteLine(SqlCode);
                success = string.CompareOrdinal(SqlCode, 1, "01P01", 1, length: 1) < 0;
            }
            return success;
        }

        public bool RemoveUser(User user)
        {
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
                var exception = (Npgsql.PostgresException)dbException.InnerException;
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