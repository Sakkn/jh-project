using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace jh_project
{
   public interface IJhCommand
    {

        Task Execute();
        void Initialize();

    }
}
