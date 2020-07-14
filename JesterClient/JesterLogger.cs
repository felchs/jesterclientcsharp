using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace JesterClient
{
    public class JesterLogger
    {
        private static List<JesterLoggerInterface> jesterLoggerList = new List<JesterLoggerInterface>();

        public static void addLogger(JesterLoggerInterface logger)
        {
            jesterLoggerList.Add(logger);
        }

        public static void log(string message)
        {
            Console.WriteLine(message);

            for (int i = 0; i < jesterLoggerList.Count; i++)
            {
                JesterLoggerInterface logger = jesterLoggerList[i];
                logger.log(message);
            }
        }

        ///////////////////////////////////////////////////////////////////////

        private JesterLogger()
        {
        }
    }
}
