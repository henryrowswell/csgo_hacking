using System;
using System.Threading;
using System.Diagnostics;

namespace WallHack
{
    class Program
    {
        public static int bClient;
        public static string process = "csgo";

        public static int oLocalPlayer = 0x00AB06EC;
        public static int oEntityList = 0x04AD3A84;
        public static int oCrossHairID = 0x0000AA70;
        public static int oTeam = 0x000000F0;   //ct = 3, t = 2, spectators = 1
        public static int oHealth = 0x000000FC;
        public static int oAttack = 0x02F13A70;
        public static int oEntityLoopDistance = 0x10;
        public static int oDormant = 0x000000E9;
        public static int oGlowIndex = 0x0000A320;
        public static int oGlowObject = 0x04FEE5BC;


        static void Main(string[] args)
        {
            VAMemory vam = new VAMemory(process);

            if (GetModuleAddy())
            {
                while (true)
                {
                    GlowStruct Enemy = new GlowStruct()
                    {
                        r = 1,
                        g = 0,
                        b = 0,
                        a = 1,
                        rwo = true,
                        rwuo = true
                    };
                    GlowStruct Team = new GlowStruct()
                    {
                        r = 0,
                        g = 0,
                        b = 1,
                        a = 1,
                        rwo = true,
                        rwuo = true
                    };

                    int address;
                    int i = 1;

                    // LOOP 65 times
                    do
                    {
                        address = bClient + oLocalPlayer;
                        int Player = vam.ReadInt32((IntPtr)address);

                        address = Player + oTeam;
                        int MyTeam = vam.ReadInt32((IntPtr)address);

                        address = bClient + oEntityList + (i - 1) * 0x10;
                        int EntityList = vam.ReadInt32((IntPtr)address);

                        address = EntityList + oTeam;
                        int HisTeam = vam.ReadInt32((IntPtr)address);

                        address = EntityList + oDormant;
                        if (!vam.ReadBoolean((IntPtr)address))
                        {
                            address = EntityList + oGlowIndex;
                            int GlowIndex = vam.ReadInt32((IntPtr)address);

                            if (MyTeam == HisTeam)
                            {
                                //Console.Write("My Team", Console.ForegroundColor = ConsoleColor.Green);
                                address = bClient + oGlowObject;
                                int GlowObject = vam.ReadInt32((IntPtr)address);

                                int calculation = GlowIndex * 0x38 + 0x4;
                                int current = GlowObject + calculation;
                                vam.WriteFloat((IntPtr)current, Team.r);

                                calculation = GlowIndex * 0x38 + 0x8;
                                current = GlowObject + calculation;
                                vam.WriteFloat((IntPtr)current, Team.g);

                                calculation = GlowIndex * 0x38 + 0xC;
                                current = GlowObject + calculation;
                                vam.WriteFloat((IntPtr)current, Team.b);

                                calculation = GlowIndex * 0x38 + 0x10;
                                current = GlowObject + calculation;
                                vam.WriteFloat((IntPtr)current, Team.a);

                                calculation = GlowIndex * 0x38 + 0x24;
                                current = GlowObject + calculation;
                                vam.WriteBoolean((IntPtr)current, Team.rwo);

                                calculation = GlowIndex * 0x38 + 0x25;
                                current = GlowObject + calculation;
                                vam.WriteBoolean((IntPtr)current, Team.rwuo);
                            }
                            else
                            {
                                // Console.Write("Enemy Team", Console.ForegroundColor = ConsoleColor.Red);
                                address = bClient + oGlowObject;
                                int GlowObject = vam.ReadInt32((IntPtr)address);

                                int calculation = GlowIndex * 0x38 + 0x4;
                                int current = GlowObject + calculation;
                                vam.WriteFloat((IntPtr)current, Enemy.r);

                                calculation = GlowIndex * 0x38 + 0x8;
                                current = GlowObject + calculation;
                                vam.WriteFloat((IntPtr)current, Enemy.g);

                                calculation = GlowIndex * 0x38 + 0xC;
                                current = GlowObject + calculation;
                                vam.WriteFloat((IntPtr)current, Enemy.b);

                                calculation = GlowIndex * 0x38 + 0x10;
                                current = GlowObject + calculation;
                                vam.WriteFloat((IntPtr)current, Enemy.a);

                                calculation = GlowIndex * 0x38 + 0x24;
                                current = GlowObject + calculation;
                                vam.WriteBoolean((IntPtr)current, Enemy.rwo);

                                calculation = GlowIndex * 0x38 + 0x25;
                                current = GlowObject + calculation;
                                vam.WriteBoolean((IntPtr)current, Enemy.rwuo);
                            }
                        }
                        i++;
                    }
                    while (i < 65);

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

        public struct GlowStruct
        {
            public float r, g, b, a;
            public bool rwo, rwuo;
        }
    }
}
