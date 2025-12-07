# 🛠️ Gerenciamento de Ordens de Serviço (GestaoOS)

> Um sistema web desenvolvido em **Razor Pages (.NET 8)** para gerenciamento de ordens de serviço em ambientes corporativos ou educacionais.

## 🚀 Funcionalidades

O projeto foi projetado para organizar e monitorar orientações de manutenção de forma eficiente:

* **🖥️ Gestão de Ativos:** Cadastro e organização de ativos (computadores, mesas, cadeiras, etc.) por sala e posição.
* **📋 Controle de Ordens de Serviço:** Criação, acompanhamento e conclusão de OS com status como "Em Andamento", "Em Espera" e "Concluída".
* **⏱️ Gestão de SLA:** Definição de prazos para resolução e alertas para vencimentos.
* **👥 Perfis de Usuário:** Acesso diferenciado e seguro para:
    * Gestores
    * Técnicos de manutenção
    * Professores

---

## 🔐 Acesso Inicial (Admin)

Ao iniciar o projeto pela primeira vez, uma conta de administrador será criada automaticamente para testes:

| Campo | Valor Padrão |
| :--- | :--- |
| **E-mail** | `admin@exemplo.com` |
| **Senha** | `Admin123!` |

### ⚠️ Importante sobre Segurança
Você pode alterar essas credenciais no arquivo `appsettings.json` ou, preferencialmente, configurando as variáveis de ambiente `AdminEmail` e `AdminPassword` no seu servidor.

---

## 🛠️ Tecnologias Utilizadas
* C#
* .NET 8 (Razor Pages)
* SQL Server (Entity Framework)
* HTML/CSS (Bootstrap)
