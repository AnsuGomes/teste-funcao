using FI.AtividadeEntrevista.BLL;
using FI.AtividadeEntrevista.DML;
using FI.WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebAtividadeEntrevista.Models;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Incluir(ClienteModel model)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    List<string> erros = (from item in ModelState.Values
                                          from error in item.Errors
                                          select error.ErrorMessage).ToList();

                    Response.StatusCode = 400;
                    return Json(string.Join(Environment.NewLine, erros));
                }

                BoCliente bo = new BoCliente();

                if (bo.VerificarExistencia(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json("CPF já cadastrado");
                }

                model.Id = bo.Incluir(new Cliente()
                {
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF
                });

                if (model.BeneficiarioModels != null && model.BeneficiarioModels.Count > 0)
                {
                    BoBeneficiario boBeneficiario = new BoBeneficiario();

                    foreach (BeneficiarioModel beneficiario in model.BeneficiarioModels)
                    {
                        if (beneficiario == null) continue;

                        beneficiario.Id = boBeneficiario.Incluir(new Beneficiario()
                        {
                            Nome = beneficiario.Nome,
                            CPF = beneficiario.CPF,
                            IdCliente = model.Id
                        });
                    }
                }

                return Json(new { Result = "OK", Message = "Cadastro efetuado com sucesso", BeneficiarioModels = model.BeneficiarioModels });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    List<string> erros = (from item in ModelState.Values
                                          from error in item.Errors
                                          select error.ErrorMessage).ToList();

                    Response.StatusCode = 400;
                    return Json(string.Join(Environment.NewLine, erros));
                }

                BoCliente bo = new BoCliente();

                Cliente cliente = bo.Consultar(model.Id);

                if (cliente == null || (cliente.CPF != model.CPF && bo.VerificarExistencia(model.CPF)))
                {
                    Response.StatusCode = 400;
                    return Json("Não foi possível alterar as informações cadastradas");
                }

                bo.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF
                });

                return Json(new { Result = "OK", Message = "Cadastro alterado com sucesso" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();

            Cliente cliente = bo.Consultar(id);

            if (cliente == null)
                return View(model: null);

            ClienteModel model = new ClienteModel()
            {
                Id = cliente.Id,
                CEP = cliente.CEP,
                Cidade = cliente.Cidade,
                Email = cliente.Email,
                Estado = cliente.Estado,
                Logradouro = cliente.Logradouro,
                Nacionalidade = cliente.Nacionalidade,
                Nome = cliente.Nome,
                Sobrenome = cliente.Sobrenome,
                Telefone = cliente.Telefone,
                CPF = cliente.CPF,
                BeneficiarioModels = GetBeneficiarioModels(id)
            };

            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult BeneficiarioModal()
        {
            return PartialView("~/Views/Beneficiario/BeneficiarioModal.cshtml");
        }

        [HttpPost]
        public ActionResult IncluirBeneficiario(BeneficiarioModel beneficiarioModel)
        {
            try
            {
                BoBeneficiario boBeneficiario = new BoBeneficiario();

                Beneficiario beneficiario = new Beneficiario
                {
                    Id = beneficiarioModel.Id,
                    Nome = beneficiarioModel.Nome,
                    CPF = beneficiarioModel.CPF,
                    IdCliente = beneficiarioModel.IdCliente
                };

                if (boBeneficiario.VerificarExistencia(beneficiario.CPF))
                    boBeneficiario.Alterar(beneficiario);
                else
                    boBeneficiario.Incluir(beneficiario);

                List<BeneficiarioModel> beneficiarioModels = GetBeneficiarioModels(beneficiarioModel.IdCliente);

                return Json(new { Result = "OK", Message = "Beneficiário salvo com sucesso!", BeneficiarioModels = beneficiarioModels });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        private static List<BeneficiarioModel> GetBeneficiarioModels(long idCliente)
        {
            return new BoBeneficiario().ListarBeneficiarios(idCliente).Select(x => new BeneficiarioModel()
            {
                Id = x.Id,
                CPF = x.CPF,
                Nome = x.Nome,
                IdCliente = x.IdCliente
            }).ToList();
        }

        [HttpPost]
        public ActionResult ExcluirBeneficiario(long id, long idCliente)
        {
            try
            {
                BoBeneficiario boBeneficiario = new BoBeneficiario();

                boBeneficiario.Excluir(id);

                if (idCliente > 0)
                {
                    List<BeneficiarioModel> beneficiarioModels = GetBeneficiarioModels(idCliente);

                    return Json(new { Result = "OK", Message = "Beneficiário excluído com sucesso!", BeneficiarioModels = beneficiarioModels });
                }

                return Json(new { Result = "OK", Message = "Beneficiário excluído com sucesso!" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}