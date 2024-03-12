using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHRoboIntegracaoSolfacil.Models
{
    public class AcionamentoRequest
    {
        public int CedenteId { get; set; }
        public int ClienteId { get; set; }
        public string NuContrato { get; set; }
        public string DataAcao { get; set; }
        public string HoraAcao { get; set; }
        public int ExternoId { get; set; }
        public string DataAgenda { get; set; }
        public string HoraAgenda { get; set; }
        public string NmAnalista { get; set; }
        public string Report { get; set; }
        public int Tipo { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
    }
}
