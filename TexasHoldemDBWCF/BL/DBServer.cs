using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace TexasHoldemDBWCF
{
   public class DBServer
    {
        static void Main(string[] args)
        {
            DBManager.Manager.DbMessagesNotifier += DbMessagesHandler;                   //Subscribe to Event

            using (ServiceHost dbServer = new ServiceHost(typeof(TexasHoldemDBService))) //define host
            {
                dbServer.Open();
                Console.WriteLine("DB server started press any key to terminate");
                Console.ReadLine();
                DBManager.Manager.CloseConnection();
                dbServer.Close();
                Console.WriteLine("DB server stopped");
            }

            DBManager.Manager.DbMessagesNotifier -= DbMessagesHandler;                  //Unsubscribe to Event

        }

        public static void DbMessagesHandler(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
