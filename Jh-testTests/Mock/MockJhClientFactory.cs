using jh_project;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jh_test
{
   public  class MockJhClientFactory : IJhClientFactory
    {

        #region Public Methods

        public IJhClient CreateTwitterClient()
        {
            return new MockJhTwitterClient();
        }

        #endregion

    }
}
