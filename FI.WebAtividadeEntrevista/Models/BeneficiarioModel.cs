using FI.WebAtividadeEntrevista.ConfigHelper;
using FI.WebAtividadeEntrevista.Validators;
using System.ComponentModel.DataAnnotations;

namespace FI.WebAtividadeEntrevista.Models
{
    public class BeneficiarioModel
    {
        public long Id { get; set; }

        [Required]
        public string Nome { get; set; }

        private string _cpf;

        [Required]
        [CPFValidator]
        public string CPF
        {
            get => _cpf;
            set => _cpf = Util.RemoveNaoNumericos(value);
        }

        public long IdCliente { get; set; }
    }
}