using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entidad;
using System.Data;
using System.Data.SqlClient;

namespace Datos
{
    public class D_PET
    {
        public int codigo = 0;
        public string descripcion = "";

       

        /**************OBTIENE LISTADO DE TRANSACCIONES CON ESTADO PENDIENTE VALIDAR (7)***********/
        public List<E_PET_DATO> ObtenerTransacciones()
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            DataTable tabla = new DataTable();
            List<E_PET_DATO> Lista = new List<E_PET_DATO>();
            SqlCommand Comando = new SqlCommand();
          
            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_GET_Transacciones";
                Comando.CommandType = CommandType.StoredProcedure;
                LeerFilas = Comando.ExecuteReader();
                tabla.Load(LeerFilas);
                foreach (DataRow row in tabla.Rows)
                {
                    E_PET_DATO dato = new E_PET_DATO();
                    dato.idPET = Int32.Parse(row["idPET"].ToString());
                    dato.transaccion = row["transaccion"].ToString();

                    Lista.Add(dato);

                }

                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();

            }
            return Lista;
        }
        /**************OBTIENE EL ID DEL CODIGO DEL ESTADO DE PETICION***********/
        public int obtenerEstadoPeticion(int codigo)
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            DataTable tabla = new DataTable();
            List<int> Lista = new List<int>();
            SqlCommand Comando = new SqlCommand();
            int codigorespuesta = 0;

            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_GET_TRANS_ESTADOS";
                Comando.Parameters.AddWithValue("@codigo", codigo);
                Comando.CommandType = CommandType.StoredProcedure;
                LeerFilas = Comando.ExecuteReader();
                tabla.Load(LeerFilas);
                foreach (DataRow row in tabla.Rows)
                {

                    codigorespuesta = Int32.Parse(row["codigorespuesta"].ToString());

                }

                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();

            }
            return codigorespuesta;

        }



        /*****************CAMBIA ESTADO TRANSACCION A VALIDANDO(7), ERRONEO (5) O PENDIENTE(0) ***********************/
        public int actualizarEstadoTransaccion(E_PET_DATO PET, int estado)
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            SqlCommand Comando = new SqlCommand();

            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_UPD_Transacciones_Estado";
                Comando.Parameters.AddWithValue("@idTrans", PET.idPET);
                Comando.Parameters.AddWithValue("@transaccion", PET.transaccion);
                Comando.Parameters.AddWithValue("@estado", estado);
                Comando.CommandType = CommandType.StoredProcedure;
                LeerFilas = Comando.ExecuteReader();


                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();
                return 0;
            }

            return 1;
        }

        /**************OBTIENE LISTADO DE ARCHIVOS DE LA TRANSACCION***********/
        public List<E_PET_FILE> obtenerArchivos(E_PET_DATO PET)
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            List<E_PET_FILE> Lista = new List<E_PET_FILE>();
            SqlCommand Comando = new SqlCommand();

            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_GET_Transaccion_FileXml";
                Comando.Parameters.AddWithValue("@id", PET.idPET);
                Comando.Parameters.AddWithValue("@transaccion", PET.transaccion);
                Comando.CommandType = CommandType.StoredProcedure;
                LeerFilas = Comando.ExecuteReader();

                while (LeerFilas.Read())
                {
                    E_PET_FILE adicional = new E_PET_FILE();
                    adicional.idPET_FILE = Int32.Parse(LeerFilas["idPET_FILE"].ToString());
                    adicional.idPET = Int32.Parse(LeerFilas["idPET"].ToString());
                    adicional.PET_NOM_ARCHV = LeerFilas["PET_NOM_ARCHV"].ToString();
                    adicional.PET_FILE_XML = (byte[])LeerFilas["PET_FILE_XML"];
                    adicional.PET_ADD1 = LeerFilas["PET_ADD1"].ToString();
                    adicional.PET_ADD2 = LeerFilas["PET_ADD2"].ToString();

                    Lista.Add(adicional);

                }
                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();
            }

            return Lista;



        }

        /**************OBTIENE EL NUMERO DE VECES QUE UN REGISTRO/FILE SE ENCUENTRA EN LA TABLA ERROR***********/
        public int obtenerRegistrosTablaError(int idFile,string transaccion)
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            DataTable tabla = new DataTable();
            List<int> Lista = new List<int>();
            SqlCommand Comando = new SqlCommand();
            int contador = 0;

            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_GET_Registro_TablaError";
                Comando.Parameters.AddWithValue("@id", idFile);
                Comando.Parameters.AddWithValue("@transaccion", transaccion);
                Comando.CommandType = CommandType.StoredProcedure;
                LeerFilas = Comando.ExecuteReader();
                tabla.Load(LeerFilas);
                foreach (DataRow row in tabla.Rows)
                {

                    contador = Int32.Parse(row["contador"].ToString());

                }

                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();

            }
            return contador;

        }


        /**REEMPLAZAR ARCHIVO XML EN LA TABLA FILE POR EL NUEVO****/
        public int actualizarArchivo(int idFile, byte[] arch_xml, string transaccion)
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            SqlCommand Comando = new SqlCommand();

            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_UPD_Registro_ArchivoFileXml";
                Comando.Parameters.AddWithValue("@idFile", idFile);       
                Comando.Parameters.AddWithValue("@reemplazo", arch_xml);
                Comando.Parameters.AddWithValue("@transaccion", transaccion);
                Comando.CommandType = CommandType.StoredProcedure;
                LeerFilas = Comando.ExecuteReader();


                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();
                return 0;
            }

            return 1;
        }


        /**INGRESAR EL REGISTRO CORREGIDO A VALIDATE**/
        public int actualizarRegistroValidate(int idFile, string reemplazo,string transaccion)
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            SqlCommand Comando = new SqlCommand();

            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_UPD_Registro_ArchivoXml";
                Comando.Parameters.AddWithValue("@idFile", idFile);
                Comando.Parameters.AddWithValue("@reemplazo", reemplazo);
                Comando.Parameters.AddWithValue("@transaccion", transaccion);
                Comando.CommandType = CommandType.StoredProcedure;
                LeerFilas = Comando.ExecuteReader();


                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();
                return 0;
            }

            return 1;
        }

        /*ACTUALIZAR EL ESTADO DE LA TABLA FILE_ERR 0 ERRONEO 1 CORREGIDO*/
        public int actualizarEstadoTablaError(int idFile, int estado,string transaccion)
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            SqlCommand Comando = new SqlCommand();

            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_UPD_VALIDATE_ERR_Estado";
                Comando.Parameters.AddWithValue("@idFile", idFile);
                Comando.Parameters.AddWithValue("@estado", estado);
                Comando.Parameters.AddWithValue("@transaccion", transaccion);
                Comando.CommandType = CommandType.StoredProcedure;
                LeerFilas = Comando.ExecuteReader();


                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();
                return 0;
            }

            return 1;
        }

        /*ACTUALIZAR EL ESTADO DE LA TABLA VALIDATE 0 ERRONEO 1 CORREGIDO*/
        public int actualizarEstadoRegistro(int idFile, int estado, string transaccion)
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            SqlCommand Comando = new SqlCommand();

            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_UPD_Registro_Estado";
                Comando.Parameters.AddWithValue("@idFile", idFile);
                Comando.Parameters.AddWithValue("@estado", estado);
                Comando.Parameters.AddWithValue("@transaccion", transaccion);
                Comando.CommandType = CommandType.StoredProcedure;
                LeerFilas = Comando.ExecuteReader();


                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();
                return 0;
            }

            return 1;
        }



        /*INSERTAR REGISTRO EN LA TABLA VALIDATE, SE INGRESA CON ESTADO 0 ERRONEO*/
        public int InsertarRegistro(int id, string transaccion)
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            SqlCommand Comando = new SqlCommand();

            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_INS_Registro_ArchivoXml";
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.AddWithValue("@idFile", id);
                Comando.Parameters.AddWithValue("@transaccion", transaccion);


                LeerFilas = Comando.ExecuteReader();

                while (LeerFilas.Read())
                {
                    string resp = Convert.ToString(LeerFilas["respuesta"]);
                    //EXISTE UN ERROR
                    if (resp.Equals("0"))
                    {
                        LeerFilas.Close();
                        Conexion.CerrarConecion();
                        return 0;
                    }
                }

                LeerFilas.Close();
                Conexion.CerrarConecion();


            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();
                return 0;

            }



            return 1;
        }

        /*GUARDAR ERROR EN LA TABLA FILE_ERR*/
        public int GuardarError(int id, E_PET_DATO PET, string mensaje,string transaccion)
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            SqlCommand Comando = new SqlCommand();

            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_INS_VALIDATE_ERR";
                Comando.CommandType = CommandType.StoredProcedure;
                Comando.Parameters.AddWithValue("@idFile", id);
                Comando.Parameters.AddWithValue("@idTrans", PET.idPET);
                Comando.Parameters.AddWithValue("@transaccion", PET.transaccion);
                Comando.Parameters.AddWithValue("@mensaje", mensaje);

                LeerFilas = Comando.ExecuteReader();

                while (LeerFilas.Read())
                {
                    string resp = Convert.ToString(LeerFilas["respuesta"]);
                    //EXISTE UN ERROR
                    if (resp.Equals("0"))
                    {
                        codigo = 0;

                    }
                }

                LeerFilas.Close();
                Conexion.CerrarConecion();



            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();
                return 0;

            }


            return 1;
        }





        /**DEVUELVE LA CANTIDAD DE VECES QUE EL REGISTOR ESTA PRESENTE EN LA TABLA FILE_ERR**/
        public int obtenerErroresTransaccion(E_PET_DATO PET)
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            DataTable tabla = new DataTable();
            List<int> Lista = new List<int>();
            SqlCommand Comando = new SqlCommand();
            int contador = 0;

            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_GET_Transaccion_ArchivosErroneos";
                Comando.Parameters.AddWithValue("@id", PET.idPET);
                Comando.Parameters.AddWithValue("@transaccion", PET.transaccion);
                Comando.CommandType = CommandType.StoredProcedure;
                LeerFilas = Comando.ExecuteReader();
                tabla.Load(LeerFilas);
                foreach (DataRow row in tabla.Rows)
                {

                    contador = Int32.Parse(row["contador"].ToString());

                }

                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();
                return contador;

            }
            return contador;

        }


        /*OBTENER LOS REGISTROS ERRONEOS DE UNA TRANSACCION*/
        public List<E_PET_FILE> obtenerRegistrosErroneos(E_PET_DATO PET)
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            List<E_PET_FILE> Lista = new List<E_PET_FILE>();
            SqlCommand Comando = new SqlCommand();

            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_GET_Transaccion_RegistrosErroneos";
                Comando.Parameters.AddWithValue("@idTrans", PET.idPET);
                Comando.Parameters.AddWithValue("@transaccion", PET.transaccion);
                Comando.CommandType = CommandType.StoredProcedure;
                LeerFilas = Comando.ExecuteReader();

                while (LeerFilas.Read())
                {
                    E_PET_FILE adicional = new E_PET_FILE();
                    adicional.idPET_FILE = Int32.Parse(LeerFilas["idPET_FILE"].ToString());
                    adicional.idPET = Int32.Parse(LeerFilas["idPET"].ToString());
                    adicional.PET_NOM_ARCHV = LeerFilas["PET_NOM_ARCHV"].ToString();
                    adicional.PET_FILE_XML = (byte[])LeerFilas["PET_FILE_XML"];
                    adicional.PET_ADD1 = LeerFilas["PET_ADD1"].ToString();
                    adicional.PET_ADD2 = LeerFilas["PET_ADD2"].ToString();

                    Lista.Add(adicional);

                }
                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();
            }

            return Lista;
        }


        /*OBTENER EL REGISTRO DE LA TABLA VALIDATE*/
        public List<E_PET_VALIDATE> ObtenerValidate(int id,string transaccion)
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            DataTable tabla = new DataTable();
            List<E_PET_VALIDATE> Lista = new List<E_PET_VALIDATE>();
            SqlCommand Comando = new SqlCommand();


            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_GET_PET_VALIDATE";
                Comando.Parameters.AddWithValue("@id", id);
                Comando.Parameters.AddWithValue("@transaccion", transaccion);

                Comando.CommandType = CommandType.StoredProcedure;
                LeerFilas = Comando.ExecuteReader();
                tabla.Load(LeerFilas);
                foreach (DataRow row in tabla.Rows)
                {
                    E_PET_VALIDATE peticion = new E_PET_VALIDATE();

                    peticion.idPET_FILE = Int32.Parse(row["idPET_FILE"].ToString());
                    peticion.PET_VALIDATE_REGISTRO_ORIGINAL = row["PET_VALIDATE_REGISTRO_ORIGINAL"].ToString();
                    peticion.PET_VALIDATE_REGISTRO_REEMPLAZO = row["PET_VALIDATE_REGISTRO_REEMPLAZO"].ToString();
                    peticion.PET_VALIDATE_FECHA = row["PET_VALIDATE_FECHA"].ToString();
                    peticion.PET_VALIDATE_ESTADO = Int32.Parse(row["PET_VALIDATE_ESTADO"].ToString());

                    Lista.Add(peticion);

                }

                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();

            }
            return Lista;
        }

        /*OBTENER TODOS LOS TIPOS DE CARACTERES CON ESTADO ACTIVO*/
        public List<E_PAR_TIP_CARACTER> ObtenerTipCaracter()
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            DataTable tabla = new DataTable();
            List<E_PAR_TIP_CARACTER> Lista = new List<E_PAR_TIP_CARACTER>();
            SqlCommand Comando = new SqlCommand();


            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_GET_PAR_TIP_CARACTER";

                Comando.CommandType = CommandType.StoredProcedure;
                LeerFilas = Comando.ExecuteReader();
                tabla.Load(LeerFilas);
                foreach (DataRow row in tabla.Rows)
                {
                    E_PAR_TIP_CARACTER peticion = new E_PAR_TIP_CARACTER();

                    peticion.PAR_TIP_CARACTER_ORIGINAL = row["PAR_TIP_CARACTER_ORIGINAL"].ToString();
                    peticion.PAR_TIP_CARACTER_REEMPLAZO = row["PAR_TIP_CARACTER_REEMPLAZO"].ToString();

                    Lista.Add(peticion);

                }

                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();

            }
            return Lista;
        }


        /*OBTENER LOS ATRIBUTOS Y ELEMNTOS QUE TIENE LA ESTRUCTURA DEL XML*/
        public List<E_PAR_TIP_ELEMENTOATRIBUTOXML> ObtenerElementoAtributo()
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            DataTable tabla = new DataTable();
            List<E_PAR_TIP_ELEMENTOATRIBUTOXML> Lista = new List<E_PAR_TIP_ELEMENTOATRIBUTOXML>();
            SqlCommand Comando = new SqlCommand();


            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_GET_PAR_TIP_ELEMENTOATRIBUTOXML";

                Comando.CommandType = CommandType.StoredProcedure;
                LeerFilas = Comando.ExecuteReader();
                tabla.Load(LeerFilas);
                foreach (DataRow row in tabla.Rows)
                {
                    E_PAR_TIP_ELEMENTOATRIBUTOXML peticion = new E_PAR_TIP_ELEMENTOATRIBUTOXML();

                    peticion.PAR_TIP_ELEMENTOATRIBUTOXML_PALABRA = row["PAR_TIP_ELEMENTOATRIBUTOXML_PALABRA"].ToString();
                    peticion.PAR_TIP_ELEMENTOATRIBUTOXML_TIPO = row["PAR_TIP_ELEMENTOATRIBUTOXML_TIPO"].ToString();

                    Lista.Add(peticion);

                }

                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();

            }
            return Lista;
        }



        /*OBTENER LOS ATRIBUTOS Y ELEMNTOS QUE TIENE LA ESTRUCTURA DEL XML COMO LISTA*/
        public List<string> ObtenerElementoAtributoPalabra()
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            DataTable tabla = new DataTable();
            List<string> Lista = new List<string>();
            SqlCommand Comando = new SqlCommand();


            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_GET_PAR_TIP_ELEMENTOATRIBUTOXML_PALABRA";

                Comando.CommandType = CommandType.StoredProcedure;
                LeerFilas = Comando.ExecuteReader();
                tabla.Load(LeerFilas);
                foreach (DataRow row in tabla.Rows)
                {
                    string peticion = row["palabra"].ToString();

                    Lista.Add(peticion);

                }

                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();

            }
            return Lista;
        }




        /*OBTENER LOS ATRIBUTOS Y ELEMNTOS QUE TIENE LA ESTRUCTURA DEL XML COMO LISTA, SOLO LAS PALABRAS*/
        public List<string> ObtenerElementoAtributoEtiqueta()
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            DataTable tabla = new DataTable();
            List<string> Lista = new List<string>();
            SqlCommand Comando = new SqlCommand();


            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_GET_PAR_TIP_ELEMENTOATRIBUTOXML_ETIQUETA";

                Comando.CommandType = CommandType.StoredProcedure;
                LeerFilas = Comando.ExecuteReader();
                tabla.Load(LeerFilas);
                foreach (DataRow row in tabla.Rows)
                {
                    string peticion = row["palabra"].ToString();

                    Lista.Add(peticion);

                }

                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();

            }
            return Lista;
        }

        /*OBTENER LOS CARACTERES NO VALIDOS ACTVOS, SOLO LOS ORIGINALES */
        public List<string> ObtenerTipCaracterOriginal()
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            DataTable tabla = new DataTable();
            List<string> Lista = new List<string>();
            SqlCommand Comando = new SqlCommand();


            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_GET_PAR_TIP_CARACTER_ORIGINAL";
                Comando.CommandType = CommandType.StoredProcedure;
                LeerFilas = Comando.ExecuteReader();
                tabla.Load(LeerFilas);
                foreach (DataRow row in tabla.Rows)
                {
                    string caracter = row["PAR_TIP_CARACTER_ORIGINAL"].ToString();


                    Lista.Add(caracter);

                }

                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();

            }
            return Lista;
        }


        /**DEVUELVE LA CANTIDAD DE VECES QUE EL REGISTOR ESTA PRESENTE EN LA TABLA FILE_ERR**/
        public string obtenerIdPadre(E_PET_DATO PET)
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            DataTable tabla = new DataTable();
            List<int> Lista = new List<int>();
            SqlCommand Comando = new SqlCommand();
            string idPadre = "";

            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_GET_idPadre_Transaccion";
                Comando.Parameters.AddWithValue("@idTrans", PET.idPET);
                Comando.Parameters.AddWithValue("@transaccion", PET.transaccion);
                Comando.CommandType = CommandType.StoredProcedure;
                LeerFilas = Comando.ExecuteReader();
                tabla.Load(LeerFilas);
                foreach (DataRow row in tabla.Rows)
                {

                    idPadre =row["idTRAN_PADRE"].ToString();

                }

                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();
                return idPadre;

            }
            return idPadre;

        }


        /*****************CAMBIA ESTADO TRANSACCION A VALIDANDO(7), ERRONEO (5) O PENDIENTE(0) ***********************/
        public int actualizarEstadoIdPadre(int idPadre, string transaccion, int estado)
        {
            ConexionBD Conexion = new ConexionBD();
            SqlDataReader LeerFilas;
            SqlCommand Comando = new SqlCommand();

            try
            {
                Comando.Connection = Conexion.AbrirConexion();
                Comando.CommandText = "P_UPD_idPadre_Estado_Transaccion";
                Comando.Parameters.AddWithValue("@idPadre", idPadre);
                Comando.Parameters.AddWithValue("@estado", estado);
                Comando.Parameters.AddWithValue("@transaccion", transaccion);
                Comando.CommandType = CommandType.StoredProcedure;
                LeerFilas = Comando.ExecuteReader();


                LeerFilas.Close();
                Conexion.CerrarConecion();
            }
            catch (Exception e)
            {
                Conexion.CerrarConecion();
                return 0;
            }

            return 1;
        }


    }
}
