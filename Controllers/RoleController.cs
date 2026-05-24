using Microsoft.AspNetCore.Mvc;
using SmartPOS.Shared.Common;
using SmartPOS.Shared.DTOs.Roles;
using SmartPOS.Shared.Interfaces;

namespace SmartPOS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET api/role
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _roleService.GetAllRoles();
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // GET api/role/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _roleService.GetRoleById(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // POST api/role
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
        {
            var result = await _roleService.CreateRole(dto);
            return result.Success
                ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result)
                : BadRequest(result);
        }

        // PUT api/role/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleDto dto)
        {
            var result = await _roleService.UpdateRole(id, dto);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // DELETE api/role/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _roleService.DeleteRole(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        // GET api/role/5/permissions
        [HttpGet("{id}/permissions")]
        public async Task<IActionResult> GetPermissions(int id)
        {
            var result = await _roleService.GetRolePermissions(id);
            return result.Success ? Ok(result) : NotFound(result);
        }

        // PUT api/role/5/permissions
        [HttpPut("{id}/permissions")]
        public async Task<IActionResult> UpdatePermissions(int id, [FromBody] PermissionsDto permissions)
        {
            var result = await _roleService.UpdateRolePermissions(id, permissions);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
