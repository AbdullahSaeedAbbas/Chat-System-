//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ChatSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Chat
    {
        public int IdChat { get; set; }
        public System.DateTime DateChat { get; set; }
        public string TextChat { get; set; }
        public int UserIdChat { get; set; }
    
        public virtual User User { get; set; }
    }
}
