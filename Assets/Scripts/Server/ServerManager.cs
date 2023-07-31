using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Net;
using System.Threading;

namespace PaperSouls.Server
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Packet
    {
        public uint packetType;
        public uint length;

        public Packet(uint packetType, uint length)
        {
            this.packetType = packetType;
            this.length = length;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    unsafe public struct Test
    {
        public fixed byte message[32];

        public Test(string message)
        {
            fixed (char* cmsg = message)
            {
                fixed (byte* data = this.message)
                {
                    Encoding.ASCII.GetBytes(cmsg, message.Length, data, 32);
                }
            }
        }
    }

    public class ServerManager : MonoBehaviour
    {
        [SerializeField] private int _port = 8080;
        [SerializeField] private string _ip = "localhost";

        private TcpClient _client;
        private Thread _clientReceiveThread;
        private void Awake()
        {
            ConnectToServer();
        }

        private void ConnectToServer()
        {
            try
            {
                _clientReceiveThread = new Thread(new ThreadStart(ListenForData));
                _clientReceiveThread.IsBackground = true;
                _clientReceiveThread.Start();
            }
            catch (Exception e)
            {
                Debug.LogError($"On client connect exception {e}");
            }
        }
        private void ListenForData()
        {
            try
            {
                Connect(_ip);
                Byte[] bytes = new Byte[1024];
                while (true)
                {
                    using NetworkStream stream = _client.GetStream();
                    int length;
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        Debug.Log($"Server message received as: {serverMessage}");
                    }
                }
            }
            catch (SocketException socketException)
            {
                Debug.LogError($"Socket exception: {socketException}");
            }
        }

        private void Connect(string server)
        {
            _client = new TcpClient(server, _port);

            NetworkStream stream = _client.GetStream();

            Test test = new Test("Hello");

            byte[] data = ConstructPacket(0, test);

            stream.Write(data, 0, data.Length);
        }

        private byte[] ConstructPacket<TStruct>(uint packetType, TStruct data) where TStruct : struct
        {
            Packet packet = new Packet(packetType, (uint)Marshal.SizeOf(typeof(TStruct)));
            byte[] packetData = GetBytes(packet);
            byte[] structData = GetBytes(data);

            byte[] res = new byte[packetData.Length + structData.Length];
            System.Buffer.BlockCopy(packetData, 0, res, 0, packetData.Length);
            System.Buffer.BlockCopy(structData, 0, res, packetData.Length, structData.Length);

            return res;
        }

        private static byte[] GetBytes<TStruct>(TStruct data) where TStruct : struct
        {
            int structSize = Marshal.SizeOf(typeof(TStruct));
            byte[] buffer = new byte[structSize];
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(data, handle.AddrOfPinnedObject(), false);
            handle.Free();
            return buffer;
        }
    }
}
