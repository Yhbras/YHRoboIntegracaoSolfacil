using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHRoboIntegracaoSolfacil.Models
{
    public class ReturnMessage
    {
        public bool Sucesso { get; set; }
        public string? Mensagem { get; set; }
        public int? Id { get; set; }
        public object? Retorno { get; set; }
    }
}
