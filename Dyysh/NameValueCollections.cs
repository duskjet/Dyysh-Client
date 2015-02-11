using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dyysh
{
    public class UserCredentialsCollection
    {
        public UserCredentialsCollection(string username, string password)
        {
            _username = username;
            _password = password;
        }

        private string _username;
        private string _password;

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

    }
}
