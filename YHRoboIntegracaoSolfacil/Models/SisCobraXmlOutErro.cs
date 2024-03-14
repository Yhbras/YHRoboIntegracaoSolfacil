using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace YHRoboIntegracaoSolfacil.Models
{
    [XmlRoot(ElementName = "ERRO")]
    public class SisCobraXmlOutErro
    {
        [XmlElement(ElementName = "CODIGO")]
        public int codigo { get; set; }

        [XmlElement(ElementName = "DESCRICAO")]
        public string descricao { get; set; }
    }
}
