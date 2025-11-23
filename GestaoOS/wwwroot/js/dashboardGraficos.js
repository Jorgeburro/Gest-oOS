    document.addEventListener('DOMContentLoaded', function () {
    let chartMensal, chartSemanal, chartSalas, chartTipos;

    function buildChart(ctx, type, labels, data, options = { }) {
            return new Chart(ctx, {
        type,
        data: {
        labels,
        datasets: [{
        label: 'Quantidade',
    data,
    borderWidth: 2,
    backgroundColor: type === 'pie' ?
    ['#4e79a7','#f28e2b','#e15759','#76b7b2','#59a14f','#edc948','#b07aa1','#ff9da7','#9c755f','#bab0ab'] :
    'rgba(13,110,253,0.25)',
    borderColor: '#0d6efd',
    tension: type === 'line' ? 0.25 : 0
                    }]
                },
    options: Object.assign({
        responsive: true,
    maintainAspectRatio: true, // FIX: evita crescer em layout flex/grid
    plugins: {legend: {display: type === 'pie' } },
    scales: (type === 'pie') ? { } : {y: {beginAtZero: true } }
                }, options)
            });
        }

        async function loadData() {


            // (1) MOSTRA O CARREGAMENTO NOS KPIs
            const kpiAtendimentoEl = document.getElementById('kpiTempoMedioAtendimento');
            const kpiConclusaoEl = document.getElementById('kpiTempoMedioConclusao');
            if (kpiAtendimentoEl) kpiAtendimentoEl.textContent = "Carregando...";
            if (kpiConclusaoEl) kpiConclusaoEl.textContent = "Carregando...";

            const start = document.getElementById('startDate').value;
            const end = document.getElementById('endDate').value;
            const qs = new URLSearchParams();
            if (start) qs.append('startDate', start);
            if (end) qs.append('endDate', end);

            const res = await fetch(`/Home/DashboardGraficosData?${qs.toString()}`);
            const json = await res.json();

            const ctxMensal = document.getElementById('chartMensal').getContext('2d');
            const ctxSemanal = document.getElementById('chartSemanal').getContext('2d');
            const ctxSalas = document.getElementById('chartSalas').getContext('2d');
            const ctxTipos = document.getElementById('chartTipos').getContext('2d');

            if (chartMensal) chartMensal.destroy();
            if (chartSemanal) chartSemanal.destroy();
            if (chartSalas) chartSalas.destroy();
            if (chartTipos) chartTipos.destroy();
            if (kpiAtendimentoEl) kpiAtendimentoEl.textContent = json.kpiTempoAtendimento;
            if (kpiConclusaoEl) kpiConclusaoEl.textContent = json.kpiTempoConclusao;

            chartMensal = buildChart(ctxMensal, 'bar', json.mensalLabels, json.mensalData);
            chartSemanal = buildChart(ctxSemanal, 'line', json.semanalLabels, json.semanalData);
            chartSalas = buildChart(ctxSalas, 'pie', json.topSalasLabels, json.topSalasData);
            chartTipos = buildChart(ctxTipos, 'bar', json.topTiposLabels, json.topTiposData);
                }

            function setLast6Months() {
                    const end = new Date();
            const start = new Date();
            start.setMonth(start.getMonth() - 6);
            document.getElementById('endDate').valueAsDate = end;
            document.getElementById('startDate').valueAsDate = start;
                }

            document.getElementById('btnAplicar').addEventListener('click', loadData);
                document.getElementById('btnUltimos6').addEventListener('click', () => {setLast6Months(); loadData(); });

            // Inicializa com últimos 6 meses
            setLast6Months();
            loadData();
            });