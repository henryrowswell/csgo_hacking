using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace BunnyHop
{
    class Program
    {
        static string process = "csgo";
        public static int bClient;      //base client address of the dll     

        public static int aLocalPlayer = 0x00AB06EC;    //a for address
        public static int oFlags = 0x100;               //o for offset
        public static int aJump = 0x04F6A6A4;

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int GetAsyncKeyState(int vKey);

        static void Main(string[] args)
        {
            VAMemory vam = new VAMemory(process);
            
            if (GetModuleAddy())
            {
                int fJump = bClient + aJump; //I thought aJump was already the address of force jump?

                aLocalPlayer = bClient + aLocalPlayer;  //I think the address variables are actually still offsets from bClient
                int LocalPlayer = vam.ReadInt32((IntPtr)aLocalPlayer);

                int aFlags = LocalPlayer + oFlags;

                
                while (true)
                {
                    while (GetAsyncKeyState(32) != 0) //32 is space bar key, he had >0 but it goes negative when I test it
                    {
                        int Flags = vam.ReadInt32((IntPtr)aFlags);

                        if (Flags == 257) //257 while on the ground, 256 in the air
                        {
                            vam.WriteInt32((IntPtr)fJump, 5); //5,4 are magic numbers for force & unforce
                            Thread.Sleep(10);
                            vam.WriteInt32((IntPtr)fJump, 4);

                            Console.Clear();
                            Console.WriteLine("Jumping", Console.ForegroundColor = ConsoleColor.Green);
                        }
                    }
                    Console.Clear();
                    Console.WriteLine("Not Jumping", Console.ForegroundColor = ConsoleColor.Yellow);
                    Thread.Sleep(10);
                }

            }
        }

        static bool GetModuleAddy()
        {
            try
            {
                Process[] p = Process.GetProcessesByName(process);

                if (p.Length > 0)
                {
                    foreach (ProcessModule m in p[0].Modules)
                    {
                        if (m.ModuleName == "client.dll")
                        {
                            bClient = (int)m.BaseAddress;
                            return true;
                        }
                    }
                    return true; //really? even if we never found a bClient?
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
