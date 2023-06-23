using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class UpdateGameplayVariables
{
    BindingFlags intanceDeclaredOnlyFileter = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Static;

    public void ProcessGameplayMessage(byte[] message, object obj)
    {
        string fieldPath = ReadFilePath(message);
        
        string className = "";
        string fieldName = "";

        int pathIndex = fieldPath.IndexOf("///");

        if (pathIndex >= 0)
        {
            className = fieldPath.Substring(0, pathIndex);
            fieldName = fieldPath.Substring(pathIndex + 3); // +3 para omitir "///"
        }

        object currentObj = obj;
        bool isCollection = false;

        if (className.Contains("Key") || className.Contains("Value")) // Diccionario
        {
            isCollection = true;
            string DictionaryName = className.Substring(0, className.IndexOf("Key") != -1 ? className.IndexOf("Key") : className.IndexOf("Value"));
           
            int index;
            string variableName;
            ParseFilePath(fieldPath, out index, out variableName);

            ReadFieldDictionary(currentObj, DictionaryName, index, variableName, message);
        }
        else if (className.Contains("[") && className.Contains("]")) // Coleccion
        {
            isCollection = true;
            Debug.Log("Es una lista");
        }
        else
        {
            currentObj = ReadField(currentObj, className);
        }

        if (currentObj == null)
        {
            // Manejo de error si el campo no existe o no se puede leer
        }

        if (!isCollection)
        {
            FieldInfo field = currentObj.GetType().GetField(fieldName, intanceDeclaredOnlyFileter);
            ModifyVariable(currentObj, message, field);
        }

    }

    object ReadFieldDictionary(object obj, string dictionaryName, int index, string nameField, byte[] message)
    {
        if (nameField == null)
        {
            //Es una key
            return null;
        }

        FieldInfo field = obj.GetType().GetField(dictionaryName, intanceDeclaredOnlyFileter);

        if (field != null && typeof(IDictionary).IsAssignableFrom(field.FieldType))
        {
            modifyDictionary(message, field, index, nameField, obj);
        }
        return null;
    }
    void modifyDictionary(byte[] message, FieldInfo field, int index, string nameField, object obj)
    {                          //Field del diccionario, ruta de la variable, nombre de la variable, instancia a cambiar


        // Verificar si el campo es un diccionario
        if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            // Obtener el valor del campo
            object fieldValue = field.GetValue(obj);

            if (fieldValue != null && fieldValue is IDictionary dictionary)
            {
                // Obtener el enumerador de claves y valores
                var keyEnumerator = dictionary.Keys.GetEnumerator();
                var valueEnumerator = dictionary.Values.GetEnumerator();

                // Mover el enumerador al índice deseado
                for (int i = 0; i <= index; i++)
                {
                    if (!keyEnumerator.MoveNext() || !valueEnumerator.MoveNext())
                    {
                        // El índice está fuera de rango
                        return;
                    }
                }

                // Obtener la clave y el valor correspondientes al índice
                var key = keyEnumerator.Current;
                var value = valueEnumerator.Current;

                Debug.Log(key);
                Debug.Log(value);

                FieldInfo fieldInfo = value.GetType().GetField(nameField, intanceDeclaredOnlyFileter);
                ModifyVariable(value, message, fieldInfo);

                // Realizar las operaciones deseadas con la clave y el valor obtenidos
                // Por ejemplo, puedes usar key y value en la función setValue()

                // setValue(key, value);
            }
        }
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

    void ModifyVariable(object obj, byte[] message, FieldInfo field)
    {
        object value = field.GetValue(obj);

        if (MessageChecker.Instance.CheckClientId(message) == NetworkManager.Instance.actualClientId)
        {
            return;
        }

        if (value is int)
        {
            field.SetValue(obj, message.ToInt());
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
            Debug.Log(value);
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

    void ParseFilePath(string fieldPath, out int index, out string variableName)
    {
        index = 0;
        variableName = "";

        int indexStart = fieldPath.IndexOf('[');
        int indexEnd = fieldPath.IndexOf(']');

        if (indexStart >= 0 && indexEnd >= 0 && indexEnd > indexStart + 1)
        {
            string indexStr = fieldPath.Substring(indexStart + 1, indexEnd - indexStart - 1);
            if (int.TryParse(indexStr, out index))
            {
                variableName = fieldPath.Substring(indexEnd + 4); // +4 para omitir "]///"
            }
            else
            {
                // No se pudo convertir el número entre corchetes en un int
            }
        }
        else
        {
            // No se encontraron corchetes o están en una posición incorrecta en 'fieldPath'
        }
    }
}
