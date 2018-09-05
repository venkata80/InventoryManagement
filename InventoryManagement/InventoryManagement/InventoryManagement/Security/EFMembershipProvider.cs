using InventoryManagement.EF;
using InventoryManagement.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Security;

namespace InventoryManagement.Security
{
    public class EFMembershipProvider : MembershipProvider
    {
        #region members
        private const int NEWPASSWORDLENGTH = 8;
        private bool enablePasswordReset;
        private bool enablePasswordRetrieval;
        private MachineKeySection machineKey; // Used when determining encryption key values.
        private int maxInvalidPasswordAttempts;
        private int minRequiredNonAlphanumericCharacters;
        private int minRequiredPasswordLength;
        private int passwordAttemptWindow;
        private MembershipPasswordFormat passwordFormat;
        private string passwordStrengthRegularExpression;
        private bool requiresQuestionAndAnswer;
        private bool requiresUniqueEmail;
        #endregion

        #region properties
        /// <summary>
        /// Gets or sets a boolean that indicates whether the membership provider is configured to allow users to retrieve their passwords.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// true if the membership provider is configured to support password retrieval; otherwise, false. The default is false.
        /// </returns>
        public override bool EnablePasswordRetrieval
        {
            get { return enablePasswordRetrieval; }
        }

        /// <summary>
        /// Gets or sets a boolean that indicates whether the membership provider is configured to allow users to reset their passwords.
        /// </summary>
        /// <returns>
        /// true if the membership provider supports password reset; otherwise, false. The default is true.
        /// </returns>
        public override bool EnablePasswordReset
        {
            get { return enablePasswordReset; }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require the user to answer a password question for password reset and retrieval.
        /// </summary>
        /// <returns>
        /// true if a password answer is required for password reset and retrieval; otherwise, false. The default is true.
        /// </returns>
        public override bool RequiresQuestionAndAnswer
        {
            get { return requiresQuestionAndAnswer; }
        }

        /// <summary>
        /// Gets or sets the name of the application using the custom membership provider.
        /// </summary>
        /// <returns>
        /// The name of the application using the custom membership provider.
        /// </returns>
        public override string ApplicationName { get; set; }

        /// <summary>
        /// Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </summary>
        /// <returns>
        /// The number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </returns>
        public override int MaxInvalidPasswordAttempts
        {
            get { return maxInvalidPasswordAttempts; }
        }

        /// <summary>
        /// Gets the number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </summary>
        /// <returns>
        /// The number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </returns>
        public override int PasswordAttemptWindow
        {
            get { return passwordAttemptWindow; }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require a unique e-mail address for each user name.
        /// </summary>
        /// <returns>
        /// true if the membership provider requires a unique e-mail address; otherwise, false. The default is true.
        /// </returns>
        public override bool RequiresUniqueEmail
        {
            get { return requiresUniqueEmail; }
        }

        /// <summary>
        /// Gets a value indicating the format for storing passwords in the membership data store.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Web.Security.MembershipPasswordFormat" /> values indicating the format for storing passwords in the data store.
        /// </returns>
        public override MembershipPasswordFormat PasswordFormat
        {
            get { return passwordFormat; }
        }

        /// <summary>
        /// Gets the minimum length required for a password.
        /// </summary>
        /// <returns>
        /// The minimum length required for a password. 
        /// </returns>
        public override int MinRequiredPasswordLength
        {
            get { return minRequiredPasswordLength; }
        }

        /// <summary>
        /// Gets the minimum number of special characters that must be present in a valid password.
        /// </summary>
        /// <returns>
        /// The minimum number of special characters that must be present in a valid password.
        /// </returns>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return minRequiredNonAlphanumericCharacters; }
        }

        /// <summary>
        /// Gets the regular expression used to evaluate a password.
        /// </summary>
        /// <returns>
        /// A regular expression used to evaluate a password.
        /// </returns>
        public override string PasswordStrengthRegularExpression
        {
            get { return passwordStrengthRegularExpression; }
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString { get; set; }
        #endregion

        #region public methods
        /// <summary>
        /// Initialize this membership provider. Loads the configuration settings.
        /// </summary>
        /// <param name="name">Membership provider name</param>
        /// <param name="config">Configuration</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            MembershipSection membershipSection = (MembershipSection)WebConfigurationManager.GetSection("system.web/membership");
            config = membershipSection.Providers["SqlProvider"].Parameters;
            // Initialize values from web.config.
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (string.IsNullOrEmpty(name))
            {
                name = "EFMembershipProvider";
            }

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Smart-Soft EF Membership Provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            ApplicationName = Convert.ToString(ProviderUtils.GetConfigValue(config, "applicationName", HostingEnvironment.ApplicationVirtualPath));
            maxInvalidPasswordAttempts = Convert.ToInt32(ProviderUtils.GetConfigValue(config, "maxInvalidPasswordAttempts", "5"));
            passwordAttemptWindow = Convert.ToInt32(ProviderUtils.GetConfigValue(config, "passwordAttemptWindow", "10"));
            minRequiredNonAlphanumericCharacters = Convert.ToInt32(ProviderUtils.GetConfigValue(config, "minRequiredNonAlphanumericCharacters", "1"));
            minRequiredPasswordLength = Convert.ToInt32(ProviderUtils.GetConfigValue(config, "minRequiredPasswordLength", "7"));
            passwordStrengthRegularExpression = Convert.ToString(ProviderUtils.GetConfigValue(config, "passwordStrengthRegularExpression", string.Empty));
            enablePasswordReset = Convert.ToBoolean(ProviderUtils.GetConfigValue(config, "enablePasswordReset", "true"));
            enablePasswordRetrieval = Convert.ToBoolean(ProviderUtils.GetConfigValue(config, "enablePasswordRetrieval", "false"));
            requiresQuestionAndAnswer = Convert.ToBoolean(ProviderUtils.GetConfigValue(config, "requiresQuestionAndAnswer", "true"));
            requiresUniqueEmail = Convert.ToBoolean(ProviderUtils.GetConfigValue(config, "requiresUniqueEmail", "true"));

            if (!string.IsNullOrEmpty(passwordStrengthRegularExpression))
            {
                passwordStrengthRegularExpression = passwordStrengthRegularExpression.Trim();
                if (!string.IsNullOrEmpty(passwordStrengthRegularExpression))
                {
                    try
                    {
                        new Regex(passwordStrengthRegularExpression);
                    }
                    catch (ArgumentException ex)
                    {
                        throw new ProviderException(ex.Message, ex);
                    }
                }

                if (minRequiredPasswordLength < minRequiredNonAlphanumericCharacters)
                {
                    throw new ProviderException("Minimal required non alphanumeric characters cannot be longer than the minimum required password length.");
                }
            }

            string temp_format = config["passwordFormat"] ?? "Hashed";

            switch (temp_format)
            {
                case "Hashed":
                    passwordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    passwordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    passwordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }

            // Initialize SqlConnection.
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[config["connectionStringName"]];
            if (connectionStringSettings == null || connectionStringSettings.ConnectionString.Trim() == string.Empty)
            {
                throw new ProviderException("Connection string cannot be blank.");
            }

            ConnectionString = connectionStringSettings.ConnectionString;

            // Get encryption and decryption key information from the configuration.
            Configuration configuration = WebConfigurationManager.OpenWebConfiguration(HostingEnvironment.ApplicationVirtualPath);
            machineKey = (MachineKeySection)configuration.GetSection("system.web/machineKey");

            if (machineKey.ValidationKey.Contains("AutoGenerate"))
            {
                if (PasswordFormat != MembershipPasswordFormat.Clear)
                {
                    throw new ProviderException("Hashed or Encrypted passwords are not supported with auto-generated keys.");
                }
            }
        }

        /// <summary>
        /// Adds a new membership user to the data source.
        /// </summary>
        /// <returns>A <see cref="T:System.Web.Security.MembershipUser" /> object populated with the information for the newly created user.</returns>
        /// <param name="username">The user name for the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="email">The e-mail address for the new user.</param>
        /// <param name="passwordQuestion">The password question for the new user.</param>
        /// <param name="passwordAnswer">The password answer for the new user</param>
        /// <param name="isApproved">Whether or not the new user is approved to be validated.</param>
        /// <param name="providerUserKey">The unique identifier from the membership data source for the user.</param>
        /// <param name="status">A <see cref="T:System.Web.Security.MembershipCreateStatus" /> enumeration value indicating whether the user was created successfully.</param>
        public override MembershipUser CreateUser(
                                                  string username,
                                                  string password,
                                                  string email,
                                                  string passwordQuestion,
                                                  string passwordAnswer,
                                                  bool isApproved,
                                                  object providerUserKey,
                                                  out MembershipCreateStatus status)
        {
            // Validate username/password
            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && GetUserNameByEmail(email) != string.Empty)
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            // Check whether user with passed username already exists
            MembershipUser user;
            try
            {
                user = GetUser(username, false);
            }
            catch (ProviderException)
            {
                user = null;
            }

            if (user == null)
            {
                DateTime creationDate = DateTime.Now;

                if (providerUserKey == null)
                {
                    providerUserKey = Guid.NewGuid();
                }
                else
                {
                    if (!(providerUserKey is Guid))
                    {
                        status = MembershipCreateStatus.InvalidProviderUserKey;
                        return null;
                    }
                }

                using (InventoryManagementEntities context = new InventoryManagementEntities())
                {
                    string passwordsalt = GenerateSalt();
                    context.aspnet_Membership.Add(new aspnet_Membership
                    {
                        UserId = (Guid)providerUserKey,
                        //Username = username,
                        Email = email,
                        IsApproved = isApproved,
                        CreateDate = creationDate,
                        //IsAnonymous = false,
                        ApplicationId = ProviderUtils.EnsureApplication(ApplicationName, context).ApplicationId,
                        Password = EncodePassword(password),
                        PasswordSalt = passwordsalt,
                        PasswordFormat = (int)MembershipPasswordFormat.Encrypted,
                        PasswordQuestion = passwordQuestion,
                        PasswordAnswer = passwordAnswer,
                        LastLoginDate = creationDate,
                        LastPasswordChangedDate = creationDate,
                        //LastActivityDate = creationDate,
                        //IsOnline = false,
                        IsLockedOut = false,
                        LastLockoutDate = Convert.ToDateTime("1754-01-01"),
                        FailedPasswordAttemptCount = 0,
                        FailedPasswordAttemptWindowStart = Convert.ToDateTime("1754-01-01"),
                        FailedPasswordAnswerAttemptCount = 0,
                        FailedPasswordAnswerAttemptWindowStart = Convert.ToDateTime("1754-01-01")
                    });

                    try
                    {
                        //context.SaveChanges();
                        context.aspnet_Users.Add(new aspnet_Users
                        {
                            ApplicationId = ProviderUtils.EnsureApplication(ApplicationName, context).ApplicationId,
                            UserId = (Guid)providerUserKey,
                            UserName = email,
                            LoweredUserName = email.ToLower(),
                            IsAnonymous = false,
                            LastActivityDate = creationDate
                        });
                        context.SaveChanges();
                        status = MembershipCreateStatus.Success;
                    }
                    catch(Exception ex)
                    {
                        status = MembershipCreateStatus.UserRejected;
                    }
                }

                return GetUser(username, false);
            }

            status = MembershipCreateStatus.DuplicateUserName;

            return null;
        }

