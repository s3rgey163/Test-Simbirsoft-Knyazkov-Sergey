using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    static class Logs
    {
        private static string _lastError;
        private static FileManager fError = new FileManager("Logs.txt");
        public static string GetLastError
        {
            get { return _lastError; }
        }

        public static void Error(Exception err)
        {   
            _lastError = err.Message;
            fError.WriteFile("[" + DateTime.Now.ToString()+"]" +"\n" + err.Message + "\n" + err.StackTrace +  "\n", FileManager.WriteFileOptions.WRITE_IF_EXISTS);
        }

    }
}
