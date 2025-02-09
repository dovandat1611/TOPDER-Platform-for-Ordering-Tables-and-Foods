﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Service.Dtos.Chat;
using TOPDER.Service.Dtos.Notification;
using TOPDER.Service.Hubs;
using TOPDER.Service.IServices;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IHubContext<AppHub> _signalRHub;

        public ChatController(IChatService chatService, IHubContext<AppHub> signalRHub)
        {
            _chatService = chatService;
            _signalRHub = signalRHub;
        }

        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Tạo tin nhắn: Customer | Restaurant")]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatDto createChatDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _chatService.AddAsync(createChatDto);
            if (result != null)
            {
                await _signalRHub.Clients.All.SendAsync("CreateChat", result);
                return Ok(result);
            }

            return BadRequest("Tạo chat thất bại.");
        }

        [HttpGet("GetChat/{chatId}")]
        [SwaggerOperation(Summary = "Lấy ra một tin nhắn để Update: Customer | Restaurant")]
        public async Task<IActionResult> GetChat(int chatId)
        {
            try
            {
                var chat = await _chatService.GetItemAsync(chatId);
                return Ok(chat);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Chat với ID {chatId} không tồn tại.");
            }
        }

        [HttpGet("GetChatList/{chatBoxId}")]
        [SwaggerOperation(Summary = "Lấy ra toàn bộ tin nhắn của ChatBox: Customer | Restaurant")]
        public async Task<IActionResult> GetChatList(int chatBoxId)
        {
            var chats = await _chatService.GetListAsync(chatBoxId);
            return Ok(chats);
        }

        [HttpDelete("Delete/{id}")]
        [SwaggerOperation(Summary = "Xóa tin nhắn: Customer | Restaurant")]
        public async Task<IActionResult> DeleteChat(int id)
        {
            var result = await _chatService.RemoveAsync(id);
            if (result)
            {
                return Ok($"Xóa chat với ID {id} thành công.");
            }
            return NotFound($"Chat với ID {id} không tồn tại.");
        }

        [HttpPut("Update")]
        [SwaggerOperation(Summary = "Cập nhật tin nhắn: Customer | Restaurant")]
        public async Task<IActionResult> UpdateChat([FromBody] UpdateChatDto updateChatDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _chatService.UpdateAsync(updateChatDto);
            if (result)
            {
                return Ok("Cập nhật chat thành công.");
            }
            return NotFound($"Chat với ID {updateChatDto.ChatId} không tồn tại.");
        }

    }
}
