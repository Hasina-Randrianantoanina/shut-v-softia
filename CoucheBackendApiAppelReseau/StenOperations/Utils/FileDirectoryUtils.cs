using StenOperations.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StenOperations.Utils
{
    public class FileDirectoryUtils
    {
        public static bool TryCreateDirectoryIfNotExist(ILogger? logger, string path)
        {
            if (string.IsNullOrEmpty(path)) 
                return false;
            try
            {
                if (!Directory.Exists(path))
                {
                    var di = Directory.CreateDirectory(path);
                    return di.Exists;
                }
                return true;
            }
            catch (Exception ex) 
            {
                logger?.Log(ex.ExceptionStackTraces());
            }
            return false;
        }
    }
}
