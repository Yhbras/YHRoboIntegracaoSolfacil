using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace YHRoboIntegracaoSolfacil.Models
{
    [XmlRoot(ElementName = "retorno")]
    public class SisCobraXmlOutSucesso
    {
        [XmlElement(ElementName = "codigo")]
        public int codigo { get; set; }

        [XmlElement(ElementName = "descricao")]
        public string descricao { get; set; }
    }
}
