# 🛠️ Gerenciamento de Ordens de Serviço (GestaoOS)

> Um sistema web desenvolvido em **Razor Pages (.NET 8)** para gerenciamento de ordens de serviço em ambientes corporativos ou educacionais.

![Status do Projeto](https://img.shields.io/badge/Status-Concluído-brightgreen) ![.NET 8](https://img.shields.io/badge/.NET-8.0-purple)

## 🚀 Funcionalidades

O projeto foi projetado para organizar e monitorar orientações de manutenção de forma eficiente:

* **🖥️ Gestão de Ativos:** Cadastro e organização de ativos (computadores, mesas, cadeiras, etc.) por sala e posição.
* **📋 Controle de Ordens de Serviço:** Criação, acompanhamento e conclusão de OS com status como "Em Andamento", "Em Espera" e "Concluída".
* **⏱️ Gestão de SLA:** Definição de prazos para resolução e alertas para vencimentos.
* **👥 Perfis de Usuário:** Acesso diferenciado e seguro para Gestores, Técnicos e Professores.

---

## 🛠️ Tecnologias Utilizadas

* **Back-end:** C# .NET 8 (Razor Pages / MVC)
* **Banco de Dados:** SQL Server (Entity Framework Core 8)
* **Front-end:** HTML5, CSS3, Bootstrap 5
* **Versionamento:** Git

---

## 🚀 Como Rodar o Projeto Localmente

Siga este guia passo a passo para configurar o ambiente e executar o **GestaoOS** na sua máquina.

### 📋 Pré-requisitos

Certifique-se de ter as seguintes ferramentas instaladas:

1.  **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** (Essencial para compilar).
2.  **[SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads)** (Express ou Developer) OU o **LocalDB** do Visual Studio.
3.  **[Git](https://git-scm.com/downloads)**.

### 🔧 Passo a Passo de Instalação

#### 1. Clonar o Repositório
Abra seu terminal e execute:

git clone [https://github.com/SEU-USUARIO/GestaoOS.git](https://github.com/JuanJorgeDEV/Gest-oOS.git)
cd GestaoOS

#### 2. Configurar o Banco de Dados (appsettings.json)

Abra o arquivo `appsettings.json` na raiz do projeto e ajuste a string de conexão conforme o seu ambiente.

**Opção A: SQL Server (Instalado via Docker ou Windows)**
*Substitua `SUA_SENHA` pela senha configurada no seu banco.*

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=GestaoOS_DB;User ID=sa;Password=SUA_SENHA;TrustServerCertificate=True"
}
```

**Opção B: Visual Studio (LocalDB)**
*Geralmente é o padrão do Visual Studio, não exige senha.*

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GestaoOS_DB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```
#### 3. Criar o Banco de Dados (Migrations)

Agora vamos criar a estrutura do banco automaticamente usando o Entity Framework. No terminal, na pasta do projeto, rode:

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update
```
#### 4. Executar a Aplicação

Inicie o servidor web:

```bash
dotnet run
O terminal mostrará o endereço local (ex: http://localhost:5129). Copie e cole no navegador.
```
## 🔐 Acesso Inicial (Admin)

Ao rodar pela primeira vez, utilize a conta de administrador padrão para testes:

| Campo | Valor Padrão |
| :--- | :--- |
| **E-mail** | `admin@gestaoos.com` |
| **Senha** | `SenhaForte!123` |

> **⚠️ Segurança:** Você pode alterar essas credenciais no arquivo `DbInitializer.cs` (pasta Services).

📄 Licença
Este projeto é destinado a fins de estudo e portfólio.
