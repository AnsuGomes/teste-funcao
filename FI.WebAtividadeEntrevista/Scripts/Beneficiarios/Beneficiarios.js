const BeneficiariosModule = (() => {
    // Variaveis de estado encapsuladas no módulo
    let beneficiarioList = [];
    let editarIndex = null;
    let originalBeneficiario = null;

    // Seletores como constantes internas
    const selectors = {
        modalBeneficiarios: '#modalBeneficiarios',
        beneficiarioModalTrigger: '#beneficiario-modal',
        incluirBtn: '#incluir-button',
        cpfInput: '#CPFBeneficiario',
        nomeInput: '#NomeBeneficiario',
        table: 'table.table',
        tableBody: 'table.table tbody'
    };

    // Funcoes internas do modulo
    const isAlterarFluxo = () => window.location.href.includes('Alterar');

    const obterIdCliente = () => {
        if (isAlterarFluxo())
            return Number(window.location.href.slice(-1));

        return 0;
    };

    const atualizarLocalStorage = () => {
        localStorage.setItem('beneficiario-list', JSON.stringify(beneficiarioList));
    };

    const resetarCampos = () => {
        $(selectors.cpfInput).val('');
        $(selectors.nomeInput).val('');
    };

    const resetarEdicao = () => {
        editarIndex = null;
        originalBeneficiario = null;
    };

    const preencherTabela = () => {
        const storedList = localStorage.getItem('beneficiario-list');
        if (!storedList) {
            $(selectors.table).addClass('hidden');
            $(selectors.tableBody).empty();
            beneficiarioList = [];
            return;
        }

        const parsedList = JSON.parse(storedList);
        if (!Array.isArray(parsedList) || parsedList.length === 0) {
            $(selectors.table).addClass('hidden');
            $(selectors.tableBody).empty();
            beneficiarioList = [];
            return;
        }

        beneficiarioList = parsedList;
        const $tbody = $(selectors.tableBody);
        $tbody.empty();

        parsedList.forEach((item, index) => {
            const row = `
                <tr>
                    <input type="hidden" name="id-benef" value="${item.Id}">
                    <input type="hidden" name="id-client" value="${item.IdCliente}">
                    <td data-label="CPF">${formatarCPF(item.CPF)}</td>
                    <td class="nome-cell" data-label="Nome" title="${item.Nome}">${item.Nome}</td>
                    <td data-label="Ações" style="display: flex; justify-content: space-between;">
                        <button data-index="${index}" class="btn btn-primary alterar-button">Alterar</button>
                        <button data-index="${index}" class="btn btn-primary excluir-button">Excluir</button>
                    </td>
                </tr>`;

            $tbody.append(row);
        });

        $(selectors.table).removeClass('hidden');
    };

    const exibirModal = () => {
        $(selectors.beneficiarioModalTrigger).on('click', (e) => {
            e.preventDefault();
            const url = e.currentTarget.baseURI.replace(/(Alterar|Incluir)/, 'BeneficiarioModal');

            $.ajax({
                url,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({}),
                success: (response) => {
                    if (response.Status === undefined) {
                        $('#modal-container').html(response);
                        $(selectors.modalBeneficiarios).modal('show');
                        $(selectors.cpfInput).mask('000.000.000-00');
                        preencherTabela();
                    } else
                        console.error('Erro na resposta:', response);
                }
            });
        });
    };

    const validarCampos = (cpf, nome) => {
        let mensagemErro = '';
        if (!cpf) mensagemErro = 'O CPF é obrigatório';
        if (!nome) mensagemErro += (mensagemErro ? '<br>' : '') + 'O Nome é obrigatório';

        if (cpf && nome) {
            if (cpf.length !== 14)
                mensagemErro = 'O CPF informado está incompleto';
            else {
                const existe = beneficiarioList.find(b => b.CPF === cpf);
                if (existe) {
                    const isEditando = editarIndex !== null && originalBeneficiario !== null;
                    if (isEditando) {
                        const cpfAlterado = cpf !== originalBeneficiario.CPF;
                        const nomeAlterado = nome !== originalBeneficiario.Nome;

                        if (cpfAlterado)
                            mensagemErro = 'O CPF informado já foi cadastrado como beneficiário';
                        else if (!cpfAlterado && !nomeAlterado) {
                            mensagemErro = 'Beneficiário com os dados informados já está cadastrado';
                            resetarCampos();
                            resetarEdicao();
                        }
                    } else
                        mensagemErro = 'O CPF informado já foi cadastrado como beneficiário';
                }
            }
        }

        if (mensagemErro) {
            ModalDialog('Campo(s) Inválido(s)', mensagemErro);
            $(selectors.beneficiarioModalTrigger).focus();
            return true;
        }

        return false;
    };

    const incluirBeneficiarioServer = (beneficiario) => {
        $.ajax({
            url: '/Cliente/IncluirBeneficiario',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(beneficiario),
            success: (resp) => {
                if (resp.Result === 'OK') {
                    alert(resp.Message);
                    if (resp.BeneficiarioModels) {
                        beneficiarioList = resp.BeneficiarioModels;
                        atualizarLocalStorage();
                        preencherTabela();
                    }
                } else
                    alert(resp.Message);
            },
            error: () => alert('Erro na comunicação com o servidor.')
        });
    };

    const excluirBeneficiarioServer = (beneficiarioId, idCliente) => {
        $.ajax({
            url: '/Cliente/ExcluirBeneficiario',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify({ Id: beneficiarioId, IdCliente: idCliente }),
            success: (resp) => {
                if (resp.Result === 'OK') {
                    alert(resp.Message);
                    if (resp.BeneficiarioModels) {
                        beneficiarioList = resp.BeneficiarioModels;
                        atualizarLocalStorage();
                        preencherTabela();
                    }
                } else
                    alert(resp.Message);
            },
            error: () => alert('Erro na comunicação com o servidor.')
        });
    };

    const configurarEventos = () => {
        // Modal close cleanup
        $(document).on('hidden.bs.modal', selectors.modalBeneficiarios, () => {
            $(selectors.beneficiarioModalTrigger).focus();
            $('.modal-backdrop').remove();
        });

        // Incluir / Alterar beneficiario
        $(document).on('click', selectors.incluirBtn, (e) => {
            e.preventDefault();
            const cpf = $(selectors.cpfInput).val();
            const nome = $(selectors.nomeInput).val();

            if (validarCampos(cpf, nome)) return;

            const fluxoAlterar = isAlterarFluxo();

            if (editarIndex === null) {
                const novo = {
                    Id: 0,
                    CPF: cpf,
                    Nome: nome,
                    IdCliente: obterIdCliente()
                };
                beneficiarioList.push(novo);

                if (fluxoAlterar) incluirBeneficiarioServer(novo);
            } else {
                const localList = JSON.parse(localStorage.getItem('beneficiario-list')) || [];
                const original = localList[editarIndex] || {};

                beneficiarioList[editarIndex] = {
                    Id: original.Id,
                    CPF: cpf,
                    Nome: nome,
                    IdCliente: original.IdCliente,
                };

                if (fluxoAlterar) incluirBeneficiarioServer(beneficiarioList[editarIndex]);
            }

            atualizarLocalStorage();
            preencherTabela();
            resetarCampos();
            resetarEdicao();
        });

        // Botao Alterar
        $(document).on('click', '.alterar-button', (e) => {
            e.preventDefault();
            editarIndex = Number($(e.currentTarget).data('index'));
            if (isNaN(editarIndex) || editarIndex < 0 || editarIndex >= beneficiarioList.length) return;

            const b = beneficiarioList[editarIndex];
            $(selectors.cpfInput).val(formatarCPF(b.CPF)).mask('000.000.000-00');
            $(selectors.nomeInput).val(b.Nome);

            originalBeneficiario = { ...b };
        });

        // Botao Excluir
        $(document).on('click', '.excluir-button', (e) => {
            e.preventDefault();
            if (!confirm('Tem certeza de que deseja excluir este beneficiário? Este processo é irreversível.')) return;

            const idx = Number($(e.currentTarget).data('index'));
            if (isNaN(idx) || idx < 0 || idx >= beneficiarioList.length) return;

            const b = beneficiarioList[idx];
            beneficiarioList.splice(idx, 1);
            atualizarLocalStorage();
            preencherTabela();
            resetarCampos();
            resetarEdicao();

            excluirBeneficiarioServer(b.Id, b.IdCliente);
        });

        // Ao abrir modal, carregar lista do localStorage
        $(document).on('click', selectors.beneficiarioModalTrigger, (e) => {
            e.preventDefault();
            const stored = JSON.parse(localStorage.getItem('beneficiario-list'));
            beneficiarioList = Array.isArray(stored) && stored.length > 0 ? stored : [];
        });
    };

    const init = () => {
        exibirModal();
        configurarEventos();
    };

    // Expor apenas o método init
    return {
        init,
    };
})();

$(document).ready(() => {
    BeneficiariosModule.init();
});
