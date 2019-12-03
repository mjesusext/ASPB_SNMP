using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNMPDiscovery.View
{
    public static class ViewHelper
    {
        //Variables for redirecting console to file
        private static FileStream ostrm;
        private static StreamWriter writer;
        private static TextWriter oldOut;

        public static void RedirectConsoleToFile(bool activate)
        {
            if (activate)
            {
                oldOut = Console.Out;
                try
                {
                    ostrm = new FileStream("./Redirect.txt", FileMode.Append, FileAccess.Write);
                    writer = new StreamWriter(ostrm);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot open Redirect.txt for writing");
                    Console.WriteLine(e.Message);
                    return;
                }
                Console.SetOut(writer);
            }
            else
            {
                Console.SetOut(oldOut);
                writer.Close();
                ostrm.Close();
            }
        }

        public static void GetConsoleInput(string requesttext, out string result, Predicate<string> validation)
        {
            throw new NotImplementedException();
        }
    }
}
