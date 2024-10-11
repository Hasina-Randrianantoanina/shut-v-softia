using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHUT.Core.Data
{
    public class LDapIdentifier
    {
        private readonly LDapSettings _ldapSettings;
        public LDapIdentifier(IOptions<LDapSettings> ldapSettings)
        {
            _ldapSettings = ldapSettings.Value;
        }

        public Dictionary<string, string> GetLDAPDataIdentifiers(string username, string password)
        {
            Dictionary<string, string> LdapData = new();
            using (LdapConnection connection = new LdapConnection())
            {
                try
                {
                    string ldapusername = $"cn={username},cn=Users," + _ldapSettings.LDapBaseDn;

                    connection.Connect(_ldapSettings.LDapHost, _ldapSettings.LDapPort);
                    connection.Bind(ldapusername, password);
                    string searchBase = "cn=Users," + _ldapSettings.LDapBaseDn;
                    //string testsn = username;
                    string[] attrs = { "sn", "mail" };
                    string searchFilter = $"(cn={username})";
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

                catch (Exception Ex)
                {
                    throw new Exception(Ex.Message, Ex);
                }
                finally
                {
                    connection.Disconnect();
                }

            }

            return LdapData;
        }
    }
}
