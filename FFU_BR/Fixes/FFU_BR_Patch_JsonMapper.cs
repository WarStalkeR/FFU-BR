using System.Collections;
using System.Reflection;
using System;
using UnityEngine;
using MonoMod;

namespace LitJson {
    public partial class patch_JsonMapper : JsonMapper {
        [MonoModReplace] private static object ReadValue(Type inst_type, JsonReader reader) {
            reader.Read();
            if (bVerbose && reader.Value != null) {
                Debug.Log(reader.Value.ToString());
            }
            if (reader.Token == JsonToken.ArrayEnd) {
                return null;
            }
            Type underlyingType = Nullable.GetUnderlyingType(inst_type);
            Type type = underlyingType ?? inst_type;
            if (reader.Token == JsonToken.Null) {
                if (inst_type.IsClass || underlyingType != null) {
                    return null;
                }
                throw new JsonException($"Can't assign null to an instance of type {inst_type}");
            }
            if (reader.Token == JsonToken.Double || reader.Token == JsonToken.Int || reader.Token == JsonToken.Single || reader.Token == JsonToken.Long || reader.Token == JsonToken.String || reader.Token == JsonToken.Boolean) {
                Type type2 = reader.Value.GetType();
                if (type.IsAssignableFrom(type2)) {
                    return reader.Value;
                }
                if (custom_importers_table.TryGetValue(type2, out var value) && value.TryGetValue(type, out var value2)) {
                    return value2(reader.Value);
                }
                if (base_importers_table.TryGetValue(type2, out value) && value.TryGetValue(type, out value2)) {
                    return value2(reader.Value);
                }
                if (type.IsEnum) {
                    return Enum.ToObject(type, reader.Value);
                }
                MethodInfo convOp = GetConvOp(type, type2);
                if (convOp != null) {
                    return convOp.Invoke(null, new object[1] { reader.Value });
                }
                throw new JsonException($"Can't assign value '{reader.Value}' (type {type2}) to type {inst_type}");
            }
            object obj = null;
            if (reader.Token == JsonToken.ArrayStart) {
                AddArrayMetadata(inst_type);
                ArrayMetadata arrayMetadata = array_metadata[inst_type];
                if (!arrayMetadata.IsArray && !arrayMetadata.IsList) {
                    throw new JsonException($"Type {inst_type} can't act as an array");
                }
                IList list;
                Type elementType;
                if (!arrayMetadata.IsArray) {
                    list = (IList)Activator.CreateInstance(inst_type);
                    elementType = arrayMetadata.ElementType;
                } else {
                    list = new ArrayList();
                    elementType = inst_type.GetElementType();
                }
                while (true) {
                    object obj2 = ReadValue(elementType, reader);
                    if (obj2 == null && reader.Token == JsonToken.ArrayEnd) {
                        break;
                    }
                    list.Add(obj2);
                }
                if (arrayMetadata.IsArray) {
                    int count = list.Count;
                    obj = Array.CreateInstance(elementType, count);
                    for (int i = 0; i < count; i++) {
                        ((Array)obj).SetValue(list[i], i);
                    }
                } else {
                    obj = list;
                }
            } else if (reader.Token == JsonToken.ObjectStart) {
                AddObjectMetadata(type);
                ObjectMetadata objectMetadata = object_metadata[type];
                obj = Activator.CreateInstance(type);
                while (true) {
                    reader.Read();
                    if (reader.Token == JsonToken.ObjectEnd) {
                        break;
                    }
                    string text = (string)reader.Value;
                    if (objectMetadata.Properties.TryGetValue(text, out var value3)) {
                        if (value3.IsField) {
                            // Check if field is writable
                            FieldInfo fieldInfo = (FieldInfo)value3.Info;
                            if (!fieldInfo.IsLiteral) {
                                fieldInfo.SetValue(obj, ReadValue(value3.Type, reader));
                            } else {
                                ReadValue(value3.Type, reader);
                            }
                            // Switch to the next entry
                            continue;
                        }
                        PropertyInfo propertyInfo = (PropertyInfo)value3.Info;
                        if (propertyInfo.CanWrite) {
                            propertyInfo.SetValue(obj, ReadValue(value3.Type, reader), null);
                        } else {
                            ReadValue(value3.Type, reader);
                        }
                    } else if (!objectMetadata.IsDictionary) {
                        if (!reader.SkipNonMembers) {
                            throw new JsonException($"The type {inst_type} doesn't have the property '{text}'");
                        }
                        ReadSkip(reader);
                    } else {
                        ((IDictionary)obj).Add(text, ReadValue(objectMetadata.ElementType, reader));
                    }
                }
            }
            return obj;
        }
    }
}

