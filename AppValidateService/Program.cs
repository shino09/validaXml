using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using static System.Net.Mime.MediaTypeNames;

namespace AppValidateService
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        static void Main()
        {

            //DESCOMENTAR CUANDO NO SE VAYA A DEPURAR

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                    new Service1()
            };
            ServiceBase.Run(ServicesToRun);


            //COMENTAR CUANDO SE VAYA A PUBLICAR

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

        }
    }
}
