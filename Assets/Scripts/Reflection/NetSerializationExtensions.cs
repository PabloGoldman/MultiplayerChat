using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NetSerializationExtensions
{

    public static List<byte> ToMsg(this int intValue, char[] fieldName)
    {
        List<byte> output = new List<byte>();

        output.AddRange(BitConverter.GetBytes((int)MessageType.Gameplay));
        output.AddRange(BitConverter.GetBytes(NetworkManager.Instance.actualClientId));

        output.AddRange(BitConverter.GetBytes(fieldName.Length));

        for (int i = 0; i < fieldName.Length; i++)
        {
            output.AddRange(BitConverter.GetBytes(fieldName[i]));
        }

        output.AddRange(BitConverter.GetBytes(intValue));

        //La cola qe nunca va a existir.

        return output;
    }

    public static int ToInt(this byte[] arrayByte)
    {
        int fieldNameLength;
        fieldNameLength = BitConverter.ToInt32(arrayByte, 8);

        int value = BitConverter.ToInt32(arrayByte, 3 * sizeof(int) + fieldNameLength * sizeof(char));
        
        return value;
    }




}