// Reference Output: ILSpy v9.0.0.7660 / C# 11.0 / 2022.4

/* LitJson.JsonMapper.ReadValue
private static object ReadValue(Type inst_type, JsonReader reader)
{
	reader.Read();
	if (bVerbose && reader.Value != null)
	{
		Debug.Log(reader.Value.ToString());
	}
	if (reader.Token == JsonToken.ArrayEnd)
	{
		return null;
	}
	Type underlyingType = Nullable.GetUnderlyingType(inst_type);
	Type type = underlyingType ?? inst_type;
	if (reader.Token == JsonToken.Null)
	{
		if (inst_type.IsClass || underlyingType != null)
		{
			return null;
		}
		throw new JsonException($"Can't assign null to an instance of type {inst_type}");
	}
	if (reader.Token == JsonToken.Double || reader.Token == JsonToken.Int || reader.Token == JsonToken.Single || reader.Token == JsonToken.Long || reader.Token == JsonToken.String || reader.Token == JsonToken.Boolean)
	{
		Type type2 = reader.Value.GetType();
		if (type.IsAssignableFrom(type2))
		{
			return reader.Value;
		}
		if (custom_importers_table.TryGetValue(type2, out var value) && value.TryGetValue(type, out var value2))
		{
			return value2(reader.Value);
		}
		if (base_importers_table.TryGetValue(type2, out value) && value.TryGetValue(type, out value2))
		{
			return value2(reader.Value);
		}
		if (type.IsEnum)
		{
			return Enum.ToObject(type, reader.Value);
		}
		MethodInfo convOp = GetConvOp(type, type2);
		if (convOp != null)
		{
			return convOp.Invoke(null, new object[1] { reader.Value });
		}
		throw new JsonException($"Can't assign value '{reader.Value}' (type {type2}) to type {inst_type}");
	}
	object obj = null;
	if (reader.Token == JsonToken.ArrayStart)
	{
		AddArrayMetadata(inst_type);
		ArrayMetadata arrayMetadata = array_metadata[inst_type];
		if (!arrayMetadata.IsArray && !arrayMetadata.IsList)
		{
			throw new JsonException($"Type {inst_type} can't act as an array");
		}
		IList list;
		Type elementType;
		if (!arrayMetadata.IsArray)
		{
			list = (IList)Activator.CreateInstance(inst_type);
			elementType = arrayMetadata.ElementType;
		}
		else
		{
			list = new ArrayList();
			elementType = inst_type.GetElementType();
		}
		while (true)
		{
			object obj2 = ReadValue(elementType, reader);
			if (obj2 == null && reader.Token == JsonToken.ArrayEnd)
			{
				break;
			}
			list.Add(obj2);
		}
		if (arrayMetadata.IsArray)
		{
			int count = list.Count;
			obj = Array.CreateInstance(elementType, count);
			for (int i = 0; i < count; i++)
			{
				((Array)obj).SetValue(list[i], i);
			}
		}
		else
		{
			obj = list;
		}
	}
	else if (reader.Token == JsonToken.ObjectStart)
	{
		AddObjectMetadata(type);
		ObjectMetadata objectMetadata = object_metadata[type];
		obj = Activator.CreateInstance(type);
		while (true)
		{
			reader.Read();
			if (reader.Token == JsonToken.ObjectEnd)
			{
				break;
			}
			string text = (string)reader.Value;
			if (objectMetadata.Properties.TryGetValue(text, out var value3))
			{
				if (value3.IsField)
				{
					((FieldInfo)value3.Info).SetValue(obj, ReadValue(value3.Type, reader));
					continue;
				}
				PropertyInfo propertyInfo = (PropertyInfo)value3.Info;
				if (propertyInfo.CanWrite)
				{
					propertyInfo.SetValue(obj, ReadValue(value3.Type, reader), null);
				}
				else
				{
					ReadValue(value3.Type, reader);
				}
			}
			else if (!objectMetadata.IsDictionary)
			{
				if (!reader.SkipNonMembers)
				{
					throw new JsonException($"The type {inst_type} doesn't have the property '{text}'");
				}
				ReadSkip(reader);
			}
			else
			{
				((IDictionary)obj).Add(text, ReadValue(objectMetadata.ElementType, reader));
			}
		}
	}
	return obj;
}
*/