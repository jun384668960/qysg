using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace register_client
{
    public enum CMD_E
    {
        CMD_REGISTER = 1,
        CMD_GET_VER = 2,
        CMD_NONE
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]  //变量在内存中的对齐方式
    public struct Tran_Head
    {
        public int length;
        public int cmd;
    }
    public struct Rg_Info
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string name;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string passwd;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string key;
    };
    public struct Ver_Info
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string ver;
    };

    public class CDataBuil
    {
        public static byte[] BuildBytes<T>(Tran_Head hd, T obj)
        {
            byte[] head = CStructBytesFormat.StructToBytes(hd);
            byte[] data = CStructBytesFormat.StructToBytes(obj);

            byte[] rbyte = new byte[data.Length + head.Length];

            Array.ConstrainedCopy(head, 0, rbyte, 0, head.Length);
            Array.ConstrainedCopy(data, 0, rbyte, head.Length, data.Length);

            return rbyte;
        }

        public static Tran_Head GetDataHead(byte[] data)
        {
            Tran_Head head = new Tran_Head();

            byte[] rcv_head = new byte[Marshal.SizeOf(head)];
            Array.ConstrainedCopy(data, 0, rcv_head, 0, Marshal.SizeOf(head));
            head = (Tran_Head)CStructBytesFormat.BytesToStruct<Tran_Head>(rcv_head);

            return head;
        }

        public static T GetDataObj<T>(byte[] data, Tran_Head head)
        {
            if (data == null) return default(T);
            if (data.Length <= 0) return default(T);
            int objLength = Marshal.SizeOf(typeof(T));

            byte[] rcv_data = new byte[head.length];
            T obj;
            Array.ConstrainedCopy(data, Marshal.SizeOf(head), rcv_data, 0, rcv_data.Length);
            obj = (T)CStructBytesFormat.BytesToStruct<T>(rcv_data);

            return obj;
        }
    };
}
