using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidad
{
    public class E_PET_FILE
    {
        public int idPET_FILE { get; set; }
        public int idPET { get; set; }
        public string PET_NOM_ARCHV { get; set; }
        public byte[] PET_FILE_XML { get; set; }
        public string PET_ADD1 { get; set; }
        public string PET_ADD2 { get; set; }
    }
}
