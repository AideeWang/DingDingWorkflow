using System;
using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace DingTalkApi.Common
{
    public static class StringExtensions
    {
        public static string MaxSubstring(this string origin, int maxLength)
        {
            return origin.Length >= maxLength ? origin.Substring(0, maxLength) : origin;
        }

        public static string ToMd5(this string origin)
        {
            if (string.IsNullOrWhiteSpace(origin))
            {
                return string.Empty;
            }

            var md5Algorithm = MD5.Create();
            var utf8Bytes = Encoding.UTF8.GetBytes(origin);
            var md5Hash = md5Algorithm.ComputeHash(utf8Bytes);
            var hexString = new StringBuilder();
            foreach (var hexByte in md5Hash)
            {
                hexString.Append(hexByte.ToString("x2"));
            }
            return hexString.ToString();
        }

        //public static string GetEnumDescription(this Enum enumValue)
        //{
        //    string value = enumValue.ToString();
        //    FieldInfo field = enumValue.GetType().GetField(value);
        //    object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);    //获取描述属性
        //    if (objs.Length == 0)    //当描述属性没有时，直接返回名称
        //        return value;
        //    DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
        //    return descriptionAttribute.Description;
        //}
    }
}