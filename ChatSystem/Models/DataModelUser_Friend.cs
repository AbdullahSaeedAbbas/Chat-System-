using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatSystem.Models
{
    public class DataModelUser_Friend
    {
        public List<User> AllUser = new List<User>();
        public List<User_Friend> UserFriend = new List<User_Friend>();
    }
}