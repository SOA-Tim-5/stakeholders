using System.Security.Claims;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

//[Authorize(Policy = "nonAdministratorPolicy")]
[Route("api/people")]
public class PersonController : BaseApiController
{
    private readonly IPersonService _personService;
    private readonly IUserService _userService;

    public PersonController(IPersonService personService, IUserService userService)
    {
        _personService = personService;
        _userService = userService; 
    }

    [HttpPut("update/{personId:long}")]
    public ActionResult<PersonResponseDto> Update([FromBody] PersonUpdateDto person, long personId)
    {
        var loggedInUserId = long.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);
        var response = _personService.Get(personId);
        if (response.IsFailed) return CreateResponse(response);

        var userId = response.Value.UserId;
        if (loggedInUserId == userId)
        {
            person.Id = personId;
            var result = _personService.UpdatePerson(person);
            return CreateResponse(result);
        }
        return Forbid();
    }

    [HttpGet]
    public ActionResult<PersonResponseDto> GetPaged(int page, int pageSize)
    {
        var result = _personService.GetAll(page, pageSize);
        return CreateResponse(result);
    }

   /* [HttpGet("person/{userId:long}")]
    public ActionResult<PersonResponseDto> GetByUserId(long userId)
    {
        var result = _personService.GetByUserId(userId);
        return CreateResponse(result);
    }*/

   /* [HttpGet("follower/search/{searchUsername}")]
    public ActionResult<PagedResult<UserResponseDto>> GetSearch([FromQuery] int page, [FromQuery] int pageSize, string searchUsername)
    {
        //long userId = int.Parse(HttpContext.User.Claims.First(i => i.Type.Equals("id", StringComparison.OrdinalIgnoreCase)).Value);
        long userId = 0;
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (identity != null && identity.IsAuthenticated)
        {
            userId = long.Parse(identity.FindFirst("id").Value);
        }

        var result = _userService.SearchUsers(0, 0, searchUsername, userId);
        return CreateResponse(result);
    }*/

}