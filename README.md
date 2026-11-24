# HuperRag

[![GitHub](https://img.shields.io/badge/GitHub-rodolfoms%2FHuperRag-blue?logo=github)](https://github.com/rodolfoms/HuperRag)
![.NET](https://img.shields.io/badge/.NET-10-512BD4)
![C#](https://img.shields.io/badge/C%23-14-239120)
![License](https://img.shields.io/badge/License-MIT-green)

HuperRag é um projeto de demonstração que explora integrações avançadas de **IA e Embeddings** com **.NET 10**. O projeto inclui modelos de embedding, Semantic Kernel, integração com Ollama e armazenamento vetorial no PostgreSQL usando pgVector.

## 📋 Sobre o Projeto

HuperRag demonstra como construir aplicações com:
- **Geração de Embeddings** usando Ollama
- **Busca Vetorial Semântica** com pgVector
- **Entity Framework Core** para persistência
- **ASP.NET Core Minimal APIs** para endpoints RESTful

### Projetos Inclusos

| Projeto | Descrição |
|---------|-----------|
| **01_OllamaSharp** | Exploração de client Ollama para LLMs |
| **02_microsoft_ai_extensions** | Extensões Microsoft para IA |
| **03_SemanticKernel** | Integração com Microsoft Semantic Kernel |
| **04_Embeddings** | Geração e processamento de embeddings |
| **05_PgVector** | API RESTful com busca vetorial em PostgreSQL |

## 🚀 Quick Start - Projeto 05_PgVector

### Pré-requisitos

- **.NET 10 SDK**
- **PostgreSQL** com extensão pgVector
- **Ollama** rodando localmente (`http://localhost:11434`)

### Instalação da Extensão pgVector no PostgreSQL

```bash
-- Conecte-se ao PostgreSQL como root/admin
CREATE EXTENSION IF NOT EXISTS vector;
```

### Configuração

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/rodolfoms/HuperRag.git
   cd HuperRag
   ```

2. **Configure a string de conexão** em `05_PgVector/appsettings.json`:
   ```json
   {
	 "ConnectionStrings": {
	   "DefaultConnection": "Host=localhost;Port=5432;Database=huperrag;Username=postgres;Password=sua_senha"
	 }
   }
   ```

3. **Configure o modelo Ollama** em `Program.cs`:
   ```csharp
   defaultModel: "mxbai-embed-large"  // Ou outro modelo disponível
   ```

4. **Aplique as migrações do banco:**
   ```bash
   cd 05_PgVector
   dotnet ef database update
   ```

5. **Execute a aplicação:**
   ```bash
   dotnet run
   ```

## 📡 API Endpoints

### 1. Health Check
```http
GET /
```
Resposta: `"Hello World!"`

### 2. Seed de Recomendações
```http
GET /seed
```
Popula a tabela de recomendações com embeddings gerados a partir dos produtos existentes.

**Resposta (200):**
```json
{
  "message": "Seeded"
}
```

### 3. Criar Produto
```http
POST /products
Content-Type: application/json

{
  "title": "Laptop Premium",
  "category": "Eletrônicos",
  "summary": "Laptop de alta performance",
  "description": "Especificações técnicas detalhadas..."
}
```

**Resposta (201):**
```
Location: /products
```

### 4. Buscar Recomendações por Similaridade Semântica
```http
POST /prompt
Content-Type: application/json

{
  "prompt": "Computador poderoso para programação"
}
```

**Resposta (200):**
```json
[
  {
	"title": "Laptop Premium",
	"category": "Eletrônicos"
  },
  {
	"title": "Tablet Profissional",
	"category": "Eletrônicos"
  },
  {
	"title": "Monitor Ultrawide",
	"category": "Periféricos"
  }
]
```

## 🏗️ Arquitetura

### Estrutura de Pastas (05_PgVector)

```
05_PgVector/
├── Data/
│   ├── ApplicationDbContext.cs     # DbContext com pgVector
│   └── script.sql                  # Scripts de inicialização
├── Models/
│   ├── Product.cs                  # Modelo de Produto
│   └── Recomendation.cs           # Modelo de Recomendação com Embedding
├── ViewModels/
│   ├── CreateProductViewModel.cs   # ViewModel para criar produto
│   └── QuestionViewModel.cs        # ViewModel para prompt
├── Program.cs                       # Configuração e endpoints
├── appsettings.json                # Configurações
└── appsettings.Development.json    # Configurações de desenvolvimento
```

### Fluxo de Dados

```
[Usuário] → [POST /prompt com texto]
		   ↓
   [Gera Embedding via Ollama]
		   ↓
   [Busca Vetorial no PostgreSQL (Cosine Distance)]
		   ↓
   [Retorna 3 Recomendações Mais Similares]
```

### Modelos de Dados

**Product**
```csharp
public class Product
{
	public int Id { get; set; }
	public string Title { get; set; }
	public string Category { get; set; }
	public string Summary { get; set; }
	public string Description { get; set; }
}
```

**Recomendation**
```csharp
public class Recomendation
{
	public int Id { get; set; }
	public string Title { get; set; }
	public string Category { get; set; }
	public Vector Embedding { get; set; }  // vector(1024)
}
```

## 🔧 Tecnologias

| Tecnologia | Versão | Propósito |
|-----------|--------|----------|
| **.NET** | 10 | Framework principal |
| **ASP.NET Core** | 10 | Web framework |
| **Entity Framework Core** | Latest | ORM |
| **Npgsql** | 10.0.4 | Driver PostgreSQL |
| **pgvector** | 0.3.0 | Extensão PostgreSQL para vetores |
| **Microsoft.SemanticKernel** | 1.82.0-alpha | IA e Orchestration |
| **Ollama** | - | Modelos LLM locais |

## 📝 Exemplos de Uso

### Exemplo 1: Criar um Produto e Gerar Recomendação

```bash
curl -X POST http://localhost:5000/products \
  -H "Content-Type: application/json" \
  -d '{
	"title": "iPhone 16 Pro",
	"category": "Smartphones",
	"summary": "Smartphone premium",
	"description": "Câmera avançada, processador A18..."
  }'
```

### Exemplo 2: Seed de Dados

```bash
curl http://localhost:5000/seed
```

### Exemplo 3: Buscar Recomendações

```bash
curl -X POST http://localhost:5000/prompt \
  -H "Content-Type: application/json" \
  -d '{
	"prompt": "Preciso de um telefone inteligente com ótima câmera"
  }'
```

## 🔍 Como Funciona a Busca Vetorial

1. **Entrada do Usuário**: O usuário submete um prompt com suas preferências
2. **Geração de Embedding**: O Ollama transforma o texto em um vetor de 1024 dimensões
3. **Similaridade Semântica**: PostgreSQL calcula a distância de cosseno entre os vetores
4. **Ranking**: Os 3 produtos com menor distância (mais similares) são retornados

```sql
-- Query interna usada pelo EF Core
SELECT TOP 3 id, title, category
FROM recomendations
ORDER BY embedding <-> @userEmbedding
LIMIT 3;
```

## 🛠️ Desenvolvimento

### Compilar
```bash
dotnet build
```

### Executar Testes (se houver)
```bash
dotnet test
```

### Aplicar Migrações
```bash
dotnet ef migrations add NomeMigracao -p 05_PgVector
dotnet ef database update -p 05_PgVector
```

## ⚠️ Notas Importantes

- **Ollama deve estar rodando** na porta 11434
- **PostgreSQL** deve ter a extensão pgVector instalada
- **Modelo Ollama**: Por padrão usa `mxbai-embed-large` (verifique se está disponível)
- **Dimensão do Embedding**: 1024 dimensões (verifique compatibilidade com seu modelo)

## 🤝 Contribuindo

Contribuições são bem-vindas! Por favor:

1. Faça um Fork do projeto
2. Crie uma branch com sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para detalhes.

## 👤 Autor

**Rodolfo Sobreira**
- GitHub: [@rodolfoms](https://github.com/rodolfoms)
- Email: contato via repositório

## 📚 Recursos Úteis

- [pgvector Documentation](https://github.com/pgvector/pgvector)
- [Semantic Kernel Documentation](https://learn.microsoft.com/en-us/semantic-kernel/)
- [Ollama Documentation](https://github.com/ollama/ollama)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [OpenAI Embeddings](https://platform.openai.com/docs/guides/embeddings)

## ❓ FAQ

**P: Qual modelo Ollama usar?**
R: Recomenda-se `mxbai-embed-large` para embeddings de 1024 dimensões. Outros modelos podem ter dimensionalidades diferentes.

**P: Como aumentar a precisão das recomendações?**
R: Ajuste o número de resultados retornados (`Take(3)` no endpoint `/prompt`), refine os textos de categoria ou use modelos de embedding mais sofisticados.

**P: Posso usar isso em produção?**
R: Este é um projeto de demonstração. Para produção, considere: autenticação, rate limiting, cache, pagination e validações mais rigorosas.

---

⭐ Se este projeto foi útil, considere deixar uma estrela no GitHub!
