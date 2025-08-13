using FI.AtividadeEntrevista.BLL;
using FI.AtividadeEntrevista.DML;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace FI.AtividadeEntrevista.DAL.Beneficiarios
{
    internal class DaoBeneficiario : AcessoDados
    {
        internal long Incluir(Beneficiario beneficiario)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("Nome", beneficiario.Nome),
                new SqlParameter("CPF", beneficiario.CPF),
                new SqlParameter("IdCliente", beneficiario.IdCliente)
            };

            DataSet ds = base.Consultar("FI_SP_IncBeneficiario", parametros);

            long ret = 0;

            if (ds.Tables[0].Rows.Count > 0)
                long.TryParse(ds.Tables[0].Rows[0][0].ToString(), out ret);
            return ret;
        }

        internal List<Beneficiario> ListarBeneficiarios(long idCliente)
        {
            List<SqlParameter> parametros = new List<SqlParameter> { new SqlParameter("IdCliente", idCliente) };

            DataSet ds = base.Consultar("FI_SP_ListarBeneficiarios", parametros);

            List<Beneficiario> beneficiarios = Converter(ds);

            return beneficiarios;
        }

        private List<Beneficiario> Converter(DataSet ds)
        {
            if (ds?.Tables == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return new List<Beneficiario>();

            return ds.Tables[0].AsEnumerable().Select(row => new Beneficiario
            {
                Id = row.Field<long>("Id"),
                Nome = row.Field<string>("Nome"),
                CPF = row.Field<string>("CPF"),
                IdCliente = row.Field<long>("IdCliente")
            }).ToList();
        }

        internal bool VerificarExistencia(string CPF)
        {
            List<SqlParameter> parametros = new List<SqlParameter> { new SqlParameter("CPF", CPF) };

            DataSet ds = base.Consultar("FI_SP_VerificaBeneficiario", parametros);

            return ds.Tables[0].Rows.Count > 0;
        }

        internal void Alterar(Beneficiario beneficiario)
        {
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("Nome", beneficiario.Nome),
                new SqlParameter("ID", beneficiario.Id),
                new SqlParameter("CPF", beneficiario.CPF)
            };

            base.Executar("FI_SP_AltBeneficiario", parametros);
        }

        internal void Excluir(long Id)
        {
            List<SqlParameter> parametros = new List<SqlParameter> { new SqlParameter("Id", Id) };

            base.Executar("FI_SP_DelBeneficiario", parametros);
        }
    }
}