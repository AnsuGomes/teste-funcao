let BENEFICIARIO_LIST = []
let ALTER_BENF_INDEX = null
let ALTER_ORIGINAL_OBJ = null

$(document).ready(function () {
    MostrarPopUp();

    $(document).on('hidden.bs.modal', '#modalBeneficiarios', function () {
        $('#beneficiario-modal').focus();
        $('.modal-backdrop').remove();
    });

    $(document).on("click", "#IncluirBeneficiario", function (e) {
        e.preventDefault();

        let cpfBeneficiario = $("#CPFBeneficiario").val()
        let nomeBeneficiario = $("#NomeBeneficiario").val()

        if (ValidarCampos(cpfBeneficiario, nomeBeneficiario))
            return;

        let fluxoAlterar = this.baseURI.includes("Alterar");

        if (ALTER_BENF_INDEX === null) {
            let novoBeneficiario = {
                'Id': 0,
                'CPF': cpfBeneficiario,
                'Nome': nomeBeneficiario,
                'IdCliente': fluxoAlterar ? Number(this.baseURI.slice(-1)) : 0
            }

            BENEFICIARIO_LIST.push(novoBeneficiario);

            if (fluxoAlterar)
                IncluirBeneficiario(BENEFICIARIO_LIST[BENEFICIARIO_LIST.length - 1])
        } else {
            const parsedList = JSON.parse(localStorage.getItem("beneficiario-list"))

            BENEFICIARIO_LIST[ALTER_BENF_INDEX] = {
                'Id': parsedList[ALTER_BENF_INDEX].Id,
                'CPF': cpfBeneficiario,
                'Nome': nomeBeneficiario,
                'IdCliente': parsedList[ALTER_BENF_INDEX].IdCliente,
            };

            if (fluxoAlterar)
                IncluirBeneficiario(BENEFICIARIO_LIST[ALTER_BENF_INDEX])
        }

        localStorage.setItem("beneficiario-list", JSON.stringify(BENEFICIARIO_LIST));

        PreencherListaBeneficiarios();
        ResetarCamposModal();
        ResetarVariaveisEdicao();
    });
});

function MostrarPopUp() {
    $("#beneficiario-modal").on("click", function (e) {
        e.preventDefault();

        let url = e.currentTarget.baseURI.replace(/(Alterar|Incluir)/, "BeneficiarioModal");

        $.ajax({
            url: url,
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({}),
            success: function (response) {
                if (response.Status === undefined) {
                    $('#modal-container').html(response);
                    $('#modalBeneficiarios').modal('show');
                    $('#cpf-modal').mask('000.000.000-00');

                    PreencherListaBeneficiarios();

                } else {
                    console.error('Erro na resposta:', response);
                }
            }
        });
    });
}

function ValidarCampos(cpf, nome) {
    let mensagemErro = "";

    if (cpf && nome) {
        const beneficiarioExistente = BENEFICIARIO_LIST.find(item => item.CPF === cpf);

        if (beneficiarioExistente) {
            const salvandoAlteracao = ALTER_BENF_INDEX !== null && ALTER_ORIGINAL_OBJ !== null;

            if (salvandoAlteracao) {
                const foiAlteradoCPF = cpf !== ALTER_ORIGINAL_OBJ.CPF;
                const foiAlteradoNome = nome !== ALTER_ORIGINAL_OBJ.Nome;

                if (foiAlteradoCPF)
                    mensagemErro = "O CPF informado já foi cadastrado como beneficiário";

                else if (!foiAlteradoNome && !foiAlteradoCPF) {
                    mensagemErro = "Beneficiário com os dados informados já está cadastrado";
                    ResetarCamposModal();
                    ResetarVariaveisEdicao();
                }

                else if (foiAlteradoNome && !foiAlteradoCPF)
                    return false;

            } else
                mensagemErro = "O CPF informado já foi cadastrado como beneficiário";
        }
        else if (cpf.length !== 14)
            mensagemErro = "O CPF informado está incompleto";
        else
            return false;
    }

    if (!cpf)
        mensagemErro = "O CPF é obrigatório";

    if (!nome)
        mensagemErro += "<br> O Nome é obrigatório";

    ModalDialog("Campo(s) Inválido(s)", mensagemErro);

    $('#beneficiario-modal').focus();

    return true;
}

function ResetarCamposModal() {
    $("#cpf-modal").val("");
    $("#nome-modal").val("");
}

function ResetarVariaveisEdicao() {
    ALTER_BENF_INDEX = null;
    ALTER_ORIGINAL_OBJ = null;
}

function IncluirBeneficiario(beneficiario) {
    $.ajax({
        url: `/Cliente/IncluirBeneficiario`,
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(beneficiario),
        success: function (response) {
            if (response.Result === "OK") {
                ModalDialog(`Beneficiário Criado/Atualiazado`, response.Message);

                localStorage.setItem("beneficiario-list", JSON.stringify(response.BeneficiarioModels));
            }
            else
                ModalDialog(`Erro ao Criado/Atualiazado Beneficiário`, response.Message);
        }
    });
}

function PreencherListaBeneficiarios() {
    let beneficiarioList = localStorage.getItem("beneficiario-list")

    if (beneficiarioList) {
        const parsedList = JSON.parse(beneficiarioList)

        $("table.table tbody").empty();
        $("table.table").addClass("hidden");

        if (Array.isArray(parsedList) && parsedList.length > 0) {
            parsedList.forEach((item, index) => {
                AdicionarLinha(item, index)
            })

            BENEFICIARIO_LIST = parsedList;
        }
    }
}