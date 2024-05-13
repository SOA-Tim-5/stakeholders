using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Grpc.Core;
using GrpcServiceTranscoding;

namespace Explorer.API.Controllers
{
    public class AuthenticationProtoController : Authorize.AuthorizeBase
    {
        private readonly ILogger<AuthenticationProtoController> _logger;
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationProtoController(ILogger<AuthenticationProtoController> logger, IAuthenticationService authenticationService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
        }

        public override async Task<AuthenticationTokens> Authorize(Credentials request,
            ServerCallContext context)
        {
            var credentials = new Stakeholders.API.Dtos.CredentialsDto { Username  = request.Username, Password = request.Password };
            var result = _authenticationService.Login(credentials);
            var task = new AuthenticationTokens
            {
                Id = (int)result.Value.Id,
                AccessToken = result.Value.AccessToken,
            };

            return task;
        }
    }
}
