using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Numerics;
using System.Linq;

namespace FunctionalScript
{
    public static class Runtime
    {
        public static readonly dynamic Undefined = new UndefinedValue();
        
        public class UndefinedValue
        {
            public override string ToString() => "undefined";
            public override bool Equals(object obj) => obj is UndefinedValue;
            public override int GetHashCode() => 0;
        }
        
        public static bool StrictEquals(dynamic left, dynamic right)
        {
            if (left == null && right == null) return true;
            if (left == null || right == null) return false;
            if (left is UndefinedValue && right is UndefinedValue) return true;
            if (left is UndefinedValue || right is UndefinedValue) return false;
            
            Type leftType = left.GetType();
            Type rightType = right.GetType();
            
            if (leftType != rightType) return false;
            
            if (left is string || left is bool || IsNumericType(leftType))
            {
                return left.Equals(right);
            }
            
            return ReferenceEquals(left, right);
        }
        
        public static bool StrictNotEquals(dynamic left, dynamic right)
        {
            return !StrictEquals(left, right);
        }
        
        public static dynamic LogicalOr(dynamic left, dynamic right)
        {
            if (IsTruthy(left)) return left;
            return right;
        }
        
        public static dynamic LogicalAnd(dynamic left, dynamic right)
        {
            if (!IsTruthy(left)) return left;
            return right;
        }
        
        public static bool IsTruthy(dynamic value)
        {
            if (value == null) return false;
            if (value is UndefinedValue) return false;
            if (value is bool) return (bool)value;
            if (value is string) return !string.IsNullOrEmpty((string)value);
            if (IsNumericType(value.GetType()))
            {
                if (value is BigInteger bi) return bi != BigInteger.Zero;
                double d = Convert.ToDouble(value);
                return d != 0 && !double.IsNaN(d);
            }
            return true;
        }
        
        public static dynamic DynamicIndex(dynamic array, dynamic index)
        {
            if (array == null || array is UndefinedValue)
                throw new NullReferenceException("Cannot read property of null or undefined");
            
            int idx = Convert.ToInt32(index);
            if (array is System.Array arr)
            {
                if (idx < 0 || idx >= arr.Length)
                    return Undefined;
                return arr.GetValue(idx);
            }
            else if (array is IList<dynamic> list)
            {
                if (idx < 0 || idx >= list.Count)
                    return Undefined;
                return list[idx];
            }
            
            throw new InvalidOperationException($"Cannot index into {array.GetType()}");
        }
        
        public static BigInteger ParseOctalBigInt(string value)
        {
            BigInteger result = BigInteger.Zero;
            BigInteger baseValue = 8;
            
            for (int i = 0; i < value.Length; i++)
            {
                int digit = value[i] - '0';
                result = result * baseValue + digit;
            }
            
            return result;
        }
        
        public static BigInteger ParseBinaryBigInt(string value)
        {
            BigInteger result = BigInteger.Zero;
            
            for (int i = 0; i < value.Length; i++)
            {
                result = result * 2;
                if (value[i] == '1')
                    result += 1;
            }
            
            return result;
        }
        
        private static bool IsNumericType(Type type)
        {
            return type == typeof(byte) || type == typeof(sbyte) ||
                   type == typeof(short) || type == typeof(ushort) ||
                   type == typeof(int) || type == typeof(uint) ||
                   type == typeof(long) || type == typeof(ulong) ||
                   type == typeof(float) || type == typeof(double) ||
                   type == typeof(decimal) || type == typeof(BigInteger);
        }
        
