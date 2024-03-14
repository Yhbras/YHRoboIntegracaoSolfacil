using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Common;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using wsSisCobra;
using YHRoboIntegracaoSolfacil.Data;
using YHRoboIntegracaoSolfacil.Models;

namespace YHRoboIntegracaoSolfacil.Services
{
    public static class YHRoboIntegracaoSolfacilServices
    {

        public static async Task BuscarAcionamentos(string connection, int cedenteId)
        {
            int tot = 0;
            Console.WriteLine("*************************************************************************");
            Console.WriteLine($" Robo - Integração SolFacil (Tabulações)");
            Console.WriteLine("*************************************************************************");
            Console.WriteLine();
            int intervalo = 300;
            DateTime dataExecucaoAtual;
            ReturnMessage retorno = null;
            bool start = true;

            for (int cont = 1; cont <= intervalo; cont++)
            {
                if (start)
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write($"** Buscando novos envios... ** ");
                    cont = intervalo;
                    start = false;
                }
                else
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write($"** Buscando novos envios em {intervalo} segundos: {cont}... ** ");
                }

                Thread.Sleep(1000);

                if (cont == intervalo)
                {
                    try
                    {
                        Console.SetCursorPosition(0, Console.CursorTop);
                        Console.Write($"                                                                                      ");

                        Fornecedor fornecedor = await YHRoboIntegracaoSolfacilData.GetFornecedor(connection, cedenteId, "SISCOBRA");

                        if(fornecedor != null) 
                        {
                            DateTime dataUltimaExecucao = await YHRoboIntegracaoSolfacilData.GetUltimaExecucao(connection, cedenteId, "INTEGRACAOTABULACAO");
                            dataExecucaoAtual = DateTime.Now;
                            List<AcionamentoRequest> acionamentos = await YHRoboIntegracaoSolfacilData.GetAcoesHumanoSolfacil(connection, cedenteId, dataUltimaExecucao);

                            foreach (AcionamentoRequest acionamento in acionamentos)
                            {
                                try
                                {   
                                    string xmlIn = CreateAcionamentoXMLRequest(fornecedor, acionamento);

                                    BasicHttpBinding binding = new BasicHttpBinding();
                                    binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.Transport;
                                    EndpointAddress endpointAddress = new EndpointAddress(fornecedor.UrlBase);
                                    var service = new wsSisCobra.WSAssessoriaSoapPortClient(binding, endpointAddress);

                                    ExecuteResponse responseSoap = await service.ExecuteAsync(fornecedor.Token, fornecedor.CodigoCliente, "INCLUIR_ACIONAMENTO", xmlIn);

                                    retorno = TratarXmlRetorno(responseSoap.Xmlout);

                                }
                                catch (Exception ex)
                                {
                                    retorno.Sucesso = false;
                                    retorno.Mensagem = ex.Message;
                                    retorno.Id = 0;
                                    //_log.GravarLogAsync(usuarioId, origem: origem, jsonNew: JsonConvert.SerializeObject(acionamento), erro: ex.Message);
                                }
                                finally
                                {
                                    await YHRoboIntegracaoSolfacilData.InserirRetornoAcionamentoSolfacil(connection,  acionamento.CedenteId, acionamento.ClienteId, "INCLUIR_ACIONAMENTO", fornecedor.CodigoCliente, null, int.Parse(acionamento.NuContrato), acionamento.DataAcao,
                                                                            acionamento.HoraAcao, acionamento.ExternoId, acionamento.DataAgenda, acionamento.NmAnalista, acionamento.Report, acionamento.Tipo,
                                                                            acionamento.Telefone, acionamento.Email, retorno.Id.Value, retorno.Mensagem, retorno.Sucesso);
                                }

                                tot++;
                                Console.SetCursorPosition(0, Console.CursorTop);
                                Console.Write($@"Enviando integração:({acionamentos.Count()}) - Enviado:({tot})");
                            }

                            //Atualizar ultima execucao 
                            await YHRoboIntegracaoSolfacilData.AtualizarUltimaExecucao(connection, cedenteId, "INTEGRACAOTABULACAO", dataExecucaoAtual);
                        }
                        else
                        {
                            throw new Exception("Falha ao enviar o acionamento para o fornecedor: Fornecedor não localizado.");
                        }
                                                
                    }
                    catch (Exception ex)
                    {
                        string erroBuscaEnvio = ex.Message;
                        Console.WriteLine($"ERRO: {ex.Message}");
                    }
                //}
                    cont = 0;
                    tot = 0;
                    Console.Clear();
                    Console.WriteLine("*************************************************************************");
                    Console.WriteLine($" Robo - Integração SolFacil (Tabulações)");
                    Console.WriteLine("*************************************************************************");
                    Console.WriteLine();
                }
            }
        }

        private static string CreateAcionamentoXMLRequest(Fornecedor fornecedor, AcionamentoRequest acionamento)
        {
            string xmlIn = @$"<incluir_acionamento>
                                        <cod_assessoria>{fornecedor.CodigoCliente}</cod_assessoria>
                                        <emp_cliente></emp_cliente>
                                        <cod_cliente>{acionamento.NuContrato}</cod_cliente>
                                        <dat_acao>{acionamento.DataAcao}</dat_acao>
                                        <hor_acao>{acionamento.HoraAcao}</hor_acao>
                                        <cod_sit>{acionamento.ExternoId}</cod_sit>
                                        <dat_agenda>{acionamento.DataAgenda}</dat_agenda>
                                        <nom_operador>{acionamento.NmAnalista}</nom_operador>
                                        <aca_descricao>{acionamento.Report}</aca_descricao>
                                        <aca_tipo>{acionamento.Tipo}</aca_tipo>
                                        <aca_telefone>{acionamento.Telefone}</aca_telefone>
                                        <aca_email>{acionamento.Email}</aca_email>
                                      </incluir_acionamento>";

            return xmlIn;
        }

        private static ReturnMessage TratarXmlRetorno(string responseString)
        {
            ReturnMessage retorno = new ReturnMessage();
            int inicio, fim, length;
            if (responseString.ToUpper().Contains("<RETORNO>"))
            {
                XmlSerializer xmls = new XmlSerializer(typeof(SisCobraXmlOutSucesso));
                var result = (SisCobraXmlOutSucesso)xmls.Deserialize(new StringReader(responseString));

                retorno.Sucesso = true;
                retorno.Mensagem = result.descricao;
                retorno.Id = result.codigo;
            }
            else
            {
                XmlSerializer xmls = new XmlSerializer(typeof(SisCobraXmlOutErro));
                var result = (SisCobraXmlOutErro)xmls.Deserialize(new StringReader(responseString));

                retorno.Sucesso = false;
                retorno.Mensagem = result.descricao;
                retorno.Id = result.codigo;
            }

            return retorno;
        }
    }
}
