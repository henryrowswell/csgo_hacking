using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace TriggerBot
{
    class Program
    {
        static string process = "csgo";
        public static int bClient;      //base client address of the dll     

        public static int oLocalPlayer = 0x00AB06EC; 
        public static int oEntityList = 0x04AD3A84;
        public static int oCrossHairID = 0x0000AA70;
        public static int oTeam = 0x000000F0;   //ct = 3, t = 2, spectators = 1
        public static int oHealth = 0x000000FC;
        public static int oAttack = 0x02F13A70;
        public static int oEntityLoopDistance = 0x10;

        static void Main(string[] args)
        {
            VAMemory vam = new VAMemory(process);

            if (GetModuleAddy())
            {

                int fAttack = bClient + oAttack;

                while (true)
                {
                    Console.Clear();
                    Console.Write("Nothing...", Console.ForegroundColor = ConsoleColor.Red);

                    int address = bClient + oLocalPlayer;
                    int LocalPlayer = vam.ReadInt32((IntPtr)address);

                    address = LocalPlayer + oTeam;
                    int MyTeam = vam.ReadInt32((IntPtr)address);

                    address = LocalPlayer + oCrossHairID;
                    int PlayerInCross = vam.ReadInt32((IntPtr)address);

                    if (PlayerInCross > 0 && PlayerInCross < 65)    //why < 65? - max 64 players in server, so not sure if checking is necessary
                    {
                        address = bClient + oEntityList + (PlayerInCross - 1) * oEntityLoopDistance;
                        int PtrToPIC = vam.ReadInt32((IntPtr)address);

                        address = PtrToPIC + oHealth;
                        int PICHealth = vam.ReadInt32((IntPtr)address);

                        address = PtrToPIC + oTeam;
                        int PicTeam = vam.ReadInt32((IntPtr)address);

                        if ((PicTeam != MyTeam) && (PicTeam > 1) && (PICHealth > 0))
                        {
                            Thread.Sleep(20);
                            Console.Clear();
                            Console.Write("SHOOT", Console.ForegroundColor = ConsoleColor.Green);
                            vam.WriteInt32((IntPtr)fAttack, 1); //again, I guess 1 and 4 are magic number for force attack and unforce
                            Thread.Sleep(1);
                            vam.WriteInt32((IntPtr)fAttack, 4);
                        }
                    }
                    // Thread.Sleep(10);
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
