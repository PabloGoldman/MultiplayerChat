using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class Reflection : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    BindingFlags intanceDeclaredOnlyFileter = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

    private void Update()
    {
        if (!NetworkManager.Instance.isServer && NetworkManager.Instance.connection != null )
        {

            List<List<byte>> GameManagerAsByte = Inspect(gameManager, typeof(GameManager));

            // Recorro GameManagerAsByte y envio los paquetes
            foreach (List<byte> list in GameManagerAsByte)
            {
                NetworkManager.Instance.SendToServer(list.ToArray());
            }
        }
    }

    private List<List<byte>> Inspect(object obj, Type type, string fieldName = "")
    {
        List<List<byte>> output = new List<List<byte>>();

        foreach (FieldInfo field in type.GetFields(intanceDeclaredOnlyFileter))
        {
            IEnumerable<Attribute> attributes = field.GetCustomAttributes();

            foreach (Attribute attribute in attributes)
            {
                if (attribute is NetAttribute)
                {
                    object value = field.GetValue(obj);

                    if (typeof(IEnumerable).IsAssignableFrom(value.GetType()))
                    {
                        if (typeof(IDictionary).IsAssignableFrom(value.GetType()))
                        {
                            int i = 0;
                            foreach (DictionaryEntry entry in (value as IDictionary))
                            {
                                ConvertToMsg(output, entry.Key, fieldName + field.Name + "Key [ " + i.ToString() + "]");
                                ConvertToMsg(output, entry.Value, fieldName + field.Name + "Value [ " + i.ToString() + "]");
                                i++;
                            }
                        }
                        else if (typeof(ICollection).IsAssignableFrom(value.GetType()))
                        {
                            int i = 0;
                            foreach (object element in (value as ICollection))
                            {
                                ConvertToMsg(output, element, field.Name + "[" + i.ToString() + "]");
                                i++;
                            }
                        }
                    }
                    else
                    {
                        ConvertToMsg(output, value, fieldName + field.Name);
                    }
                }
            }

            //if (typeof(INet).IsAssignableFrom(type))   //Para que el usuario mande cosas a serializar
            //{
            //    ConvertToMsg(output, (type as INet).Serialize(), fieldName);
            //}

        }

        if (type.BaseType != null)
        {
            foreach (List<byte> msg in Inspect(obj, type.BaseType, fieldName))
            {
                output.Add(msg);
            }
        }

        return output;
    }

    private void ConvertToMsg(List<List<byte>> MsgStack, object obj, string fieldName)
    {
        char[] fielNameChar = fieldName.ToCharArray();

        if (obj is int)
        {
            MsgStack.Add(((int)obj).ToMsg(fielNameChar));
        }

        else if (obj is float)
        {
            Debug.Log(fieldName + ": " + (float)obj);
        }

        else if (obj is bool)
        {
            Debug.Log(fieldName + ": " + (bool)obj);
        }
        else if (obj is char)
        {
            Debug.Log(fieldName + ": " + (char)obj);
        }
        else if (obj is Vector3)
        {
            Debug.Log(fieldName + ": " + (Vector3)obj);
        }
        else if (obj is byte[])
        {

        }
        else
        {
            foreach (List<byte> msg in Inspect(obj, obj.GetType(), fieldName + "///"))
            {
                MsgStack.Add(msg);
            }
        }
    }
}




