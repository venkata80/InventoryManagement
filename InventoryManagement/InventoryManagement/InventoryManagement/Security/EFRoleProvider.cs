using InventoryManagement.EF;
using InventoryManagement.Util;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Hosting;
using System.Web.Security;

namespace InventoryManagement.Security
{
    public class EFRoleProvider : RoleProvider
    {
        #region members
        private const string EVENTSOURCE = "EFRoleProvider";
        private const string EVENTLOG = "Application";
        private const string exceptionMessage = "An exception occurred. Please check the Event Log.";
        private string connectionString;
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets the name of the application to store and retrieve role information for.
        /// </summary>
        /// <returns>
        /// The name of the application to store and retrieve role information for.
        /// </returns>
        public override string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [write exceptions to event log].
        /// </summary>
        /// <value><c>true</c> if [write exceptions to event log]; otherwise, <c>false</c>.
        /// </value>
        public bool WriteExceptionsToEventLog { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// System.Configuration.Provider.ProviderBase.Initialize Method
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The name of the provider is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The name of the provider has a length of zero.
        /// </exception>
        /// <exception cref="T:System.InvalidOperationException">
        /// An attempt is made to call <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> on a provider after the provider has already been initialized.
        /// </exception>
        public override void Initialize(string name, NameValueCollection config)
        {
            // Initialize values from web.config.
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (String.IsNullOrEmpty(name))
            {
                name = "EFRoleProvider";
            }

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Smart-Soft EF Role Provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            ApplicationName = (string)ProviderUtils.GetConfigValue(config, "applicationName", HostingEnvironment.ApplicationVirtualPath);
            WriteExceptionsToEventLog = (bool)ProviderUtils.GetConfigValue(config, "writeExceptionsToEventLog", false);

            // Read connection string.
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[config["connectionStringName"]];

            if (connectionStringSettings == null || connectionStringSettings.ConnectionString.Trim() == string.Empty)
            {
                throw new ProviderException("Connection string cannot be blank.");
            }

            connectionString = connectionStringSettings.ConnectionString;
        }

        /// <summary>
        /// Gets a value indicating whether the specified user is in the specified role for the configured applicationName.
        /// </summary>
        /// <param name="username">The user name to search for.</param>
        /// <param name="roleName">The role to search in.</param>
        /// <returns>true if the specified user is in the specified role for the configured applicationName; otherwise, false.</returns>
        public override bool IsUserInRole(string username, string roleName)
        {
            try
            {
                using (InventoryManagementEntities context = new InventoryManagementEntities())
                {
                    if (!EFMembershipProvider.CheckUser(username, ApplicationName, context))
                    {
                        return false;
                    }

                    return (from u in context.aspnet_Membership
                            where u.aspnet_Users.UserName == username && u.aspnet_Applications.ApplicationName == ApplicationName
                            from r in u.aspnet_Users.aspnet_Roles
                            where r.RoleName == roleName && r.aspnet_Applications.ApplicationName == ApplicationName
                            select r).Count() > 0;
                }
            }
            catch (Exception ex)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(ex, "IsUserInRole");
                }

                throw;
            }
        }

