using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YHRoboIntegracaoSolfacil.Models;
using Dapper;
using System.Globalization;

namespace YHRoboIntegracaoSolfacil.Data
{
    public class YHRoboIntegracaoSolfacilData
    {
        public static async Task<Fornecedor> GetFornecedor(string conection, int cedenteId, string nomeFornecedor)
        {
            SqlConnection conn = new SqlConnection(conection);
            conn.Open();

            Fornecedor fornecedor = await conn.QueryFirstOrDefaultAsync<Fornecedor>($"API_SPS_GetFornecedor_V2",
                                                            new
                                                            {
                                                                CedenteId = cedenteId,
                                                                NmFornecedor = nomeFornecedor
                                                            },
                                                                commandTimeout: 3600,
                                                                commandType: CommandType.StoredProcedure);
           
            conn.Close();

            return fornecedor;
        }

        public static async Task<DateTime> GetUltimaExecucao(string strCon, int cedenteId, string nomeProcesso)
        {
            SqlConnection conn = new SqlConnection(strCon);
            conn.Open();

            DateTime dtUltimaExecucao = await conn.QuerySingleAsync<DateTime>($"API_sps_ControleExecucao_GetUltimaExecucao",
                                                                 new
                                                                 {
                                                                     CedenteId = cedenteId,
                                                                     NmProcesso = nomeProcesso
                                                                 },
                                                                commandTimeout: 3600,
                                                                commandType: CommandType.StoredProcedure);

            conn.Close();

            return dtUltimaExecucao;
        }

        public static async Task<bool> AtualizarUltimaExecucao(string strCon, int cedenteId, string nomeProcesso, DateTime dataExecucao)
        {
            SqlConnection conn = new SqlConnection(strCon);
            conn.Open();

            int rowId = await conn.QuerySingleAsync<int>($"API_spu_ControleExecucao_UltimaExecucao",
                                                new
                                                {
                                                    CedenteId = cedenteId,
                                                    NmProcesso = nomeProcesso,
                                                    DataExecucao = dataExecucao
                                                },
                                            commandTimeout: 3600,
                                            commandType: CommandType.StoredProcedure);

            conn.Close();

            return rowId > 0;
        }

        public static async Task<List<AcionamentoRequest>> GetAcoesHumanoSolfacil(string conection, int cedenteId, DateTime dataUltimaExecucao)
        {
            SqlConnection conn = new SqlConnection(conection);
            conn.Open();

            var result = await conn.QueryAsync<AcionamentoRequest>($"API_SPS_GetAcoesHumanoSolfacil",
                                                            new
                                                            {
                                                                CedenteId = cedenteId,
                                                                DtUltimaExecucao = dataUltimaExecucao
                                                            },
                                                                commandTimeout: 3600,
                                                                commandType: CommandType.StoredProcedure);

            List<AcionamentoRequest> acionamentos = result.ToList();

            conn.Close();

            return acionamentos;
        }

        public static async Task InserirRetornoAcionamentoSolfacil(string connection, int cedenteId, int clienteId, string metodo, int codigoAssessoria, int? empresaCliente, int codigoCliente, string dataAcao, 
                                                            string horaAcao, int codigoSituacao, string dataAgenda, string nomeOperador, string descricaoAcao, int tipoAcao, string telefoneAcao, 
                                                            string emailAcao, int codigoRetorno, string mensagemRetorno, bool sucesso)
        {
            SqlConnection conn = new SqlConnection(connection);
            int rows = 0;

            try
            {
                conn.Open();
                DateTime? dtAgenda = null;
                if (!string.IsNullOrEmpty(dataAgenda))
                {
                    dtAgenda = DateTime.ParseExact(dataAgenda, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                } 
                rows = await conn.ExecuteAsync("API_spi_SolfacilRetornoAcionamento",
                                                new
                                                {
                                                    CedenteId = cedenteId,
                                                    ClienteId = clienteId,
                                                    NmMetodo = metodo,
                                                    NumCodAssessoria = codigoAssessoria,
                                                    NumEmpCliente = empresaCliente,
                                                    NumCodClilente = codigoCliente,
                                                    DtAcao = DateTime.ParseExact(dataAcao, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                                    HoraAcao = horaAcao,
                                                    NumCodSit = codigoSituacao,
                                                    DtAgenda = dtAgenda,
                                                    NmNomeOperador = nomeOperador,
                                                    NmAcaDescricao = descricaoAcao,
                                                    NumAcaTipo = tipoAcao,
                                                    NmAcaTelefone = telefoneAcao,
                                                    NmAcaEmail = emailAcao,
                                                    NumRetornoCodigo = codigoRetorno,
                                                    NmRetornoDescricao = mensagemRetorno,
                                                    BoolSucesso = sucesso
                                                },
                                                commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception("");
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
