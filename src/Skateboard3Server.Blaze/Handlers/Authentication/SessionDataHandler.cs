﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Skateboard3Server.Blaze.Handlers.Authentication.Messages;
using Skateboard3Server.Blaze.Managers;
using Skateboard3Server.Blaze.Server;
using Skateboard3Server.Data;

namespace Skateboard3Server.Blaze.Handlers.Authentication
{
    public class SessionDataHandler : IRequestHandler<SessionDataRequest, SessionDataResponse>
    {
        private readonly Skateboard3Context _skateboard3Context;
        private readonly ClientContext _clientContext;
        private readonly IUserSessionManager _userSessionManager;

        public SessionDataHandler(Skateboard3Context skateboard3Context, ClientContext clientContext, IUserSessionManager userSessionManager)
        {
            _skateboard3Context = skateboard3Context;
            _clientContext = clientContext;
            _userSessionManager = userSessionManager;
        }

        public async Task<SessionDataResponse> Handle(SessionDataRequest request, CancellationToken cancellationToken)
        {
            if (_clientContext.UserId == null || _clientContext.UserSessionId == null)
            {
                throw new Exception("UserId/UserSessionId not on context");
            }
            var currentUserId = _clientContext.UserId.Value;
            var currentSessionId = _clientContext.UserSessionId.Value;

            var user = await _skateboard3Context.Users.SingleOrDefaultAsync(x => x.Id == currentUserId, cancellationToken);

            if (user == null)
            {
                return null;
            }

            var userSessionKey = _userSessionManager.GetSessionKey(currentSessionId);

            var response = new SessionDataResponse
            {
                BlazeId = user.Id,
                FirstLogin = false,
                SessionKey = userSessionKey,
                LastLoginTime = 0, 
                Email = "", //nobody@ea.com normally
                Persona = new SessionDataPersona
                {
                    DisplayName = user.Username,
                    LastUsed = 0,
                    PersonaId = user.PersonaId,
                    ExternalId = 0,
                    ExternalIdType = ExternalIdType.Unknown,
                },
                AccountId = user.AccountId,
            };
            return response;
            
        }
    }
}