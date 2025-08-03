## Como Executar o Projeto 🚀

A forma mais simples de rodar tudo é com o Docker.

**Pré-requisitos:**
* Git
* .NET 8 SDK
* Docker Desktop

**Passos:**

1.  **Clone o repositório:**
    ```bash
    git clone https://github.com/allanse/BankMore-API.git
    cd BankMore-API
    ```

2.  **Configure o Segredo do JWT:**
    O projeto usa um arquivo `.env` para carregar a chave secreta do JWT no Docker. Crie um arquivo chamado `.env` na raiz do projeto e adicione a seguinte linha:
    ```
    JWT_SECRET=chave_usada_apenas_para_portifolio
    ```

3.  **Suba os containers:**
    Este comando vai construir as imagens das APIs, do Kafka, do Redis e iniciar todos os containers.
    ```bash
    docker-compose up --build
    ```

4.  **Pronto!** As APIs estarão rodando.
