# Alocação de Veículos (Projeto .NET MAUI)

## Descrição

Este é um projeto de alocação de veículos desenvolvido em .NET MAUI. Ele permite o gerenciamento de usuários, veículos e reservas, com funcionalidades como cadastro, visualização em mapa e dashboards.

## Tecnologias Utilizadas

* .NET MAUI
* .NET 8.0
* SQLite.NET-PCL

## Funcionalidades Principais

* Cadastro de Usuários
* Cadastro de Veículos
* Dashboard para Funcionários
* Visualização em Mapa
* Reservas de Usuários

## Como Configurar e Rodar o Projeto

1. Certifique-se de ter o .NET 8 SDK e a workload do .NET MAUI instalados. Você pode instalá-los seguindo a documentação oficial do .NET MAUI.

2. Clone este repositório para o seu ambiente local.

3. Abra o arquivo da solução `AlocacaoVeiculoMaui.sln` em um IDE compatível, como Visual Studio (Windows/Mac) ou Visual Studio Code com as extensões relevantes.

4. Restaure as dependências do NuGet. Geralmente, isso acontece automaticamente ao abrir a solução no IDE, mas você pode fazer manualmente via terminal navegando até o diretório do projeto (`AlocacaoVeiuculo`) e executando:

   ```bash
   dotnet restore
   ```

5. Escolha a plataforma de destino (Android, iOS, Windows, Mac Catalyst) e o dispositivo/emulador.

6. Compile e execute o projeto a partir do seu IDE ou via terminal. Para rodar no Android, por exemplo:

   ```bash
   dotnet build -t:Run -f net8.0-android
   ```

## Estrutura do Projeto

- `AlocacaoVeiuculo/`: Contém os arquivos principais do projeto .NET MAUI.
  - `UI.MauiApp/Views/`: Páginas XAML da interface do usuário.
  - `Model/`: Modelos de dados (provavelmente).
  - `Data/`: Lógica de acesso a dados (provavelmente relacionada ao SQLite).
  - `Resources/`: Assets como imagens, fontes, etc.
- `AlocacaoVeiculoMaui.sln`: Arquivo de solução principal.

## Contribuição

Se você quiser contribuir, por favor, forke o repositório e crie um pull request com suas alterações. 
