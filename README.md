Instruções para executar o projeto:

---Clonar/Baixar projeto

---Abrir com Visual Studio

---Restaurar os pacotes NuGet(necessário apenas na primeira vez que abrir o projeto em sua máquina):

Vá até o Gerenciador de Pacotes NuGet, clique em Restaurar pacotes (ou use o terminal com: Update-Package -reinstall se necessário)

---Executar a migration (somente na primeira vez que abrir o projeto):

No Gerenciador de Pacotes do NuGet, rode:

Update-Database

Esse comando vai criar o banco de dados automaticamente usando o Code First com a migration InitialCreate.

---Executar projeto

O banco de dados será gerado automaticamente ao rodar a aplicação, graças ao Entity Framework (Code First + Migrations).
