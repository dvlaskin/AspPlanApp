using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspPlanApp.Models.AccountViewModels
{
    public class LoginOrRegisterViewModel
    {
        public LoginViewModel Login { get; set; }
        public RegisterViewModel Register { get; set; }
    }
}
