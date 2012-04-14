using UnityEngine;

namespace SlimNet.Unity
{
    public class Server : SlimNet.Server
    {
        #region Static

        static Log log = Log.GetLogger(typeof(Server));

        public static Server Instance
        {
            get;
            private set;
        }

        public static Server Create(int port)
        {
            ServerConfiguration config = PeerUtils.LoadConfigurationAsset<ServerConfiguration>(SlimNet.Constants.ServerConfigName);
            config.Port = port;

            return new Server(config);
        }

        #endregion

        Server(ServerConfiguration config)
            : base(config)
        {
            Instance = this;
        }

        public override void Start()
        {
            log.Info("Starting server on *:{0}", ServerConfiguration.Port);

            // Start server listener
            NetworkServer.Listen();

            // Start time manager
            Context.Time.Start();
        }

        protected override void LoadAssemblies()
        {

        }

        public override void BeforeActorStart(Actor actor)
        {
            PeerUtils.CreateGameObject(actor);

            GameObject go = GameObjectMap.Retrieve(actor);
            go.transform.position = Converter.Convert(actor.Transform.Position);
            go.transform.rotation = Converter.Convert(actor.Transform.Rotation);
        }

        public override void BeforeActorDestroy(Actor actor)
        {
            GameObject.Destroy(actor.GetGameObject());
            GameObjectMap.Remove(actor);
        }

        public override void BeforeSimulate()
        {
            PeerUtils.CopyTransformsToSlimNet(Context);
        }

        public void Update(float deltaTime)
        {
            while (NetworkServer.ReceiveOne())
            {
                //TODO: This loop needs to be time-limited.
            }

            Context.Render();
            Context.Simulate();
            Context.Send();
        }
    }
}
