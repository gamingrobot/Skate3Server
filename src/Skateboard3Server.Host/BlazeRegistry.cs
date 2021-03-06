using Autofac;
using MediatR;
using Skateboard3Server.Blaze;
using Skateboard3Server.Blaze.Handlers.Authentication;
using Skateboard3Server.Blaze.Handlers.GameManager;
using Skateboard3Server.Blaze.Handlers.Redirector;
using Skateboard3Server.Blaze.Handlers.SkateStats;
using Skateboard3Server.Blaze.Handlers.Social;
using Skateboard3Server.Blaze.Handlers.Teams;
using Skateboard3Server.Blaze.Handlers.UserSession;
using Skateboard3Server.Blaze.Handlers.Util;
using Skateboard3Server.Blaze.Managers;
using Skateboard3Server.Blaze.Serializer;
using Skateboard3Server.Blaze.Server;
using Skateboard3Server.Common.Decoders;
using Skateboard3Server.Host.Blaze;

namespace Skateboard3Server.Host
{
    public class BlazeRegistry : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BlazeMessageHandler>().As<IBlazeMessageHandler>();
            builder.RegisterType<BlazeSerializer>().As<IBlazeSerializer>();
            builder.RegisterType<BlazeDeserializer>().As<IBlazeDeserializer>();
            builder.RegisterType<BlazeDebugParser>().As<IBlazeDebugParser>();
            builder.RegisterType<BlazeTypeLookup>().As<IBlazeTypeLookup>();
            builder.RegisterType<Ps3TicketDecoder>().As<IPs3TicketDecoder>();

            //Connection Handling
            builder.RegisterType<BlazeProtocol>().SingleInstance();
            builder.RegisterType<ClientManager>().As<IClientManager>().SingleInstance(); //manages all clients
            builder.RegisterType<BlazeClientContext>().As<ClientContext>().InstancePerLifetimeScope(); //each connection gets one
            builder.RegisterType<BlazeNotificationHandler>().As<IBlazeNotificationHandler>().SingleInstance(); //I think this is correct
            builder.RegisterType<UserSessionManager>().As<IUserSessionManager>().SingleInstance(); //Handles sessions

            //Mediator
            builder.RegisterType<Mediator>().As<IMediator>().InstancePerLifetimeScope();
            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            //Blaze Message Handlers
            builder.RegisterType<ServerInfoHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<PreAuthHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<PingHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<LoginHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<PostAuthHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<ClientMetricsHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<SessionDataHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<HardwareFlagsHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<FriendsListHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<NetworkInfoHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<SkateStatsHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<DlcHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<TeamMembershipHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<Unknown640Handler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<StartMachmakingHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<GameSessionHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<SetGameAttributesHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<FinalizeGameCreationHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<CreateGameHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<SetGameStateHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<SetGameSettingsHandler>().AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterType<RemovePlayerHandler>().AsImplementedInterfaces().InstancePerDependency();
        }
    }
}