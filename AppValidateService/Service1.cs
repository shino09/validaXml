using Negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Datos;
using System.IO;


namespace AppValidateService
{
    public partial class Service1 : ServiceBase
    {
        private const String origen = "Aplicacion APP Validate";
        private const String log = "Application";
        D_PAR_TIP_CARACTER datoParTipRsp = new D_PAR_TIP_CARACTER();

        N_PET Negocio = new N_PET();
        


        public Service1()
        {
            InitializeComponent();
            // Se cambia el tiempo de ejecucion a traves del app.config en la variable "Timer" 1.000 = 1s
            GetVal.Interval = Convert.ToDouble(Properties.Settings.Default.Timer);
        }

        protected override void OnStart(string[] args)
        {
            GetVal.Start();
            eLog("Inicia el Aplicativo AppPeticiones de Inicio Manual");
        }

        protected override void OnStop()
        {
            GetVal.Stop();
            eLog("Detiene el Aplicativo AppPeticiones de Inicio Manual");
        }


        private void GetVal_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string id_Ejec = datoParTipRsp.Get_fecha_x_numero();
            id_Ejec = id_Ejec.Replace(",", "");

            eLog("Se Ejecuta el AppPeticiones de forma automática mediante el Servicio de Windows - ID Ejecución:" + id_Ejec, id_Ejec);

            string Demo = Properties.Settings.Default.Demo;

            try
            {
                //AGREGAMOS ESTA LINEA PARA SABER SI SE LOGRA REALIZAR LA CONEXION
                if (Demo.Equals("NO"))
                {
                    try
                    {
                        try
                        {
                            if (!EventLog.SourceExists(origen))
                                EventLog.CreateEventSource(origen, log);

                            EventLog.WriteEntry("Se inicio el proceso - ID Ejecución:" + id_Ejec, EventLogEntryType.Information);
                        }
                        catch (Exception er)
                        {
                            eLog("Excepcion EventLog.WriteEntry ERROR: " + er.Message + " - ID Ejecución:" + id_Ejec, id_Ejec);
                        }

             
                        string respuesta = "";
                        Thread P = new Thread(delegate ()
                        {
                            respuesta = Negocio.Ejecuta(respuesta, id_Ejec);
                        });
                        P.Start();
                        P.Join();

                
                        
                    }
                    catch (Exception ex)
                    {
                        eLog("Conexion con Error, cod " + ex.Message + "- ID Ejecución:" + id_Ejec);

                        try
                        {

                            //blBandera = true;
                            if (!EventLog.SourceExists(origen))
                                EventLog.CreateEventSource(origen, log);
                            EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
                        }
                        catch (Exception er)
                        {
                            eLog("Error - EventLog.WriteEntry" + er.Message + "- ID Ejecución:" + id_Ejec);
                        }
                    }
                }
                else
                {
                   // Demo_Conexion();
                }
            }
            catch (Exception ex)
            {
                eLog("Se Generó Excepcion en la ejecución del App Peticiones - Error: " + ex.Message + "- ID Ejecución:" + id_Ejec);
                try
                {
                    //blBandera = true;
                    if (!EventLog.SourceExists(origen))
                        EventLog.CreateEventSource(origen, log);
                    EventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
                }
                catch (Exception er)
                {
                    eLog("Se Generó Excepcion - EventLog.WriteEntry" + er.Message + "- ID Ejecución:" + id_Ejec);
                }
            }

            eLog("Se Termina la Ejecución del AppPeticiones de forma automática mediante el Servicio de Windows - ID Ejecución:" + id_Ejec, id_Ejec);
            eLog("**********************************", id_Ejec);
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
