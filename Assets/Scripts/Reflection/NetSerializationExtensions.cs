using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetSerializationExtensions
{
    public static List<byte> ToMsg(this int intValue, char[] fieldName)
    {
        List<byte> output = new List<byte>();

        AddHeaderMessage(output, fieldName);

        output.AddRange(BitConverter.GetBytes(intValue));

        //La cola qe nunca va a existir.

        return output;
    }
    public static List<byte> ToMsg(this bool boolValue, char[] fieldName)
    {
        List<byte> output = new List<byte>();

        AddHeaderMessage(output, fieldName);

        output.AddRange(BitConverter.GetBytes(boolValue));

        return output;
    }
    public static List<byte> ToMsg(this char charValue, char[] fieldName)
    {
        List<byte> output = new List<byte>();

        AddHeaderMessage(output, fieldName);

        output.AddRange(BitConverter.GetBytes(charValue));

        return output;
    }
    public static List<byte> ToMsg(this string stringValue, char[] fieldName)
    {
        List<byte> output = new List<byte>();


        AddHeaderMessage(output, fieldName);

        for (int i = 0; i < stringValue.Length; i++)
        {
            output.AddRange(BitConverter.GetBytes(stringValue[i]));
        }

        return output;
    }
    public static List<byte> ToMsg(this Vector3 Vector3Value, char[] fieldName)
    {
        List<byte> output = new List<byte>();

        AddHeaderMessage(output, fieldName);

        output.AddRange(BitConverter.GetBytes(Vector3Value.x));
        output.AddRange(BitConverter.GetBytes(Vector3Value.y));
        output.AddRange(BitConverter.GetBytes(Vector3Value.z));

        return output;
    }
    public static List<byte> ToMsg(this Vector2 Vector2Value, char[] fieldName)
    {
        List<byte> output = new List<byte>();

        AddHeaderMessage(output, fieldName);

        output.AddRange(BitConverter.GetBytes(Vector2Value.x));
        output.AddRange(BitConverter.GetBytes(Vector2Value.y));

        return output;
    }
    public static List<byte> ToMsg(this Quaternion quaternionValue, char[] fieldName)
    {
        List<byte> output = new List<byte>();
        
        AddHeaderMessage(output, fieldName);

        output.AddRange(BitConverter.GetBytes(quaternionValue.x));
        output.AddRange(BitConverter.GetBytes(quaternionValue.y));
        output.AddRange(BitConverter.GetBytes(quaternionValue.z));
        output.AddRange(BitConverter.GetBytes(quaternionValue.w));


        return output;
    }
    public static List<byte> ToMsg(this Transform transformValue, char[] fieldName)
    {
        List<byte> output = new List<byte>();

        AddHeaderMessage(output, fieldName);

        output.AddRange(BitConverter.GetBytes(transformValue.position.x));
        output.AddRange(BitConverter.GetBytes(transformValue.position.y));
        output.AddRange(BitConverter.GetBytes(transformValue.position.z));

        output.AddRange(BitConverter.GetBytes(transformValue.rotation.x));
        output.AddRange(BitConverter.GetBytes(transformValue.rotation.y));
        output.AddRange(BitConverter.GetBytes(transformValue.rotation.z));
        output.AddRange(BitConverter.GetBytes(transformValue.rotation.w));

        output.AddRange(BitConverter.GetBytes(transformValue.localScale.x));
        output.AddRange(BitConverter.GetBytes(transformValue.localScale.y));
        output.AddRange(BitConverter.GetBytes(transformValue.localScale.z));

        return output;
    }
    public static List<byte> ToMsg(this Color colorValue, char[] fieldName)
    {
        List<byte> output = new List<byte>();

        output.AddRange(BitConverter.GetBytes(colorValue.r));
        output.AddRange(BitConverter.GetBytes(colorValue.g));
        output.AddRange(BitConverter.GetBytes(colorValue.b));
        output.AddRange(BitConverter.GetBytes(colorValue.a));

        return output;
    }

    public static int ToInt(this byte[] arrayByte)
    {
        int fieldNameLength;
        fieldNameLength = BitConverter.ToInt32(arrayByte, 8);

        int value = BitConverter.ToInt32(arrayByte, 3 * sizeof(int) + fieldNameLength * sizeof(char));
        
        return value;
    }

    public static char ToChar(this byte[] arrayByte)
    {
        int fieldNameLength;
        fieldNameLength = BitConverter.ToInt32(arrayByte, 8);

        char value = BitConverter.ToChar(arrayByte, 3 * sizeof(int) + fieldNameLength * sizeof(char));

        return value;
    }
    
    public static float ToFloat(this byte[] arrayByte)
    {
        int fieldNameLength;
        fieldNameLength = BitConverter.ToInt16(arrayByte, 8);

        float value = BitConverter.ToSingle(arrayByte, 3 * sizeof(int) + fieldNameLength * sizeof(char));

        return value;
    }
    
    public static bool ToBool(this byte[] arrayByte)
    {
        int fieldNameLength;
        fieldNameLength = BitConverter.ToInt16(arrayByte, 8);

        bool value = BitConverter.ToBoolean(arrayByte, 3 * sizeof(int) + fieldNameLength * sizeof(char));

        return value;
    }

    public static string ToExtendedString(this byte[] arrayByte)
    {
        int fieldNameLength;
        fieldNameLength = BitConverter.ToInt16(arrayByte, 8);

        string value = BitConverter.ToString(arrayByte, 3 * sizeof(int) + fieldNameLength * sizeof(char));

        return value;
    }

    public static Vector2 ToVector2(this byte[] arrayByte)
    {
        int fieldNameLength;
        fieldNameLength = BitConverter.ToInt32(arrayByte, 8);

        int positionToRead = 3 * sizeof(int) + fieldNameLength * sizeof(char);

        Vector2 value = new Vector2(BitConverter.ToInt32(arrayByte, positionToRead), 
                                    BitConverter.ToInt32(arrayByte, positionToRead + sizeof(int)));

        return value;
    }

    public static Vector3 ToVector3(this byte[] arrayByte)
    {
        int fieldNameLength;
        fieldNameLength = BitConverter.ToInt32(arrayByte, 8);

        int positionToRead = 3 * sizeof(int) + fieldNameLength * sizeof(char);

        Vector3 value = new Vector3(BitConverter.ToInt32(arrayByte, positionToRead),
                                    BitConverter.ToInt32(arrayByte, positionToRead + sizeof(int)),
                                    BitConverter.ToInt32(arrayByte, positionToRead + 2 * sizeof(int)));

        return value;
    }
    public static Quaternion ToQuaternion(this byte[] arrayByte)
    {
        int fieldNameLength;
        fieldNameLength = BitConverter.ToInt32(arrayByte, 8);

        int positionToRead = 3 * sizeof(int) + fieldNameLength * sizeof(char);

        Quaternion value = new Quaternion(
                               BitConverter.ToSingle(arrayByte, positionToRead),
                               BitConverter.ToSingle(arrayByte, positionToRead + sizeof(int)),
                               BitConverter.ToSingle(arrayByte, positionToRead + 2 * sizeof(int)),
                               BitConverter.ToSingle(arrayByte, positionToRead) + 3 *sizeof(int));

        return value;
    }

    public static Color ToColor(this byte[] arrayByte)
    {
        int fieldNameLength;
        fieldNameLength = BitConverter.ToInt32(arrayByte, 8);

        int positionToRead = 3 * sizeof(int) + fieldNameLength * sizeof(char);

        Color value = new Color(
                               BitConverter.ToSingle(arrayByte, positionToRead),
                               BitConverter.ToSingle(arrayByte, positionToRead + sizeof(int)),
                               BitConverter.ToSingle(arrayByte, positionToRead + 2 * sizeof(int)),
                               BitConverter.ToSingle(arrayByte, positionToRead) + 3 * sizeof(int));

        return value;
    }

    public static Transform ToTransform(this byte[] arrayByte)
    {
        int fieldNameLength;
        fieldNameLength = BitConverter.ToInt32(arrayByte, 8);
        int positionToRead = 3 * sizeof(int) + fieldNameLength * sizeof(char);


        Vector3 position = new Vector3(
            BitConverter.ToSingle(arrayByte, positionToRead),
            BitConverter.ToSingle(arrayByte, positionToRead + sizeof(float)),
            BitConverter.ToSingle(arrayByte, positionToRead + 2 * sizeof(float)));

        positionToRead += 3 * sizeof(float);

        Quaternion rotation = new Quaternion(
            BitConverter.ToSingle(arrayByte, positionToRead),
            BitConverter.ToSingle(arrayByte, positionToRead + sizeof(float)),
            BitConverter.ToSingle(arrayByte, positionToRead + 2 * sizeof(float)),
            BitConverter.ToSingle(arrayByte, positionToRead + 3 * sizeof(float)));

        positionToRead += 4 * sizeof(float);

        Vector3 scale = new Vector3(
            BitConverter.ToSingle(arrayByte, positionToRead),
            BitConverter.ToSingle(arrayByte, positionToRead + sizeof(float)),
            BitConverter.ToSingle(arrayByte, positionToRead + 2 * sizeof(float)));

        Transform value;

        value.position = position;
        value.rotation = rotation;
        value.localScale = scale;

        return value;
    }
}



    static void AddHeaderMessage(List<byte> output, char[] fieldName)
    {
        output.AddRange(BitConverter.GetBytes((int)MessageType.Gameplay));
        output.AddRange(BitConverter.GetBytes(NetworkManager.Instance.actualClientId));

        output.AddRange(BitConverter.GetBytes(fieldName.Length));

        for (int i = 0; i < fieldName.Length; i++)
        {
            output.AddRange(BitConverter.GetBytes(fieldName[i]));
        }
    }
}



