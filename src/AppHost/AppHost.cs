var builder = DistributedApplication.CreateBuilder(args);

var rabbitmqUser = builder.AddParameter("rabbitmq-user", secret: true);
var rabbitmqPass = builder.AddParameter("rabbitmq-password", secret: true);

var keycloakUser = builder.AddParameter("keycloak-user", secret: true);
var keycloakPass = builder.AddParameter("keycloak-password", secret: true);

var gameStateDbUser = builder.AddParameter("game-state-db-user", secret: true);
var gameStateDbPass = builder.AddParameter("game-state-db-password", secret: true);

var accountDbUser = builder.AddParameter("account-db-user", secret: true);
var accountDbPass = builder.AddParameter("account-db-password", secret: true);

var lobbyDbUser = builder.AddParameter("lobby-db-user", secret: true);
var lobbyDbPass = builder.AddParameter("lobby-db-password", secret: true);

var contentServiceDbUser = builder.AddParameter("content-service-db-user", secret: true);
var contentServiceDbPass = builder.AddParameter("content-service-db-password", secret: true);

var redis = builder.AddRedis("redis");

var rabbitmq = builder.AddRabbitMQ("rabbit-mq", rabbitmqUser, rabbitmqPass)
                      .WithManagementPlugin();

var rabbitMqProvisioner = builder.AddProject<Projects.Bunker_Provisioner_RabbitMq>("rabbit-mq-provisioner")
       .WithReference(rabbitmq)
       .WaitFor(rabbitmq)
       .WithParentRelationship(rabbitmq);

var auth = builder.AddKeycloak("auth", adminUsername: keycloakUser, adminPassword: keycloakPass)
                      .WithDataVolume();

var gameStateDbServer = builder.AddPostgres("game-state-db-server", gameStateDbUser, gameStateDbPass).WithPgAdmin();
var gameStateDb = gameStateDbServer.AddDatabase("game-state-db");

var accountDbServer = builder.AddPostgres("account-db-server", accountDbUser, accountDbPass).WithPgAdmin();
var accountDb = accountDbServer.AddDatabase("account-db");

var lobbyDbServer = builder.AddPostgres("lobby-db-server", lobbyDbUser, lobbyDbPass).WithPgAdmin();
var lobbyDb = lobbyDbServer.AddDatabase("lobby-db");
var lobbyAccountsDb = lobbyDbServer.AddDatabase("lobby-accounts-replica-db");

var contentServiceDbServer = builder.AddPostgres("content-service-db-server", contentServiceDbUser, contentServiceDbPass).WithPgAdmin();
var contentServiceDb = contentServiceDbServer.AddDatabase("content-service-db");


builder.AddProject<Projects.Bunker_AccountService>("account-service")
       .WithReference(auth)
       .WithReference(accountDb)
       .WithReference(rabbitmq)
       .WaitFor(accountDb);

builder.AddProject<Projects.Bunker_LobbyService>("lobby-service")
       .WithReference(auth)
       .WithReference(lobbyDb)
       .WithReference(lobbyAccountsDb)
       .WithReference(redis)
       .WithReference(rabbitmq)
       .WaitFor(lobbyDb)
       .WaitFor(lobbyAccountsDb);

builder.AddProject<Projects.Bunker_ContentService>("content-service")
       .WithReference(auth)
       .WithReference(contentServiceDb)
       .WithReference(redis)
       .WithReference(rabbitmq)
       .WaitFor(contentServiceDb);

builder.Build().Run();
