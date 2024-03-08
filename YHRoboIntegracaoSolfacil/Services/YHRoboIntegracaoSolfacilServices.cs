using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace YHRoboIntegracaoSolfacil.Services
{
    public static class YHRoboIntegracaoSolfacilServices
    {

        public static async Task BuscarTabulacoes(string conection)
        {
            int tot = 0;
            Console.WriteLine("*************************************************************************");
            Console.WriteLine($" Robo - Integração SolFacil (Tabulações)");
            Console.WriteLine("*************************************************************************");
            Console.WriteLine();
            int intervalo = 30;

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

                        List<MensagemEnvio> envios = YHData.BuscarEnviosWhatsApp(conection, _settings.Procedure);

                        int? erroEnvioId = null;
                        int? erroHNId = null;

                        CedenteHorarios horarioExecucao = null;
                        if (envios != null && envios.Count > 0)
                        {
                            //Pegar o CEDENTE e consultar os horarios;
                            horarioExecucao = YHData.BuscarCedenteHorarios(conection, envios[0].CedenteId);
                        }

                        foreach (var dados in envios)
                        {
                            try
                            {
                                //Validar o horario
                                if (dados.BoolAgendamento || ValidarHorario(horarioExecucao))
                                {
                                    Retorno retorno = new Retorno();

                                    if (dados.TipoFlow.ToUpper() == "BOT")
                                    {
                                        retorno = await DigitalManagerServices.EnviarMensagemBot(dados);
                                    }
                                    else //if (dados.TipoFlow.ToUpper() == "COMUNICADO")
                                    {
                                        retorno = await DigitalManagerServices.EnviarMensagemSimples(dados);
                                    }

                                    //if (retorno.Sucesso)
                                    //{
                                    //    if (!string.IsNullOrEmpty(dados.UrlHN))
                                    //    {
                                    //        //ENVIAR PARA O HELLO NEXT ********************************************************************
                                    //        HttpResponseMessage response = EnviarHelloNext(dados.UrlHN, dados.UUIDCliente, dados.IdFlow, dados.IdTemplate, retorno.SessionId, retorno.Id);

                                    //        if (!response.IsSuccessStatusCode)
                                    //        {
                                    //            string erroHelloNext = await response.Content.ReadAsStringAsync();
                                    //            erroHNId = YHData.GravarErroEnvioHelloNext(retorno.Id, retorno.SessionId, erroHelloNext, conection);
                                    //        }
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    erroEnvioId = YHData.GravarErroEnvioMensagem(retorno.Mensagem, conection);
                                    //}

                                    if (!retorno.Sucesso)
                                    {
                                        erroEnvioId = YHData.GravarErroEnvioMensagem(retorno.Mensagem, conection);
                                    }

                                    YHData.AtualizarEnviosWhatsApp(dados.EnvioId, retorno.Sucesso ? 2 : 3, retorno.Id, erroEnvioId, erroHNId, conection);
                                }
                                else
                                {
                                    //Se estiver fora do horario, update para de statusenvio 1 para 4 onde boolagendameto = 0 ou null;
                                    YHData.AtualizarEnviosForaDoHorario(conection, envios[0].CedenteId);
                                }

                            }
                            catch (Exception ex)
                            {
                                string erroEnvio = ex.Message;
                            }

                            tot++;
                            Console.SetCursorPosition(0, Console.CursorTop);
                            Console.Write($@"Enviando mensagem:({envios.Count()}) - Enviado:({tot})");
                        }

                        goto proximo;
                    }
                    catch (Exception ex)
                    {
                        string erroBuscaEnvio = ex.Message;
                    }
                //}

                proximo:
                    cont = 0;
                    tot = 0;
                    Console.Clear();
                    Console.WriteLine("*************************************************************************");
                    Console.WriteLine($" Robo - Envio WhatsApp Digital Manager ({_settings.Procedure})");
                    Console.WriteLine("*************************************************************************");
                    Console.WriteLine();
                }
            }
        }

    }
}