        /// <summary>
        /// Gets a list of the roles that a specified user is in for the configured applicationName.
        /// </summary>
        /// <param name="username">The user to return a list of roles for.</param>
        /// <returns>A string array containing the names of all the roles that the specified user is in for the configured applicationName.</returns>
        public override string[] GetRolesForUser(string username)
        {
            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                if (!EFMembershipProvider.CheckUser(username, ApplicationName, context))
                {
                    throw new ArgumentNullException("username");
                }

                return (from u in context.aspnet_Membership
                        where u.aspnet_Users.UserName == username && u.aspnet_Applications.ApplicationName == ApplicationName
                        from r in u.aspnet_Users.aspnet_Roles
                        where r.aspnet_Applications.ApplicationName == ApplicationName
                        select r.RoleName).ToArray();
            }
        }

        /// <summary>
        /// Adds a new role to the data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to create.</param>
        public override void CreateRole(string roleName)
        {
            // Validate role name
            if (roleName.Contains(","))
            {
                throw new ArgumentException("Role names cannot contain commas.");
            }

            if (RoleExists(roleName))
            {
                throw new ProviderException("Role name already exists.");
            }

            try
            {
                using (InventoryManagementEntities context = new InventoryManagementEntities())
                {
                    aspnet_Applications application = ProviderUtils.EnsureApplication(ApplicationName, context);

                    // Create new role
                    aspnet_Roles newRole = new aspnet_Roles
                    {
                        RoleId = Guid.NewGuid(),
                        RoleName = roleName,
                        LoweredRoleName = roleName.ToLower(),
                        ApplicationId = application.ApplicationId
                    };
                    context.aspnet_Roles.Add(newRole);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(ex, "CreateRole");
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Removes a role from the data source for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to delete.</param>
        /// <param name="throwOnPopulatedRole">If true, throw an exception if <paramref name="roleName"/> has one or more members and do not delete <paramref name="roleName"/>.</param>
        /// <returns>
        /// true if the role was successfully deleted; otherwise, false.
        /// </returns>
        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            // Validate role
            if (!RoleExists(roleName))
            {
                throw new ProviderException("Role does not exist.");
            }

            if (throwOnPopulatedRole && GetUsersInRole(roleName).Length > 0)
            {
                throw new ProviderException("Cannot delete a populated role.");
            }

            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                aspnet_Roles role = GetRole(r => r.RoleName == roleName, context);
                if (role == null)
                {
                    return false;
                }

                try
                {
                    context.aspnet_Roles.Remove(role);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    if (WriteExceptionsToEventLog)
                    {
                        WriteToEventLog(ex, "DeleteRole");
                        return false;
                    }

                    throw;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified role name already exists in the role data source for the configured applicationName.
        /// </summary>
        /// <returns>true if the role name already exists in the data source for the configured applicationName; otherwise, false.</returns>
        /// <param name="roleName">The name of the role to search for in the data source.</param>
        public override bool RoleExists(string roleName)
        {
            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                try
                {
                    return GetRole(r => r.RoleName == roleName, context) != null;
                }
                catch (ProviderException)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Adds the specified user names to the specified roles for the configured applicationName.
        /// </summary>
        /// <param name="userNames">A string array of user names to be added to the specified roles.</param>
        /// <param name="roleNames">A string array of the role names to add the specified user names to.</param>
        public override void AddUsersToRoles(string[] userNames, string[] roleNames)
        {
            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                IQueryable<aspnet_Roles> roles = context.aspnet_Roles.Where(MatchRoleApplication()).Where(ProviderUtils.BuildContainsExpression<aspnet_Roles, string>(r => r.RoleName, roleNames));
                if (roles.Count() != roleNames.Length)
                {
                    throw new ProviderException("Role not found.");
                }

                IQueryable<aspnet_Membership> users = context.aspnet_Membership.Where(MatchUserApplication()).Where(ProviderUtils.BuildContainsExpression<aspnet_Membership, string>(u => u.aspnet_Users.UserName, userNames));
                if (users.Count() != userNames.Length)
                {
                    throw new ProviderException("User not found.");
                }

                try
                {
                    foreach (aspnet_Membership user in users)
                    {
                        foreach (aspnet_Roles role in roles)
                        {
                            // Check whether user is already in role
                            if (IsUserInRole(user.aspnet_Users.UserName, role.RoleName))
                            {
                                throw new ProviderException(string.Format("User is already in role '{0}'.", role.RoleName));
                            }

                            user.aspnet_Users.aspnet_Roles.Add(role);
                        }
                    }

                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    if (WriteExceptionsToEventLog)
                    {
                        WriteToEventLog(ex, "AddUsersToRoles");
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    //context..Connection.Close();
                }
            }
        }

        /// <summary>
        /// Removes the specified user names from the specified roles for the configured applicationName.
        /// </summary>
        /// <param name="userNames">A string array of user names to be removed from the specified roles.</param>
        /// <param name="roleNames">A string array of role names to remove the specified user names from.</param>
        public override void RemoveUsersFromRoles(string[] userNames, string[] roleNames)
        {
            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                IQueryable<aspnet_Roles> roles = context.aspnet_Roles.Where(MatchRoleApplication()).Where(ProviderUtils.BuildContainsExpression<aspnet_Roles, string>(r => r.RoleName, roleNames));
                if (roles.Count() != roleNames.Length)
                {
                    throw new ProviderException("Role not found.");
                }

                IQueryable<aspnet_Membership> users = context.aspnet_Membership.Include("Role").Where(MatchUserApplication()).Where(ProviderUtils.BuildContainsExpression<aspnet_Membership, string>(u => u.aspnet_Users.UserName, userNames));
                if (users.Count() != userNames.Length)
                {
                    throw new ProviderException("User not found.");
                }

                try
                {
                    foreach (aspnet_Membership user in users)
                    {
                        foreach (aspnet_Roles role in roles)
                        {
                            /*if (!user.Role.IsLoaded)
                            {
                                user.Role.Load();
                            }*/

                            if (user.aspnet_Users.aspnet_Roles.Contains(role))
                            {
                                user.aspnet_Users.aspnet_Roles.Remove(role);
                            }
                        }
                    }

                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    if (WriteExceptionsToEventLog)
                    {
                        WriteToEventLog(ex, "RemoveUsersFromRoles");
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a list of users in the specified role for the configured applicationName.
        /// </summary>
        /// <param name="roleName">The name of the role to get the list of users for.</param>
        /// <returns>
        /// A string array containing the names of all the users who are members of the specified role for the configured applicationName.
        /// </returns>
        public override string[] GetUsersInRole(string roleName)
        {
            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                aspnet_Roles role = GetRole(r => r.RoleName == roleName, context);
                if (role == null)
                {
                    throw new ProviderException("Role not found.");
                }

                return role.aspnet_Users.Select(u => u.UserName).ToArray();
            }
        }

        /// <summary>
        /// Gets a list of all the roles for the configured applicationName.
        /// </summary>
        /// <returns>A string array containing the names of all the roles stored in the data source for the configured applicationName.</returns>
        public override string[] GetAllRoles()
        {
            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                return context.aspnet_Roles.Where(MatchRoleApplication()).Select(r => r.RoleName).ToArray();
            }
        }

        /// <summary>
        /// Gets an array of user names in a role where the user name contains the specified user name to match.
        /// </summary>
        /// <returns>A string array containing the names of all the users where the user name matches <paramref name="usernameToMatch" /> 
        /// and the user is a member of the specified role.</returns>
        /// <param name="roleName">The role to search in.</param>
        /// <param name="usernameToMatch">The user name to search for.</param>
        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                aspnet_Roles role = GetRole(r => r.RoleName == roleName, context);
                if (role == null)
                {
                    throw new ProviderException("Role not found.");
                }

                
                return role.aspnet_Users.Where(u => u.aspnet_Membership.IsLockedOut != false).Select(u => u.UserName).Where(un => un.Contains(usernameToMatch)).ToArray();
            }
        }

        #endregion

        #region private methods
        /// <summary>
        /// Get role from database. Throws an error if the role could not be found.
        /// </summary>
        /// <param name="query">The role query.</param>
        /// <param name="context">The context.</param>
        /// <returns>Found role entity.</returns>
        private aspnet_Roles GetRole(Expression<Func<aspnet_Roles, bool>> query, InventoryManagementEntities context)
        {
            aspnet_Roles role = context.aspnet_Roles.Where(query).Where(MatchRoleApplication()).FirstOrDefault();
            if (role == null)
            {
                throw new ProviderException("The supplied role name could not be found.");
            }

            return role;
        }

        /// <summary>
        /// Matches the local application name.
        /// </summary>
        /// <returns>Status whether passed in user matches the application.</returns>
        private Expression<Func<aspnet_Roles, bool>> MatchRoleApplication()
        {
            return role => role.aspnet_Applications.ApplicationName == ApplicationName;
        }

        /// <summary>
        /// Matches the local application name.
        /// </summary>
        /// <returns>Status whether passed in user matches the application.</returns>
        private Expression<Func<aspnet_Membership, bool>> MatchUserApplication()
        {
            return user => user.aspnet_Applications.ApplicationName == ApplicationName;
        }

        /// <summary>
        /// Writes exception to event log.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="action">The action.</param>
        /// <remarks>A helper function that writes exception detail to the event log. Exceptions
        /// are written to the event log as a security measure to avoid private database
        /// details from being returned to the browser. If a method does not return a status
        /// or boolean indicating the action succeeded or failed, a generic exception is also
        /// thrown by the caller.</remarks>
        private static void WriteToEventLog(Exception exception, string action)
        {
            EventLog log = new EventLog { Source = EVENTSOURCE, Log = EVENTLOG };

            StringBuilder message = new StringBuilder();
            message.Append(exceptionMessage + "\n\n");
            message.Append("Action: " + action + "\n\n");
            message.Append("Exception: " + exception);

            log.WriteEntry(message.ToString());
        }
        #endregion
    }
}
