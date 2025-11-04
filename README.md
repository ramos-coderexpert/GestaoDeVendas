# Gestão de Vendas API

API REST para gerenciamento de vendas, clientes e produtos desenvolvida em .NET 8 com autenticação JWT.

## 🚀 Tecnologias

- **.NET 8**
- **Entity Framework Core 8**
- **SQL Server 2022**
- **Docker & Docker Compose**
- **JWT Authentication**
- **Swagger/OpenAPI**
- **FluentValidation**

---

## 📋 Pré-requisitos

Para rodar este projeto, você precisa ter instalado:

### **1. Docker Desktop**
- **Download:** https://www.docker.com/products/docker-desktop
- **⚠️ IMPORTANTE:** Certifique-se de que o Docker está configurado para usar **Linux Containers**

#### Como verificar/configurar Linux Containers:
1. Clique com botão direito no ícone do Docker na bandeja do sistema
2. Se aparecer **"Switch to Linux containers..."**, clique nele
3. Aguarde o Docker reiniciar

### **2. Git**
- **Download:** https://git-scm.com/downloads

### **3. (Opcional) .NET 8 SDK**
Apenas necessário se quiser rodar localmente fora do Docker:
- **Download:** https://dotnet.microsoft.com/download/dotnet/8.0

---

## 🔧 Como Rodar o Projeto
### **Passo 1: Clonar o Repositório**

- abra um prompt de comando dentro do diretório C:
- digite o seguinte comando: git clone https://github.com/testeCmCapital/TesteLucasRamos.git cd TesteLucasRamos e pressione enter

### **Passo 2: Build da Imagem Docker**
- abra um prompt de comando dentro da raiz do projeto (no mesmo diretório onde você encontra o arquivo de solution "GestaoDeVendas.sln")
- rode o seguinte comando: docker build -t gestao-vendas-api .

### **Passo 3: Subir os Containers**

- no mesmo prompt, rode o comando: docker-compose up -d


Pronto! A API estará disponível em: http://localhost:5000/swagger
