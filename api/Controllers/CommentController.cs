using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockrepository _stockRepo;
        public CommentController(ICommentRepository commentRepo, IStockrepository stockRepo)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _commentRepo.GetAllAsync();
            var commentsDto = comments.Select(c => c.ToCommentDto());

            return Ok(commentsDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var comment = await _commentRepo.GetByIdAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment.ToCommentDto());

        }
        [HttpPost("{stockId}")]
        public async Task<IActionResult> Create([FromRoute] int stockId, CreateCommentDto createCommentDto)
        {
            if (await _stockRepo.StockExists(stockId) == false)
            {
                return BadRequest("Stock Doesn't exist");
            }
            var commentModel = createCommentDto.ToCommentFromCreateCommentDto(stockId);
            await _commentRepo.CreateAsync(commentModel);
            return CreatedAtAction(nameof(GetById), new { id = commentModel.Id }, commentModel.ToCommentDto());
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentRequestDto updateCommentDto)
        {
            var comment = await _commentRepo.UpdateAsync(id, updateCommentDto.ToCommentFromUpdateCommentDto());
            if (comment == null)
            {
                NotFound("Comment not found");
            }
            return Ok(comment.ToCommentDto());
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var commentModel = await _commentRepo.DeleteAsync(id);
            if (commentModel == null)
            {
                return NotFound("Comment does not exist");
            }
            return Ok(commentModel);
        }
    }
}