        public static class Object
        {
            public static dynamic Assign(dynamic target, params dynamic[] sources)
            {
                if (target == null || target is UndefinedValue)
                    throw new ArgumentNullException(nameof(target));
                
                foreach (var source in sources)
                {
                    if (source == null || source is UndefinedValue)
                        continue;
                    
                    if (source is IDictionary<string, object> dict)
                    {
                        if (target is IDictionary<string, object> targetDict)
                        {
                            foreach (var kvp in dict)
                                targetDict[kvp.Key] = kvp.Value;
                        }
                    }
                    else if (source is ExpandoObject expando)
                    {
                        var dict2 = (IDictionary<string, object>)expando;
                        if (target is ExpandoObject targetExpando)
                        {
                            var targetDict = (IDictionary<string, object>)targetExpando;
                            foreach (var kvp in dict2)
                                targetDict[kvp.Key] = kvp.Value;
                        }
                    }
                }
                
                return target;
            }
            
            public static dynamic[] Entries(dynamic obj)
            {
                if (obj == null || obj is UndefinedValue)
                    return new dynamic[0];
                
                var entries = new List<dynamic>();
                
                if (obj is IDictionary<string, object> dict)
                {
                    foreach (var kvp in dict)
                        entries.Add(new dynamic[] { kvp.Key, kvp.Value });
                }
                else if (obj is ExpandoObject expando)
                {
                    var dict2 = (IDictionary<string, object>)expando;
                    foreach (var kvp in dict2)
                        entries.Add(new dynamic[] { kvp.Key, kvp.Value });
                }
                
                return entries.ToArray();
            }
            
            public static string[] Keys(dynamic obj)
            {
                if (obj == null || obj is UndefinedValue)
                    return new string[0];
                
                if (obj is IDictionary<string, object> dict)
                    return dict.Keys.ToArray();
                else if (obj is ExpandoObject expando)
                {
                    var dict2 = (IDictionary<string, object>)expando;
                    return dict2.Keys.ToArray();
                }
                
                return new string[0];
            }
            
            public static dynamic[] Values(dynamic obj)
            {
                if (obj == null || obj is UndefinedValue)
                    return new dynamic[0];
                
                if (obj is IDictionary<string, object> dict)
                    return dict.Values.ToArray();
                else if (obj is ExpandoObject expando)
                {
                    var dict2 = (IDictionary<string, object>)expando;
                    return dict2.Values.ToArray();
                }
                
                return new dynamic[0];
            }
            
            public static dynamic Freeze(dynamic obj)
            {
                return obj;
            }
            
            public static dynamic Seal(dynamic obj)
            {
                return obj;
            }
            
            public static bool Is(dynamic obj1, dynamic obj2)
            {
                return StrictEquals(obj1, obj2);
            }
        }
        
        public static class Array
        {
            public static dynamic From(dynamic iterable, dynamic mapFn = null)
            {
                var result = new List<dynamic>();
                
                if (iterable == null || iterable is UndefinedValue)
                    return result.ToArray();
                
                if (iterable is System.Array arr)
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        var item = arr.GetValue(i);
                        result.Add(mapFn != null ? mapFn(item, i) : item);
                    }
                }
                else if (iterable is IEnumerable<dynamic> enumerable)
                {
                    int i = 0;
                    foreach (var item in enumerable)
                    {
                        result.Add(mapFn != null ? mapFn(item, i++) : item);
                    }
                }
                
                return result.ToArray();
            }
            
            public static dynamic Of(params dynamic[] items)
            {
                return items;
            }
            
            public static bool IsArray(dynamic obj)
            {
                return obj != null && (obj is System.Array || obj is IList<dynamic>);
            }
        }
        
        public static class JSON
        {
            public static string Stringify(dynamic value, dynamic replacer = null, dynamic space = null)
            {
                return System.Text.Json.JsonSerializer.Serialize(value);
            }
            
            public static dynamic Parse(string text)
            {
                return System.Text.Json.JsonSerializer.Deserialize<dynamic>(text);
            }
        }
        
        public static class BigInt
        {
            public static BigInteger From(dynamic value)
            {
                if (value is BigInteger bi) return bi;
                if (value is string s) return BigInteger.Parse(s);
                if (IsNumericType(value.GetType()))
                    return new BigInteger(Convert.ToInt64(value));
                throw new ArgumentException($"Cannot convert {value} to BigInt");
            }
        }
    }
}