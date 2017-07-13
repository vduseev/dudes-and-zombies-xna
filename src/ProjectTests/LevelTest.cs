using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using Project.Objects;
using Moq;

namespace ProjectTests
{
    [TestClass]
    public class LevelTest: Game
    {
        [TestMethod]
        public void FireBulletTest()
        {
            GraphicsDeviceManager graphicsDeviceManager = new GraphicsDeviceManager(this);

            Level level = new Level(GraphicsDevice, graphicsDeviceManager, Content, 1f);

            Mock gameTimeMock = new Mock<GameTime>();
            //level.FireBullet(gameTimeMock.Object,  );

        }
    }
}
