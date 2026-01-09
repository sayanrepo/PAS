using BaseSite.Controllers;
using BaseSite.Data;
using BaseSite.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace BaseSite.Models.Account
{
    public class AccountManager
    {
        public static string GetMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static Account_Users Login(string userName, string password, string userip = "")
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return new Account_Users();
            password = GetMD5(password);

            using (var context = new PantaEntities())
            {
                if (context.Account_Users.Any(u => u.Status == (byte)UserStatus.Active && u.UserName == userName && u.Password == password))
                {
                    var user = context.Account_Users
                              .Include(inc => inc.Account_PersonTypes)
                              .Include(inc => inc.Account_PartnerTypes)
                              .Include(inc => inc.Location_Cities)
                              .Include(inc => inc.Location_Cities1)
                              .Include(inc => inc.Account_UserPost)
                              .Where(u => u.Status == (byte)UserStatus.Active && u.UserName == userName && u.Password == password)
                              .SingleOrDefault();
                    if (user.ImagePath == null) user.ImagePath = "profile.png";
                    return user;
                }
                else
                {
                    return new Account_Users();
                }
            }
        }

        public static List<OPERATIONS> Account_Operation_Get(AccountRole role)
        {
            List<OPERATIONS> res = new List<OPERATIONS>();

            if (role == AccountRole.Foroosh_Operator || role == AccountRole.Foroosh_Assistant || role == AccountRole.Foroosh_Mali || role == AccountRole.Foroosh_Manager || role == AccountRole.Foroosh_Admin)
            {
                res.Add(OPERATIONS.Order);
                res.Add(OPERATIONS.Order_Search);
                res.Add(OPERATIONS.Order_Add);
                res.Add(OPERATIONS.Order_Detail);
                res.Add(OPERATIONS.Order_Print);

                res.Add(OPERATIONS.Plan_Print);

                res.Add(OPERATIONS.Sale);
                res.Add(OPERATIONS.Sale_Search);
                res.Add(OPERATIONS.Sale_Add);
                res.Add(OPERATIONS.Sale_Detail);
                res.Add(OPERATIONS.Sale_Print);

                res.Add(OPERATIONS.Store);
                res.Add(OPERATIONS.Store_Search);
                res.Add(OPERATIONS.Store_Add);
                res.Add(OPERATIONS.Store_Detail);
                res.Add(OPERATIONS.Store_Print);

                res.Add(OPERATIONS.Service);
                res.Add(OPERATIONS.Service_Search);
                res.Add(OPERATIONS.Service_Add);
                res.Add(OPERATIONS.Service_Detail);
                res.Add(OPERATIONS.Service_Print);

                res.Add(OPERATIONS.Payment);
                res.Add(OPERATIONS.Payment_Search);
                res.Add(OPERATIONS.Payment_Add);
                res.Add(OPERATIONS.Payment_Detail);
                res.Add(OPERATIONS.Payment_Print);

                res.Add(OPERATIONS.Delivery);
                res.Add(OPERATIONS.Delivery_Search);
                res.Add(OPERATIONS.Delivery_Add);
                res.Add(OPERATIONS.Delivery_Detail);
                res.Add(OPERATIONS.Delivery_Print);

                res.Add(OPERATIONS.Setting);
                res.Add(OPERATIONS.Setting_Cities);
                res.Add(OPERATIONS.Setting_Cities_Add);
                res.Add(OPERATIONS.Setting_Cities_Edit);
                res.Add(OPERATIONS.Setting_Cities_Delete);

                res.Add(OPERATIONS.Setting_Persons);
                res.Add(OPERATIONS.Setting_Persons_Customer);
                res.Add(OPERATIONS.Setting_Persons_Search);
                res.Add(OPERATIONS.Setting_Persons_Detail);
                res.Add(OPERATIONS.Setting_Persons_Edit);
                res.Add(OPERATIONS.Setting_Persons_Add);

                res.Add(OPERATIONS.CRM);
                res.Add(OPERATIONS.CRM_Persons);

                res.Add(OPERATIONS.Setting_TruthTable);

                res.Add(OPERATIONS.Help_Help);
            }
            if (role == AccountRole.Foroosh_Assistant || role == AccountRole.Foroosh_Mali || role == AccountRole.Foroosh_Manager || role == AccountRole.Foroosh_Admin)
            {
                res.Add(OPERATIONS.Order_Edit_Factor);
                res.Add(OPERATIONS.Order_Delete);

                res.Add(OPERATIONS.Sale_Edit);
                res.Add(OPERATIONS.Sale_Delete);

                res.Add(OPERATIONS.Store_Edit);
                res.Add(OPERATIONS.Store_Delete);

                res.Add(OPERATIONS.Service_Edit);
                res.Add(OPERATIONS.Service_Delete);

                res.Add(OPERATIONS.Report);
                res.Add(OPERATIONS.Report_CustomerBill);
                res.Add(OPERATIONS.Report_CustomersBill);
                res.Add(OPERATIONS.Report_Statistic);
                res.Add(OPERATIONS.Report_Statistic2);
                res.Add(OPERATIONS.Report_SaleControlling);
                res.Add(OPERATIONS.Report_Orders_Monthly_OrderDate);
                res.Add(OPERATIONS.Report_Orders_Monthly_FactorDate);
                res.Add(OPERATIONS.Report_Sales_Payments_Monthly);
                res.Add(OPERATIONS.Report_Lending);
                res.Add(OPERATIONS.Report_CustomersInfo);

                res.Add(OPERATIONS.Setting_Persons_Foroosh);
                res.Add(OPERATIONS.Setting_Persons_AssignUserName);
                res.Add(OPERATIONS.Setting_Persons_AssignAccess);

                res.Add(OPERATIONS.Setting_Name);
                res.Add(OPERATIONS.Setting_Description);
                res.Add(OPERATIONS.Setting_Cost);

                res.Add(OPERATIONS.Setting_Order);
                res.Add(OPERATIONS.Setting_Order_Packet);
                res.Add(OPERATIONS.Setting_Order_Packet_Add);
                res.Add(OPERATIONS.Setting_Order_Packet_Edit);
                res.Add(OPERATIONS.Setting_Order_Packet_Delete);

                res.Add(OPERATIONS.Setting_Order_ElevatorBoard);
                res.Add(OPERATIONS.Setting_Order_ElevatorBoard_Add);
                res.Add(OPERATIONS.Setting_Order_ElevatorBoard_Edit);
                res.Add(OPERATIONS.Setting_Order_ElevatorBoard_Delete);

                res.Add(OPERATIONS.Setting_Order_Deduction);
                res.Add(OPERATIONS.Setting_Order_Deduction_Add);
                res.Add(OPERATIONS.Setting_Order_Deduction_Edit);
                res.Add(OPERATIONS.Setting_Order_Deduction_Delete);

                res.Add(OPERATIONS.Setting_Order_Addition);
                res.Add(OPERATIONS.Setting_Order_Addition_Add);
                res.Add(OPERATIONS.Setting_Order_Addition_Edit);
                res.Add(OPERATIONS.Setting_Order_Addition_Delete);

                res.Add(OPERATIONS.Setting_Attachment);
                res.Add(OPERATIONS.Setting_Attachment_Add);
                res.Add(OPERATIONS.Setting_Attachment_Edit);
                res.Add(OPERATIONS.Setting_Attachment_Delete);

                res.Add(OPERATIONS.Setting_PushButton);
                res.Add(OPERATIONS.Setting_PushButton_Add);
                res.Add(OPERATIONS.Setting_PushButton_Edit);
                res.Add(OPERATIONS.Setting_PushButton_Delete);

                res.Add(OPERATIONS.Setting_Monitor);
                res.Add(OPERATIONS.Setting_Monitor_Add);
                res.Add(OPERATIONS.Setting_Monitor_Edit);
                res.Add(OPERATIONS.Setting_Monitor_Delete);

                res.Add(OPERATIONS.Setting_CabinPanel);
                res.Add(OPERATIONS.Setting_CabinPanel_Add);
                res.Add(OPERATIONS.Setting_CabinPanel_Edit);
                res.Add(OPERATIONS.Setting_CabinPanel_Delete);

                res.Add(OPERATIONS.Setting_CabinSurfaceMetal);
                res.Add(OPERATIONS.Setting_CabinSurfaceMetal_Add);
                res.Add(OPERATIONS.Setting_CabinSurfaceMetal_Edit);
                res.Add(OPERATIONS.Setting_CabinSurfaceMetal_Delete);

                res.Add(OPERATIONS.Setting_HallPanel);
                res.Add(OPERATIONS.Setting_HallPanel_Add);
                res.Add(OPERATIONS.Setting_HallPanel_Edit);
                res.Add(OPERATIONS.Setting_HallPanel_Delete);

                res.Add(OPERATIONS.Setting_HallSurfaceMetal);
                res.Add(OPERATIONS.Setting_HallSurfaceMetal_Add);
                res.Add(OPERATIONS.Setting_HallSurfaceMetal_Edit);
                res.Add(OPERATIONS.Setting_HallSurfaceMetal_Delete);

                res.Add(OPERATIONS.Setting_DoorTopPanel);
                res.Add(OPERATIONS.Setting_DoorTopPanel_Add);
                res.Add(OPERATIONS.Setting_DoorTopPanel_Edit);
                res.Add(OPERATIONS.Setting_DoorTopPanel_Delete);

                res.Add(OPERATIONS.Setting_DoorTopSurfaceMetal);
                res.Add(OPERATIONS.Setting_DoorTopSurfaceMetal_Add);
                res.Add(OPERATIONS.Setting_DoorTopSurfaceMetal_Edit);
                res.Add(OPERATIONS.Setting_DoorTopSurfaceMetal_Delete);

                res.Add(OPERATIONS.Setting_Product);
                res.Add(OPERATIONS.Setting_Product_Add);
                res.Add(OPERATIONS.Setting_Product_Edit);
                res.Add(OPERATIONS.Setting_Product_Delete);
            }
            if (role == AccountRole.Foroosh_Manager || role == AccountRole.Foroosh_Admin)
            {
                res.Add(OPERATIONS.Payment_Delete);
                res.Add(OPERATIONS.Payment_ForoshConfirm);
                res.Add(OPERATIONS.Setting_TruthTable_Edit);
                res.Add(OPERATIONS.Report_KPI);
            }
            if (role == AccountRole.Foroosh_Mali || role == AccountRole.Foroosh_Admin)
            {
                res.Add(OPERATIONS.Order_ChangeStatus);
                res.Add(OPERATIONS.Sale_ChangeStatus);
                res.Add(OPERATIONS.Store_ChangeStatus);
                res.Add(OPERATIONS.Service_ChangeStatus);

                res.Add(OPERATIONS.Payment_MaliConfirm);

                res.Add(OPERATIONS.Delivery_Confirm);

                res.Add(OPERATIONS.Report_ProductFactor);
                res.Add(OPERATIONS.Report_productFactor_AllOperators);
            }
            if (role == AccountRole.Foroosh_Admin)
            {
                res.Add(OPERATIONS.Payment_ChangeStatus);
                res.Add(OPERATIONS.Delivery_ChangeStatus);

                res.Add(OPERATIONS.Logs_Logs);
                res.Add(OPERATIONS.Logs_Search);
                res.Add(OPERATIONS.Logs_Detail);
            }

            //------------------------------------------------------------------------------------------------------------------------

            if (role == AccountRole.Product_Operator || role == AccountRole.Product_Mechanical_Assembler || role == AccountRole.Product_Assistant || role == AccountRole.Product_Manager)
            {
                res.Add(OPERATIONS.Plan);
                res.Add(OPERATIONS.Plan_Search);
                res.Add(OPERATIONS.Plan_Detail);

                res.Add(OPERATIONS.Product);
                res.Add(OPERATIONS.Product_Search);
                res.Add(OPERATIONS.Product_Detail);

                res.Add(OPERATIONS.Process);

                res.Add(OPERATIONS.Delivery);
                res.Add(OPERATIONS.Delivery_Search);
                res.Add(OPERATIONS.Delivery_Detail);
                res.Add(OPERATIONS.Delivery_Print);

                res.Add(OPERATIONS.Report);
                res.Add(OPERATIONS.Report_ProductFactor);
            }
            if (role == AccountRole.Product_Mechanical_Assembler || role == AccountRole.Product_Assistant || role == AccountRole.Product_Manager)
            {
                res.Add(OPERATIONS.Cartable);
                res.Add(OPERATIONS.Cartable_Search);
                res.Add(OPERATIONS.Cartable_Detail);
            }
            if (role == AccountRole.Product_Assistant || role == AccountRole.Product_Manager)
            {
                res.Add(OPERATIONS.Plan_Print);

                res.Add(OPERATIONS.Product_Print);

                res.Add(OPERATIONS.Plan_StartCommand);
                res.Add(OPERATIONS.Plan_FinishCommand);

                res.Add(OPERATIONS.Delivery_Confirm);

                res.Add(OPERATIONS.Help_Help);
            }
            if (role == AccountRole.Product_Manager)
            {
                res.Add(OPERATIONS.Report_productFactor_AllOperators);

                res.Add(OPERATIONS.Setting);
                res.Add(OPERATIONS.Setting_Persons);
                res.Add(OPERATIONS.Setting_Persons_Tolid);
                res.Add(OPERATIONS.Setting_Persons_Search);
                res.Add(OPERATIONS.Setting_Persons_Detail);
                res.Add(OPERATIONS.Setting_Persons_Edit);
                res.Add(OPERATIONS.Setting_Persons_Add);
                res.Add(OPERATIONS.Setting_Persons_AssignUserName);
                res.Add(OPERATIONS.Setting_Persons_AssignAccess);

                res.Add(OPERATIONS.Setting_ProductFactor);
                res.Add(OPERATIONS.Setting_Available);
                res.Add(OPERATIONS.Setting_SurfaceArea);
                res.Add(OPERATIONS.Setting_Size);

                res.Add(OPERATIONS.Setting_Attachment);
                res.Add(OPERATIONS.Setting_Attachment_Edit);

                res.Add(OPERATIONS.Setting_PushButton);
                res.Add(OPERATIONS.Setting_PushButton_Edit);

                res.Add(OPERATIONS.Setting_Monitor);
                res.Add(OPERATIONS.Setting_Monitor_Edit);

                res.Add(OPERATIONS.Setting_CabinPanel);
                res.Add(OPERATIONS.Setting_CabinPanel_Edit);

                res.Add(OPERATIONS.Setting_CabinSurfaceMetal);
                res.Add(OPERATIONS.Setting_CabinSurfaceMetal_Edit);

                res.Add(OPERATIONS.Setting_HallPanel);
                res.Add(OPERATIONS.Setting_HallPanel_Edit);

                res.Add(OPERATIONS.Setting_HallSurfaceMetal);
                res.Add(OPERATIONS.Setting_HallSurfaceMetal_Edit);

                res.Add(OPERATIONS.Setting_DoorTopPanel);
                res.Add(OPERATIONS.Setting_DoorTopPanel_Edit);

                res.Add(OPERATIONS.Setting_DoorTopSurfaceMetal);
                res.Add(OPERATIONS.Setting_DoorTopSurfaceMetal_Edit);

                res.Add(OPERATIONS.Setting_ProductFactorCost);
                res.Add(OPERATIONS.Setting_ProductFactorCost_Add);
                res.Add(OPERATIONS.Setting_ProductFactorCost_Edit);
                res.Add(OPERATIONS.Setting_ProductFactorCost_Delete);

                res.Add(OPERATIONS.Setting_CollectiveProducePercent);
                res.Add(OPERATIONS.Setting_CollectiveProducePercent_Add);
                res.Add(OPERATIONS.Setting_CollectiveProducePercent_Edit);
                res.Add(OPERATIONS.Setting_CollectiveProducePercent_Delete);

                res.Add(OPERATIONS.Process_Backward);
                res.Add(OPERATIONS.Process_Project);

                res.Add(OPERATIONS.Setting_TruthTable);
                res.Add(OPERATIONS.Setting_TruthTable_Edit);
            }

            return res;
        }

        #region User
        public static List<Account_Users> Account_User_Get()
        {
            using (var context = new PantaEntities())
            {
                var list = context.Account_Users
                          .Include(inc => inc.Account_PersonTypes)
                          .OrderBy(m => m.Name).ToList();
                return list;
            }
        }

        public static List<Account_Users> Account_User_Get(string term)
        {
            using (var context = new PantaEntities())
            {
                var list = context.Account_Users
                          .Include(inc => inc.Account_PersonTypes)
                          .Where(m => (m.Name + m.LastName).Replace(" ", "").ToLower().Contains(term.Replace(" ", "").ToLower()))
                          .OrderBy(m => m.Name).ToList();
                return list;
            }
        }

        public static Account_Users Account_User_Get(int Id)
        {
            if (Id == 0)
            {
                Account_Users user = new Account_Users();
                user.Id = 0;
                user.PersonTypeId = 1;      //حقیقی
                user.PartnerTypeId = 2;     //مشتری
                user.DepartmentId = 0;      //نامعلوم
                user.Status = 2;            //کاربر غیرفعال
                return user;
            }

            using (var context = new PantaEntities())
            {
                var list = context.Account_Users
                          .Include(inc => inc.Account_PersonTypes)
                          .Include(inc => inc.Account_PartnerTypes)
                          .Include(inc => inc.Location_Cities)
                          .Include(inc => inc.Location_Cities.Location_Provinces)
                          .Include(inc => inc.Location_Cities.Location_Provinces.Location_Countries)
                          .Include(inc => inc.Location_Cities1)
                          .Include(inc => inc.Location_Cities1.Location_Provinces)
                          .Include(inc => inc.Location_Cities1.Location_Provinces.Location_Countries)
                          .Include(inc => inc.Account_UserPost)
                          .Include(inc => inc.Account_UserStatus)
                          .Include(inc => inc.Account_Users2)
                          .Where(inc => inc.Id == Id)
                          .SingleOrDefault();
                if (list.ImagePath == null) list.ImagePath = "profile.png";
                return list;
            }
        }

        public static int Account_User_Edit(Account_Users user)
        {
            using (PantaEntities context = new PantaEntities())
            {
                if (user.Id == 0)
                {
                    Account_Users person = new Account_Users()
                    {
                        TableId = 1,
                        Status = 2
                    };

                    person.DepartmentId = user.DepartmentId;
                    person.Name = user.Name;
                    person.LastName = user.LastName;
                    person.FatherName = user.FatherName;
                    person.NationalNumber = user.NationalNumber;
                    person.EconomicalNumber = user.EconomicalNumber;
                    person.PersonTypeId = user.PersonTypeId;
                    person.PartnerTypeId = user.PartnerTypeId;
                    person.FindoutWay = user.FindoutWay;
                    person.Website = user.Website;
                    person.Email = user.Email;
                    person.Fax = user.Fax;
                    person.Phone1 = user.Phone1;
                    person.Phone2 = user.Phone2;
                    person.Mobile1 = user.Mobile1;
                    person.Mobile2 = user.Mobile2;
                    person.CityId1 = user.CityId1;
                    person.CityId2 = user.CityId2;
                    person.Address1 = user.Address1;
                    person.Address2 = user.Address2;
                    person.PostalCode1 = user.PostalCode1;
                    person.PostalCode2 = user.PostalCode2;
                    person.Responsible1 = user.Responsible1;
                    person.ResponsiblePhone1 = user.ResponsiblePhone1;
                    person.Responsible2 = user.Responsible2;
                    person.ResponsiblePhone2 = user.ResponsiblePhone2;
                    person.Responsible3 = user.Responsible3;
                    person.ResponsiblePhone3 = user.ResponsiblePhone3;

                    person.RegistrarId = user.RegistrarId;
                    person.RegistrationDate = DateTime.Now;

                    context.Account_Users.Add(person);
                    context.SaveChanges();
                    Cache.Update();
                    return person.Id;
                }
                else
                {
                    Account_Users person = context.Account_Users
                              .Include(inc => inc.Account_PersonTypes)
                              .Include(inc => inc.Account_PartnerTypes)
                              .Include(inc => inc.Location_Cities)
                              .Include(inc => inc.Location_Cities1)
                              .Include(inc => inc.Account_UserPost)
                              .Include(inc => inc.Account_UserStatus)
                              .Where(inc => inc.Id == user.Id)
                              .SingleOrDefault();
                    if (person.ImagePath == null) person.ImagePath = "profile.png";

                    person.DepartmentId = user.DepartmentId;
                    person.Name = user.Name;
                    person.LastName = user.LastName;
                    person.FatherName = user.FatherName;
                    person.NationalNumber = user.NationalNumber;
                    person.EconomicalNumber = user.EconomicalNumber;
                    person.PersonTypeId = user.PersonTypeId;
                    person.PartnerTypeId = user.PartnerTypeId;
                    person.FindoutWay = user.FindoutWay;
                    person.Website = user.Website;
                    person.Email = user.Email;
                    person.Fax = user.Fax;
                    person.Phone1 = user.Phone1;
                    person.Phone2 = user.Phone2;
                    person.Mobile1 = user.Mobile1;
                    person.Mobile2 = user.Mobile2;
                    person.CityId1 = user.CityId1;
                    person.CityId2 = user.CityId2;
                    person.Address1 = user.Address1;
                    person.Address2 = user.Address2;
                    person.PostalCode1 = user.PostalCode1;
                    person.PostalCode2 = user.PostalCode2;
                    person.Responsible1 = user.Responsible1;
                    person.ResponsiblePhone1 = user.ResponsiblePhone1;
                    person.Responsible2 = user.Responsible2;
                    person.ResponsiblePhone2 = user.ResponsiblePhone2;
                    person.Responsible3 = user.Responsible3;
                    person.ResponsiblePhone3 = user.ResponsiblePhone3;

                    context.SaveChanges();
                    Cache.Update();
                    return person.Id;
                }
            }
        }

        public static List<Account_Users> Account_User_Search(string name, byte? departmentId, byte? partnerTypeId, byte? statusId, int? postId, int? hcountryId, int? hprovinceId, int? hcityId)
        {
            List<byte> deps = new List<byte>();
            if (CustomAuthorizeAttribute.isAuthorize(OPERATIONS.Setting_Persons_Customer))
            {
                deps.Add((byte)BaseSite.Models.Department.Unknown);
            }
            if (CustomAuthorizeAttribute.isAuthorize(OPERATIONS.Setting_Persons_Foroosh))
            {
                deps.Add((byte)BaseSite.Models.Department.Foroosh);
            }
            if (CustomAuthorizeAttribute.isAuthorize(OPERATIONS.Setting_Persons_Tolid))
            {
                deps.Add((byte)BaseSite.Models.Department.Tolid);
            }

            using (var context = new PantaEntities())
            {
                if (string.IsNullOrWhiteSpace(name) && departmentId == null && partnerTypeId == null && statusId == null && postId == null && hcountryId == null && hprovinceId == null && hcityId == null)
                {
                    List<Account_Users> result = context.Account_Users.Include(inc => inc.Account_PersonTypes)
                          .Include(inc => inc.Account_PartnerTypes).Include(inc => inc.Location_Cities)
                          .Include(inc => inc.Location_Cities1).Include(inc => inc.Account_UserPost)
                          .Include(inc => inc.Account_UserStatus).Include(inc => inc.Account_Users2)
                          .Where(m => m.DepartmentId.HasValue && deps.Contains(m.DepartmentId.Value)).OrderByDescending(m => m.Id).Take(1000).ToList();

                    return result;
                }
                else
                {
                    var list = from p in context.Account_Users.Include(inc => inc.Account_PersonTypes)
                          .Include(inc => inc.Account_PartnerTypes)
                          .Include(inc => inc.Location_Cities)
                          .Include(inc => inc.Location_Cities.Location_Provinces)
                          .Include(inc => inc.Location_Cities.Location_Provinces.Location_Countries)
                          .Include(inc => inc.Location_Cities1)
                          .Include(inc => inc.Location_Cities1.Location_Provinces)
                          .Include(inc => inc.Location_Cities1.Location_Provinces.Location_Countries)
                          .Include(inc => inc.Account_UserPost)
                          .Include(inc => inc.Account_UserStatus)
                          .Include(inc => inc.Account_Users2)
                               where (p.DepartmentId.HasValue && deps.Contains(p.DepartmentId.Value))
                               select p;

                    if (!string.IsNullOrWhiteSpace(name)) list = list.Where(m => (m.Name + m.LastName + (string.IsNullOrEmpty(m.Responsible1) ? " " : m.Responsible1) + (string.IsNullOrEmpty(m.Responsible2) ? " " : m.Responsible2) + (string.IsNullOrEmpty(m.Responsible3) ? " " : m.Responsible3)).Replace(" ", "").ToLower().Contains(name.Replace(" ", "").ToLower()));
                    if (departmentId != null) list = list.Where(x => x.DepartmentId == departmentId);
                    if (partnerTypeId != null) list = list.Where(x => x.PartnerTypeId == partnerTypeId);
                    if (statusId != null) list = list.Where(x => x.Status == statusId);
                    if (postId != null) list = list.Where(x => x.Account_UserPost.Any(up => up.PostId == postId));
                    if (hcountryId != null) list = list.Where(x => x.Location_Cities.Location_Provinces.CountryId == hcountryId);
                    if (hprovinceId != null) list = list.Where(x => x.Location_Cities.ProvinceId == hprovinceId);
                    if (hcityId != null) list = list.Where(x => x.Location_Cities.Id == hcityId);
                    list = list.OrderByDescending(p => p.Id);

                    // Execute the query
                    List<Account_Users> result = list.ToList();

                    return result;
                }
            }
        }

        public static void Account_User_EditAccess(int Id, byte Status, int PostId)
        {
            using (var context = new PantaEntities())
            {
                Account_Users user = context.Account_Users.Where(u => u.Id == Id).FirstOrDefault();
                user.Status = Status;
                context.SaveChanges();

                List<Account_UserPost> userPost = context.Account_UserPost.Where(up => up.UserId == Id).ToList();
                foreach (Account_UserPost up in userPost)
                {
                    context.Account_UserPost.Remove(up);
                    context.SaveChanges();
                }

                Account_UserPost newPost = new Account_UserPost() { UserId = Id, PostId = PostId, CategoryId = context.Account_Categories.FirstOrDefault().Id };
                context.Account_UserPost.Add(newPost);
                context.SaveChanges();
            }
        }

        public static void Account_User_AssignUserName(int Id, string UserName, string Password)
        {
            using (var context = new PantaEntities())
            {
                if (context.Account_Users.Any(u => u.Id != Id && u.UserName.ToLower() == UserName.ToLower()))
                {
                    throw new Exception("نام کاربری وارد شده در سیستم موجود می باشد. لطفا نام کاربری دیگری را انتخاب نمایید");
                }

                Account_Users user = context.Account_Users.Where(u => u.Id == Id).FirstOrDefault();
                user.UserName = UserName;
                user.Password = GetMD5(Password);
                context.SaveChanges();
            }
        }

        public static string Account_User_ChangePassword(int Id, string newUserName, string currentPassword, string newPassword)
        {
            using (var context = new PantaEntities())
            {
                if (context.Account_Users.Any(u => u.UserName == newUserName && u.Id != Id))
                    return "نام کاربری وارد شده در سیستم وجود دارد";

                Account_Users user = context.Account_Users.Where(u => u.Id == Id).FirstOrDefault();
                if (user.Password != GetMD5(currentPassword))
                    return "رمز عبور فعلی اشتباه است";

                if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 4)
                    return "رمز عبور حداقل باید چهار کارکتر داشته باشد";

                user.UserName = newUserName;
                user.Password = GetMD5(newPassword);
                context.SaveChanges();

                return "نام کاربری و رمز عبور با موفقیت تغییر کرد";
            }
        }

        public static string Account_User_ChangeImage(int Id, string fileName)
        {
            using (var context = new PantaEntities())
            {
                Account_Users user = context.Account_Users.Where(u => u.Id == Id).FirstOrDefault();
                user.ImagePath = fileName;
                context.SaveChanges();

                return "عکس پروفایل با موفقیت تغییر کرد";
            }
        }

        public static void Location_Country_Delete(int userId)
        {
            using (var context = new PantaEntities())
            {
                Account_Users u = context.Account_Users.SingleOrDefault(x => x.Id == userId);
                u.Status = (byte)UserStatus.Deleted;
                context.SaveChanges();
            }
        }

        public static void Location_Country_Edit(Account_Users user)
        {
            using (var context = new PantaEntities())
            {
                context.Entry(user).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
            }
        }

        #endregion
    }
}