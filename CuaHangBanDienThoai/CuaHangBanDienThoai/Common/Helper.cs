using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
namespace CuaHangBanDienThoai.Common
{
    public static class Helper
    {
        public static string EncodeCustomerId(int customerId)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(customerId.ToString());
            return Convert.ToBase64String(plainTextBytes);
        }
        public static string MaskPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone) || phone.Length < 10)
                return phone;

            return phone.Substring(0, 2) + new string('*', phone.Length - 4) + phone.Substring(phone.Length - 4);
        }

        // Ẩn email
        public static string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
                return email;

            var emailParts = email.Split('@');
            var localPart = emailParts[0];
            var domainPart = emailParts[1];

            if (localPart.Length <= 2)
                return email;

            return localPart.Substring(0, 2) + new string('*', localPart.Length - 2) + "@" + domainPart;
        }

        // Ẩn địa chỉ
        public static string MaskAddress(string address)
        {
            if (string.IsNullOrEmpty(address) || address.Length < 10)
                return address;

            return address.Substring(0, 3) + new string('*', address.Length - 6) + address.Substring(address.Length - 8);
        }

    }
}