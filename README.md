# Controle-Financeiro-Pessoal
Sistema de Controle Financeiro Pessoal utilizando as tecnologias .NET 8 e VUE, que recebe informações e realiza um fluxo de caixa detalhando as movimentações diariamente.

## Backend

### Tecnologias Utilizadas

- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM para acesso a dados
- **SQL Server** - Banco de dados
- **AutoMapper** - Mapeamento de objetos
- **Swagger/OpenAPI** - Documentação da API
- **xUnit** - Framework de testes unitários

### Estrutura do Projeto

O projeto segue os princípios da Arquitetura Limpa, organizando o código em camadas bem definidas:

```
Backend/
├── ControleFinanceiroPessoal.API/              # Camada de Apresentação
│   ├── Controllers/                            # Controllers da API
│   ├── Program.cs                              # Configuração da aplicação
│   └── appsettings.json                        # Configurações
├── ControleFinanceiroPessoal.Application/      # Camada de Aplicação
│   ├── DTOs/                                   # Data Transfer Objects
│   ├── Interfaces/                             # Contratos de serviços
│   ├── Mappings/                               # Perfis do AutoMapper
│   └── Services/                               # Serviços de negócio
├── ControleFinanceiroPessoal.Domain/           # Camada de Domínio
│   ├── Entities/                               # Entidades do domínio
│   └── Interfaces/                             # Contratos de repositórios
├── ControleFinanceiroPessoal.Infrastructure/   # Camada de Infraestrutura
│   ├── Data/                                   # Contexto do Entity Framework
│   ├── Migrations/                             # Migrações do banco
│   └── Repositories/                           # Implementação dos repositórios
└── Tests/                                      # Projetos de testes
    ├── ControleFinanceiroPessoal.API.Tests/
    ├── ControleFinanceiroPessoal.Application.Tests/
    ├── ControleFinanceiroPessoal.Domain.Tests/
    └── ControleFinanceiroPessoal.Infrastructure.Tests/
```

### Arquitetura do Projeto

O projeto implementa Arquitetura Limpa com as seguintes características:

#### **Domain Layer (Domínio)**
- **Entidades**: `Movimentacao` com propriedades Id, Tipo, Data, Descrição, Valor
- **Enums**: `TipoMovimentacao` (Receita/Despesa)
- **Interfaces**: Contratos para repositórios

#### **Application Layer (Aplicação)**
- **Services**: Lógica de negócio (`MovimentacaoService`, `EmailService`, `JobService`)
- **DTOs**: Objetos de transferência de dados
- **Interfaces**: Contratos de serviços
- **Mappings**: Configuração do AutoMapper

#### **Infrastructure Layer (Infraestrutura)**
- **Repositories**: Implementação de acesso a dados
- **Context**: Configuração do Entity Framework
- **Migrations**: Controle de versão do banco de dados

#### **API Layer (Apresentação)**
- **Controllers**: Endpoints REST
- **Configuration**: Injeção de dependência, CORS, Swagger

### Funcionalidades

#### **Gestão de Movimentações Financeiras**
- Cadastro de receitas e despesas
- Consulta de movimentações por ID
- Listagem de todas as movimentações
- Atualização de movimentações existentes
- Exclusão de movimentações
- Cálculo de fluxo de caixa diário

#### **Notificações Automáticas**
- Job Service para verificação diária de saldo
- Envio de e-mail quando saldo negativo
- Configuração de SMTP para envio de e-mails

#### **Recursos Técnicos**
- API RESTful com documentação Swagger
- Validação de dados com Data Annotations
- Mapeamento automático com AutoMapper
- Injeção de dependência nativa do .NET
- CORS configurado para integração com frontend
- Testes unitários

### Endpoints da API

#### **Movimentações** (`/api/movimentacao`)

| Método | Endpoint | Descrição | Parâmetros |
|--------|----------|-----------|------------|
| `GET` | `/api/movimentacao` | Lista todas as movimentações | - |
| `GET` | `/api/movimentacao/{id}` | Obtém movimentação por ID | `id`: int |
| `GET` | `/api/movimentacao/fluxo-caixa` | Obtém resumo do fluxo de caixa diário | - |
| `POST` | `/api/movimentacao` | Cria nova movimentação | Body: `CreateMovimentacaoDto` |
| `PUT` | `/api/movimentacao/{id}` | Atualiza movimentação existente | `id`: int, Body: `UpdateMovimentacaoDto` |
| `DELETE` | `/api/movimentacao/{id}` | Remove movimentação | `id`: int |

