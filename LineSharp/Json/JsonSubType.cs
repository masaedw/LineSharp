using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// copied from http://stackoverflow.com/questions/11099466/using-a-custom-type-discriminator-to-tell-json-net-which-type-of-a-class-hierarc
namespace LineSharp.Json
{
    //  Discriminated Json Converter (JsonSubtypes) implementation for .NET
    //
    //  MIT License
    //
    //  Copyright (c) 2016 Anatoly Ressin

    //  Permission is hereby granted, free of charge, to any person obtaining a
    //  copy of this software and associated documentation files (the "Software"),
    //  to deal in the Software without restriction, including without limitation
    //  the rights to use, copy, modify, merge, publish, distribute, sublicense,
    //  and/or sell copies of the Software, and to permit persons to whom the
    //  Software is furnished to do so, subject to the following conditions:
    //
    //  The above copyright notice and this permission notice shall be included in
    //  all copies or substantial portions of the Software.
    //
    //  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
    //  OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    //  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    //  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    //  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    //  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
    //  DEALINGS IN THE SOFTWARE.

#if false
    ////////////////////// USAGE ////////////////////////////////////////////////////////////////////////////////

    [JsonConverter(typeof(JsonSubtypes))]     // Discriminated base class SHOULD NOT be abstract
    public class ShapeBase
    {
        [JsonTag, JsonProperty("@type")]      // it SHOULD contain a property marked with [JsonTag]
        public string Type { get; set; }         // only one [JsonTag] annotation allowed per discriminated class

        // it COULD contain other properties, however this is NOT RECOMMENDED
        // Rationale: instances of this class will be created at deserialization
        // only for tag sniffing, and then thrown away.
    }

    public abstract class Shape : ShapeBase
    {  // If you want abstract parent - extend the root
        public abstract double GetArea();     // with needed abstract stuff, then use this class everywhere (see DEMO below)
    }

    [JsonSubtype("circle")]                   // Every final class-case SHOULD be marked with [JsonSubtype(tagValue)]
    public class Circle : Shape
    {              // Two disctinct variant classes MUST have distinct tagValues
        [JsonProperty("super-radius")]        // You CAN use any Json-related annotation as well
        public double Radius { get; set; }
        public override double GetArea()
        {
            return Radius * Radius * Math.PI;
        }
    }

    [JsonSubtype("rectangle")]
    public class Rectangle : Shape
    {
        public double Height { get; set; }
        public double Width { get; set; }
        public override double GetArea()
        {
            return Width * Height;
        }
    }

    [JsonSubtype("group")]
    public class Group : Shape
    {
        [JsonProperty("shapes")]
        public List<Shape> Items { get; set; }
        public override double GetArea()
        {
            return Items.Select(item => item.GetArea()).Sum();
        }
    }

    ////////////////// DEMO /////////////////////////////////////////////////////////////////////////////////

    // Every final class-case SHOULD be registered with JsonSubtypes.register(typeof(YourConcreteClass))
    // either manually or with auto-register capability:
    // You can auto-register all classes marked with [JsonSubtype(tag)] in given Assembly
    // using JsonSubtypes.autoRegister(yourAssembly)

    public class Program
    {
        public static void Main()
        {
            JsonSubtypes.autoRegister(Assembly.GetExecutingAssembly());
            Shape original = new Group()
            {
                Items = new List<Shape> {
                new Circle() { Radius = 5 },
                new Rectangle() { Height = 10, Width = 20 }
            }
            };
            string str = JsonConvert.SerializeObject(original);
            Console.WriteLine(str);
            var copy = JsonConvert.DeserializeObject(str, typeof(Shape)) as Shape;

            // Note: we can deserialize object using any class from the hierarchy.
            // Under the hood, anyway, it will be deserialized using the top-most
            // base class annotated with [JsonConverter(typeof(JsonSubtypes))].
            // Thus, only soft-casts ("as"-style) are safe here.

            Console.WriteLine("original.area = {0}, copy.area = {1}", original.GetArea(), copy.GetArea());
        }
    }
#endif

    //////////////////////// IMPLEMENTATION //////////////////////////////////////////////////////////////////

