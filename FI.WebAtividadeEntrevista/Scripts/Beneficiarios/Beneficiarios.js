$(document).ready(function () {
    MostrarPopUp();

    // fechar modal
    $(document).on('hidden.bs.modal', '#modalBeneficiarios', function () {
        $('#beneficiario-modal').focus();
        $('.modal-backdrop').remove();
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