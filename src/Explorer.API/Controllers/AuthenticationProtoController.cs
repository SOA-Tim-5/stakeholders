using System;
using System.Security.Claims;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.UseCases;
using Grpc.Core;
using GrpcServiceTranscoding;
using Microsoft.AspNetCore.Mvc;
using static Google.Rpc.Context.AttributeContext.Types;

namespace Explorer.API.Controllers
{
    public class AuthenticationProtoController : Authorize.AuthorizeBase
    {
        private readonly ILogger<AuthenticationProtoController> _logger;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserService _userService;
        private readonly IPersonService _personService;

        public AuthenticationProtoController(ILogger<AuthenticationProtoController> logger, IAuthenticationService authenticationService, IUserService userService, IPersonService personService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
            _userService = userService;
            _personService = personService;
        }

        public override Task<AuthenticationTokens> Authorize(Credentials request,
            ServerCallContext context)
        {
            var credentials = new Stakeholders.API.Dtos.CredentialsDto { Username  = request.Username, Password =request.Password};
            var result = _authenticationService.Login(credentials);
            var task = new AuthenticationTokens
            {
                Id = (int)result.Value.Id,
                AccessToken = result.Value.AccessToken,
            };

            return Task.FromResult(new AuthenticationTokens
            {
                Id = (int)result.Value.Id,
                AccessToken = result.Value.AccessToken,
            }); 
        }
        
        public override async Task<ListUserResponseDtoA> GetSearch(SearchUsernameA request,
            ServerCallContext context)
        {
            long userId = long.Parse(request.UserId);
         
            List<GrpcServiceTranscoding.UserResponseDtoA> results = new List<GrpcServiceTranscoding.UserResponseDtoA>();
            List<Stakeholders.API.Dtos.UserResponseDto> result = _userService.SearchUsers(0, 0, request.SearchUsername, userId).Value;
            foreach(var r in result)
            {
                results.Add(new GrpcServiceTranscoding.UserResponseDtoA
                {
                    Username = r.Username,
                    Id = r.Id,
                    IsActive = r.IsActive,
                    ProfilePicture = r.ProfilePicture,
                    Role = r.Role
                });
            }
            return await Task.FromResult(new ListUserResponseDtoA { ResponseList = { results } });
        }
        
        public override async Task<GrpcServiceTranscoding.PersonResponseDtoA> GetByUserId(UserId request,
            ServerCallContext context)
        {
            long userId = request.UsersId;

            Stakeholders.API.Dtos.PersonResponseDto result = _personService.GetByUserId(userId).Value;
           

            return await Task.FromResult(new GrpcServiceTranscoding.PersonResponseDtoA {
                Id = result.Id,
                UserId = result.UserId,
                Name = result.Name,
                Surname = result.Surname,
                Email = result.Email,
                User = new GrpcServiceTranscoding.UserResponseDtoA
                {
                    Username = result.User.Username,
                    Id = result.User.Id,
                    IsActive = result.User.IsActive,
                    ProfilePicture = result.User.ProfilePicture,
                    Role = result.User.Role
                },
                Bio = result.Bio,
            });
       }
       
    }
}
