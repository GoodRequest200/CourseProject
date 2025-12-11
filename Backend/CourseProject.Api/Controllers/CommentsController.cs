using CourseProject.Api.DTO;
using CourseProject.Core.Abstractions;
using CourseProject.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourseProject.Api.Controllers 
{
    [ApiController]
    [Route("api/comments")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _repo;

        public CommentsController(ICommentRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _repo.GetAllAsync();
            var dto = comments.Select(c => new CommentResponseDto(
                c.Id, c.EventId, c.UserId, c.Content, c.CreatedAt));
            return Ok(dto);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var comment = await _repo.GetByIdAsync(id);
            if (comment == null) return NotFound();

            var dto = new CommentResponseDto(
                comment.Id, comment.EventId, comment.UserId, comment.Content, comment.CreatedAt);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CommentRequestDto dto)
        {
            var modelResult = CommentModel.Create(
                id: 0,
                dto.EventId,
                dto.UserId,
                dto.Content);

            if (modelResult.IsFailure)
                return BadRequest(modelResult.Error);

            var created = await _repo.CreateAsync(modelResult.Value);
            var response = new CommentResponseDto(
                created.Id, created.EventId, created.UserId, created.Content, created.CreatedAt);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CommentRequestDto dto)
        {
            var modelResult = CommentModel.Create(
                id,
                dto.EventId,
                dto.UserId,
                dto.Content);

            if (modelResult.IsFailure)
                return BadRequest(modelResult.Error);

            var updated = await _repo.UpdateAsync(modelResult.Value);
            if (updated == null) return NotFound();

            var response = new CommentResponseDto(
                updated.Id, updated.EventId, updated.UserId, updated.Content, updated.CreatedAt);
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _repo.DeleteAsync(id);
            return ok ? Ok() : NotFound();
        }

        [HttpGet("event/{eventId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByEvent(int eventId)
        {
            var list = await _repo.GetByEventIdAsync(eventId);
            var dto = list.Select(c => new CommentResponseDto(
                c.Id, c.EventId, c.UserId, c.Content, c.CreatedAt));
            return Ok(dto);
        }
    }
}
