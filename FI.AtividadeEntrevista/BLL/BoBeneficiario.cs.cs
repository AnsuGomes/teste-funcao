using FI.AtividadeEntrevista.DAL.Beneficiarios;
using FI.AtividadeEntrevista.DML;
using System.Collections.Generic;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoBeneficiario
    {
        public long Incluir(Beneficiario beneficiario)
        {
            DaoBeneficiario beneficiarioDao = new DaoBeneficiario();

            return beneficiarioDao.Incluir(beneficiario);
        }

        public List<Beneficiario> ListarBeneficiarios(long idCliente)
        {
            DaoBeneficiario beneficiarioDao = new DaoBeneficiario();

            return beneficiarioDao.ListarBeneficiarios(idCliente);
        }

        public bool VerificarExistencia(string CPF)
        {
            DaoBeneficiario beneficiarioDao = new DaoBeneficiario();

            return beneficiarioDao.VerificarExistencia(CPF);
        }

        public void Alterar(Beneficiario beneficiario)
        {
            DaoBeneficiario beneficiarioDao = new DaoBeneficiario();

            beneficiarioDao.Alterar(beneficiario);
        }

        public void Excluir(long id)
        {
            DaoBeneficiario beneficiarioDao = new DaoBeneficiario();

            beneficiarioDao.Excluir(id);
        }
    }
}