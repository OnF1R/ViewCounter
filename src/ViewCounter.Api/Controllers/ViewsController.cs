using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using ViewCounter.Application.Abstractions.Repositories;
using ViewCounter.Application.Views.Commands;
using ViewCounter.Application.Views.Handlers;
using ViewCounter.Domain.DTO;
using ViewCounter.Domain.Entities;

namespace ViewCounter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ViewsController : Controller
    {
        private readonly RegisterViewHandler _registerHandler;
        private readonly IViewEventRepository _repository;
        private readonly IMemoryCache _cache;

        public ViewsController(
            RegisterViewHandler registerHandler,
            IViewEventRepository repository,
            IMemoryCache cache)
        {
            _registerHandler = registerHandler;
            _repository = repository;
            _cache = cache;
        }

        /// <summary>
        /// Создание токена для последующего подтверждения регистрации просмотра
        /// </summary>
        [HttpPost("ping")]
        public IActionResult PingView([FromBody] ViewPingDto dto)
        {
            var token = Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(32));

            _cache.Set(
                $"view:{token}",
                new
                {
                    dto.EntityType,
                    dto.EntityId,
                    dto.UserId,
                },
                TimeSpan.FromSeconds(10)
            );

            return Ok(new { viewToken = token });
        }

        /// <summary>
        /// Подверждение регистрации сущности просмотра
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ConfirmView([FromBody] ViewConfirmDto dto)
        {
            if (!_cache.TryGetValue($"view:{dto.ViewToken}", out dynamic payload))
                return Ok(new { ignored = "expired" });

            _cache.Remove($"view:{dto.ViewToken}");

            var ip = Request.Headers["X-Forwarded-For"].FirstOrDefault()
                ?? HttpContext.Connection.RemoteIpAddress?.ToString()
                ?? "unknown";

            var ua = Request.Headers["User-Agent"].ToString();

            var cmd = new RegisterViewCommand
            (
                payload.EntityType,
                payload.EntityId,
                payload.UserId,
                ip,
                ua
            );

            var counted = await _registerHandler.HandleAsync(cmd);

            if (!counted)
                return Conflict(new { message = "View already counted recently" });

            return Ok(new { message = "View registered" });
        }

        /// <summary>
        /// Получить статистику просмотров сущности
        /// </summary>
        [HttpGet("{entityType}/{entityId}")]
        public async Task<IActionResult> GetStats(string entityType, string entityId)
        {
            var total = await _repository.CountAsync(entityType, entityId);
            var unique = await _repository.CountUniqueAsync(entityType, entityId);

            return Ok(new
            {
                entityType,
                entityId,
                total,
                unique
            });
        }

        /// <summary>
        /// Получение последних просмотров
        /// </summary>
        [HttpGet("recent/{entityType}/{count}")]
        public async Task<IActionResult> GetRecent(string entityType, int count)
        {
            if (!_cache.TryGetValue($"view:recent:count-{count}", out List<ViewEvent> recent))
            {
                recent = await _repository.GetRecentAsync(entityType, count);

                _cache.Set(
                    $"view:recent:count-{count}",
                    recent,
                    TimeSpan.FromMinutes(10)
                );
            }

            if (recent == null)
                recent = await _repository.GetRecentAsync(entityType, count);

            return Ok(recent);
        }

        /// <summary>
        /// Получение самых популярных просмотров за опредленное время
        /// </summary>
        [HttpGet("popular/{entityType}/{count}")]
        public async Task<IActionResult> GetPopular(string entityType, int count)
        {

            var window = DateTime.UtcNow.Subtract(TimeSpan.FromDays(7));

            if (!_cache.TryGetValue($"view:popular:count-{count}", out List<PopularEntityDto> popular))
            {
                popular = await _repository.GetPopularAsync(entityType, count, window);

                _cache.Set(
                    $"view:popular:count-{count}",
                    popular,
                    TimeSpan.FromMinutes(10)
                );
            }

            if (popular == null)
                popular = await _repository.GetPopularAsync(entityType, count, window);

            return Ok(popular);
        }

        /// <summary>
        /// TODO
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var allViews = await _repository.GetAll();
            return Ok(allViews);
        }
    }
}
