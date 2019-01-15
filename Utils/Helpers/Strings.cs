﻿using System;

namespace Utils
{
    public static class Strings
    {
        public static Tuple<string, string> SplitString(this string str, char separator)
        {
            var index = str.IndexOf(separator);
            var str2 = str.Length > index?str.Substring(index + 1):string.Empty;
            var str1 = str.Substring(0, index);
            return new Tuple<string, string>(str1, str2);
        }
        
        public static void Trim<T>(this T obj, Expression<Func<T, string>> action) where T : class
        {
            var expression = (MemberExpression)action.Body;
            var member = expression.Member;
            Action<string> setProperty;
            Func<string> getPropertyValue;
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    setProperty = val =>  ((FieldInfo)member).SetValue(obj,val);
                    getPropertyValue = () => ((FieldInfo)member).GetValue(obj)?.ToString();

                    break;
                case MemberTypes.Property:
                    setProperty = val => ((PropertyInfo)member).SetValue(obj, val);
                    getPropertyValue = () => ((PropertyInfo) member).GetValue(obj)?.ToString();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var trimmedString = getPropertyValue().Trim();
            setProperty(trimmedString);
        }
    }
}
