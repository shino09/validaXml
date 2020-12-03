using Entidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Configuration;
using System.IO;


namespace Datos
{
    public class D_PAR_TIP_CARACTER
    {

        public int estado = 0;
        public E_PAR_TIP_CARACTER Obtener_PAR_TIP_CARACTER(string PAR_TIP_CARACTER_TYP)
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            List<E_PAR_TIP_CARACTER> Lista = new List<E_PAR_TIP_CARACTER>();
            SqlCommand Comando = new SqlCommand();
            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_GET_PAR_TIP_CARACTER";
                Comando.Parameters.AddWithValue("@PAR_TIP_CARACTER_TYP", PAR_TIP_CARACTER_TYP);
                Comando.CommandType = CommandType.StoredProcedure;

                LeerFilas = Comando.ExecuteReader();

                while (LeerFilas.Read())
                {
                    E_PAR_TIP_CARACTER rsp = new E_PAR_TIP_CARACTER();
                   rsp.PAR_TIP_CARACTER_COD = Convert.ToString(LeerFilas["PAR_TIP_CARACTER_COD"]);

                    rsp.PAR_TIP_CARACTER_ORIGINAL = Convert.ToString(LeerFilas["PAR_TIP_CARACTER_ORIGINAL"]);
                    rsp.PAR_TIP_CARACTER_REEMPLAZO = Convert.ToString(LeerFilas["PAR_TIP_CARACTER_REEMPLAZO"]);
                    rsp.PAR_TIP_CARACTER_EST = Convert.ToString(LeerFilas["PAR_TIP_CARACTER_EST"]);

                    Lista.Add(rsp);

                }
                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();

            }

            return Lista[0];

        }

        public string Get_fecha_x_numero()
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            SqlCommand Comando = new SqlCommand();
            string dato = "";
            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "GET_FECHA_X_NUMERO";
                Comando.CommandType = CommandType.StoredProcedure;

                LeerFilas = Comando.ExecuteReader();

                while (LeerFilas.Read())
                {
                    dato = Convert.ToString(LeerFilas["dato"]);
                }

                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();
                return "0";
            }

            return dato;
        }

  

        public void eLog(string mensaje, string id_Ejec)
        {
            string Directorio = Properties.Settings.Default.rutaLogs_dll_datos;
            string Genera_logs = Properties.Settings.Default.GeneraLogs_dll_datos;//Indica si debe o no generar logs.

            //Obtiene la ruta del proyecto concatenandola con la carpeta asignada en el config
            string ruta = System.AppDomain.CurrentDomain.BaseDirectory + Directorio + "/";

            // CONSULTA SI ESCRIBE LOGS
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

                String ArchLog = ruta + @id_Ejec + "_" + "_Log_APPVALIDATE_" + fecha + ".log";

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
                        estado = 99;
                    }
                    finally
                    {
                        estado = 1;
                        sw.Close();
                        sw.Dispose();
                    }
                }
                catch (Exception e1)
                {
                    estado = 99;
                }
            }
        }
    }
}
