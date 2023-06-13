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

    public static List<byte> ToMsg(this Vector3 vector3Value, char[] fieldName)
    {
        List<byte> output = new List<byte>();

        AddHeaderMessage(output, fieldName);

        output.AddRange(BitConverter.GetBytes(vector3Value.x));
        output.AddRange(BitConverter.GetBytes(vector3Value.y));
        output.AddRange(BitConverter.GetBytes(vector3Value.z));

        return output;
    }

    public static List<byte> ToMsg(this Vector2 vector2Value, char[] fieldName)
    {
        List<byte> output = new List<byte>();

        AddHeaderMessage(output, fieldName);

        output.AddRange(BitConverter.GetBytes(vector2Value.x));
        output.AddRange(BitConverter.GetBytes(vector2Value.y));

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

        AddHeaderMessage(output, fieldName);

        output.AddRange(BitConverter.GetBytes(colorValue.r));
        output.AddRange(BitConverter.GetBytes(colorValue.g));
        output.AddRange(BitConverter.GetBytes(colorValue.b));
        output.AddRange(BitConverter.GetBytes(colorValue.a));

        return output;
    }

    public static int ToInt(this byte[] arrayByte)
    {
        int fieldNameLength = BitConverter.ToInt32(arrayByte, 8);
        int value = BitConverter.ToInt32(arrayByte, 3 * sizeof(int) + fieldNameLength * sizeof(char));

        return value;
    }

    public static char ToChar(this byte[] arrayByte)
    {
        int fieldNameLength = BitConverter.ToInt32(arrayByte, 8);
        char value = BitConverter.ToChar(arrayByte, 3 * sizeof(int) + fieldNameLength * sizeof(char));

        return value;
    }

    public static float ToFloat(this byte[] arrayByte)
    {
        int fieldNameLength = BitConverter.ToInt32(arrayByte, 8);
        float value = BitConverter.ToSingle(arrayByte, 3 * sizeof(int) + fieldNameLength * sizeof(char));

        return value;
    }

    public static bool ToBool(this byte[] arrayByte)
    {
        int fieldNameLength = BitConverter.ToInt32(arrayByte, 8);
        bool value = BitConverter.ToBoolean(arrayByte, 3 * sizeof(int) + fieldNameLength * sizeof(char));

        return value;
    }

    public static string ToExtendedString(this byte[] arrayByte)
    {
        int fieldNameLength = BitConverter.ToInt32(arrayByte, 8);
        string value = BitConverter.ToString(arrayByte, 3 * sizeof(int) + fieldNameLength * sizeof(char));

        return value;
    }

    public static Vector2 ToVector2(this byte[] arrayByte)
    {
        int fieldNameLength = BitConverter.ToInt32(arrayByte, 8);
        int positionToRead = 3 * sizeof(int) + fieldNameLength * sizeof(char);

        float x = BitConverter.ToSingle(arrayByte, positionToRead);
        float y = BitConverter.ToSingle(arrayByte, positionToRead + sizeof(float));

        Vector2 value = new Vector2(x, y);

        return value;
    }

    public static Vector3 ToVector3(this byte[] arrayByte)
    {
        int fieldNameLength = BitConverter.ToInt32(arrayByte, 8);
        int positionToRead = 3 * sizeof(int) + fieldNameLength * sizeof(char);

        float x = BitConverter.ToSingle(arrayByte, positionToRead);
        float y = BitConverter.ToSingle(arrayByte, positionToRead + sizeof(float));
        float z = BitConverter.ToSingle(arrayByte, positionToRead + 2 * sizeof(float));

        Vector3 value = new Vector3(x, y, z);

        return value;
    }

    public static Quaternion ToQuaternion(this byte[] arrayByte)
    {
        int fieldNameLength = BitConverter.ToInt32(arrayByte, 8);
        int positionToRead = 3 * sizeof(int) + fieldNameLength * sizeof(char);

        float x = BitConverter.ToSingle(arrayByte, positionToRead);
        float y = BitConverter.ToSingle(arrayByte, positionToRead + sizeof(float));
        float z = BitConverter.ToSingle(arrayByte, positionToRead + 2 * sizeof(float));
        float w = BitConverter.ToSingle(arrayByte, positionToRead + 3 * sizeof(float));

        Quaternion value = new Quaternion(x, y, z, w);

        return value;
    }

    public static Color ToColor(this byte[] arrayByte)
    {
        int fieldNameLength = BitConverter.ToInt32(arrayByte, 8);
        int positionToRead = 3 * sizeof(int) + fieldNameLength * sizeof(char);

        float r = BitConverter.ToSingle(arrayByte, positionToRead);
        float g = BitConverter.ToSingle(arrayByte, positionToRead + sizeof(float));
        float b = BitConverter.ToSingle(arrayByte, positionToRead + 2 * sizeof(float));
        float a = BitConverter.ToSingle(arrayByte, positionToRead + 3 * sizeof(float));

        Color value = new Color(r, g, b, a);

        return value;
    }

    public static void SetTransformFromBytes(this Transform transform, byte[] byteArray)
    {
        int positionToRead = 0;

        float posX = BitConverter.ToSingle(byteArray, positionToRead);
        float posY = BitConverter.ToSingle(byteArray, positionToRead + sizeof(float));
        float posZ = BitConverter.ToSingle(byteArray, positionToRead + 2 * sizeof(float));
        Vector3 position = new Vector3(posX, posY, posZ);

        positionToRead += 3 * sizeof(float);

        float rotX = BitConverter.ToSingle(byteArray, positionToRead);
        float rotY = BitConverter.ToSingle(byteArray, positionToRead + sizeof(float));
        float rotZ = BitConverter.ToSingle(byteArray, positionToRead + 2 * sizeof(float));
        float rotW = BitConverter.ToSingle(byteArray, positionToRead + 3 * sizeof(float));
        Quaternion rotation = new Quaternion(rotX, rotY, rotZ, rotW);

        positionToRead += 4 * sizeof(float);

        float scaleX = BitConverter.ToSingle(byteArray, positionToRead);
        float scaleY = BitConverter.ToSingle(byteArray, positionToRead + sizeof(float));
        float scaleZ = BitConverter.ToSingle(byteArray, positionToRead + 2 * sizeof(float));
        Vector3 scale = new Vector3(scaleX, scaleY, scaleZ);

        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;
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