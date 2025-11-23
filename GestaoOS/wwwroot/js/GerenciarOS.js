document.addEventListener('DOMContentLoaded', function () {
    const containerDetalhes = document.getElementById('detalhes-posicao-container');
    const posicoes = document.querySelectorAll('.posicao-mapa');

    // Lógica principal para carregar a Partial View ao clicar numa posição do mapa
    posicoes.forEach(posicao => {
        posicao.addEventListener('click', function () {
            posicoes.forEach(p => p.classList.remove('active'));
            this.classList.add('active');
            const url = this.dataset.url;

            containerDetalhes.innerHTML = '<div class="caixa-detalhes-vazio"><p>Carregando...</p></div>';

            fetch(url)
                .then(response => response.text())
                .then(html => {
                    containerDetalhes.innerHTML = html;
                    // IMPORTANTE: Após carregar o novo HTML, ativamos o script dele
                    inicializarLogicaDaPartial();
                })
                .catch(error => {
                    console.error('Erro ao carregar detalhes:', error);
                    containerDetalhes.innerHTML = '<div class="alert alert-danger">Falha ao carregar.</div>';
                });
        });
    });
});

// Esta função será chamada DEPOIS que a Partial View for carregada via AJAX.
// Ela ativa os botões e formulários DENTRO da partial.
function inicializarLogicaDaPartial() {
    const botoesAtivo = document.querySelectorAll('#lista-ativos-selecao .list-group-item-action');
    const formOS = document.getElementById('form-os-fixo');

    if (formOS) { // Só executa se o formulário existir na partial
        const hiddenInputId = document.getElementById('ativo-id-selecionado');
        const nomeAtivoSpan = document.getElementById('nome-ativo-selecionado');

        botoesAtivo.forEach(botao => {
            botao.addEventListener('click', function () {
                botoesAtivo.forEach(b => b.classList.remove('active'));
                this.classList.add('active');

                const ativoId = this.dataset.ativoid;
                const nomeAtivo = this.innerText.trim();

                hiddenInputId.value = ativoId;
                nomeAtivoSpan.innerText = `"${nomeAtivo}"`;

                formOS.classList.remove('d-none'); // Mostra o formulário
            });
        });
    }
}