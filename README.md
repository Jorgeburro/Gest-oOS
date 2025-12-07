Gestão de Ordens de Serviço (GestaoOS)
Um sistema web desenvolvido em Razor Pages (.NET 8) para gerenciar ordens de serviço em ambientes corporativos ou educacionais.
O projeto permite:
•	Gestão de Ativos: Cadastro e organização de ativos (computadores, mesas, cadeiras, etc.) por sala e posição.
•	Controle de Ordens de Serviço: Criação, acompanhamento e conclusão de OS com status como "Em Andamento", "Em Espera" e "Concluída".
•	Gestão de SLA: Definição de prazos para resolução e alertas para vencimentos.
•	Perfis de Usuário: Acesso diferenciado para gestores, técnicos de manutenção e professores.
Ideal para instituições que precisam organizar e monitorar solicitações de manutenção e ativos de forma eficiente.

 Ao iniciar o projeto, uma conta de administrador será criada automaticamente:
 - **E-mail:** admin@exemplo.com
 - **Senha:** Admin123!
 
 Você pode alterar essas credenciais no arquivo `appsettings.json` ou configurando as variáveis de ambiente `AdminEmail` e `AdminPassword`.
