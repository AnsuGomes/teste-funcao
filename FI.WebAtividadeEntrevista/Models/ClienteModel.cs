using FI.WebAtividadeEntrevista.ConfigHelper;
using FI.WebAtividadeEntrevista.Models;
using FI.WebAtividadeEntrevista.Validators;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAtividadeEntrevista.Models
{
    /// <summary>
    /// Classe de Modelo de Cliente
    /// </summary>
    public class ClienteModel
    {
        public long Id { get; set; }

        /// <summary>
        /// CEP
        /// </summary>
        [Required]
        public string CEP { get; set; }

        /// <summary>
        /// Cidade
        /// </summary>
        [Required]
        public string Cidade { get; set; }

        /// <summary>
        /// E-mail
        /// </summary>
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Digite um e-mail válido")]
        public string Email { get; set; }

        /// <summary>
        /// Estado
        /// </summary>
        [Required]
        [MaxLength(2)]
        public string Estado { get; set; }

        /// <summary>
        /// Logradouro
        /// </summary>
        [Required]
        public string Logradouro { get; set; }

        /// <summary>
        /// Nacionalidade
        /// </summary>
        [Required]
        public string Nacionalidade { get; set; }

        /// <summary>
        /// Nome
        /// </summary>
        [Required]
        public string Nome { get; set; }

        /// <summary>
        /// Sobrenome
        /// </summary>
        [Required]
        public string Sobrenome { get; set; }

        /// <summary>
        /// Telefone
        /// </summary>
        public string Telefone { get; set; }

        private string _cpf;

        /// <summary>
        /// CPF
        /// </summary>
        [Required]
        [CPFValidator(ErrorMessage = "CPF inválido")]
        public string CPF
        {
            get => _cpf;
            set => _cpf = Util.RemoveNaoNumericos(value);
        }

        /// <summary>
        /// Beneficiários
        /// </summary>
        public List<BeneficiarioModel> BeneficiarioModels { get; set; }

    }
}