#### **Modelos de Dados**

**CreateMovimentacaoDto / UpdateMovimentacaoDto:**
```json
{
  "tipo": 1,           // 1 = Receita, 2 = Despesa
  "data": "2024-01-15",
  "descricao": "Salário",
  "valor": 5000.00
}
```

**MovimentacaoDto (Response):**
```json
{
  "id": 1,
  "tipo": 1,
  "data": "2024-01-15",
  "descricao": "Salário",
  "valor": 5000.00,
  "inclusao": "2024-01-15T10:30:00",
  "alteracao": null
}
```

**FluxoCaixaDto:**
```json
{
  "saldoAtual": 2500.00,
  "totalReceitas": 7500.00,
  "totalDespesas": 5000.00,
  "dataUltimaMovimentacao": "2024-01-15"
}
```

## Frontend

### Tecnologias Utilizadas

- **Vue.js 3** - Framework JavaScript progressivo
- **Vite** - Build tool e bundler moderno
- **Vue Router 4** - Roteamento oficial do Vue.js
- **Naive UI** - Biblioteca de componentes UI
- **Axios** - Cliente HTTP para requisições à API
- **Chart.js** - Biblioteca para gráficos e visualizações
- **Vue Chart.js** - Wrapper do Chart.js para Vue.js
- **Vicons/Ionicons5** - Ícones para interface

### Estrutura do Projeto

O frontend segue uma arquitetura modular e organizada:

```
Frontend/
├── public/                     # Arquivos estáticos
├── src/
│   ├── assets/                 # Recursos (imagens, estilos)
│   ├── components/             # Componentes Vue reutilizáveis
│   │   ├── Layout.vue          # Layout principal da aplicação
│   │   ├── MovimentacaoForm.vue # Formulário de inclusão/edição de movimentações
│   │   └── SaldoDiarioChart.vue # Gráfico de saldo diário
│   ├── pages/                  # Páginas da aplicação
│   │   ├── Home.vue            # Página inicial com dashboard
│   │   └── Movimentacoes.vue   # Página de gestão de movimentações
│   ├── router/                 # Configuração de rotas
│   │   └── index.js            # Definição das rotas
│   ├── services/               # Serviços de integração com API
│   │   ├── api.js              # Configuração do Axios
│   │   ├── fluxoCaixaService.js # Serviços de fluxo de caixa
│   │   └── movimentacaoService.js # Serviços de movimentações
│   ├── App.vue                 # Componente raiz
│   ├── main.js                 # Ponto de entrada da aplicação
│   └── style.css               # Estilos globais
├── index.html                  # Template HTML principal
├── package.json                # Dependências e scripts
└── vite.config.js              # Configuração do Vite
```

### Arquitetura do Frontend

#### **Componentes**
- **Layout.vue**: Layout principal com navegação e estrutura base
- **MovimentacaoForm.vue**: Formulário para criar/editar movimentações
- **SaldoDiarioChart.vue**: Componente de visualização gráfica do saldo

#### **Páginas**
- **Home.vue**: Dashboard principal com resumo financeiro e gráficos
- **Movimentacoes.vue**: Página de gestão completa das movimentações

#### **Serviços**
- **api.js**: Configuração centralizada do Axios com interceptors
- **movimentacaoService.js**: Serviços para operações CRUD de movimentações
- **fluxoCaixaService.js**: Serviços para dados de fluxo de caixa

#### **Roteamento**
- Configuração com Vue Router 4
- Navegação SPA (Single Page Application)
- Rotas: `/` (Home) e `/movimentacoes` (Gestão)

### Funcionalidades

#### **Dashboard Financeiro**
- Visualização do saldo atual
- Gráficos interativos com Chart.js
- Resumo de receitas e despesas
- Indicadores visuais de performance

#### **Gestão de Movimentações**
- Listagem de todas as movimentações
- Formulário para criar novas movimentações
- Edição de movimentações existentes
- Exclusão de movimentações

### Como Executar

1. **Pré-requisitos:**
   - Node.js (versão 16 ou superior)
   - npm

2. **Instalação:**
   ```bash
   cd Frontend
   npm install
   ```

3. **Configuração:**
   - Verificar se a URL da API está correta em `src/services/api.js`
   - Por padrão: `https://localhost:7234/api`

4. **Desenvolvimento:**
   ```bash
   npm run dev
   ```

5. **Build para Produção:**
   ```bash
   npm run build
   ```