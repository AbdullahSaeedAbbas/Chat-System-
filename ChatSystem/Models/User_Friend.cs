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
    
    public partial class User_Friend
    {
        public int UserId1 { get; set; }
        public int UserId2 { get; set; }
        public string Text { get; set; }
        public string File { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<int> Status { get; set; }
        public string Sound { get; set; }
        public string Emotion { get; set; }
        public int messegeID { get; set; }
        public byte[] Image { get; set; }
    
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
    }
}