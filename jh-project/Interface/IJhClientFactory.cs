using System;
using System.Collections.Generic;
using System.Text;

namespace jh_project
{
    public interface IJhClientFactory
    {

        public IJhClient CreateTwitterClient();

    }
}