        /// <summary>
        /// Processes a request to update the password question and answer for a membership user.
        /// </summary>
        /// <returns>true if the password question and answer are updated successfully; otherwise, false.</returns>
        /// <param name="username">The user to change the password question and answer for.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <param name="newPasswordQuestion">The new password question for the specified user.</param>
        /// <param name="newPasswordAnswer">The new password answer for the specified user.</param>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password,
                                                             string newPasswordQuestion, string newPasswordAnswer)
        {
            // Check if user is authenticated
            if (!ValidateUser(username, password))
            {
                return false;
            }

            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                aspnet_Membership user = GetUser(u => u.Email == username, context);
                user.PasswordAnswer = EncodePassword(newPasswordAnswer);
                user.PasswordQuestion = newPasswordQuestion;

                try
                {
                    context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the password for the specified user name from the data source.
        /// </summary>
        /// <returns>The password for the specified user name.</returns>
        /// <param name="username">The user to retrieve the password for.</param>
        /// <param name="answer">The password answer for the user.</param>
        public override string GetPassword(string username, string answer)
        {
            if (!EnablePasswordRetrieval)
            {
                throw new ProviderException("Password Retrieval Not Enabled.");
            }

            if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                throw new ProviderException("Cannot retrieve Hashed passwords.");
            }

            string password = string.Empty;
            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                aspnet_Membership user = GetUser(u => u.Email == username, context);

                // Check whether the user is locked out
                if (Convert.ToBoolean(user.IsLockedOut))
                {
                    throw new MembershipPasswordException("The supplied user is locked out.");
                }

                if (RequiresQuestionAndAnswer && !CheckPassword(answer, user.PasswordAnswer))
                {
                    UpdateFailureCount(username, "passwordAnswer");
                    throw new MembershipPasswordException("Incorrect password answer.");
                }

                if (PasswordFormat == MembershipPasswordFormat.Encrypted)
                {
                    password = UnEncodePassword(user.Password);
                }
            }

            return password;
        }

