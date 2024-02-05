using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Telegram.Configurations;
using Telegram.Enums;
using Telegram.Events;
using Telegram.Helpers;
using Telegram.Types;

namespace Telegram
{
    public class TelegramUpdatesHandler : IDisposable
    {
        public bool WebhookIsReady { get; private set; }
        public event EventHandler<IncomingUpdateReceivedEventArgs> IncomingUpdateReceived;

        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly TelegramClient _telegramClient;
        private readonly WebhookConfiguration? _webhookConfiguration;
        private readonly GetUpdatesWay _getUpdatesWay;
        private readonly string _botToken;
        private bool _incomingUpdatesHandling;

        public TelegramUpdatesHandler(GetUpdatesWay getUpdatesWay, string botToken, WebhookConfiguration? webhookConfiguration = null)
        {
            _cancellationTokenSource = new();
            _jsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
            _telegramClient = new TelegramClient(botToken);
            _webhookConfiguration = webhookConfiguration;
            _getUpdatesWay = getUpdatesWay;
            _botToken = botToken;
        }

        public void StartReceivingUpdates()
        {
            if (!_incomingUpdatesHandling)
            {
                _incomingUpdatesHandling = true;
                new Thread(async () =>
                {
                    await _telegramClient.DeleteWebhookAsync(true);
                    if (_getUpdatesWay == GetUpdatesWay.Polling)
                    {
                        var offset = 0;
                        while (_incomingUpdatesHandling)
                        {
                            try
                            {
                                var updates = await _telegramClient.GetUpdatesAsync(offset, timeout: 300, cancellationToken: _cancellationTokenSource.Token);
                                if (updates != null && updates.Length > 0)
                                {
                                    offset = updates.Select(update => update.UpdateId).Max() + 1;
                                    await Parallel.ForEachAsync(updates, (update, _) =>
                                    {
                                        IncomingUpdateReceived?.Invoke(this, new IncomingUpdateReceivedEventArgs
                                        {
                                            UpdateReceivedDate = DateTime.UtcNow,
                                            UpdateWay = _getUpdatesWay,
                                            Update = update,
                                        });

                                        return ValueTask.CompletedTask;
                                    });
                                }
                            }
                            catch (TaskCanceledException) { }
                        }
                    }
                    else if (_getUpdatesWay == GetUpdatesWay.Webhook)
                    {
                        if (_webhookConfiguration == null)
                        {
                            throw new ArgumentNullException(nameof(WebhookConfiguration));
                        }

                        var sslCertificate = _webhookConfiguration.UseHttps ? (_webhookConfiguration.Certificate ?? GenerateSelfSignedCertificate()) : null;
                        var secretToken = StringExtensions.GenerateRandomString(30);
                        await _telegramClient.SetWebhookAsync($"{_webhookConfiguration.Domain}/webhook", _webhookConfiguration.SendPublicCertificateToTelegram == true ? sslCertificate?.GetByteArrayAsPem() : null, null, _webhookConfiguration.MaxConnections, null, _webhookConfiguration.DropPendingUpdates, secretToken, _cancellationTokenSource.Token);

                        var builder = WebApplication.CreateSlimBuilder();
                        builder.Logging.ClearProviders();
                        builder.WebHost.ConfigureKestrel(config =>
                        {
                            if (sslCertificate != null)
                            {
                                config.Listen(IPAddress.Any, 443, listenOptions => listenOptions.UseHttps(sslCertificate));
                            }
                            else
                            {
                                config.Listen(IPAddress.Any, 80);
                            }
                        });

                        var webHost = builder.Build();
                        webHost.UseRouting();
                        webHost.MapGet("/webhook", () => "The webhook is online.");
                        webHost.MapPost("/webhook", async (HttpRequest request) =>
                        {
                            if (request.Headers.TryGetValue("X-Telegram-Bot-Api-Secret-Token", out var telegramSecretToken) && telegramSecretToken.ToString() == secretToken)
                            {
                                var update = await JsonSerializer.DeserializeAsync<Update>(request.Body, _jsonSerializerOptions, _cancellationTokenSource.Token);

                                if (update != null)
                                {
                                    IncomingUpdateReceived?.Invoke(this, new IncomingUpdateReceivedEventArgs
                                    {
                                        UpdateReceivedDate = DateTime.UtcNow,
                                        UpdateWay = _getUpdatesWay,
                                        Update = update,
                                    });
                                }

                                return Results.Ok();
                            }
                            else
                            {
                                return Results.Unauthorized();
                            }
                        });

                        try
                        {
                            WebhookIsReady = true;
                            await webHost.RunAsync(_cancellationTokenSource.Token);
                        }
                        catch (TaskCanceledException) { }
                        await _telegramClient.DeleteWebhookAsync(_webhookConfiguration.DropPendingUpdates);
                    }
                }).Start();
            }
        }

        public void StopReceivingUpdates()
        {
            if (_incomingUpdatesHandling)
            {
                _cancellationTokenSource.Cancel();
                _incomingUpdatesHandling = false;
                WebhookIsReady = false;
            }
        }

        private X509Certificate2 GenerateSelfSignedCertificate()
        {
            using var rsa = RSA.Create(2048);
            var csr = new CertificateRequest($"cn={_webhookConfiguration!.Domain}", rsa, HashAlgorithmName.SHA384, RSASignaturePadding.Pss);
            var certificate = csr.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(10));
            return new X509Certificate2(certificate.Export(X509ContentType.Pkcs12));
        }

        public void Dispose()
        {
            StopReceivingUpdates();
            _cancellationTokenSource.Dispose();
            _telegramClient.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}