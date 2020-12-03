using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Negocio;
using System.ServiceProcess;
using System.Threading;
//using InterfazCavali;
using Datos;
using System.IO;

namespace AppValidateService
{
    public partial class Form1 : Form
    {
        private const String origen = "Aplicacion APP Validate";
        private const String log = "Application";
        D_PAR_TIP_CARACTER datoParTipRsp = new D_PAR_TIP_CARACTER();

        N_PET Negocio = new N_PET();
      

        public Form1()
        {
            InitializeComponent();
        }

        private void Btn_04002_Click(object sender, EventArgs e)
        {
            string id_Ejec = datoParTipRsp.Get_fecha_x_numero();
            id_Ejec = id_Ejec.Replace(",", "");
            Negocio.Ejecuta("Ejecutado", id_Ejec);
        }

        public void eLog(string mensaje, string id_Ejec = "0")
        {
            string Directorio = Properties.Settings.Default.rutaLogs;
            string Genera_logs = Properties.Settings.Default.GeneraLogs;//Indica si debe o no generar logs.


            //Obtiene la ruta del proyecto concatenandola con la carpeta asignada en el config
            string ruta = System.AppDomain.CurrentDomain.BaseDirectory + Directorio + "/";

            //CONSULTA SI ESCRIBE LOGS
            if (Genera_logs.Equals("SI"))
            {
                // Valida y crea la carpeta definida en el config
                if (!(Directory.Exists(ruta)))
                {
                    Directory.CreateDirectory(ruta);
                }

                string fecha = DateTime.Now.ToString("dd/MM/yyyy");
                fecha = fecha.Replace("/", ".").Replace("\",", ".");
                id_Ejec = id_Ejec.Replace(",", "");

                String ArchLog = ruta + @id_Ejec + "_Log_Servicio_APP_Peticiones_ID_" + fecha + ".log";

                try
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(ArchLog, true);

                    try
                    {
                        fecha = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
                        mensaje = fecha + "; " + mensaje + ";";
                        sw.WriteLine(mensaje);
                    }
                    catch (Exception e1)
                    {
                    }
                    finally
                    {
                        sw.Close();
                        sw.Dispose();
                    }
                }
                catch (Exception e1)
                {
                }
            }
        }

       


    }
}


