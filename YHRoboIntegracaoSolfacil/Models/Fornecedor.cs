using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YHRoboIntegracaoSolfacil.Models
{
    public class Fornecedor
    {
        public int FornecedorId { get; set; }
        public int CodigoFornecedor { get; set; }
        public int CodigoCliente { get; set; }
        public string NomeFornecedor { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public string Token { get; set; }
        public DateTime ValidadeToken { get; set; }
        public string UrlBase { get; set; }
        public string UrlLogin { get; set; }
        public string TipoAutenticacao { get; set; }
        public bool Digital { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public bool Ativo { get; set; }
    }
}
