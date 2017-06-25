using LightInject;
using log4net;
using Outsurance.MemberManager.Core.Repositories;
using Outsurance.MemberManager.Core.Services;
using Outsurance.MemberManager.Repositories;
using Outsurance.MemberManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Outsurance.MemberManager.ConsoleApplication
{
    public class Worker
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ServiceContainer _container = new ServiceContainer();

        public Worker()
        {
            RegisterServices();
        }

        public void Run()
        {
            try
            {                
                WriteMessage("*********Outsurance Members Analysis Assessment By Simba Sithole.***\n");
                WriteMessage("Starting members analysis");
                using (_container.BeginScope())
                {
                    var memberService = _container.GetInstance<IMemberService>();
                    memberService.Analyse();
                }
                WriteMessage("Successfully completed members analysis, check your folders.");
            }
            catch (Exception exception)
            {
                LogException(exception);
            }

            finally
            {
                WriteMessage("*********Please press any key to exit******************");
                Console.ReadLine();
            }
        }

        private static void RegisterServices()
        {
            _container.Register<IMemberService, MemberService>();
            _container.Register<IMemberRepository, MemberRepository>();
        }

        private static void WriteMessage(string message)
        {
            _log.Info(message);
        }

        private static void LogException(Exception exception)
        {
            _log.Error(exception);
        }
    }
}
