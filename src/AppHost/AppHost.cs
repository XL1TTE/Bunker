var builder = DistributedApplication.CreateBuilder(args);

var rabbitmqUser = builder.AddParameter("rabbitmq-user", secret: true);
var rabbitmqPass = builder.AddParameter("rabbitmq-password", secret: true);

var keycloakUser = builder.AddParameter("keycloak-user", secret: true);
var keycloakPass = builder.AddParameter("keycloak-password", secret: true);

var gameStateDbUser = builder.AddParameter("game-state-db-user", secret: true);
var gameStateDbPass = builder.AddParameter("game-state-db-password", secret: true);

var playerDbUser = builder.AddParameter("player-db-user", secret: true);
var playerDbPass = builder.AddParameter("player-db-password", secret: true);

var lobbyDbUser = builder.AddParameter("lobby-db-user", secret: true);
var lobbyDbPass = builder.AddParameter("lobby-db-password", secret: true);

var redis = builder.AddRedis("redis");

var rabbitmq = builder.AddRabbitMQ("rabbit-mq", rabbitmqUser, rabbitmqPass)
                      .WithManagementPlugin();

var auth = builder.AddKeycloak("auth", adminUsername: keycloakUser, adminPassword: keycloakPass)
                      .WithDataVolume();

var gameStateDbServer = builder.AddPostgres("game-state-db-server", gameStateDbUser, gameStateDbPass).WithPgAdmin();
var gameStateDb = gameStateDbServer.AddDatabase("game-state-db");

var playerDbServer = builder.AddPostgres("player-db-server", playerDbUser, playerDbPass).WithPgAdmin();
var playerDb = playerDbServer.AddDatabase("player-db");

var lobbyDbServer = builder.AddPostgres("lobby-db-server", lobbyDbUser, lobbyDbPass).WithPgAdmin();
var lobbyDb = lobbyDbServer.AddDatabase("lobby-db");

builder.AddProject<Projects.GameStateService>("game-state-service")
       .WithReference(gameStateDb)
       .WithReference(redis)
       .WithReference(rabbitmq)
       .WaitFor(gameStateDb);

builder.AddProject<Projects.PlayerService>("player-service")
       .WithReference(auth)
       .WithReference(playerDb)
       .WithReference(rabbitmq)
       .WaitFor(playerDb);

builder.AddProject<Projects.LobbyService>("lobby-service")
       .WithReference(auth)
       .WithReference(lobbyDb)
       .WithReference(redis)
       .WithReference(rabbitmq)
       .WaitFor(lobbyDb);

builder.Build().Run();
