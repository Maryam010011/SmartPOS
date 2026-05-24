using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Users;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Policy = "AdminOnly")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetAll([FromQuery] UserFilterDto filter)
        {
            var response = await _userService.GetAllUsers(filter);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<UserDetailDto>>> GetById(int id)
        {
            var response = await _userService.GetUserById(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserDto>>> Create([FromBody] CreateUserDto dto)
        {
            var response = await _userService.CreateUser(dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Update(int id, [FromBody] UpdateUserDto dto)
        {
            var response = await _userService.UpdateUser(id, dto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse>> Delete(int id)
        {
            var response = await _userService.DeleteUser(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("{id:int}/activate")]
        public async Task<ActionResult<ApiResponse>> Activate(int id)
        {
            var response = await _userService.ActivateUser(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("{id:int}/deactivate")]
        public async Task<ActionResult<ApiResponse>> Deactivate(int id)
        {
            var response = await _userService.DeactivateUser(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("bulk-activate")]
        public async Task<ActionResult<ApiResponse>> BulkActivate([FromBody] List<int> ids)
        {
            var response = await _userService.BulkActivate(ids);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("bulk-deactivate")]
        public async Task<ActionResult<ApiResponse>> BulkDeactivate([FromBody] List<int> ids)
        {
            var response = await _userService.BulkDeactivate(ids);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
