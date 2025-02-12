using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.DTOs.Mensaje
{
    public class MensajeRequest : RequestBase
    {
        public MensajeEncabezadoDTO? MensajeEncabezado { get; set; }
    }
}
