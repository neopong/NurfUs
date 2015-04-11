using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NurfUs.Models
{
    public class UserInfoModel
    {
        public String UserId
        {
            get;
            set;
        }
        public bool TempUser
        {
            get;
            set;
        }

        public int Currency
        {
            get;
            set;
        }

        public int CorrectGuesses
        {
            get;
            set;
        }

        public int IncorrectGuesses
        {
            get;
            set;
        }

    }
}