﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginTest.Models
{
    public class LoginUser
    {
        public string success { get; set; }
        public string error { get; set; }
        public string message { get; set; }
        public string token { get; set; }
    }
}
