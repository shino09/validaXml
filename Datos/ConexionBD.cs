using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos
{
    class ConexionBD
    {
        private SqlConnection Conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["unica"].ConnectionString);

        public SqlConnection AbrirConexion()
        {
            if (Conexion.State == ConnectionState.Closed)
                Conexion.Open();

            return Conexion;
        }

        public SqlConnection CerrarConecion()
        {
            if (Conexion.State == ConnectionState.Open)
                Conexion.Close();
            return Conexion;
        }
    }
}
