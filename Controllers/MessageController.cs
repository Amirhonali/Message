using Message.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Message.Services;
using Message.Entity;
using System.Net.WebSockets;

namespace Message.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageRepository _repository;
        private readonly WebSocketService _webs;
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(IMessageRepository repository, WebSocketService webs, ILogger<MessagesController> logger)
        {
            _repository = repository;
            _webs = webs;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] string text)
        {
            _logger.LogInformation("Получен POST-запрос /api/messages/send с текстом: {Text}", text);

            if (string.IsNullOrWhiteSpace(text) || text.Length > 128)
            {
                _logger.LogWarning("Некорректный текст сообщения");
                return BadRequest("Message must be 1-128 characters long.");
            }

            var message = new Sobsh { Text = text, Time = DateTime.UtcNow };
            message.Id = await _repository.AddMessageAsync(message);

            _logger.LogInformation("Сообщение сохранено в БД с ID: {MessageId}", message.Id);

            await _webs.BroadcastMessageAsync(JsonSerializer.Serialize(message));

            return Ok(message);
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentMessages()
        {
            _logger.LogInformation("Получен GET-запрос /api/messages/recent");
            var messages = await _repository.GetMessagesAsync(TimeSpan.FromMinutes(10));

            _logger.LogInformation("Возвращено {Count} сообщений", messages.Count());
            return Ok(messages);
        }
    }

}
