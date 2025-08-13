using FI.WebAtividadeEntrevista.ConfigHelper;
using System.ComponentModel.DataAnnotations;

namespace FI.WebAtividadeEntrevista.Validators
{
    public class CPFValidator : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                ErrorMessage = "O CPF não pode estar vazio.";
                return false;
            }

            string cpf = Util.RemoveNaoNumericos(value.ToString());

            if (cpf.Length != 11 || !ValidaCPF(cpf))
            {
                ErrorMessage = $"O CPF '{value}' informado é inválido. <br>";
                return false;
            }

            return true;
        }

        internal static int GerarDigitoVerificador(string digitos)
        {
            int soma = 0;
            int multiplicador = digitos.Length + 1;

            foreach (char c in digitos)
            {
                soma += (c - '0') * multiplicador;
                multiplicador--;
            }

            int resto = soma % 11;

            return resto > 1 ? 11 - resto : 0;
        }

        internal static bool ValidaCPF(string cpf)
        {
            if (new string(cpf[0], cpf.Length) == cpf)
                return false;

            string baseCpf = cpf.Substring(0, 9);
            int primeiroDigito = GerarDigitoVerificador(baseCpf);
            int segundoDigito = GerarDigitoVerificador(baseCpf + primeiroDigito);

            return cpf == $"{baseCpf}{primeiroDigito}{segundoDigito}";
        }
    }
}