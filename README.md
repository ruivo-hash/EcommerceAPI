# EcommerceAPI
API que simula um aplicativo de ecommerce desenvolvida com C# .NET Core, EntityFramework Core, fazendo conexão com SQL Server, autorização e autenticação via JWT

# Rotas
### /categories - rota responsável pelo CRUD de categorias
GET - https://localhost:7200/categories - Precisa estar autenticado no sistema. Não é necessário passar nada no body. Retorna todas categorias cadastradas no banco.
POST - https://localhost:7200/categories - Precisa estar autenticado e autorizado como funcionário no sistema.  É preciso passar o parâmetro "name" do tipo string no body da requisição no formato JSON. Retorna StatusCode 201 junto com o id da categoria criada.
PUT - https://localhost:7200/categories/{id} - Precisa estar autenticado e autorizado como funcionário no sistema. É preciso passar o id da categoria junto a rota e os parâmetros "name" do tipo string e "active" do tipo boleano no body da requisição no formato JSON. Retorna StatusCode 200.
DELETE - https://localhost:7200/categories/{id} - Precisa estar autenticado e autorizado como funcionário no sistema.  É preciso passar o id da categoria junto a rota. Não é necessário preenchimento do body. Retorna StatusCode 200.

### /products - rota responsável pelo cadastro e consulta de produtos
GET - https://localhost:7200/products - Precisa estar autenticado e autorizado como funcionário no sistema. Não é necessário passar nada no body. Retorna todos os produtos cadastrados no banco.
GET - https://localhost:7200/products/showcase?page=1&row=10&orderBy=name - Não precisa estar autenticado. Não precisa passar nada no body. São passados os parâmetros page, row e orderBy pela rota para paginar e ordenar o retorno dos produtos. Retorna os produtos cadastrados no banco que estão como ativos e possuem estoque.
GET - https://localhost:7200/products/mostsold - Precisa estar autenticado e autorizado como funcionário no sistema. Não precisa passar nada no body. Retorna os produtos com o total de vendas que cada produto possui.
POST - https://localhost:7200/products - Precisa estar autenticado e autorizado como funcionário no sistema. É preciso passar os parâmetros "name", "description" e "categoryId" do tipo string, "hasStock" do tipo boleano e "price" do tipo decimal no body da requisição no formato JSON. Retorna StatusCode 201 junto com o id do produto criado.

### /employees - rota responsável pelo cadastro e consulta de funcionários
GET - https://localhost:7200/employees?page=1&rows=2 - Precisa estar autenticado e autorizado como funcionário no sistema. Não é necessário passar nada no body. São passados os parâmetros page e rows pela rota para paginar o retorno dos funcionários. Retorna todos os funcionários cadastrados no banco.
POST - https://localhost:7200/employees - Precisa estar autenticado e autorizado como funcionário no sistema. É preciso passar os parâmetros "email", "password", "name" e "employeeCode" do tipo string no body da requisição no formato JSON. Retorna StatusCode 201 junto com o id do funcionário criado.

### /clients -  rota responsável pelo cadastro e consulta de clientes
GET - https://localhost:7200/clients - Precisa estar autenticado como cliente. Não precisa passar nada no body. Retorna as informações do atual cliente logado.
POST - https://localhost:7200/clients - Não precisa estar autenticado. É preciso passar os parâmetros "email", "password", "name" e "cpf" do tipo string no body da requisição no formato JSON. Retorna StatusCode 201 junto com o id do cliente criado.

### /orders - rota responsável pelo cadastro e consulta de pedidos
GET - https://localhost:7200/orders/{id} - Precisa estar autenticado no sistema. Não é necessário passar nada no body, apenas o id do cliente junto a rota da requisição. Retorna os pedidos feitos pelo id do cliente informado na rota.
POST - https://localhost:7200/orders - Precisa estar autenticado no sistema. É preciso passar os parâmetros "productsId" que vai ser um array de strings representando os ids de produtos e "deliveryAddress" do tipo string no body da requisição no formato JSON. Retorna StatusCode 201 junto com o id do pedido criado.

### /token  - rota responsável por gerar token de autenticação e autorização no login
POST - https://localhost:7200/token - Não precisa estar autenticado. É preciso passar os parâmetros "email" e "password" do tipo string no body da requisição no formato JSON. Retorna StatusCode 200 junto com o token de autenticação e autorização com base no usuário.

