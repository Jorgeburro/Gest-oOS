$(document).ready(function () {
    const detailsPane = $('#details-pane');

    // --- CARREGAR DETALHES DA POSIÇÃO (SEU CÓDIGO - JÁ FUNCIONA) ---
    $('.ativo').on('click', function (e) {
        e.preventDefault();
        $('.ativo').removeClass('selected');
        $(this).addClass('selected');
        var posicaoId = $(this).data('posicao-id');
        detailsPane.html('<p class="descrption d2">Carregando...</p>');

        $.get('/Ativos/DetalhesPosicaoPartial?posicaoId=' + posicaoId, function (data) {
            detailsPane.html(data);
        }).fail(function () {
            detailsPane.html('<p class="text-danger">Erro ao carregar os detalhes.</p>');
        });
    });

    // --- ENVIAR FORMULÁRIO DE NOVO ATIVO FIXO (SEU CÓDIGO - JÁ FUNCIONA) ---
    $(document).on('submit', '#form-adicionar-ativo-fixo', function (e) {
        e.preventDefault();
        var form = $(this);
        $.ajax({
            url: form.attr('action'),
            type: form.attr('method'),
            data: form.serialize(),
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    location.reload();
                }
            },
            error: function (response) {
                alert(response.responseJSON.message || "Ocorreu um erro.");
            }
        });
    });

    // --- ENVIAR FORMULÁRIO PARA ADICIONAR CADEIRA (LÓGICA CORRETA) ---
    $(document).on('submit', '#form-adicionar-cadeira', function (e) {
        e.preventDefault();
        var form = $(this);
        $.ajax({
            url: form.attr('action'),
            type: form.attr('method'),
            data: form.serialize(),
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    $('#geralModal').modal('hide');
                    location.reload();
                }
            },
            error: function (response) {
                alert(response.responseJSON.message || "Ocorreu um erro ao adicionar a cadeira.");
            }
        });
    });

});