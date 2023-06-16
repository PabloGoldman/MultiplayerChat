using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class UpdateGameplayVariables
{

    BindingFlags intanceDeclaredOnlyFileter = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static;

    public void ProcessGameplayMessage(byte[] message, object obj)
    {
        string fieldPath = ReadFilePath(message);

        string[] fields = fieldPath.Split(new[] { "///" }, StringSplitOptions.RemoveEmptyEntries);
        string lastFieldName = "";

        object currentObj = obj;
        object previousObj = null;

        foreach (string fieldName in fields)
        {
            previousObj = currentObj;

            if (fieldName.Contains("Key") || fieldName.Contains("Value"))
            {
                Debug.Log("Es un diccionario");

                string substring = fieldName.Substring(0, fieldName.IndexOf("Key") != -1 ? fieldName.IndexOf("Key") : fieldName.IndexOf("Value"));
                ReadFieldDictionary(currentObj, substring, fieldName, message);
            }
            else if (fieldName.Contains("[") && fieldName.Contains("]"))
            {
                Debug.Log("Es una lista");
            }
            else
            {
                currentObj = ReadField(currentObj, fieldName);
            }

            if (currentObj == null)
            {
                // Manejo de error si el campo no existe o no se puede leer
                break;
            }

            lastFieldName = fieldName;
        }

        ModifyVariable(previousObj, currentObj, message, lastFieldName);
    }

    object ReadFieldDictionary(object obj, string dictionaryName, string name, byte[] message)
    {
        FieldInfo field = obj.GetType().GetField(dictionaryName, intanceDeclaredOnlyFileter);

        if (field != null && typeof(IDictionary).IsAssignableFrom(field.FieldType))
        {
            modifyDictionary(message, field, obj);
        }
            return null;
    }
    void modifyDictionary(byte[] message, FieldInfo field, object obj)
    {
        //hay que traducir el mensaje y setearlo en el obj
        Debug.Log("es un diccionario");
    }

    object ReadFieldCollection(object obj, string name)
    {

        return null;
    }
    object ReadField(object obj, string name)
    {
        FieldInfo field = obj.GetType().GetField(name, intanceDeclaredOnlyFileter);

        if (field == null)
        {
            return null;
        }

        object value = field.GetValue(obj);
        return value;
    }

    void ModifyVariable(object prevObj, object obj, byte[] message, string fieldName)
    {
        FieldInfo field = prevObj.GetType().GetField(fieldName, intanceDeclaredOnlyFileter);

        object value = field.GetValue(prevObj);

        if (MessageChecker.Instance.CheckClientId(message) == NetworkManager.Instance.actualClientId)
        {
            return;
        }


        if (value is int)
        {
            field.SetValue(prevObj, message.ToInt());
            Debug.Log("llega un int");
        }
        else if (value is float)
        {
            Debug.Log("llega un flaot");
        }
        else if (value is bool)
        {
            Debug.Log("llega un bool");
        }
        else if (value is char)
        {
            Debug.Log("llega un char");
        }
        else if (value is Vector3)
        {
            Debug.Log("llega un vector3");
        }
        else if (value is byte[])
        {
            Debug.Log("llega un arrayByte");
        }
        else
        {

        }

    }


    string ReadFilePath(byte[] message)
    {
        int fieldNameLength = BitConverter.ToInt32(message, 8);
        char[] fieldName = new char[fieldNameLength];

        for (int i = 0; i < fieldNameLength; i++)
        {
            fieldName[i] = BitConverter.ToChar(message, (3 * sizeof(int) + i * sizeof(char)));
        }

        return new string(fieldName);
    }
}
