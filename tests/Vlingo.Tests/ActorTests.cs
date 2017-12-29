using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using Shouldly;
using Xunit;

namespace Vlingo.Tests
{


    public abstract class ActorsTest
        : IDisposable
    {
        protected int Delay = 100;

        protected void Pause()
        {
            try
            {
                Thread.Sleep(Delay);
            }
            catch (Exception e)
            {

            }
        }

        public virtual void Dispose()
        {
        }

    }

    public class WorldTests
        : ActorsTest
    {
        private World _world;

        public WorldTests()
        {
            var k  =typeof(WorldTests).Assembly.GetManifestResourceNames();
            
            _world = World.Start("test");
        }

        [Fact]
        public void TestStartWorld()
        {
           

            _world.Configuration.ShouldNotBeNull();
            _world.DeadLetters.ShouldNotBeNull();

            _world.Name.ShouldBe("test");
            _world.Scheduler.ShouldNotBeNull();
            _world.Stage().ShouldNotBeNull();
            _world.Stage().ShouldNotBeNull();
            _world.Stage().World.ShouldBe(_world);
            

            //assertFalse(world.isTerminated());
            //assertNotNull(world.findDefaultMailboxName());
            //assertEquals("queueMailbox", world.findDefaultMailboxName());
            //assertNotNull(world.assignMailbox("queueMailbox", 10));
            //assertNotNull(world.defaultParent());
            //assertNotNull(world.privateRoot());
            //assertNotNull(world.publicRoot());
        }

        public override void Dispose()
        {
            _world.Terminate();

            _world.Stage().IsStopped().ShouldBeTrue();
            _world.IsTerminated().ShouldBeTrue();
            base.Dispose();
        }
    }
}