    public class JsonSubtypeClashException : Exception
    {
        public string TagValue { get; private set; }
        public Type RootType { get; private set; }
        public Type OldType { get; private set; }
        public Type NewType { get; private set; }

        public JsonSubtypeClashException(Type rootType, string tagValue, Type oldType, Type newType) : base(
            String.Format(
                "JsonSubtype Clash for {0}[tag={1}]: oldType = {2}, newType = {3}",
                rootType.FullName,
                tagValue,
                oldType.FullName,
                newType.FullName
            )
        )
        {
            TagValue = tagValue;
            RootType = rootType;
            OldType = oldType;
            NewType = newType;
        }
    }

    public class JsonSubtypeNoRootException : Exception
    {
        public Type SubType { get; private set; }

        public JsonSubtypeNoRootException(Type subType) : base(
            String.Format(
                "{0} should be inherited from the class with the [JsonConverter(typeof(JsonSubtypes))] attribute",
                subType.FullName
            )
        )
        {
            SubType = subType;
        }
    }

    public class JsonSubtypeNoTagException : Exception
    {
        public Type SubType { get; private set; }

        public JsonSubtypeNoTagException(Type subType) : base(
            String.Format(
                @"{0} should have [JsonSubtype(""..."")] attribute",
                subType.FullName
            )
        )
        {
            SubType = subType;
        }
    }

    public class JsonSubtypeNotRegisteredException : Exception
    {
        public Type Root { get; private set; }
        public string TagValue { get; private set; }

