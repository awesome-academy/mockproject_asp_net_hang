using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hotel_management.Enums
{
    public enum TokenType : byte
    {
        EmailVerification = 1,
        PasswordReset = 2
    }

    public enum TokenStatus : byte
    {
        Used = 1,
        Unused = 2
    }
}