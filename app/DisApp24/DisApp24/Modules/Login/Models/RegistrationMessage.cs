﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisApp24.Models
{
    public class RegistrationMessage
    {
        public bool IsSuccessful { get; }

        public RegistrationMessage(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }
    }

}