        public JsonSubtypeNotRegisteredException(Type root, string tagValue) : base(
            String.Format(
                @"Unknown tag={1} for class {0}",
                root.FullName,
                tagValue
            )
        )
        {
            Root = root;
            TagValue = tagValue;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class JsonSubtypeAttribute : Attribute
    {
        private string tagValue;

        public JsonSubtypeAttribute(string tagValue)
        {
            this.tagValue = tagValue;
        }

        public string TagValue
        {
            get
            {
                return tagValue;
            }
        }
    }

    public static class JsonSubtypesExtension
    {
        public static bool TryGetAttribute<T>(this Type t, out T attribute) where T : Attribute
        {
            attribute = t.GetCustomAttributes(typeof(T), false).Cast<T>().FirstOrDefault();
            return attribute != null;
        }

        private static Dictionary<Type, PropertyInfo> tagProperties = new Dictionary<Type, PropertyInfo>();

        public static bool TryGetTagProperty(this Type t, out PropertyInfo tagProperty)
        {
            if (!tagProperties.TryGetValue(t, out tagProperty))
            {
                JsonConverterAttribute conv;
                if (t.TryGetAttribute(out conv) && conv.ConverterType == typeof(JsonSubtypes))
                {
                    var props = (from prop in t.GetProperties() where prop.GetCustomAttribute(typeof(JsonTagAttribute)) != null select prop).ToArray();
                    if (props.Length == 0) throw new Exception("No tag");
                    if (props.Length > 1) throw new Exception("Multiple tags");
                    tagProperty = props[0];
                }
                else
                {
                    tagProperty = null;
                }
                tagProperties[t] = tagProperty;
            }
            return tagProperty != null;
        }

        public static bool TryGetTagValue(this Type t, out string tagValue)
        {
            JsonSubtypeAttribute subtype;
            if (t.TryGetAttribute(out subtype))
            {
                tagValue = subtype.TagValue;
                return true;
            }
            else
            {
                tagValue = null;
                return false;
            }
        }

        public static bool TryGetJsonRoot(this Type t, out Type root, out PropertyInfo tagProperty)
        {
            root = t;
            do
            {
                if (root.TryGetTagProperty(out tagProperty))
                {
                    return true;
                }
                root = root.BaseType;
            } while (t != null);
            return false;
        }
    }

    public class JsonTagAttribute : Attribute
    {
    }

    public class JsonTagInfo
    {
        public PropertyInfo Property { get; set; }
        public string Value { get; set; }
    }

    public class JsonRootInfo
    {
        public PropertyInfo Property { get; set; }
        public Type Root { get; set; }
    }

    public abstract class DefaultJsonConverter : JsonConverter
    {
        [ThreadStatic]
        private static bool silentWrite;

        [ThreadStatic]
        private static bool silentRead;

        public sealed override bool CanWrite
        {
            get
            {
                var canWrite = !silentWrite;
                silentWrite = false;
                return canWrite;
            }
        }

        public sealed override bool CanRead
        {
            get
            {
                var canRead = !silentRead;
                silentRead = false;
                return canRead;
            }
        }

        protected void _WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
        {
            silentWrite = true;
            serializer.Serialize(writer, value);
        }

        protected Object _ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
        {
            silentRead = true;
            return serializer.Deserialize(reader, objectType);
        }
    }

    public class JsonSubtypes : DefaultJsonConverter
    {
        private static Dictionary<Type, Dictionary<string, Type>> implementations = new Dictionary<Type, Dictionary<string, Type>>();
        private static Dictionary<Type, JsonTagInfo> tags = new Dictionary<Type, JsonTagInfo>();
        private static Dictionary<Type, JsonRootInfo> roots = new Dictionary<Type, JsonRootInfo>();

        public static void register(Type newType)
        {
            PropertyInfo tagProperty;
            Type root;
            if (newType.TryGetJsonRoot(out root, out tagProperty))
            {
                for (var t = newType; t != root; t = t.BaseType)
                {
                    roots[t] = new JsonRootInfo()
                    {
                        Property = tagProperty,
                        Root = root
                    };
                }
                roots[root] = new JsonRootInfo()
                {
                    Property = tagProperty,
                    Root = root
                };
                Dictionary<string, Type> implementationMap;
                if (!implementations.TryGetValue(root, out implementationMap))
                {
                    implementationMap = new Dictionary<string, Type>();
                    implementations[root] = implementationMap;
                }
                JsonSubtypeAttribute attr;
                if (!newType.TryGetAttribute(out attr))
                {
                    throw new JsonSubtypeNoTagException(newType);
                }
                var tagValue = attr.TagValue;
                Type oldType;
                if (implementationMap.TryGetValue(tagValue, out oldType))
                {
                    throw new JsonSubtypeClashException(root, tagValue, oldType, newType);
                }
                implementationMap[tagValue] = newType;
                tags[newType] = new JsonTagInfo()
                {
                    Property = tagProperty,
                    Value = tagValue
                };
            }
            else
            {
                throw new JsonSubtypeNoRootException(newType);
            }
        }

        public static void autoRegister(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes().Where(type => type.GetCustomAttribute<JsonSubtypeAttribute>() != null))
            {
                register(type);
            }
        }

        public override bool CanConvert(Type t)
        {
            return true;
        }

        public static T EnsureTag<T>(T value)
        {
            JsonTagInfo tagInfo;
            if (tags.TryGetValue(value.GetType(), out tagInfo))
            {
                tagInfo.Property.SetValue(value, tagInfo.Value);
            }
            return value;
        }

        public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
        {
            _WriteJson(writer, EnsureTag(value), serializer);
        }

        public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
        {
            JsonTagInfo tagInfo;
            if (tags.TryGetValue(objectType, out tagInfo))
            {
                return _ReadJson(reader, objectType, existingValue, serializer);
            }
            else
            {
                JsonRootInfo rootInfo;
                if (roots.TryGetValue(objectType, out rootInfo))
                {
                    JToken t = JToken.ReadFrom(reader);
                    var stub = _ReadJson(t.CreateReader(), rootInfo.Root, existingValue, serializer);
                    var tagValue = rootInfo.Property.GetValue(stub) as string;
                    var implementationMap = implementations[rootInfo.Root];
                    Type implementation;
                    if (implementationMap.TryGetValue(tagValue, out implementation))
                    {
                        return ReadJson(t.CreateReader(), implementation, null, serializer);
                    }
                    else
                    {
                        throw new JsonSubtypeNotRegisteredException(rootInfo.Root, tagValue);
                    }
                }
                else
                {
                    return _ReadJson(reader, objectType, existingValue, serializer);
                }
            }
        }

        public static T Deserialize<T>(string s) where T : class
        {
            return JsonConvert.DeserializeObject(s, typeof(T)) as T;
        }

        public static string Serialize<T>(T value) where T : class
        {
            return JsonConvert.SerializeObject(value);
        }
    }
}
