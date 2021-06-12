using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test
{

    class FileManager
    {
        private string _path;

        public enum WriteFileOptions
        {
            WRITE_IF_EXISTS = 1,
            REWRITE_ALWAYS = 0
        }

        public FileManager(string path)
        {
            _path = path;
        }


        public string GetPath
        {
            get { return _path; }
        }



        public bool WriteFile(string value, WriteFileOptions writeFileOptions)
        {
            bool isWrite = false;
            bool creationFlag;
            _path = Path.GetFullPath(_path);
            if (writeFileOptions == WriteFileOptions.REWRITE_ALWAYS)
            {
                creationFlag = false;
            }
            else
            {
                creationFlag = true;
            }
            try
            {
                using (StreamWriter sw = new StreamWriter(GetPath, creationFlag))
                {
                    sw.AutoFlush = true;
                    sw.WriteLine(value);
                }
            }
            catch(ArgumentException e)
            {
                Logs.Error(e);
            }
            catch(DirectoryNotFoundException e)
            {
                Logs.Error(e);
            }
            catch(IOException e)
            {
                Logs.Error(e);
            }
            return isWrite;
        }
    }
}
