using ChatSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;

namespace ChatSystem.Controllers
{
    public class HomeController : Controller
    {
        // Database Connection
        ChatSystemEntities1 db =     new ChatSystemEntities1();
        // Class contain mutli-model 
        DataModelUser_Friend     CollectData = new DataModelUser_Friend();

        // properties
        public string            UserIDStore;
        public string            UserNameStore;
        public string            UserPasswordStore;
     // Methods
        public ActionResult      Index()
        {
            var NameUser = db.Users.ToList();
            return View(NameUser);
        }
        public ActionResult      Login(string UserName, string Password)
        {
            try{
                var AllUser = (from r in db.Users select r).ToList();
                var emp0 = (db.Users.Where(x => x.UserName == UserName & x.Password == Password));
                /////////////////////////////////////////////
                var UserId = from x in db.Users
                             where (x.UserName == UserName & x.Password == Password)
                             select x.UserId;
                ViewBag.UserIDAccount = UserId;

                /////////////////////////////////////////////

                if (emp0.Count() != 0)
                {
                    UserIDStore = UserId.First().ToString();
                    Session["UserAccountID"] = UserIDStore;

                }
                else
                {
                    return View("LoginCorrect0");
                }
                Session["UserName"] = UserName;
                Session["Password"] = Password;
                Read_from_file();
                ViewBag.UserNameShow = Session["UserName"];
                // try to put this block in function that return redirect to this.function.................
                int UserIDG = int.Parse(UserIDStore);
                var FriendUser = (from REQ in db.Request_Demand_User
                                  where REQ.userRequst == UserIDG & REQ.Flag == 1
                                  select REQ.UserRespond).ToList();
                ViewBag.FriendUser = FriendUser;
                //.........................................................................................

                return View("UserAccount", AllUser);

            }
            catch
            {
                return View("SessionTimeOut");
            }
                   }
        public PartialViewResult ListOfFriend()
        {
            var AllUser = from User in db.Users select User;
            Read_from_file();
            int UserIDG = int.Parse(UserIDStore);
            var FriendUser = (from REQ in db.Request_Demand_User
                              where REQ.userRequst == UserIDG & REQ.Flag == 1
                              select REQ.UserRespond).ToList();

            ViewBag.FriendUser = FriendUser;
            return PartialView("_ListOfFriend", AllUser);
        }
        public ActionResult      Register_page()
        {
            var NameUser = db.Users.ToList();
            return View("Register_page");
        }
        public ActionResult      ShowUser()
        {
            var NameUser = db.Users.ToList();
            return View(NameUser);
        }
        public ActionResult      SearchUser(string SearchUser, string bool_value)
        {
            Read_from_file();
            ViewBag.OwnerAccount = int.Parse(UserIDStore);
            var SearchResult = from data in db.Users
                               select data;
            if (bool_value == "nav")
            {
                int UserIDG = int.Parse(UserIDStore);

                var FriendUser = (from REQ in db.Request_Demand_User
                                  where REQ.userRequst == UserIDG & REQ.Flag == 1
                                  select REQ.UserRespond).ToList();
                SearchResult = from data in db.Users
                                   //where !(FriendUser).Contains(data.UserId)        
                               select data;

                var requestUser = (from REQ in db.Request_Demand_User
                                   where REQ.userRequst == UserIDG & REQ.Flag == 0
                                   select REQ.UserRespond).ToList();

                var RespondUser = (from RES in db.Request_Demand_User
                                   where RES.UserRespond == UserIDG & RES.Flag == 0
                                   select RES.userRequst).ToList();



                ViewBag.FriendUser = FriendUser;
                ViewBag.RequestUser = requestUser;
                ViewBag.RespondUser = RespondUser;
                ViewBag.UserNameShow = UserNameStore;

            }
            else
            {
                int UserIDG = int.Parse(UserIDStore);
                SearchResult = from data in db.Users
                               where (data.FirstName.StartsWith(SearchUser)) || (data.LastName.StartsWith(SearchUser))
                               select data;

                var requestUser = (from REQ in db.Request_Demand_User
                                   where REQ.userRequst == UserIDG & REQ.Flag == 0
                                   select REQ.UserRespond).ToList();

                var RespondUser = (from RES in db.Request_Demand_User
                                   where RES.UserRespond == UserIDG & RES.Flag == 0
                                   select RES.userRequst).ToList();

                var FriendUser = (from REQ in db.Request_Demand_User
                                  where REQ.userRequst == UserIDG & REQ.Flag == 1
                                  select REQ.UserRespond).ToList();

                ViewBag.FriendUser = FriendUser;
                ViewBag.RequestUser = requestUser;
                ViewBag.RespondUser = RespondUser;

                ///////////////////////////////////////////////

            }
            //var users_Filter = db.call_user_specified(1);

            return View("SearchUser", SearchResult.ToList());
        }
        public PartialViewResult SearchUserInsideList(string SearchUser)
        {
            Read_from_file();
            int UserIDG = int.Parse(UserIDStore);

            var FriendUser = (from REQ in db.Request_Demand_User
                              where REQ.userRequst == UserIDG & REQ.Flag == 1 &
                              (REQ.User.FirstName.StartsWith(SearchUser) ||
                              (REQ.User.LastName.StartsWith(SearchUser)))
                              select REQ.User).ToList();


            return PartialView("_SearchMethod", FriendUser.ToList());
        }
        public ActionResult      ProceedRequestFriend(string UserRequestID, string UserDemandID, string flag = "0")
        {
            Read_from_file();
            ViewBag.OwnerAccount = int.Parse(UserIDStore);
            int UserIDG = int.Parse(UserIDStore);
            int UserDemand_;
            int Flag = int.Parse(flag);
            try
            {
                if (UserRequestID != null)
                {

                    int UserRequest_ = int.Parse(UserRequestID);
                    UserDemand_ = int.Parse(UserDemandID);

                    var ChechUser_REQ_OR_DEM = (from REQ in db.Request_Demand_User
                                                where REQ.userRequst == UserRequest_ & REQ.UserRespond == UserDemand_ & REQ.Flag == 0
                                                select REQ.UserRespond).ToList();

                    var ChechIfUserSendOrDemand = (from REQ in db.Request_Demand_User
                                                   where REQ.userRequst == UserDemand_ & REQ.UserRespond == UserRequest_ & REQ.Flag == 0
                                                   select REQ.UserRespond).ToList();

                    // user request friend and cancel request friend so i remove it from requestDemand user
                    if (ChechUser_REQ_OR_DEM.Contains(UserDemand_))
                    {
                        db.DeleteRecordFromReq_Dem_Table(UserDemand_);
                    }
                    else
                    {
                        if ((Flag == 1) || (Flag == -1))
                        {
                            db.DeleteRecordFromReq_Dem_Table_(UserDemand_, UserRequest_);
                            db.SaveChanges();
                            Models.Request_Demand_User REQ_DEMa = new Request_Demand_User()
                            {
                                userRequst = int.Parse(UserRequestID),
                                UserRespond = int.Parse(UserDemandID),
                                Flag = Flag
                            };
                            db.Request_Demand_User.Add(REQ_DEMa);
                            db.SaveChanges();


                            //////////////////////////////////////////////////////
                            Models.Request_Demand_User REQ_DEMa_2 = new Request_Demand_User()
                            {
                                userRequst = int.Parse(UserDemandID),
                                UserRespond = int.Parse(UserRequestID),
                                Flag = Flag
                            };
                            db.Request_Demand_User.Add(REQ_DEMa_2);
                            db.SaveChanges();
                        }
                        else
                        {
                            Models.Request_Demand_User REQ_DEMa = new Request_Demand_User()
                            {
                                userRequst = int.Parse(UserRequestID),
                                UserRespond = int.Parse(UserDemandID),
                                Flag = int.Parse(flag)
                            };
                            db.Request_Demand_User.Add(REQ_DEMa);
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch { }

            // user that become frind to userid =  UserIDG
            var FriendUser = (from REQ in db.Request_Demand_User
                              where REQ.userRequst == UserIDG & REQ.Flag == 1
                              select REQ.UserRespond).ToList();
            //var users_Filter = db.call_user_specified(1);
            var SearchResult = from data in db.Users
                                   //where !(FriendUser).Contains(data.UserId)
                               select data;
            // user that userid =UserIDG  sent for demand friend  
            var requestUser = (from REQ in db.Request_Demand_User
                               where REQ.userRequst == UserIDG & REQ.Flag == 0
                               select REQ.UserRespond).ToList();

            // to make decision accept or reject 
            var RespondUser = (from RES in db.Request_Demand_User
                               where RES.UserRespond == UserIDG & RES.Flag == 0
                               select RES.userRequst).ToList();


            ViewBag.FriendUser = FriendUser;
            ViewBag.RespondUser = RespondUser;
            ViewBag.RequestUser = requestUser;
            ViewBag.UserNameShow = UserNameStore;
            return View("UpdateUserSearch", SearchResult.ToList());
        }
        public ActionResult      ProceedRequestReject(string UserRequestID, string UserDemandID, string flagReject)
        {
            Read_from_file();
            int UserIDG = int.Parse(UserIDStore);
            int UserDemand_;
            try
            {
                if (UserRequestID != null)
                {
                    int UserRequest_ = int.Parse(UserRequestID);
                    UserDemand_ = int.Parse(UserDemandID);

                    var ChechUser_REQ_OR_DEM = (from REQ in db.Request_Demand_User
                                                where REQ.userRequst == UserRequest_ & REQ.UserRespond == UserDemand_ & REQ.Flag == 0
                                                select REQ.UserRespond).ToList();

                    // user request friend and cancel request friend so i remove it from requestDemand user
                    if (ChechUser_REQ_OR_DEM.Contains(UserDemand_) == true)
                    {
                        db.DeleteRecordFromReq_Dem_Table(UserDemand_);
                    }
                    else
                    {
                        db.DeleteRecordFromReq_Dem_Table_(UserIDG, UserDemand_);
                    }
                }

            }
            catch { }

            //var users_Filter = db.call_user_specified(1);
            var SearchResult = from data in db.Users
                               select data;
            var requestUser = (from REQ in db.Request_Demand_User
                               where REQ.userRequst == UserIDG & REQ.Flag == 0
                               select REQ.UserRespond).ToList();
            ViewBag.RequestUser = requestUser;
            ViewBag.UserNameShow = UserNameStore;
            return View("UpdateUserSearch", SearchResult.ToList());
        }
        public ActionResult      SearchUserBtn()
        {
            Read_from_file();
            ViewBag.UserNameShow = UserNameStore;
            return View("SearchUserBtn");
        }
        public ActionResult      Login_session()
        {
            Read_from_file();
            var AllUser = (from r in db.Users select r).ToList();
            var emp0 = (db.Users.Where(x => x.UserName == UserNameStore & x.Password == UserPasswordStore)).ToList();
            /////////////////////////////////////////////
            var UserId = (from x in db.Users
                          where (x.UserName == UserNameStore & x.Password == UserPasswordStore)
                          select x.UserId).ToList();
            ViewBag.UserIDAccount = UserId;
            if (emp0.Count() != 0)
                UserIDStore = UserId.First().ToString();
            else
            {
                ViewBag.valideate = "Incorrect Username Or Password ";
                return View("LoginCorrect0", AllUser);
            }
            Read_from_file();
            // As Navbar Owner Account
            ViewBag.UserNameShow = UserNameStore;
            return View("UserAccount", AllUser);
        }
        public ActionResult      ProceedRequestFriend2(string UserRequestID, string UserDemandID, string flag)
        {
            Read_from_file();
            int R = int.Parse(UserRequestID);
            int D = int.Parse(UserDemandID);
            int UserIDG = int.Parse(UserIDStore);
            var ChechUser_REQ_OR_DEM = (from REQ in db.Request_Demand_User
                                        where REQ.userRequst == R & REQ.UserRespond == D & REQ.Flag == 0
                                        select REQ.UserRespond).ToList();
            if (ChechUser_REQ_OR_DEM.Contains(D) == true)
            {
                try
                {
                    db.DeleteRecordFromReq_Dem_Table(D);
                }
                catch { }
            }
            else
            {
                try
                {
                    Models.Request_Demand_User REQ_DEMa = new Request_Demand_User()
                    {
                        userRequst = int.Parse(UserRequestID),
                        UserRespond = int.Parse(UserDemandID),
                        Flag = int.Parse(flag)
                    };
                    db.Request_Demand_User.Add(REQ_DEMa);
                    db.SaveChanges();
                }
                catch
                { }
            }

            //var users_Filter = db.call_user_specified(1);
            var SearchResult = from data in db.Users
                               select data;
            var requestUser = (from REQ in db.Request_Demand_User
                               where REQ.userRequst == UserIDG & REQ.Flag == 0
                               select REQ.UserRespond).ToList();
            ViewBag.RequestUser = requestUser;
            ViewBag.UserNameShow = UserNameStore;

            ///////////////////////////////////////////////


            //return View("Index");
            return View("RequiestFriend", SearchResult.ToList());
        }
        [HttpPost]
        public ActionResult      Register(string FirstName,
                                      string LastName,
                                      string EmailRegister,
                                      string UserNameRegister,
                                      string PasswordRegister,
                                      string PasswordRegister2,
                                      string errorpasword,
                                      string description,
                                      HttpPostedFileBase ProfileImage
                                     )
        {
            bool flag = false;
            var Username_from_db = (from user in db.Users
                                    select user.UserName).ToList();
            var Email_from_db = (from user in db.Users
                                 select user.Email).ToList();
            string username_erorr = " ", Email_erorr = " ", password_Erorr = " ", description_Erorr = " ";
            if (Username_from_db.Contains(UserNameRegister))
            {
                username_erorr = " ERORR ! UserName is already exist Please try to Register Again  ";
                flag = true;
            }
            if (Email_from_db.Contains(EmailRegister))
            {
                Email_erorr = "ERORR ! This Email already exist With another Account Please Use another it ";
                flag = true;
            }
            if (PasswordRegister != PasswordRegister2)
            {
                password_Erorr = "ERORR ! Confirm Password not the same Password Please Re-Confirm Password Again ";
                flag = true;
            }
            if (description == " ")
            {
                password_Erorr = "ERORR ! Add Description About You  ";
                flag = true;
            }
            if (flag)
            {
                ViewBag.flag = flag;
                ViewBag.username_erorr = username_erorr;
                ViewBag.Email_erorr = Email_erorr;
                ViewBag.password_Erorr = password_Erorr;
                ViewBag.description_Erorr = description_Erorr;


                return View("Register_page");
            }

            try
            {
                //string xxx = ProfileImage.ToString();
                //xxx = ProfileImage.ToString();
                byte[] bytes;
                using (BinaryReader br = new BinaryReader(ProfileImage.InputStream))
                {
                    bytes = br.ReadBytes(ProfileImage.ContentLength);
                }
                Models.User ObjectUser = new Models.User()
                {
                    FirstName = FirstName.ToString(),
                    LastName = LastName.ToString(),
                    Email = EmailRegister.ToString(),
                    Password = PasswordRegister.ToString(),
                    UserName = UserNameRegister.ToString(),
                    Description = description.ToString(),
                    Logo = bytes
                };
                db.Users.Add(ObjectUser);
                db.SaveChanges();
            }
            catch
            {
            }

            return View("Index");
        }
        public ActionResult      UserAccount_Update(string FriendUserID)
        {
            try
            {
                Read_from_file();
                read_image();
                var AllUser = (from r in db.Users select r).ToList();
                var emp0 = (db.Users.Where(x => x.UserName == UserNameStore & x.Password == UserPasswordStore)).ToList();
                /////////////////////////////////////////////
                ViewBag.UserNameShow = UserNameStore;
                int UserIDG = int.Parse(UserIDStore);
                var FriendUser = (from REQ in db.Request_Demand_User
                                  where REQ.userRequst == UserIDG & REQ.Flag == 1
                                  select REQ.UserRespond).ToList();

                ViewBag.FriendUser = FriendUser;
                ViewBag.FriendUserID = FriendUserID;
                int FriendUserID1 = int.Parse(FriendUserID);

                // Store in cookie--------------------------
                Session["FriendUserID"] = FriendUserID;
                Session["_FriendUserID"] = FriendUserID;
                var FriendUserID_Query = (from UserFrien in db.Users where UserFrien.UserId == FriendUserID1 select UserFrien.UserId).ToList();
                ViewBag.SelectedFriend = FriendUserID_Query;
                //////////////////////////////////////////////////
                int User_Owner = int.Parse(UserIDStore);
                int UserFriend = int.Parse(FriendUserID);
                var Messege = (from m in db.User_Friend
                               where (m.UserId1 == User_Owner & m.UserId2 == UserFriend) ||
                                     (m.UserId1 == UserFriend & m.UserId2 == User_Owner)
                               orderby m.messegeID descending
                               select m).ToList();
                ViewBag.Messege = Messege;
                foreach (var item in AllUser)
                {
                    CollectData.AllUser.Add(item);
                }
                foreach (var item in Messege)
                {
                    CollectData.UserFriend.Add(item);
                }
                Session["UserAccountID"] = UserIDStore;
                // catch timeout 
                int UserAccountID = int.Parse(Session["UserAccountID"].ToString());
                return View("UserAccount_Update", CollectData);
            }
            catch
            {
                return View("SessionTimeOut");
            }
        }
        public ActionResult      RequiestFriend(/*string UserName, string Password*/)
        {
            Read_from_file();
            //var users_Filter = db.call_user_specified(1);
            int UserIDG = int.Parse(UserIDStore);

            var requestUser = (from REQ in db.Request_Demand_User
                               where REQ.userRequst == UserIDG & REQ.Flag == 1
                               select REQ.UserRespond).ToList();
            ViewBag.RequestUser = requestUser;
            var requestUser0 = (from REQ in db.Request_Demand_User
                                where REQ.userRequst == UserIDG & REQ.Flag == 0
                                select REQ.UserRespond).ToList();
            ViewBag.RequestUser0 = requestUser0;
            ViewBag.UserNameShow = UserNameStore;
            var SearchResult = from data in db.Users select data;
            return View("RequiestFriend", SearchResult.ToList());
        }
        public ActionResult      RegisterError(/*string UserName, string Password*/)
        {

            Read_from_file();
            return View();
        }
        //public ActionResult      InsertTextMessege(string Write_messeage)
        //{
        //    Read_from_file();
        //    var AllUser = (from r in db.Users select r).ToList();
        //    var emp0 = (db.Users.Where(x => x.UserName == UserNameStore & x.Password == UserPasswordStore)).ToList();
        //    /////////////////////////////////////////////
        //    //get cookie----------
        //    int IDFriendReturned = int.Parse(Session["FriendUserID"].ToString());
        //    ViewBag.UserNameShow = UserNameStore;
        //    int UserIDG = int.Parse(UserIDStore);
        //    var FriendUser = (from REQ in db.Request_Demand_User
        //                      where REQ.userRequst == UserIDG & REQ.Flag == 1
        //                      select REQ.UserRespond).ToList();
        //    ViewBag.FriendUser = FriendUser;
        //    var FriendUserID_Query = (from UserFrien in db.Users where UserFrien.UserId == IDFriendReturned select UserFrien.UserId).ToList();
        //    ViewBag.SelectedFriend = FriendUserID_Query;
        //    ///////////////////////////////////////////////////////////////////////
        //    int User_Owner = int.Parse(UserIDStore);
        //    if (Write_messeage == "")
        //    { }
        //    else
        //    {
        //        Models.User_Friend AddTextMessege = new User_Friend()
        //        {
        //            UserId1 = User_Owner,
        //            UserId2 = IDFriendReturned,
        //            Text = Write_messeage,
        //            Date = DateTime.Now
        //        };
        //        db.User_Friend.Add(AddTextMessege);
        //        db.SaveChanges();
        //    }
        //    var Messege = (from m in db.User_Friend
        //                   where (m.UserId1 == User_Owner & m.UserId2 == IDFriendReturned) ||
        //                         (m.UserId1 == IDFriendReturned & m.UserId2 == User_Owner)
        //                   orderby m.messegeID descending
        //                   select m).ToList();
        //    ViewBag.Messege = Messege;

        //    foreach (var item in AllUser)
        //    {
        //        CollectData.AllUser.Add(item);
        //    }
        //    foreach (var item in Messege)
        //    {
        //        CollectData.UserFriend.Add(item);
        //    }


        //    return View("UserAccount_Update_For_Text", CollectData);
        //}
        public PartialViewResult _InsertTextMessege(string Write_messeage ,HttpPostedFileBase send_img )
        {
            try
            {
                Read_from_file();
                var AllUser = (from r in db.Users select r).ToList();
                var emp0 = (db.Users.Where(x => x.UserName == UserNameStore & x.Password == UserPasswordStore)).ToList();
                /////////////////////////////////////////////
                //get cookie----------
                int IDFriendReturned = int.Parse(Session["FriendUserID"].ToString());
                ViewBag.UserNameShow = UserNameStore;
                int UserIDG = int.Parse(UserIDStore);
                var FriendUser = (from REQ in db.Request_Demand_User
                                  where REQ.userRequst == UserIDG & REQ.Flag == 1
                                  select REQ.UserRespond).ToList();
                ViewBag.FriendUser = FriendUser;
                var FriendUserID_Query = (from UserFrien in db.Users where UserFrien.UserId == IDFriendReturned select UserFrien.UserId).ToList();
                ViewBag.SelectedFriend = FriendUserID_Query;
                ///////////////////////////////////////////////////////////////////////
                int User_Owner = int.Parse(UserIDStore);
                byte[] bytes;
                if (send_img != null)
                {
                    using (BinaryReader br = new BinaryReader(send_img.InputStream))
                    {
                        bytes = br.ReadBytes(send_img.ContentLength);
                    }
                    if (Write_messeage != "")
                    {
                        Models.User_Friend AddTextMessege = new User_Friend()
                        {
                            UserId1 = User_Owner,
                            UserId2 = IDFriendReturned,
                            Text = Write_messeage,
                            Image = bytes,
                            Date = DateTime.Now
                        };
                        db.User_Friend.Add(AddTextMessege);
                        db.SaveChanges();
                    }
                    else
                    {
                        Models.User_Friend AddTextMessege = new User_Friend()
                        {
                            UserId1 = User_Owner,
                            UserId2 = IDFriendReturned,
                            Image = bytes,
                            Date = DateTime.Now
                        };
                        db.User_Friend.Add(AddTextMessege);
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (Write_messeage != "")
                    {
                        Models.User_Friend AddTextMessege = new User_Friend()
                        {
                            UserId1 = User_Owner,
                            UserId2 = IDFriendReturned,
                            Text = Write_messeage,
                            Date = DateTime.Now
                        };
                        db.User_Friend.Add(AddTextMessege);
                        db.SaveChanges();
                    }
                    else
                    {

                    }
                }
                var Messege = (from m in db.User_Friend
                               where (m.UserId1 == User_Owner & m.UserId2 == IDFriendReturned) ||
                                     (m.UserId1 == IDFriendReturned & m.UserId2 == User_Owner)
                               orderby m.messegeID descending
                               select m).ToList();
                ViewBag.Messege = Messege;
                foreach (var item in AllUser)
                {
                    CollectData.AllUser.Add(item);
                }
                foreach (var item in Messege)
                {
                    CollectData.UserFriend.Add(item);
                }
                return PartialView("_MessegeBox", CollectData);
            }
            catch
            {
                return PartialView("SessionTimeOut");
            }
            }
        public PartialViewResult InsertImageMessege(string imgName /*, string UserAccountID *//*, string friendID*/)
        {
            Read_from_file();
            read_image();
            int UserIDG = int.Parse(UserIDStore);
            int User_Owner = int.Parse(UserIDStore);
            var f = Session["FriendUserID"].ToString();
            int friendid = int.Parse(f);
            var AllUser = (from r in db.Users select r).ToList();

            var FriendUser = (from REQ in db.Request_Demand_User
                              where REQ.userRequst == UserIDG & REQ.Flag == 1
                              select REQ.UserRespond).ToList();

            var FriendUserID_Query = (from UserFrien in db.Users where UserFrien.UserId == friendid select UserFrien.UserId).ToList();
            ViewBag.SelectedFriend = FriendUserID_Query;
            ViewBag.FriendUser = FriendUser;

            Models.User_Friend InsertImageMessege = new User_Friend()
            {
                UserId1 = User_Owner,
                UserId2 = friendid,
                Emotion = imgName,
                Date = DateTime.Now
            };
            db.User_Friend.Add(InsertImageMessege);
            db.SaveChanges();
            var Messege = (from m in db.User_Friend
                           where (m.UserId1 == User_Owner & m.UserId2 == friendid/*UserFriend*/) ||
                                 (m.UserId1 == friendid /*UserFriend*/ & m.UserId2 == User_Owner)
                           orderby m.messegeID descending
                           select m).ToList();
            ViewBag.Messege = Messege;
            foreach (var item in AllUser)
            {
                CollectData.AllUser.Add(item);
            }
            foreach (var item in Messege)
            {
                CollectData.UserFriend.Add(item);
            }
            //int FriendID = int.Parse(friendID);

            return PartialView("_MessegeBox", CollectData);
        }
        public void              read_image()
        {
            List<string> emo = new List<string>();
            //var Images = Directory.EnumerateFiles(Server.MapPath("/Image/Emoij/")).Select(f => "" + f);
            var Images = Directory.GetFiles(Server.MapPath("~/Image/Emoij/"));
            foreach (var item in Images)
            {
                emo.Add(Path.GetFileName(item));
            }
            ViewBag.PakImage = emo;

        }
        public void              Read_from_file()
        {
            try
            {
                UserIDStore = Session["UserAccountID"].ToString();
                UserNameStore = Session["UserName"].ToString();
                UserPasswordStore = Session["Password"].ToString();
                ViewBag.LoginUser = UserIDStore;
            }
            catch
            {

            }
    
        }
        public ActionResult      Setting()
        {
            Read_from_file();
            ViewBag.UserNameShow = UserNameStore;
            ViewBag.LoginUser = UserIDStore;

            return View();
        }
        public PartialViewResult _Account_Info()
        {
            Read_from_file();
            ViewBag.UserNameShow = UserNameStore;
            ViewBag.LoginUser = UserIDStore;
            var User_Information = (db.Users.Where(x => x.UserName == UserNameStore & x.Password == UserPasswordStore)).ToList();


            return PartialView("_Account_Info", User_Information);
        }

        public ActionResult      SessionTimeOut()
        { 
            return View();
        }

    }
}

