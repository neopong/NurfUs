//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NurfUs.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserInfo
    {
        public string UserKey { get; set; }
        public bool TempUser { get; set; }
        public long Currency { get; set; }
        public int CorrectGuesses { get; set; }
        public int InCorrectGuesses { get; set; }
        public int Id { get; set; }
        public string ASPNetUserId { get; set; }
    }
}