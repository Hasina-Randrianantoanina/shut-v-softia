using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SHUT.Core.Data
{
    public class LDapIdentifier
    {
        private readonly LDapSettings _ldapSettings;

        const string LdapAdminCn = "Softia Softia";
        const string LdapAdminPassword = "Azerty1234";


        private readonly ILogger<LDapIdentifier> _logger;
        public LDapIdentifier(IOptions<LDapSettings> ldapSettings, ILogger<LDapIdentifier> logger)
        {
            _ldapSettings = ldapSettings.Value;
            _logger = logger;
        }

        public Dictionary<string, string> GetLDAPDataIdentifiers(string mail)
        {
            Dictionary<string, string> LdapData = new();
            using (LdapConnection connection = new LdapConnection())
            {
                try
                {
                    _logger.LogInformation($"Tentative de connexion LDAP pour l'utilisateur: {mail}");
                    string ldapusername = $"cn={LdapAdminCn},cn=Users," + _ldapSettings.LDapBaseDn;
                    // string ldapusername = $"cn={username},{_ldapSettings.LDapBaseDn}";
                    _logger.LogInformation($"DN utilisé: {ldapusername}");

                    connection.Connect(_ldapSettings.LDapHost, _ldapSettings.LDapPort);
                    _logger.LogInformation($"Connexion LDAP établie. Tentative de bind pour: {ldapusername}");
                    connection.Bind(ldapusername, LdapAdminPassword);
                    _logger.LogInformation("Bind LDAP réussi");
                    string searchBase = "cn=Users," + _ldapSettings.LDapBaseDn;
                    //string testsn = username;
                    string[] attrs = { "cn", "mail" };
                    string searchFilter = $"(mail={mail})";
                    var response = connection.Search(
                        searchBase,
                        LdapConnection.ScopeOne,
                        searchFilter,
                        attrs,
                        false,
                        (LdapSearchQueue)null,
                        (LdapSearchConstraints)null
                    );
                    LdapMessage message = response.GetResponse();
                    while (message != null)
                    {
                        if (message is LdapSearchResult)
                        {
                            LdapSearchResult result = (LdapSearchResult)message;
                            LdapEntry entry = result.Entry;
                            LdapAttributeSet attributeSet = entry.GetAttributeSet();
                            IEnumerator enumerator = attributeSet.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                LdapAttribute attribute = (LdapAttribute)enumerator.Current;
                                string attributeName = attribute.Name;
                                string attributeVal = attribute.StringValue;
                                LdapData.Add(attributeName, attributeVal);
                            }
                        }
                        else
                        {
                            break;
                        }
                        break;
                    }
                    connection.Disconnect();
                }

                catch (LdapException ldapEx)
                {
                    _logger.LogError(ldapEx, $"Erreur LDAP pour l'utilisateur {mail}: {ldapEx.Message}");
                    throw;
                }
                finally
                {
                    connection.Disconnect();
                }

            }

            return LdapData;
        }

        public bool ValidateConnect(string username, string password)
        {
            bool result = false;
            using (LdapConnection connection = new LdapConnection())
            {
                try
                {
                    _logger.LogInformation($"Tentative de connexion LDAP pour l'utilisateur: {username}");
                    string ldapusername = $"cn={username},cn=Users," + _ldapSettings.LDapBaseDn;
                    // string ldapusername = $"cn={username},{_ldapSettings.LDapBaseDn}";
                    _logger.LogInformation($"DN utilisé: {ldapusername}");

                    connection.Connect(_ldapSettings.LDapHost, _ldapSettings.LDapPort);
                    _logger.LogInformation($"Connexion LDAP établie. Tentative de bind pour: {ldapusername}");
                    connection.Bind(ldapusername, password);
                    result = true;


                }
                catch (LdapException ldapEx)
                {
                    result = false;
                    _logger.LogError(ldapEx, $"Erreur LDAP pour l'utilisateur {username}: {ldapEx.Message}");
                }
                finally
                {
                    connection.Disconnect();
                }

            }

            return result;
        }
    }
}
