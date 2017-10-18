using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace FeiniuBus.AspNetCore.Mvc.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method |
                    AttributeTargets.Parameter)]
    public class IgnoreCaseEnumDataTypeAttribute : DataTypeAttribute
    {
        public IgnoreCaseEnumDataTypeAttribute(Type enumType) : base("Enumeration")
        {
            EnumType = enumType;
        }

        public Type EnumType { get; }

        public override bool IsValid(object value)
        {
            if (EnumType == null)
            {
                throw new InvalidOperationException("指定的枚举类型不能为NULL");
            }
            if (!EnumType.GetTypeInfo().IsEnum)
            {
                throw new InvalidOperationException($"类型{EnumType.FullName}必须是枚举类型");
            }

            var stringValue = value as string;
            if (stringValue != null && string.IsNullOrEmpty(stringValue))
            {
                return false;
            }

            var valueType = value.GetType();
            if (valueType.GetTypeInfo().IsEnum && EnumType != valueType)
            {
                return false;
            }

            if (!valueType.GetTypeInfo().IsValueType && valueType != typeof(string))
            {
                return false;
            }

            if (valueType == typeof(bool) || valueType == typeof(float) || valueType == typeof(double) ||
                valueType == typeof(decimal) || valueType == typeof(char))
            {
                return false;
            }

            object convertedValue;
            if (valueType.GetTypeInfo().IsEnum)
            {
                convertedValue = value;
            }
            else
            {
                try
                {
                    if (stringValue != null)
                    {
                        convertedValue = Enum.Parse(EnumType, stringValue, true);
                    }
                    else
                    {
                        convertedValue = Enum.ToObject(EnumType, value);
                    }
                }
                catch (ArgumentException)
                {
                    return false;
                }
            }

            if (IsEnumTypeInFlagsMode(EnumType))
            {
                var underlying = GetUnderlyingTypeValueString(EnumType, convertedValue);
                var converted = convertedValue.ToString();
                return !underlying.Equals(converted);
            }

            return Enum.IsDefined(EnumType, convertedValue);
        }

        private static bool IsEnumTypeInFlagsMode(Type enumType)
        {
            return enumType.GetTypeInfo().GetCustomAttributes(typeof(FlagsAttribute), false).Any();
        }

        private static string GetUnderlyingTypeValueString(Type enumType, object enumValue)
        {
            return Convert.ChangeType(enumValue, Enum.GetUnderlyingType(enumType), CultureInfo.InvariantCulture)
                .ToString();
        }
    }
}