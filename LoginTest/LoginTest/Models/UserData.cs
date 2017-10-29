using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginTest.Models
{
    public class UserData
    {
        public int id   { get; set; }
        public string nombres { get; set; }
        public string apellidos { get; set; }
        public string correo { get; set; }
        public string numero_documento { get; set; }
        public DateTime ultima_sesion { get; set; }
        public int eliminado { get; set; }
        public int documentos_id { get; set; }
        public string documentos_abrev { get; set; }
        public string documentos_label { get; set; }
        public string estados_usuarios_label { get; set; }
    }
}

