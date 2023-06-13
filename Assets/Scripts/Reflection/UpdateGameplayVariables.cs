using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class UpdateGameplayVariables 
{

    BindingFlags intanceDeclaredOnlyFileter = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static;


    public void ProcessGameplayMessage(byte[] message, object obj, Type type)
    {
        string fieldPath = ReadFilePath(message);

        string[] fields = fieldPath.Split(new[] { "///" }, StringSplitOptions.RemoveEmptyEntries);
        string lastFieldName = "";

        object currentObj = GameManager.Instance;
        object previousObj = null;

        foreach (string fieldName in fields)
        {
            previousObj = currentObj;
            currentObj = ReadField(currentObj, fieldName);

            if (currentObj == null)
            {
                // Manejo de error si el campo no existe o no se puede leer
                break;
            }

            lastFieldName = fieldName;
        }

        ModifyVariable(previousObj, currentObj, message, lastFieldName);
    }

    object ReadField(object obj, string name)
    {
        FieldInfo field = obj.GetType().GetField(name, intanceDeclaredOnlyFileter);

        if (field == null)
        {
            return null;
        }

        IEnumerable<Attribute> attributes = field.GetCustomAttributes();

        foreach (Attribute attribute in attributes)
        {
            object value = field.GetValue(obj);
            return value;
        }

        return null;
    }

    void ModifyVariable(object prevObj, object obj, byte[] message, string fieldName)
    {
        FieldInfo field = prevObj.GetType().GetField(fieldName, intanceDeclaredOnlyFileter);


        if (field != null)
        {
            if (obj is int)
            {
                field.SetValue(prevObj, message.ToInt());
                Debug.Log("llega un int");
            }
            else if (obj is float)
            {
                Debug.Log("llega un flaot");
            }
            else if (obj is bool)
            {
                Debug.Log("llega un bool");
            }
            else if (obj is char)
            {
                Debug.Log("llega un char");
            }
            else if (obj is Vector3)
            {
                Debug.Log("llega un vector3");
            }
            else if (obj is byte[])
            {
                Debug.Log("llega un arrayByte");

            }
            else
            {
                // foreach (List<byte> msg in Inspect(obj, obj.GetType(), fieldName + "///"))
                {
                    //                MsgStack.Add(msg);
                }
            }

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