        /// <summary>
        /// Processes a request to update the password for a membership user.
        /// </summary>
        /// <returns>true if the password was updated successfully; otherwise, false.</returns>
        /// <param name="username">The user to update the password for.</param>
        /// <param name="oldPassword">The current password for the specified user.</param>
        /// <param name="newPassword">The new password for the specified user.</param>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            // Check if user is authenticated
            if (!ValidateUser(username, oldPassword))
            {
                return false;
            }

            // Notify that password is going to change
            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPassword, true);
            OnValidatingPassword(args);

            if (args.Cancel)
            {
                if (args.FailureInformation != null)
                {
                    throw args.FailureInformation;
                }

                throw new MembershipPasswordException("Change password canceled due to new password validation failure.");
            }

            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                aspnet_Membership user = GetUser(u => u.Email == username, context);
                user.Password = EncodePassword(newPassword);
                user.LastPasswordChangedDate = DateTime.Now;

                try
                {
                    context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Resets a user's password to a new, automatically generated password.
        /// </summary>
        /// <returns>The new password for the specified user.</returns>
        /// <param name="username">The user to reset the password for.</param>
        /// <param name="answer">The password answer for the specified user.</param>
        public override string ResetPassword(string username, string answer)
        {
            if (!EnablePasswordReset)
            {
                throw new NotSupportedException("Password reset is not enabled.");
            }

            if (answer == null && RequiresQuestionAndAnswer)
            {
                UpdateFailureCount(username, "passwordAnswer");
                throw new ProviderException("Password answer required for password reset.");
            }

            string newPassword = Membership.GeneratePassword(NEWPASSWORDLENGTH, MinRequiredNonAlphanumericCharacters);

            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPassword, true);
            OnValidatingPassword(args);

            if (args.Cancel)
            {
                if (args.FailureInformation != null)
                {
                    throw args.FailureInformation;
                }

                throw new MembershipPasswordException("Reset password canceled due to password validation failure.");
            }

            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                aspnet_Membership user = GetUser(u => u.Email == username, context);

                if (Convert.ToBoolean(user.IsLockedOut))
                {
                    throw new MembershipPasswordException("The supplied user is locked out.");
                }

                if (RequiresQuestionAndAnswer && !CheckPassword(answer, user.PasswordAnswer))
                {
                    UpdateFailureCount(username, "passwordAnswer");
                    throw new MembershipPasswordException("Incorrect password answer.");
                }

                try
                {
                    user.Password = EncodePassword(newPassword);
                    user.LastPasswordChangedDate = DateTime.Now;

                    context.SaveChanges();
                    return newPassword;
                }
                catch
                {
                    throw new MembershipPasswordException("User not found, or user is locked out. Password not Reset.");
                }
            }
        }

        /// <summary>
        /// Updates information about a user in the data source.
        /// </summary>
        /// <param name="membershipUser">A <see cref="T:System.Web.Security.MembershipUser" /> object that represents the user to update and the updated information for the user.</param>
        public override void UpdateUser(MembershipUser membershipUser)
        {
            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                aspnet_Membership user = GetUser(u => u.Email == membershipUser.UserName, context);
                user.Email = membershipUser.Email;
                user.Comment = membershipUser.Comment;
                user.IsApproved = membershipUser.IsApproved;
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Verifies that the specified user name and password exist in the data source.
        /// </summary>
        /// <returns>true if the specified username and password are valid; otherwise, false.</returns>
        /// <param name="username">The name of the user to validate.</param>
        /// <param name="password">The password for the specified user.</param>
        public override bool ValidateUser(string username, string password)
        {
            bool isValid = false;

            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                aspnet_Membership user;
                try
                {
                    user = GetUser(u => u.Email == username, context);
                    if (user == null)
                    {
                        return false;
                    }
                }
                catch (ProviderException)
                {
                    return false;
                }

                if (CheckPassword(password, user.Password))
                {
                    if (user.IsApproved && !user.IsLockedOut)
                    {
                        isValid = true;

                        user.LastLoginDate = DateTime.Now;
                        user.aspnet_Users.LastActivityDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }
                else
                {
                    UpdateFailureCount(username, "password");
                }

                return isValid;
            }
        }

        /// <summary>
        ///  Clears a lock so that the membership user can be validated.
        /// </summary>
        /// <returns>true if the membership user was successfully unlocked; otherwise, false.</returns>
        /// <param name="username">The membership user whose lock status you want to clear.</param>
        public override bool UnlockUser(string username)
        {
            try
            {
                using (InventoryManagementEntities context = new InventoryManagementEntities())
                {
                    aspnet_Membership user;
                    try
                    {
                        user = GetUser(u => u.Email == username, context);
                        if (user == null)
                        {
                            return false;
                        }
                    }
                    catch (ProviderException)
                    {
                        return false;
                    }

                    user.LastLockoutDate = DateTime.Now;
                    context.SaveChanges();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets user information from the data source based on the unique identifier for the membership user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <returns>A <see cref="T:System.Web.Security.MembershipUser" /> object populated with the specified user's information from the data source.</returns>
        /// <param name="providerUserKey">The unique identifier for the membership user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                MembershipUser membershipUser = null;
                aspnet_Membership user;
                try
                {
                    Guid userKey = (Guid)providerUserKey;
                    user = GetUser(u => u.UserId == userKey, context);
                }
                catch (ProviderException)
                {
                    user = null;
                }

                if (user != null)
                {
                    membershipUser = GetMembershipUserFromPersistedEntity(user);

                    if (userIsOnline)
                    {
                        user.aspnet_Users.LastActivityDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }

                return membershipUser;
            }
        }

        /// <summary>
        /// Gets information from the data source for a user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <returns>A <see cref="T:System.Web.Security.MembershipUser" /> object populated with the specified user's information from the data source.</returns>
        /// <param name="username">The name of the user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                MembershipUser membershipUser = null;
                aspnet_Membership user;
                try
                {
                    user = GetUser(u => u.Email == username, context);
                }
                catch (ProviderException)
                {
                    user = null;
                }

                if (user != null)
                {
                    membershipUser = GetMembershipUserFromPersistedEntity(user);

                    if (userIsOnline)
                    {
                        user.aspnet_Users.LastActivityDate = DateTime.Now;
                        context.SaveChanges();
                    }
                }

                return membershipUser;
            }
        }

        /// <summary>
        /// Gets the user name associated with the specified e-mail address.
        /// </summary>
        /// <returns>The user name associated with the specified e-mail address. If no match is found, return null.</returns>
        /// <param name="email">The e-mail address to search for.</param>
        public override string GetUserNameByEmail(string email)
        {
            try
            {
                using (InventoryManagementEntities context = new InventoryManagementEntities())
                {
                    aspnet_Membership user = GetUser(u => u.Email == email, context);
                    return user == null ? string.Empty : user.aspnet_Users.UserName;
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Removes a user from the membership data source. 
        /// </summary>
        /// <returns>true if the user was successfully deleted; otherwise, false.</returns>
        /// <param name="username">The name of the user to delete.</param>
        /// <param name="deleteAllRelatedData">true to delete data related to the user from the database; false to leave data related to the user in the database.</param>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            try
            {
                using (InventoryManagementEntities context = new InventoryManagementEntities())
                {
                    aspnet_Membership user;
                    try
                    {
                        user = GetUser(u => u.Email == username, context);
                        if (user == null)
                        {
                            return false;
                        }
                    }
                    catch (ProviderException)
                    {
                        return false;
                    }

                    context.aspnet_Membership.Remove(user);
                    context.SaveChanges();

                    if (deleteAllRelatedData)
                    {
                        // TODO: delete user related data
                    }

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a collection of all the users in the data source in pages of data.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection" /> collection that contains a page of <paramref name="pageSize" /><see cref="T:System.Web.Security.MembershipUser" /> objects beginning at the page specified by <paramref name="pageIndex" />.
        /// </returns>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex" /> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();

            // Retrieve all users for the current application name from the database
            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                totalRecords = context.aspnet_Membership.Where(MatchApplication()).Distinct().Count();
                if (totalRecords <= 0)
                {
                    return users;
                }

                IEnumerable<aspnet_Membership> userEntities = context.aspnet_Membership.Where(MatchApplication()).OrderBy(u => u.aspnet_Users.UserName).Skip(pageIndex * pageSize).Take(pageSize);
                foreach (aspnet_Membership user in userEntities)
                {
                    users.Add(GetMembershipUserFromPersistedEntity(user));
                }

                return users;
            }
        }

        /// <summary>
        /// Gets the number of users currently accessing the application.
        /// </summary>
        /// <returns>The number of users currently accessing the application.</returns>
        public override int GetNumberOfUsersOnline()
        {
            TimeSpan onlineSpan = new TimeSpan(0, Membership.UserIsOnlineTimeWindow, 0);
            DateTime compareTime = DateTime.Now.Subtract(onlineSpan);

            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                return context.aspnet_Membership.Where(MatchApplication()).Where(u => u.aspnet_Users.LastActivityDate > compareTime).Distinct().Count();
            }
        }

        /// <summary>
        /// Gets a collection of membership users where the user name contains the specified user name to match.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection" /> collection that contains a page of <paramref name="pageSize" /><see cref="T:System.Web.Security.MembershipUser" /> objects beginning at the page specified by <paramref name="pageIndex" />.
        /// </returns>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex" /> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection membershipUsers = new MembershipUserCollection();
            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                IOrderedQueryable<aspnet_Membership> users = context.aspnet_Membership.Where(MatchApplication()).Where(u => u.aspnet_Users.UserName.Contains(usernameToMatch)).OrderBy(u => u.aspnet_Users.UserName);
                totalRecords = users.Count();
                if (users.Count() > 0)
                {
                    foreach (aspnet_Membership user in users.Skip(pageIndex * pageSize).Take(pageSize))
                    {
                        membershipUsers.Add(GetMembershipUserFromPersistedEntity(user));
                    }
                }

                return membershipUsers;
            }
        }

        /// <summary>
        /// Gets a collection of membership users where the e-mail address contains the specified e-mail address to match.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection" /> collection that contains a page of <paramref name="pageSize" /><see cref="T:System.Web.Security.MembershipUser" /> objects beginning at the page specified by <paramref name="pageIndex" />.
        /// </returns>
        /// <param name="emailToMatch">The e-mail address to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex" /> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection membershipUsers = new MembershipUserCollection();
            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                IOrderedQueryable<aspnet_Membership> users = context.aspnet_Membership.Where(MatchApplication()).Where(u => u.Email.Contains(emailToMatch)).OrderBy(u => u.aspnet_Users.UserName);
                totalRecords = users.Count();
                if (users.Count() > 0)
                {
                    foreach (aspnet_Membership user in users.Skip(pageIndex * pageSize).Take(pageSize))
                    {
                        membershipUsers.Add(GetMembershipUserFromPersistedEntity(user));
                    }
                }

                return membershipUsers;
            }
        }

        /// <summary>
        /// Checks the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="applicationName">Name of the application.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static bool CheckUser(string username, string applicationName, InventoryManagementEntities context)
        {
            aspnet_Membership user = context.aspnet_Membership.Where(u => u.aspnet_Users.UserName == username && u.aspnet_Applications.ApplicationName == applicationName).FirstOrDefault();
            if (user == null)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region private methods
        /// <summary>
        /// A helper function that takes the current persistent user and creates a MembershiUser from the values.
        /// </summary>
        /// <param name="user">User object containing the user data retrieved from database</param>
        /// <returns>Membership user object</returns>
        private MembershipUser GetMembershipUserFromPersistedEntity(aspnet_Membership user)
        {
            return new MembershipUser(
                                      Name,
                                      user.Email,
                                      user.UserId,
                                      user.Email,
                                      user.PasswordQuestion,
                                      user.Comment,
                                      user.IsApproved,
                                      Convert.ToBoolean(user.IsLockedOut),
                                      Convert.ToDateTime(user.CreateDate),
                                      Convert.ToDateTime(user.LastLoginDate),
                                      Convert.ToDateTime(user.LastLoginDate),
                                      Convert.ToDateTime(user.LastPasswordChangedDate),
                                      Convert.ToDateTime(user.LastLockoutDate));
        }

        /// <summary>
        /// Get user from database. Throws an error if the user could not be found.
        /// </summary>
        /// <param name="query">The user query.</param>
        /// <param name="context">The context.</param>
        /// <returns>Found user entity.</returns>
        private aspnet_Membership GetUser(Expression<Func<aspnet_Membership, bool>> query, InventoryManagementEntities context)
        {
            aspnet_Membership user = context.aspnet_Membership.Where(query).Where(MatchApplication()).FirstOrDefault();
            if (user == null)
            {
                throw new ProviderException("The supplied user name could not be found.");
            }

            return user;
        }

        /// <summary>
        /// Matches the local application name.
        /// </summary>
        /// <returns>Status whether passed in user matches the application.</returns>
        private Expression<Func<aspnet_Membership, bool>> MatchApplication()
        {
            return user => user.aspnet_Applications.ApplicationName == ApplicationName;
        }

        /// <summary>
        /// A helper method that performs the checks and updates associated with password failure tracking.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="failureType">Type of the failure.</param>
        private void UpdateFailureCount(string username, string failureType)
        {
            using (InventoryManagementEntities context = new InventoryManagementEntities())
            {
                aspnet_Membership user = GetUser(u => u.Email == username, context);

                DateTime windowStart = new DateTime();
                int failureCount = 0;

                if (failureType == "password")
                {
                    failureCount = Convert.ToInt32(user.FailedPasswordAttemptCount);
                    windowStart = Convert.ToDateTime(user.FailedPasswordAttemptWindowStart);
                }

                if (failureType == "passwordAnswer")
                {
                    failureCount = Convert.ToInt32(user.FailedPasswordAnswerAttemptCount);
                    windowStart = Convert.ToDateTime(user.FailedPasswordAnswerAttemptWindowStart);
                }

                DateTime windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);

                if (failureCount == 0 || DateTime.Now > windowEnd)
                {
                    // First password failure or outside of PasswordAttemptWindow. 
                    // Start a new password failure count from 1 and a new window starting now.
                    if (failureType == "password")
                    {
                        user.FailedPasswordAttemptCount = 1;
                        user.FailedPasswordAttemptWindowStart = DateTime.Now;
                    }

                    if (failureType == "passwordAnswer")
                    {
                        user.FailedPasswordAnswerAttemptCount = 1;
                        user.FailedPasswordAnswerAttemptWindowStart = DateTime.Now;
                    }

                    try
                    {
                        context.SaveChanges();
                    }
                    catch
                    {
                        throw new ProviderException("Unable to update failure count and window start.");
                    }
                }
                else
                {
                    if (failureCount++ >= MaxInvalidPasswordAttempts)
                    {
                        // Max password attempts have exceeded the failure threshold. Lock out the user.
                        user.IsLockedOut = true;
                        //user.LastLockedOutDate = DateTime.Now;

                        try
                        {
                            context.SaveChanges();
                        }
                        catch
                        {
                            throw new ProviderException("Unable to lock out user.");
                        }
                    }
                    else
                    {
                        // Max password attempts have not exceeded the failure threshold. Update
                        // the failure counts. Leave the window the same.
                        if (failureType == "password")
                        {
                            user.FailedPasswordAttemptCount = failureCount;
                        }

                        if (failureType == "passwordAnswer")
                        {
                            user.FailedPasswordAnswerAttemptCount = failureCount;
                        }

                        try
                        {
                            context.SaveChanges();
                        }
                        catch
                        {
                            throw new ProviderException("Unable to update failure count.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Compares password values based on the MembershipPasswordFormat.
        /// </summary>
        /// <param name="password">password</param>
        /// <param name="dbpassword">database password</param>
        /// <returns>whether the passwords are identical</returns>
        private bool CheckPassword(string password, string dbpassword)
        {
            string pass1 = password;
            string pass2 = dbpassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = UnEncodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass1 = EncodePassword(password);
                    break;
                default:
                    break;
            }

            return pass1 == pass2;
        }

        /// <summary>
        /// Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private string EncodePassword(string password)
        {
            string encodedPassword = password;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword = Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    HMACSHA1 hash = new HMACSHA1 { Key = HexToByte(machineKey.ValidationKey) };
                    encodedPassword = Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                    break;
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return encodedPassword;
        }

        /// <summary>
        /// Decrypts or leaves the password clear based on the PasswordFormat.
        /// </summary>
        /// <param name="encodedPassword"></param>
        /// <returns></returns>
        private string UnEncodePassword(string encodedPassword)
        {
            string password = encodedPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password = Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Cannot unencode a hashed password.");
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return password;
        }

        /// <summary>
        /// Converts a hexadecimal string to a byte array. Used to convert encryption key values from the configuration.
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private static byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return returnBytes;
        }

        private string GenerateSalt()
        {
            byte[] buf = new byte[16];
            (new RNGCryptoServiceProvider()).GetBytes(buf);
            return Convert.ToBase64String(buf);
        }

        //internal string EncodePassword(string pass, int passwordFormat, string salt)
        //{
        //    if (passwordFormat == 0) // MembershipPasswordFormat.Clear
        //        return pass;

        //    byte[] bIn = Encoding.Unicode.GetBytes(pass);
        //    byte[] bSalt = Convert.FromBase64String(salt);
        //    byte[] bAll = new byte[bSalt.Length + bIn.Length];
        //    byte[] bRet = null;

        //    Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
        //    Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);
        //    if (passwordFormat == 1)
        //    { // MembershipPasswordFormat.Hashed
        //        HashAlgorithm s = HashAlgorithm.Create("SHA1");
        //        // Hardcoded "SHA1" instead of Membership.HashAlgorithmType
        //        bRet = s.ComputeHash(bAll);
        //    }
        //    else
        //    {
        //        bRet = EncryptPassword(bAll);
        //    }
        //    return Convert.ToBase64String(bRet);
        //}
        #endregion
    }
}
