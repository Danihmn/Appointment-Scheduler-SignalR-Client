using Microsoft.AspNetCore.SignalR.Client;

Console.WriteLine("=== AppointmentScheduler - SignalR Notification Client ===\n");

Console.Write("Digite o URL base da API [http://localhost:8080]: ");

var baseUrl = Console.ReadLine()?.Trim();

if (string.IsNullOrEmpty(baseUrl))
    baseUrl = "http://localhost:8080";

Console.Write("JWT Token (deixe em branco se não precisar): ");

var token = Console.ReadLine()?.Trim();
var hubUrl = $"{baseUrl.TrimEnd('/')}/hubs/notifications";

if (!string.IsNullOrEmpty(token)) hubUrl += $"?access_token={token}";

var connection = new HubConnectionBuilder()
    .WithUrl(hubUrl)
    .WithAutomaticReconnect()
    .Build();

connection.On<string>("ReceiveNotification", message =>
{
    var time = DateTime.Now.ToString("HH:mm:ss");

    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write($"[{time}] ");
    Console.ResetColor();
    Console.WriteLine(message);
});

#region Connection Events
connection.Reconnecting += ex =>
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Reconectando... ({ex?.Message})");
    Console.ResetColor();
    return Task.CompletedTask;
};

connection.Reconnected += _ =>
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Reconectado.");
    Console.ResetColor();
    return Task.CompletedTask;
};

connection.Closed += ex =>
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Conexão encerrada. {ex?.Message}");
    Console.ResetColor();
    return Task.CompletedTask;
};
#endregion

Console.WriteLine($"\nConectando em {hubUrl.Split('?')[0]}...");

try
{
    await connection.StartAsync();

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Conectado! Aguardando notificações...\n");
    Console.ResetColor();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Erro ao conectar: {ex.Message}");
    Console.ResetColor();

    return;
}

Console.WriteLine("Pressione ENTER para encerrar.\n");
Console.ReadLine();

await connection.StopAsync();
Console.WriteLine("Desconectado.");
