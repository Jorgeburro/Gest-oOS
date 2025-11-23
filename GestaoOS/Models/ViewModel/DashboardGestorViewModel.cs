using System.Collections.Generic;

namespace GestaoOS.Models.ViewModels
{
    // Um único ViewModel para a página de gráficos
    public class RankingItem
    {
        public string Nome { get; set; }
        public int Contagem { get; set; }
    }

    public class DashboardGraficosViewModel
    {
        // Métricas de Tempo Médio (do Histórico)
        public string TempoMedioAtendimento { get; set; }
        public string TempoMedioConclusao { get; set; }

        // Métricas de OS Ativas
        public int AtivasAbertas { get; set; }
        public int AtivasEmAndamento { get; set; }
        public int AtivasEmEspera { get; set; }

        // --- NOVAS MÉTRICAS DE RANKING ---
        public List<RankingItem> TopTecnicos { get; set; } = new();
        public List<RankingItem> TopAtivos { get; set; } = new();
    }


    // O ViewModel para os dados da API (que o Javascript busca) continua o mesmo.
    // Podemos mantê-lo no mesmo arquivo para organização.
    public class GraficoDataViewModel
    {
        public List<string> MensalLabels { get; set; } = new();
        public List<int> MensalData { get; set; } = new();
        public List<string> SemanalLabels { get; set; } = new();
        public List<int> SemanalData { get; set; } = new();
        public List<string> TopSalasLabels { get; set; } = new();
        public List<int> TopSalasData { get; set; } = new();
        public List<string> TopTiposLabels { get; set; } = new();
        public List<int> TopTiposData { get; set; } = new();
    }
    public class DashboardGestorViewModel
    {
        // KPIs do topo (continuam iguais)
        public int OrdensAbertasNaoAtribuidas { get; set; }
        public int OrdensEmExecucao { get; set; }
        public int OrdensEmEspera { get; set; }

        // Propriedades do gráfico (continuam iguais)
        public List<string> GraficoMesesLabels { get; set; } = new List<string>();
        public List<int> GraficoContagemDados { get; set; } = new List<int>();

        // NOVA propriedade para a lista limitada.; i i i i
        public List<OrdemDeServico> OrdensNaoAtribuidasRecentes { get; set; } = new List<OrdemDeServico>();
    }
}