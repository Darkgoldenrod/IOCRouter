using System;
using System.Collections.Generic;
using System.Linq;

namespace User
{
    public class UserInfo
    {
        public string UserName { get; set; }

        public string PhoneNumber { get; set; }
    }


    public class User
    {
        public static List<UserInfo> _userInfo = new List<UserInfo>();

        /// <summary>
        /// 获取所有用用户
        /// </summary>
        /// <returns></returns>
        public static string GetAllUserInfo()
        {         
            if (_userInfo.Count == 0)            
                return "当前没有用户，请添加";
            
            string content = string.Empty;     
            foreach (var user in _userInfo)
            {
                content += "用户名：" + user.UserName;
                content += ",";
                content += "手机号：" + user.PhoneNumber;
                content += Environment.NewLine;
            }

            return content;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public static string AddNewUser(string userName, string phoneNumber)
        {
            if (_userInfo.Any(item => item.UserName == userName))
                return "用户已存在";

            var user = new UserInfo()
            {
                UserName = userName,
                PhoneNumber = phoneNumber
            };
            _userInfo.Add(user);
            return "添加用户成功";
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static string DeleteUser(string userName)
        {
            var user = _userInfo.Find(item => item.UserName == userName);
            if (user == null)
                return "指定用户不存在";

            _userInfo.Remove(user);
            return "删除成功";
        }

        public static string ModifyPhoneNumber(string userName, string phoneNumber)
        {
            var user = _userInfo.Find(item => item.UserName == userName);
            if (user == null)
                return "指定用户不存在";

            user.PhoneNumber = phoneNumber;
            return "修改手机号成功";
        }
    }
}
